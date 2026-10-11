// -----------------------------------------------------------------------------
// SplineArchitect
// Filename: SplineObjectUndoState_ObjectCloning.cs
//
// Author: Mikael Danielsson
// Date Created: 28-07-2026
// (C) 2023 Mikael Danielsson. All rights reserved.
// -----------------------------------------------------------------------------

#if UNITY_EDITOR

using System;
using System.Collections.Generic;

using UnityEngine;

namespace SplineArchitect.ScriptableObjects
{
    internal partial class SplineObjectUndoState : ScriptableObject
    {
        [SerializeField] internal int cloneVersion;
        [NonSerialized] internal int oldCloneVersion;
        [NonSerialized] internal bool updateFitting;

#if UNITY_6000_4_OR_NEWER
        [HideInInspector, SerializeField] internal List<ulong> cloneIds;
        [HideInInspector, SerializeField] internal List<ulong> originCloneIds;
#else
        [HideInInspector, SerializeField] internal List<int> cloneIds;
        [HideInInspector, SerializeField] internal List<int> originCloneIds;
#endif
    }
}

#endif
