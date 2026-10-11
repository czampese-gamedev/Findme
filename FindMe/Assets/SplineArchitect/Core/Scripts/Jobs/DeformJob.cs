// -----------------------------------------------------------------------------
// SplineArchitect
// Filename: DeformJob.cs
//
// Author: Mikael Danielsson
// Date Created: 12-02-2023
// (C) 2023 Mikael Danielsson. All rights reserved.
// -----------------------------------------------------------------------------

using UnityEngine;
using Unity.Burst;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Collections;

using SplineArchitect.Utility;

namespace SplineArchitect.Jobs
{
    [BurstCompile(FloatPrecision.Low, FloatMode.Fast, OptimizeFor = OptimizeFor.Performance)]
    public struct DeformJob : IJobParallelFor
    {
        public NativeArray<Vector3> vertices;
        public NativeArray<Vector3> meshNormals;
        public NativeArray<Vector4> meshTangents;
        public NativeArray<RaycastCommand> raycastCommands;

        [ReadOnly] public int splineObjectCount;
        [ReadOnly] public NativeHashMap<int, float4x4> localSpaces;
        [ReadOnly] public NativeArray<int> localSpaceMap;
        [ReadOnly] public NativeArray<NormalType> soNormalTypeMap;
        [ReadOnly] public NativeArray<bool> mirrorMap;
        [ReadOnly] public NativeArray<bool> alignToEndMap;
        [ReadOnly] public NativeArray<bool> skipTangentsMap;
        [ReadOnly] public NativeArray<LayerMask> layerMaskMap;
        [ReadOnly] public NativeArray<NativeSegment> nativeSegments;
        [ReadOnly] public NativeArray<NoiseLayer> noises;
        [ReadOnly] public NativeArray<HermiteSegment> scaleCurveSegments;
        [ReadOnly] public float4x4 splineLocalToWorldMatrix;
        [ReadOnly] public float splineLength;
        [ReadOnly] public NativeArray<SnapData> snapDatas;
        [ReadOnly] public NativeList<float> distanceMap;
        [ReadOnly] public NativeList<float3> normals;
        [ReadOnly] public NativeList<int> normalsMap;
        [ReadOnly] public NativeList<float3> positionMapLocal;
        [ReadOnly] public float splineResolution;
        [ReadOnly] public bool loop;

