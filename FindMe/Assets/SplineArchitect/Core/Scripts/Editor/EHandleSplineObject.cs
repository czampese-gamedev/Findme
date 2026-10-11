// -----------------------------------------------------------------------------
// SplineArchitect
// Filename: EHandleSplineObject.cs
//
// Author: Mikael Danielsson
// Date Created: 18-02-2024
// (C) 2023 Mikael Danielsson. All rights reserved.
// -----------------------------------------------------------------------------

using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

using SplineArchitect.Utility;
using SplineArchitect.Ui;
using SplineArchitect.CustomTools;

namespace SplineArchitect
{
#if UNITY_6000_5_OR_NEWER
    [Unity.Scripting.LifecycleManagement.NoAutoStaticsCleanup]
#endif
    public static class EHandleSplineObject
    {
        private static HashSet<SplineObject> noOriginMeshWarningContainer = new HashSet<SplineObject>();
        private static List<SplineObject> splineObjectContainer = new List<SplineObject>();

        internal static void OnSceneGUI(Spline spline, Event e)
        {
            if(PositionTool.activePart == PositionTool.Part.NONE)
            {
                noOriginMeshWarningContainer.Clear();
            }

            if (e.type == EventType.Layout)
            {
                for (int i = spline.AllSplineObjectCount - 1; i >= 0; i--)
                {
                    //Many spline objects can be deleted or added during the same frame so we need this check
                    if (i > spline.AllSplineObjectCount - 1)
                        continue;

                    SplineObject so = spline.GetSplineObjectAtIndex(i);

                    if (so == null)
                    {
                        spline.RemoveSplineObject(so);
                        continue;
                    }
                }
            }
        }

        internal static void Update(Spline spline)
        {
            for (int i = 0; i < spline.AllSplineObjectCount; i++)
            {
                SplineObject so = spline.GetSplineObjectAtIndex(i);

                if (so == null)
                    continue;

                if (so.Monitor.EditorComponentCountChange())
                {
                    EHandleMeshContainer.Initialize(so);
                    EHandleMeshContainer.DeleteUnvalidMeshContainers(so);
                }

                if (so.Monitor.EditorStaticChange())
                {
                    WindowExtended.RepaintAll();
                }
            }
        }

        public static bool HasReadWriteAccessEnabled(SplineObject so)
        {
            for(int i = 0; i < so.MeshContainerCount; i++)
            {
                MeshContainer mc = so.GetMeshContainerAtIndex(i);
                Mesh instanceMesh = mc.GetInstanceMesh();

                if(instanceMesh != null)

                if(!instanceMesh.isReadable)
                    return false;
            }

            return true;
        }

        public static void ToSplineCenter(SplineObject so)
        {
            so.localSplinePosition = new Vector3(0, 0, GeneralUtility.RoundToClosest(so.localSplinePosition.z, EditorSnapSettings.move.z));
            so.activationPosition = so.localSplinePosition;
            so.localSplineRotation.eulerAngles = new Vector3(0, 0, 0);
        }

