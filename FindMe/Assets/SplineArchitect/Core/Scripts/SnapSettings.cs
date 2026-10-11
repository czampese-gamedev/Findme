// -----------------------------------------------------------------------------
// SplineArchitect
// Filename: SnapSettings.cs
//
// Author: Mikael Danielsson
// Date Created: 02-02-2026
// (C) 2023 Mikael Danielsson. All rights reserved.
// -----------------------------------------------------------------------------

using SplineArchitect.Utility;
using System;

using UnityEngine;

namespace SplineArchitect
{
    [Serializable]
    public struct SnapSettings
    {
        public float startSnapDistance;
        public float endSnapDistance;
        public float startSnapOffset;
        public float endSnapOffset;
        public float snapTargetPoint;
        public SnapMode snapMode;

        public SnapSettings(SnapMode snapMode)
        {
            startSnapDistance = 1f;
            endSnapDistance = 1f;
            startSnapOffset = 0f;
            endSnapOffset = 0f;
            snapTargetPoint = 0f;
            this.snapMode = snapMode;
        }

        public bool Equals(SnapSettings other)
        {
            return GeneralUtility.IsEqual(startSnapDistance, other.startSnapDistance) &&
                   GeneralUtility.IsEqual(endSnapDistance, other.endSnapDistance) &&
                   GeneralUtility.IsEqual(startSnapOffset, other.startSnapOffset) &&
                   GeneralUtility.IsEqual(endSnapOffset, other.endSnapOffset) &&
                   GeneralUtility.IsEqual(snapTargetPoint, other.snapTargetPoint) &&
                   snapMode == other.snapMode;
        }

        public override bool Equals(object obj)
        {
            return obj is SnapSettings other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(startSnapDistance, endSnapDistance, startSnapOffset, endSnapOffset, snapTargetPoint, snapMode);
        }

        public static bool operator ==(SnapSettings left, SnapSettings right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(SnapSettings left, SnapSettings right)
        {
            return !left.Equals(right);
        }
    }
}