        public void Execute(int i)
        {
            float3 vertice = vertices[i];
            int snapDataIndex = i;

            float4x4 localSpace;
            bool alignToEnd = false;
            bool skipTangents = false;
            LayerMask layerMask = ~(1 << 2);
            NormalType soNormalType = NormalType.SPLINE_SPACE;
            localSpace = localSpaces[0];

            for (int i2 = 0; i2 < splineObjectCount; i2++)
            {
                if (i < localSpaceMap[i2])
                {
                    if (mirrorMap[i2])
                        vertice = -vertice;

                    localSpace = localSpaces[i2];
                    snapDataIndex = i2;

                    if(alignToEndMap.Length > 0)
                        alignToEnd = alignToEndMap[i2];

                    if (soNormalTypeMap.Length > 0)
                        soNormalType = soNormalTypeMap[i2];

                    if (skipTangentsMap.Length > 0)
                        skipTangents = skipTangentsMap[i2];

                    if (layerMaskMap.Length > 0)
                        layerMask = layerMaskMap[i2];

                    break;
                }
            }

            //Vertice to spline position
            vertice = math.transform(localSpace, vertice);

            //Snapping, needs to be before time.
            if (snapDatas.Length > 0 && snapDatas.Length > snapDataIndex && (snapDatas[snapDataIndex].end || snapDatas[snapDataIndex].start))
            {
                SnapData snapData = snapDatas[snapDataIndex];

                if (snapData.end && snapData.start)
                {
                    float point = snapData.soStartPoint + (snapData.soEndPoint - snapData.soStartPoint) / 2;

                    float partLength = vertice.z - point;
                    float length = math.abs(snapData.soEndPoint - snapData.soStartPoint) / 2;

                    if(partLength > 0) vertice.z = math.lerp(point, snapData.snapEndPoint, math.abs(partLength) / length);
                    else vertice.z = math.lerp(point, snapData.snapStartPoint, math.abs(partLength) / length);
                }
                else if (snapData.start)
                {
                    float partLength = math.abs(vertice.z - snapData.soEndPoint);
                    float length = math.abs(snapData.soStartPoint - snapData.soEndPoint);
                    vertice.z = math.lerp(snapData.soEndPoint, snapData.snapStartPoint, partLength / length);
                }
                else
                {
                    float partLength = math.abs(vertice.z - snapData.soStartPoint);
                    float length = math.abs(snapData.soEndPoint - snapData.soStartPoint);
                    vertice.z = math.lerp(snapData.soStartPoint, snapData.snapEndPoint, partLength / length);
                }
            }

            if (alignToEnd) vertice.z = splineLength - vertice.z;
            float time = vertice.z / splineLength;
            float fixedTime = SplineUtilityNative.TimeToFixedTime(distanceMap, splineResolution, time, loop);

            int segment = SplineUtility.GetSegmentIndex(nativeSegments.Length, fixedTime);
            float segmentTime = SplineUtility.GetSegmentTime(segment, nativeSegments.Length, fixedTime);
            float constratedSegementTime = SplineUtilityNative.GetContrastedSegmentTime(nativeSegments, segment, segmentTime);
            if (float.IsNaN(constratedSegementTime)) constratedSegementTime = 1;
            float3 splinePoint;

            //Is exstension
            if (!loop && (time >= 1 || time <= 0)) splinePoint = SplineUtilityNative.GetPositionExtended(nativeSegments, splineLength, time);
            else splinePoint = SplineUtilityNative.GetPositionFast(positionMapLocal, nativeSegments, splineResolution, fixedTime);

            // Get normal
            (float3, float3, float3) normal = SplineUtilityNative.GetNormal(segment, segmentTime, alignToEnd, nativeSegments, normals, normalsMap);
            float3 xDirection = normal.Item1;
            float3 yDirection = normal.Item2;
            float3 zDirection = normal.Item3;

            //Saddle skew
            float2 saddleSkew = SplineUtilityNative.GetSadleSkew(nativeSegments, segment, constratedSegementTime);
            bool saddleSkewIsEnabled = !GeneralUtility.IsZeroFast(saddleSkew, 0.001f);
            if (saddleSkewIsEnabled)
            {
                vertice.y += saddleSkew.y * (vertice.x * vertice.x);
                vertice.x += saddleSkew.x * (vertice.y * vertice.y) * -math.sign(vertice.x);
            }

            //Scale
            float2 scale = SplineUtilityNative.GetScale(nativeSegments, segment, constratedSegementTime);
            if (scaleCurveSegments.Length > 0)
            {
                float hermiteScale = HermiteUtility.Evaluate(scaleCurveSegments, fixedTime);
                hermiteScale = math.clamp(hermiteScale, 0f, 10f);
                scale *= hermiteScale;
            }
            bool scaleIsEnabled = !GeneralUtility.IsEqualFast(scale, new float2(1f, 1f), 0.001f);

            vertice.x *= scale.x;
            vertice.y *= scale.y;

            //NoiseLayer
            float noiseModification = 1;
            bool noiseIsEnabled = noises.Length > 0;
            if (noiseIsEnabled)
            {
                noiseModification = SplineUtilityNative.GetNoise(nativeSegments, segment, constratedSegementTime);
                vertice.y += NoiseUtility.GetNoiseValue(noises, vertice, noiseModification);
            }

            bool effectsEnabled = saddleSkewIsEnabled || scaleIsEnabled || noiseIsEnabled;

            float3x3 effectMatrix = float3x3.identity;
            if (effectsEnabled)
                effectMatrix = GeEffectsMatrix(saddleSkew, vertice, scale, noiseModification);

            //Rotation
            quaternion rotation = SplineUtilityNative.GetZRotation(nativeSegments, zDirection, fixedTime, constratedSegementTime);
            if (!alignToEnd) rotation = math.inverse(rotation);
            xDirection = math.mul(rotation, xDirection);
            yDirection = math.mul(rotation, yDirection);

            //Calculate the new world position
            vertice = splinePoint + xDirection * vertice.x + yDirection * vertice.y;

            //Create raycast command
            Vector3 direction = math.rotate(splineLocalToWorldMatrix, new float3(0, -1, 0));
            raycastCommands[i] = new RaycastCommand(math.transform(splineLocalToWorldMatrix, vertice), direction, new QueryParameters(layerMask), float.MaxValue);

            //To objects local space
            vertices[i] = math.transform(math.inverse(localSpace), vertice);

            // Normals
            if (meshNormals.Length > 0)
            {
                float3 meshNormal = meshNormals[i];

                if (soNormalType == NormalType.SPLINE_SPACE)
                {
                    float3x3 localSpaceMatrix = (float3x3)localSpace;
                    float3x3 worldToLocalMatrix = (float3x3)math.inverse(localSpace);

                    float3 worldNormal = math.mul(math.transpose(worldToLocalMatrix), meshNormal);
                    worldNormal = math.normalize(worldNormal);

                    if (effectsEnabled)
                    {
                        float3x3 em = math.transpose(math.inverse(effectMatrix));
                        worldNormal = math.mul(em, worldNormal);
                        worldNormal = math.normalize(worldNormal);
                    }

                    float3 newWorldNormal = xDirection * worldNormal.x +
                                            yDirection * worldNormal.y +
                                            zDirection * worldNormal.z;

                    float3 newLocalNormal = math.mul(math.transpose(localSpaceMatrix), newWorldNormal);
                    newLocalNormal = math.normalize(newLocalNormal);
                    meshNormals[i] = newLocalNormal;
                }
                else if (soNormalType == NormalType.CYLINDER_BASED)
                {
                    splinePoint = math.transform(math.inverse(localSpace), splinePoint);
                    float3 newLocalNormal = (float3)vertices[i] - splinePoint;
                    newLocalNormal = math.normalize(newLocalNormal);
                    meshNormals[i] = newLocalNormal;
                }

                // Tangent
                if (meshTangents.Length > 0 && !skipTangents)
                {
                    float3 tangent = new Vector3(meshTangents[i].x, meshTangents[i].y, meshTangents[i].z);
                    float3 worldTangent = math.mul((float3x3)localSpace, tangent);

                    if (effectsEnabled)
                        worldTangent = math.mul(effectMatrix, worldTangent);

                    float3 newWorldTangent = xDirection * worldTangent.x +
                                             yDirection * worldTangent.y +
                                             zDirection * worldTangent.z;

                    float3 newLocalTangent = math.mul((float3x3)math.inverse(localSpace), newWorldTangent);
                    newLocalTangent = newLocalTangent - (float3)meshNormals[i] * math.dot(newLocalTangent, meshNormals[i]);
                    newLocalTangent = math.normalize(newLocalTangent);

                    if (!math.all(math.isfinite(newLocalTangent)))
                        newLocalTangent = new float3(1, 0, 0);

                    meshTangents[i] = new Vector4(newLocalTangent.x, newLocalTangent.y, newLocalTangent.z, meshTangents[i].w);
                }
            }
        }