        public static Vector3 SnapPosition(Vector3 position)
        {
            float snapValue = EditorSnapSettings.move.x + EditorSnapSettings.move.y + EditorSnapSettings.move.z;

#if UNITY_6000_0_OR_NEWER
            if (EditorSnapSettings.snapEnabled && snapValue > 0)
            {
#else
            if (EditorSnapSettings.gridSnapEnabled && snapValue > 0)
            {
#endif
                position.x = GeneralUtility.RoundToClosest(position.x, EditorSnapSettings.move.x);
                position.y = GeneralUtility.RoundToClosest(position.y, EditorSnapSettings.move.y);
                position.z = GeneralUtility.RoundToClosest(position.z, EditorSnapSettings.move.z);
            }

            return position;
        }

        public static bool ValidForEditorDeformation(SplineObject so)
        {
            bool xZero = GeneralUtility.IsZero(so.transform.lossyScale.x);
            bool yZero = GeneralUtility.IsZero(so.transform.lossyScale.y);
            bool zZero = GeneralUtility.IsZero(so.transform.lossyScale.z);

            if (so.Type == SplineObjectType.DEFORMATION && (xZero || yZero || zZero))
                return false;

            for (int i = 0; i < so.MeshContainerCount; i++)
            {
                MeshContainer mc = so.GetMeshContainerAtIndex(i);
                Mesh instanceMesh = mc.GetInstanceMesh();
                Mesh originMesh = mc.GetOriginMesh();

                if (originMesh == null)
                {
                    if (!noOriginMeshWarningContainer.Contains(so) && PositionTool.activePart != PositionTool.Part.NONE)
                    {
                        noOriginMeshWarningContainer.Add(so);
                        Debug.LogWarning($"[Spline Architect] Could not find the origin mesh for {so.name}, deformation aborted. Has the asset been deleted?");
                    }

                    return false;
                }
                if (instanceMesh == null) 
                    return false;

                if (instanceMesh == originMesh) 
                    return false;
            }

            return true;
        }

        public static void Convert(Spline spline, SplineObject so, SplineObjectType oldType)
        {
            if (so.Type == SplineObjectType.DEFORMATION)
                ConvertToDeformation(spline, so, oldType);
            else if (so.Type == SplineObjectType.FOLLOWER)
                ConvertToFollower(spline, so, oldType);
            else if (so.Type == SplineObjectType.NONE)
                ConvertToNone(spline, so, oldType);

            EHandleSpline.MarkForInfoUpdate(spline);
        }

        public static void ConvertToFollower(Spline spline, SplineObject so, SplineObjectType oldType)
        {
            for (int i = 0; i < so.gameObject.GetComponentCount(); i++)
            {
                Component c = so.gameObject.GetComponentAtIndex(i);

                if (c == null)
                    continue;

                MeshFilter meshFilter = c as MeshFilter;
                MeshCollider meshCollider = c as MeshCollider;

                if(meshFilter != null)
                {
                    Mesh originMesh = ESplineObjectUtility.GetOriginMeshFromMeshNameId(meshFilter.sharedMesh);

                    if(originMesh == null)
                        continue;

                    meshFilter.sharedMesh = originMesh;
                }
                else if (meshCollider != null)
                {
                    Mesh originMesh = ESplineObjectUtility.GetOriginMeshFromMeshNameId(meshCollider.sharedMesh);

                    if (originMesh == null)
                        continue;

                    meshCollider.sharedMesh = originMesh;
                }
            }

            SplineObject[] cildSos = so.transform.GetComponentsInChildren<SplineObject>();
            for (int i = 0; i < cildSos.Length; i++)
            {
                SplineObject childSo = cildSos[i];

                if (childSo == null)
                    continue;

                if (childSo == so)
                    continue;

                if (childSo.Type == SplineObjectType.DEFORMATION)
                {
                    childSo.SetInstanceMeshesToOriginMesh();
                    childSo.UpdateExternalComponents(true);
                }
                else if(childSo.transform.parent == so.transform)
                {
                    childSo.transform.localPosition = childSo.localSplinePosition;
                    childSo.transform.localRotation = childSo.localSplineRotation;
                }

                if (!EHandleUndo.UndoTriggered())
                    EHandleUndo.DestroyObjectImmediate(childSo);
            }

            so.Type = SplineObjectType.FOLLOWER;
            spline.directSystemWorker.Deform(so, true);
            EHandleTool.UpdateOrientationForPositionTool(EHandleSceneView.GetCurrent(), spline);
        }

        public static void ConvertToDeformation(Spline spline, SplineObject so, SplineObjectType oldType)
        {
            so.Type = SplineObjectType.DEFORMATION;

            Transform[] childs = so.transform.GetComponentsInChildren<Transform>();

            splineObjectContainer.Clear();
            for (int i = 0; i < childs.Length; i++)
            {
                Transform child = childs[i];

                if (child == null)
                    continue;

                if (child == so.transform)
                    continue;

                ESplineObjectUtility.TryAttacheOnTransformEditor(spline, child, true, EHandleUndo.UndoTriggered());
                SplineObject soChild = child.GetComponent<SplineObject>();

                if(so != null)
                    splineObjectContainer.Add(soChild);
            }

            //so.transform.localPosition = so.localSplinePosition;
            //so.transform.localRotation = so.localSplineRotation;
            EHandleTool.UpdateOrientationForPositionTool(EHandleSceneView.GetCurrent(), spline);

            so.SyncMeshContainers();
            so.SyncInstanceMeshesFromCache();

            so.RenderMeshInternal(true);
            foreach (SplineObject soChild in splineObjectContainer) soChild.RenderMeshInternal(true);
        }

        public static void ConvertToNone(Spline spline, SplineObject so, SplineObjectType oldType)
        {
            ConvertToFollower(spline, so, oldType);
            so.Type = SplineObjectType.NONE;
        }
    }
}
