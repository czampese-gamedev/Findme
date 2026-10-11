// -----------------------------------------------------------------------------
// SplineArchitect
// Filename: Spline_CachedData.cs
//
// Author: Mikael Danielsson
// Date Created: 25-03-2023
// (C) 2023 Mikael Danielsson. All rights reserved.
// -----------------------------------------------------------------------------

using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Rendering;
using Unity.Collections;

using SplineArchitect.Monitor;
using SplineArchitect.Utility;

namespace SplineArchitect
{
    public partial class Spline : MonoBehaviour
    {
        [NonSerialized] private bool initalized;

        private void OnEnable()
        {
            Initalize();
        }

        private void OnDestroy()
        {
            InvokeBeforeSplineDestroy();
            DisposeCache();
            UnlinkAll();
            HandleRegistry.RemoveSpline(this);
#if UNITY_EDITOR
            EHandleEvents.InvokeAfterDestroySpline(this);
#endif

            void UnlinkAll()
            {
#if UNITY_EDITOR
                if (EHandlePrefab.prefabStageClosedLastFrame)
                    return;

                if (EHandleEvents.sceneIsClosing)
                    return;
#endif

                for (int i = 0; i < segments.Count; i++)
                {
                    Segment segment = segments[i];

                    if (segment.LinkCount == 0)
                    {
                        if (segment.linkTarget == LinkTarget.SPLINE_CONNECTOR && segment.SplineConnector != null)
                        {
                            segment.SplineConnector.RemoveConnection(segment);
                        }

                        continue;
                    }

                    //Unlink
                    for (int i3 = segment.LinkCount - 1; i3 >= 0; i3--)
                    {
                        Segment link = segment.GetLinkAtIndex(i3);

                        //Skip self
                        if (link == segment)
                            continue;

                        if (link == null || link.SplineParent == null)
                            continue;

                        if (link.LinkCount <= 2)
                        {
#if UNITY_EDITOR
                            UnityEditor.Undo.RecordObject(link.SplineParent, "Unlink");
#endif
                            link.links.Clear();
                            link.linkTarget = LinkTarget.NONE;
                        }
                        else
                        {
                            for (int i2 = link.LinkCount - 1; i2 >= 0; i2--)
                            {
                                Segment link2 = link.GetLinkAtIndex(i2);

                                if (link2 == segment)
                                {
                                    link.links.RemoveAt(i2);
                                    break;
                                }
                            }
                        }
                    }

                    segment.linkTarget = LinkTarget.NONE;
                    segment.links.Clear();
                }
            }
        }

        private void Start()
        {
#if UNITY_EDITOR
            if (EHandleEvents.dragActive)
                return;
#endif
            EstablishLinks();

            if (!Application.isPlaying)
                return;

            int oldJobInterval = jobInterval;
            initialJobDelayCounter = initialJobDelay;
            jobInterval = 0;

            ProcessSplineObjects();
            ProcessJobs(false);

            jobInterval = oldJobInterval;

#if !UNITY_EDITOR
            //1. Handle SplineObject components.
            for (int i = allSplineObjects.Count - 1; i >= 0; i--)
            {
                SplineObject so = allSplineObjects[i];

                if (so.componentMode == ComponentMode.ACTIVE)
                    continue;

                RemoveAtSplineObject(i);
                so.enabled = false;
            }

            //2. Handle Spline component.
            if (componentMode == ComponentMode.ACTIVE)
                return;

            allSplineObjects.Clear();
            enabled = false;
            DisposeCache();
            HandleRegistry.RemoveSpline(this);
#endif
        }

        private void Update()
        {
            if (jobStartType == JobType.UPDATE)
            {
                ProcessSplineObjects();
            }

            if (jobEndType == JobType.UPDATE)
            {
                ProcessJobs(true);
            }
        }