        private float3x3 GeEffectsMatrix(float2 saddleSkew, float3 vertice, float2 scale, float noiseModification)
        {
            float dyDx = 2f * saddleSkew.y * vertice.x;
            float dxDy = -2f * saddleSkew.x * math.sign(vertice.x) * (vertice.y / scale.y);

            float dxDx = 1f + dxDy * dyDx;

            float3x3 matrix1 = new float3x3(new float3(scale.x * dxDx, scale.y * dyDx, 0f), new float3(scale.x * dxDy, scale.y, 0f), new float3(0f, 0f, 1f));

            float epsilon = 0.01f;

            float noiseXMinus = NoiseUtility.GetNoiseValue(noises, vertice - new float3(epsilon, 0f, 0f), noiseModification);
            float noiseXPlus = NoiseUtility.GetNoiseValue(noises, vertice + new float3(epsilon, 0f, 0f), noiseModification);

            float noiseDx = (noiseXPlus - noiseXMinus) / (2f * epsilon);

            float noiseZMinus = NoiseUtility.GetNoiseValue(noises, vertice - new float3(0f, 0f, epsilon), noiseModification);
            float noiseZPlus = NoiseUtility.GetNoiseValue(noises, vertice + new float3(0f, 0f, epsilon), noiseModification);
            float noiseDz = (noiseZPlus - noiseZMinus) / (2f * epsilon);

            float3x3 matrix2 = new float3x3(new float3(1f, noiseDx, 0f), new float3(0f, 1f, 0f), new float3(0f, noiseDz, 1f));

            return math.mul(matrix2, matrix1);
        }
    }
}
