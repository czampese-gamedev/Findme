// -----------------------------------------------------------------------------
// SplineArchitect
// Filename: SplineChange.cs
//
// Author: Mikael Danielsson
// Date Created: 21-08-2026
// (C) 2023 Mikael Danielsson. All rights reserved.
// -----------------------------------------------------------------------------

namespace SplineArchitect
{
    public enum SplineChange : byte
    {
        SEGMENT_TO_SURFACE,
        SEGMENT_TO_PLANE,
        SEGMENT_LINKED,
        SEGMENT_ALIGNED,
        TANGENT_ALIGNED,
        SPLINE_REVERSED,
        SPLINE_LOOP,
        SPLINE_JOIN,
        CUSTOM,
    }
}
