// -----------------------------------------------------------------------------
// SplineArchitect
// Filename: LinePointsJob.cs
//
// Author: Mikael Danielsson
// Date Created: 10-08-2026
// (C) 2023 Mikael Danielsson. All rights reserved.
// -----------------------------------------------------------------------------

using Unity.Burst;
using Unity.Jobs;
using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;

using SplineArchitect.Utility;

namespace SplineArchitect.Jobs
{
    [BurstCompile(FloatPrecision.Low, FloatMode.Fast, OptimizeFor = OptimizeFor.Performance)]
    public struct LinePointsJob : IJob
    {
        public NativeList<Vector3> linePoints;
        public NativeList<bool> linePointsViewMap;

        [ReadOnly] public float resolution;
        [ReadOnly] public float splineResolution;
        [ReadOnly] public float viewDistance;
        [ReadOnly] public NativeList<float3> positionMapLocal;
        [ReadOnly] public NativeArray<NativeSegment> nativeSegmentsLocal;
        [ReadOnly] public float4x4 splineWorldToLocal;
        [ReadOnly] public float4x4 splineLocalToWorld;
        [ReadOnly] public float3 cameraPosition;
        [ReadOnly] public bool orthographicCamera;
        [ReadOnly] public float orthographicCameraSize;

        public void Execute()
        {
            linePoints.Clear();
            linePointsViewMap.Clear();

            float3 localCameraPoint = math.transform(splineWorldToLocal, cameraPosition);

            for (int i = 0; i < nativeSegmentsLocal.Length - 1; i++)
            {
                float3 anchorA = nativeSegmentsLocal[i].anchor;
                float3 tangnetA = nativeSegmentsLocal[i].tangentA;
                float3 tangentB = nativeSegmentsLocal[i + 1].tangentB;
                float3 anchorB = nativeSegmentsLocal[i + 1].anchor;
                float length = nativeSegmentsLocal[i].length;

                int lines = GetSegmentLinesCount(i, anchorA, tangnetA, tangentB, anchorB, length, localCameraPoint, 
                                                                                               resolution, 
                                                                                               orthographicCamera,
                                                                                               orthographicCameraSize, 
                                                                                               viewDistance,
                                                                                               positionMapLocal, nativeSegmentsLocal,
                                                                                               resolution,
                                                                                               out bool inViewDistance);

                int start = (i == 0) ? 0 : 1;
                for (int i2 = start; i2 <= lines; i2++)
                {
                    float segmentTime = (float)i2 / lines;
                    float t = (i + segmentTime) / (nativeSegmentsLocal.Length - 1);

                    float3 point = SplineUtilityNative.GetPositionFast(positionMapLocal, nativeSegmentsLocal, splineResolution, t);
                    point = math.transform(splineLocalToWorld, point);
                    linePoints.Add(point);
                    linePointsViewMap.Add(inViewDistance);
                }
            }
        }

        public static int GetSegmentLinesCount(int segmentIndex,
                                               float3 anchorA,
                                               float3 tangentA,
                                               float3 tangentB,
                                               float3 anchorB,
                                               float segmentLength,
                                               float3 cameraPosition,
                                               float resolution,
                                               bool orthographicCamera,
                                               float orthographicCameraSize,
                                               float viewDistance,
                                               NativeList<float3> positionMapLocal, NativeArray<NativeSegment> nativeSegmentsLocal,
                                               float splineResolution,
                                               out bool inViewDistance)
        {
            //Settings
            const int testRange = 10;

            inViewDistance = false;
            float3 center = (anchorA + anchorB) / 2;
            float distanceToCamera = math.distance(center, cameraPosition);

            //Second in view test
            int times = (int)(segmentLength / testRange);
            times = math.clamp(times, 1, 10);
            for (int sampleIndex = 0; sampleIndex <= times; sampleIndex++)
            {
                float segmentTime = (float)sampleIndex / times;
                float st = (segmentIndex + segmentTime) / (nativeSegmentsLocal.Length - 1);
                float3 point = SplineUtilityNative.GetPositionFast(positionMapLocal, nativeSegmentsLocal, splineResolution, st);

                float dCheck = math.distance(point, cameraPosition);
                if (dCheck < distanceToCamera)
                    distanceToCamera = dCheck;

                if(dCheck < viewDistance)
                    inViewDistance = true;
            }

            float3 tADirection = math.normalizesafe(tangentA - anchorA);
            float3 tBDirection = math.normalizesafe(anchorB - tangentB);
            float distanceBetween = math.distance(anchorA, anchorB);

            //If segment is aligned draw one line
            if (GeneralUtility.IsEqual(tADirection, tBDirection) && GeneralUtility.IsEqual(anchorB, anchorA + tADirection * distanceBetween, 0.01f))
                return 2;

            resolution = resolution / 100;
            float clampedUserModification = math.clamp(resolution, 0.1f, 10);

            //Distance modifier
            float boostedLength = GeneralUtility.BoostedValue(segmentLength, 100 * clampedUserModification, 0.7f);
            if (GeneralUtility.IsZero(distanceToCamera)) distanceToCamera = 0.001f;
            float t = distanceToCamera / boostedLength;

            if (orthographicCamera)
                t = orthographicCameraSize * 1.33f / boostedLength;

            float distanceModifier = math.lerp(13, 0, t);

            //Segment segmentLength modifier
            float time = math.clamp(segmentLength, 0, 100) / 100;
            float lengthModifier = math.lerp(0, 15, time) * math.clamp(100 / distanceToCamera, 0, 1);

            //Final
            float final = (distanceModifier + lengthModifier) * resolution;
            final = math.clamp(final, 1 + (clampedUserModification * 2), 100);
            return (int)final;
        }
    }
}
