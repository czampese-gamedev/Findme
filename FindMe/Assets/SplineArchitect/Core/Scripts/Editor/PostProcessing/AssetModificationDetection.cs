// -----------------------------------------------------------------------------
// SplineArchitect
// Filename: AssetModificationDetection.cs
//
// Author: Mikael Danielsson
// Date Created: 23-03-2023
// (C) 2023 Mikael Danielsson. All rights reserved.
// -----------------------------------------------------------------------------

using System;
using System.Collections.Generic;

using UnityEditor;
using Object = UnityEngine.Object;

namespace SplineArchitect.PostProcessing
{
#if UNITY_6000_5_OR_NEWER
    [Unity.Scripting.LifecycleManagement.NoAutoStaticsCleanup]
#endif
    public class AssetModificationDetection : AssetPostprocessor
    {
        private static bool assembliesReloaded = true;
        private static bool invokedAssetImport = false;
        private static HashSet<Object> objects = new HashSet<Object>();
        private static string[] modelFileTypes = { ".fbx", ".dae", ".dxf", ".obj", ".blend" };

        public static void UpdateGlobal()
        {
            assembliesReloaded = false;
            invokedAssetImport = false;
        }

        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
        {
            if (assembliesReloaded)
            {
                assembliesReloaded = false;
                return;
            }

            if (importedAssets.Length > 0 && !invokedAssetImport)
            {
                invokedAssetImport = true;
                EHandleEvents.InvokeBeforeAssetImport();
            }

            objects.Clear();
            foreach (string assetPath in importedAssets)
            {
                Object obj = AssetDatabase.LoadMainAssetAtPath(assetPath);
                if (objects.Contains(obj)) continue;

                foreach(string fileType in modelFileTypes)
                {
                    if (assetPath.EndsWith(fileType, StringComparison.OrdinalIgnoreCase))
                    {
                        EHandleMeshContainer.Refresh();
                        break;
                    }
                }

                objects.Add(obj);
            }
        }
    }
}
