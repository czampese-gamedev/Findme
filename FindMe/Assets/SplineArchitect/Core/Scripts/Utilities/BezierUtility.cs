// -----------------------------------------------------------------------------
// SplineArchitect
// Filename: BezierUtility.cs
//
// Author: Mikael Danielsson
// Date Created: 11-06-2023
// (C) 2023 Mikael Danielsson. All rights reserved.
// -----------------------------------------------------------------------------

using UnityEngine;
using Unity.Mathematics;

namespace SplineArchitect.Utility
{
    public static class BezierUtility
    {
        public static float3 Linear(float3 a, float3 b, float t)
        {
            float u = 1f - t;
            return a * u + b * t;
        }

        public static float3 Quadratic(float3 a, float3 at, float3 b, float t)
        {
            float u = 1f - t;
            float tt = t * t;
            float uu = u * u;
            return a * uu + at * (2f * u * t) + b * tt;
        }

        public static Vector3 Cubic(Vector3 a, Vector3 ata, Vector3 btb, Vector3 b, float t)
        {
            float ax = -a.x + 3f * ata.x - 3f * btb.x + b.x;
            float bx = 3f * a.x - 6f * ata.x + 3f * btb.x;
            float cx = -3f * a.x + 3f * ata.x;

            float ay = -a.y + 3f * ata.y - 3f * btb.y + b.y;
            float by = 3f * a.y - 6f * ata.y + 3f * btb.y;
            float cy = -3f * a.y + 3f * ata.y;

            float az = -a.z + 3f * ata.z - 3f * btb.z + b.z;
            float bz = 3f * a.z - 6f * ata.z + 3f * btb.z;
            float cz = -3f * a.z + 3f * ata.z;

            float x = ((ax * t + bx) * t + cx) * t + a.x;
            float y = ((ay * t + by) * t + cy) * t + a.y;
            float z = ((az * t + bz) * t + cz) * t + a.z;

            return new Vector3(x, y, z);
        }

        public static float3 Cubic(float3 a, float3 ata, float3 btb, float3 b, float t)
        {
            float u = 1f - t;
            float tt = t * t;
            float uu = u * u;
            float uuu = uu * u;
            float ttt = tt * t;

            return a * uuu
                   + ata * (3f * uu * t)
                   + btb * (3f * u * tt)
                   + b * ttt;
        }

        public static float3 GetDirection(float3 a, float3 ata, float3 btb, float3 b, float t)
        {
            float u = 1 - t;
            float3 tangent = 3 * u * u * (ata - a);
            tangent += 6 * u * t * (btb - ata);
            tangent += 3 * t * t * (b - btb);
            return math.normalizesafe(tangent);
        }
    }
}
