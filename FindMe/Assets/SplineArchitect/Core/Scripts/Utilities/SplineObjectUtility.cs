// -----------------------------------------------------------------------------
// SplineArchitect
// Filename: SplineObjectUtility.cs
//
// Author: Mikael Danielsson
// Date Created: 11-09-2024
// (C) 2023 Mikael Danielsson. All rights reserved.
// -----------------------------------------------------------------------------

using System.Collections.Generic;

using Unity.Mathematics;
using UnityEngine;

namespace SplineArchitect.Utility
{
#if UNITY_6000_5_OR_NEWER
    [Unity.Scripting.LifecycleManagement.NoAutoStaticsCleanup]
#endif
    public class SplineObjectUtility
    {
        private static List<Segment> segmentContainer = new List<Segment>();

        public static Bounds GetCombinedBounds(SplineObject so)
        {
            Bounds bounds = new Bounds();
            bool initalized = false;

            for(int i = 0; i < so.MeshContainerCount; i++)
            {
                MeshContainer mc = so.GetMeshContainerAtIndex(i);
                Mesh sharedMesh = mc.GetInstanceMesh();

                if (sharedMesh == null)
                    continue;

                Bounds localBounds = sharedMesh.bounds;
                Vector3 worldCenter = so.transform.TransformPoint(localBounds.center);
                Vector3 worldSize = Vector3.Scale(localBounds.size, so.transform.lossyScale);
                Bounds worldBounds = new Bounds(worldCenter, worldSize);

                if (!initalized)
                {
                    initalized = true;
                    bounds = worldBounds;
                }
                else
                {
                    bounds.Encapsulate(worldBounds);
                }
            }

            return bounds;
        }

        public static Vector3 GetCombinedParentPositions(SplineObject so)
        {
            Vector3 value = Vector3.zero;

            for (int i = 0; i < 25; i++)
            {
                if (so == null)
                    break;

                value += so.localSplinePosition;
                so = so.SoParent;
            }

            return value;
        }

        public static Vector3 GetCombinedParentScales(SplineObject so)
        {
            Vector3 value = Vector3.one;

            for (int i = 0; i < 25; i++)
            {
                if (so == null)
                    break;

                value = Vector3.Scale(value, so.transform.localScale);
                so = so.SoParent;
            }

            return value;
        }

        public static Quaternion GetCombinedParentRotations(SplineObject so)
        {
            Quaternion value = Quaternion.identity;

            for (int i = 0; i < 25; i++)
            {
                if (so == null)
                    break;

                value = so.localSplineRotation * value;
                so = so.SoParent;
            }

            return value;
        }

        public static float4x4 GetCombinedParentMatrixs(SplineObject so, bool forceSplineSpace = false)
        {
            float4x4 matrix = float4x4.identity;

            for (int i = 0; i < 25; i++)
            {
                if (so == null)
                    break;

                if (forceSplineSpace || so.Type == SplineObjectType.DEFORMATION)
                    matrix = math.mul(float4x4.TRS(so.localSplinePosition, so.localSplineRotation, so.transform.localScale), matrix);
                else
                    matrix = math.mul(float4x4.TRS(so.transform.localPosition, so.transform.localRotation, so.transform.localScale), matrix);

                so = so.SoParent;
            }

            return matrix;
        }

        public static int GetCombinedParentHashCodes(SplineObject so)
        {
            int haschCode = 0;

            for (int i = 0; i < 25; i++)
            {
                so = so.SoParent;

                if (so == null)
                    break;

                if (so.Type != SplineObjectType.DEFORMATION)
                    break;

                haschCode += so.GetHashCode();
            }

            return haschCode;
        }

        public static SplineObject GetRootSplineObject(Transform transform)
        {
            SplineObject so = null;

            for (int i = 0; i < 25; i++)
            {
                if (transform == null)
                    break;

                SplineObject so2 = transform.GetComponent<SplineObject>();

                if (so2 != null)
                    so = so2;

                transform = transform.parent;
            }

            return so;
        }

        public static SplineObject GetRootSplineObject(SplineObject so)
        {
            if (so == null)
                return null;

            for (int i = 0; i < 25; i++)
            {
                if (so.SoParent == null)
                    break;

                so = so.SoParent;
            }

            return so;
        }

