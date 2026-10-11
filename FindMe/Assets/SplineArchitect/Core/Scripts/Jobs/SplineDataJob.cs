// -----------------------------------------------------------------------------
// SplineArchitect
// Filename: SplineDataJob.cs
//
// Author: Mikael Danielsson
// Date Created: 16-04-2023
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
    [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, OptimizeFor = OptimizeFor.Performance)]
    public struct SplineDataJob : IJob
    {
        public NativeList<float> distanceMap;
        public NativeList<float3> positionMapLocal;
        public NativeArray<float> splineLength;
        public NativeArray<NativeSegment> nativeSegments;

        [ReadOnly] public float resolution;
        [ReadOnly] public float samplingStep;

        public void Execute()
        {
            int segCount = nativeSegments.Length - 1;
            int lowSteps = (int)math.floor(1f / resolution);
            int highSteps = (int)math.floor(1f / samplingStep);
            float invLow = 1f / lowSteps;
            float invHigh = 1f / highSteps;
            float lowTotal = 0f;

            // --- Low-Res Pass ---
            positionMapLocal.Clear();
            positionMapLocal.Capacity = lowSteps + 2;
            positionMapLocal.Add(SplineUtilityNative.GetPosition(nativeSegments, 0f));

            float3 lastLow = SplineUtilityNative.GetPosition(nativeSegments, 0f);
            for (int i = 1; i <= lowSteps; i++)
            {
                float t = (i == lowSteps) ? 1f : i * invLow;
                float3 p = SplineUtilityNative.GetPosition(nativeSegments, t);
                lowTotal += math.distance(lastLow, p);
                lastLow = p;

                positionMapLocal.Add(p);
            }

            // --- High-Res Pass ---
            distanceMap.Clear();
            distanceMap.Capacity = lowSteps + 2;
            distanceMap.Add(0f);

            float totalLen = 0f;
            float3 lastHigh = SplineUtilityNative.GetPosition(nativeSegments, 0f);
            float lastT = 0f;
            int thresholdIndex = 1;
            int currentSegment = 0;

            // First segment
            NativeSegment ns0 = nativeSegments[0];
            ns0.zPosition = 0f;
            nativeSegments[0] = ns0;

            // Last segment
            ns0 = nativeSegments[nativeSegments.Length - 1];
            ns0.length = 0;
            nativeSegments[nativeSegments.Length - 1] = ns0;

            for (int i = 1; i <= highSteps; i++)
            {
                float t = (i == highSteps) ? 1f : i * invHigh;
                float3 p = SplineUtilityNative.GetPosition(nativeSegments, t);

                float previousLen = totalLen;
                float d = math.distance(lastHigh, p);
                totalLen += d;

                // Add all distance-map thresholds crossed during this step.
                while (thresholdIndex < lowSteps)
                {
                    float targetDistance = lowTotal * (thresholdIndex * invLow);

                    if (targetDistance > totalLen)
                        break;

                    float stepFraction = d > 0f ? (targetDistance - previousLen) / d : 0f;

                    stepFraction = math.saturate(stepFraction);
                    float mappedT = math.lerp(lastT, t, stepFraction);
                    distanceMap.Add(mappedT);

                    thresholdIndex++;
                }

                lastHigh = p;
                lastT = t;

                int newSeg = math.min((int)(t * segCount), segCount - 1);

                if (newSeg != currentSegment)
                {
                    currentSegment = newSeg;

                    NativeSegment ns = nativeSegments[newSeg];
                    ns.zPosition = totalLen;
                    nativeSegments[newSeg] = ns;

                    NativeSegment ns2 = nativeSegments[newSeg - 1];
                    ns2.length = totalLen - ns2.zPosition;
                    nativeSegments[newSeg - 1] = ns2;
                }
            }

            distanceMap.Add(1f);

            // Last control point
            NativeSegment lastNs = nativeSegments[currentSegment + 1];
            lastNs.zPosition = totalLen;
            nativeSegments[currentSegment + 1] = lastNs;

            // Last segment
            NativeSegment previousNs = nativeSegments[currentSegment];
            previousNs.length = totalLen - previousNs.zPosition;
            nativeSegments[currentSegment] = previousNs;

            // Write out total spline length
            splineLength[0] = totalLen;
        }
    }
}