        private void LateUpdate()
        {
            if (jobStartType == JobType.LATE_UPDATE)
            {
                ProcessSplineObjects();
            }

            if (jobEndType == JobType.LATE_UPDATE)
            {
                ProcessJobs(true);
            }

            //Attach spline objects
            for (int i = 0; i < attachList.Count; i++)
            {
                SplineObject so = attachList[i];

                if (so.SplineParent == null || so.SplineParent != this)
                    continue;

                so.SyncParentData();
                if (so.Type == SplineObjectType.DEFORMATION)
                {
                    so.SyncMeshContainers();
                    if (so.MeshContainerCount > 0)
                        so.SyncInstanceMeshesFromCache();
                }

#if UNITY_EDITOR
                if (EHandleEvents.isSplineObjectSelected) so.MarkVersionDirty();
                else directSystemWorker.Add(so);
#else
                directSystemWorker.Add(so);
#endif
            }

            attachList.Clear();

#if UNITY_EDITOR
            if (!EHandleEvents.isSplineObjectSelected)
            {
                if (directSystemWorker.HasWork()) directSystemWorker.Complete();
            }
#else
            if (directSystemWorker.HasWork()) directSystemWorker.Complete();
#endif

            //Detach spline objects
            for (int i = detachList.Count - 1; i >= 0; i--)
            {
                //0 = none
                //1 = world
                //2 = local + skip undo
                int detachType = detachList[i].Item2;
                SplineObject so = detachList[i].Item1;

                if (so.SplineParent != null)
                {
                    detachList.RemoveAt(i);
                    continue;
                }

                RemoveFromActiveWorkers(so);

#if UNITY_EDITOR
                if (!Application.isPlaying && detachType != 2)
                {
                    UnityEditor.Undo.RecordObject(so.transform, "Detached Spline Object from spline");
                }
#endif

                for (int i2 = 0; i2 < so.MeshContainerCount; i2++)
                {
                    MeshContainer mc = so.GetMeshContainerAtIndex(i2);
                    Mesh originMesh = mc.GetOriginMesh();
                    if (originMesh != null) mc.SetInstanceMeshToOriginMesh();
                }

                if (detachType == 1)
                {
                    Vector3 point = so.splinePosition;
                    so.transform.position = SplinePositionToWorldPosition(point);
                    so.transform.rotation = SplineRotationToWorldRotation(so.splineRotation, point.z / length);
                }
                else if (detachType == 2)
                {
                    so.transform.localPosition = so.localSplinePosition;
                    so.transform.localRotation = so.localSplineRotation;
                }

                if(so.Type == SplineObjectType.DEFORMATION)
                    so.UpdateExternalComponents(true);

                so.SyncParentData();
                detachList.RemoveAt(i);
            }

            ProcessSplineInstances();

#if UNITY_EDITOR
            if(Application.isPlaying)
                DrawInstances();
#else
                DrawInstances();
#endif

            RebuildSplineLine();
        }

        internal void Initalize()
        {
#if UNITY_EDITOR
            if (EHandleEvents.dragActive)
            {
                EHandleEvents.InitalizeAfterDrag(this);
                return;
            }
#endif

            if (initalized)
                return;

            initalized = true;

            if (segments == null)
                segments = new List<Segment>();

            for (int i = 0; i < segments.Count; i++)
            {
                Segment s = segments[i];
                s.indexInSpline = i;
                s.splineParent = this;
                s.localSpace = transform;
            }

            if (noises == null)
                noises = new List<NoiseLayer>();

            if (monitor == null)
                monitor = new MonitorSpline(this);

#if UNITY_EDITOR
            ScriptableObjects.SplineUndoState undoState = GetUndoState();
            undoState.oldCacheVersion = undoState.cacheVersion;
#endif

            InitializeWorkers();
            RebuildCache();
            HandleRegistry.AddSpline(this);

            if (componentMode == ComponentMode.NONE)
            {
                if (Application.isPlaying) componentMode = ComponentMode.ACTIVE;
                else componentMode = ComponentMode.REMOVE_FROM_BUILD;
            }
        }