        public static SplineObjectType GetAutoType(SplineObject so)
        {
            Spline spline = so.SplineParent;
            Mesh originMesh = null;

            if (so.MeshContainerCount == 0)
            {
                MeshFilter meshFilter = so.gameObject.GetComponent<MeshFilter>();

                if (meshFilter == null)
                {
                    MeshCollider meshCollider = so.gameObject.GetComponent<MeshCollider>();

                    if (meshCollider != null)
                        originMesh = meshCollider.sharedMesh;
                }
                else
                    originMesh = meshFilter.sharedMesh;
            }
            else
            {
                originMesh = so.GetMeshContainerAtIndex(0).GetOriginMesh();
            }

            if (originMesh == null)
            {
                Debug.LogError($"[Spline Architect] Could not find origin mesh on {so.name} for calculating auto type.");
                return so.Type;
            }

            segmentContainer.Clear();
            Bounds bounds = originMesh.bounds;
            bounds = GeneralUtility.TransformBounds(originMesh.bounds, GetCombinedParentMatrixs(so, true));
            float bak = bounds.min.z;
            float front = bounds.max.z;
            float closest = 99999;

            if (front < 0 || bak > spline.Length)
            {
                return SplineObjectType.FOLLOWER;
            }

            if (so.AlignToEnd)
            {
                for (int i = spline.segments.Count - 1; i > 0; i--)
                {
                    Segment s1 = spline.segments[i - 1];
                    Segment s2 = spline.segments[i];

                    if ((bak <= s1.zPosition && front >= s1.zPosition) || (bak <= s2.zPosition && front >= s2.zPosition) || (bak >= s1.ZPosition && front <= s2.zPosition))
                    {
                        if (!segmentContainer.Contains(s1))
                            segmentContainer.Add(s1);

                        if (!segmentContainer.Contains(s2))
                            segmentContainer.Add(s2);
                    }

                    float bakDisToS1 = Mathf.Abs(bak - s1.zPosition);
                    float bakDisToS2 = Mathf.Abs(bak - s2.zPosition);
                    float frontDisToS1 = Mathf.Abs(front - s1.zPosition);
                    float frontDisToS2 = Mathf.Abs(front - s2.zPosition);

                    if (bakDisToS1 < closest) closest = bakDisToS1;
                    if (bakDisToS2 < closest) closest = bakDisToS2;
                    if (frontDisToS1 < closest) closest = frontDisToS1;
                    if (frontDisToS2 < closest) closest = frontDisToS2;
                }
            }
            else
            {
                for (int i = 1; i < spline.segments.Count; i++)
                {
                    Segment s1 = spline.segments[i - 1];
                    Segment s2 = spline.segments[i];

                    if ((bak <= s1.zPosition && front >= s1.zPosition) || (bak <= s2.zPosition && front >= s2.zPosition) || (bak >= s1.ZPosition && front <= s2.zPosition))
                    {
                        if (!segmentContainer.Contains(s1))
                            segmentContainer.Add(s1);

                        if (!segmentContainer.Contains(s2))
                            segmentContainer.Add(s2);
                    }

                    float bakDisToS1 = Mathf.Abs(bak - s1.zPosition);
                    float bakDisToS2 = Mathf.Abs(bak - s2.zPosition);
                    float frontDisToS1 = Mathf.Abs(front - s1.zPosition);
                    float frontDisToS2 = Mathf.Abs(front - s2.zPosition);

                    if (bakDisToS1 < closest) closest = bakDisToS1;
                    if (bakDisToS2 < closest) closest = bakDisToS2;
                    if (frontDisToS1 < closest) closest = frontDisToS1;
                    if (frontDisToS2 < closest) closest = frontDisToS2;
                }
            }

            if (segmentContainer.Count == 0)
            {
                Debug.LogError($"[Spline Architect] Could not find valid segments for {so.name}, cant calculate auto type.");
                return so.Type;
            }

            bool sameDirection = true;

            Segment originSegment = segmentContainer[0];

            Vector3 originAnchor = originSegment.GetPosition(ControlHandle.ANCHOR);
            Vector3 originDirection = originSegment.GetDirection();

            for (int i = 1; i < segmentContainer.Count; i++)
            {
                Segment segment = segmentContainer[i];

                if (originSegment.GetInterpolationType() == InterpolationType.LINE && segment.GetInterpolationType() == InterpolationType.LINE)
                {
                    float factor = originSegment.length + segment.length;
                    factor = factor / 100;

                    if (factor < closest)
                        continue;
                    else
                    {
                        sameDirection = false;
                        break;
                    }
                }

                Vector3 anchor = segment.GetPosition(ControlHandle.ANCHOR);

                // Is the anchor has the expected position
                float anchorDistance = Vector3.Distance(originAnchor, anchor);
                Vector3 expectedAnchor;
                if (so.AlignToEnd) expectedAnchor = originAnchor + -originDirection * anchorDistance;
                else expectedAnchor = originAnchor + originDirection * anchorDistance;

                if (!GeneralUtility.IsEqual(anchor, expectedAnchor, 0.001f))
                {
                    sameDirection = false;
                    break;
                }

                Vector3 direction = segment.GetDirection();
                direction.Normalize();

                // Check if the segments has the same direction.
                if (!GeneralUtility.IsEqual(direction, originDirection, 0.001f))
                {
                    sameDirection = false;
                    break;
                }
            }

            SnapSettings sSet = so.SnapSettings;
            if (sameDirection && (sSet.snapMode == SnapMode.NONE || (GeneralUtility.IsZero(sSet.endSnapDistance) && GeneralUtility.IsZero(sSet.startSnapDistance))))
            {
                return SplineObjectType.FOLLOWER;
            }
            else
            {
                return SplineObjectType.DEFORMATION;
            }
        }
    }
}