        internal void EstablishLinks()
        {
#if UNITY_EDITOR
            if (editorInitializedLinks)
                return;

            editorInitializedLinks = true;
            FixUnityPrefabBoundsCase();
#endif
            int count = -1;
            foreach (Segment s in segments)
            {
                count++;
#if UNITY_EDITOR
                s.oldLinkTarget = s.linkTarget;

                if (!Application.isPlaying) s.linkCreatedThisFrameOnly = true;
#endif
                if (s.linkTarget == LinkTarget.ANCHOR)
                {
                    //Has allready been established by another spline during Start.
                    if (s.LinkCount > 0)
                        continue;

                    s.LinkToSegment(s.GetPosition(ControlHandle.ANCHOR), false);

                    if (s.LinkCount < 2)
                    {
                        s.linkTarget = LinkTarget.NONE;
                        if(s.links != null)
                            s.links.Clear();
                    }
                }
                else if (s.linkTarget == LinkTarget.SPLINE_CONNECTOR)
                {
                    if (s.SplineConnector != null)
                        continue;

                    s.LinkToConnector(s.GetPosition(ControlHandle.ANCHOR));

                    if (s.SplineConnector == null)
                    {
                        s.linkTarget = LinkTarget.NONE;
                    }
                }

#if UNITY_EDITOR
                if (!Application.isPlaying) s.linkCreatedThisFrameOnly = false;
#endif
            }
        }

        private void RebuildSplineLine(bool forceUpdate = false)
        {
            if (!renderInGame || !Application.isPlaying || componentMode != ComponentMode.ACTIVE)
            {
                if (lineRenderer != null) lineRenderer.enabled = false;
                return;
            }

            if (lineRenderCamera == null)
                lineRenderCamera = Camera.main;

            if (lineRenderCamera == null)
                return;

            if (!forceUpdate && GeneralUtility.IsEqual(oldRenderCameraPos, lineRenderCamera.transform.position))
                return;

            if (lineRenderer == null)
            {
                lineRenderer = gameObject.AddComponent<LineRenderer>();
                lineWidthCurve = new AnimationCurve();
            }

            if (!linePoints.IsCreated) linePoints = new NativeList<Vector3>(0, Allocator.Persistent);
            NativeList<bool> linePointsViewMap = new NativeList<bool>(Allocator.TempJob);
            CalculateLinePoints(linePoints, linePointsViewMap, lineResolution, 9999, lineRenderCamera);
            linePointsViewMap.Dispose();

            lineRenderer.enabled = true;
            lineWidthCurve.ClearKeys();

            float finalWidth = width;
            if (finalWidth < 2f)
                finalWidth = 2f;

            if (lineRenderCamera.orthographic)
            {
                float orthographicWidth = lineRenderCamera.orthographicSize * finalWidth / 400f;
                lineWidthCurve.AddKey(0f, orthographicWidth);
                lineWidthCurve.AddKey(1f, orthographicWidth);
            }
            else
            {
                for (int i = 0; i < segments.Count - 1; i++)
                {
                    Vector3 anchorA = segments[i].GetPosition(ControlHandle.ANCHOR);
                    Vector3 anchorB = segments[i + 1].GetPosition(ControlHandle.ANCHOR);

                    float time = (float)i / (segments.Count - 1);
                    float widthA = Vector3.Distance(anchorA, lineRenderCamera.transform.position) / 100f * finalWidth * 0.125f;

                    lineWidthCurve.AddKey(time, widthA);

                    if (i == segments.Count - 2)
                    {
                        float widthB = Vector3.Distance(anchorB, lineRenderCamera.transform.position) / 100f * finalWidth * 0.125f;
                        lineWidthCurve.AddKey(1f, widthB);
                    }
                }
            }

            lineRenderer.material = RenderMaterial;
            lineRenderer.startColor = color;
            lineRenderer.endColor = color;

            lineRenderer.positionCount = linePoints.Length;
            lineRenderer.SetPositions(linePoints.AsArray());
            lineRenderer.widthCurve = lineWidthCurve;
            oldRenderCameraPos = lineRenderCamera.transform.position;
        }
    }
}
