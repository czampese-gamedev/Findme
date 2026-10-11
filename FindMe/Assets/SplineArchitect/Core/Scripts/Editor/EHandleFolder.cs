// -----------------------------------------------------------------------------
// SplineArchitect
// Filename: EHandleMainFolder.cs
//
// Author: Mikael Danielsson
// Date Created: 23-11-2024
// (C) 2023 Mikael Danielsson. All rights reserved.
// -----------------------------------------------------------------------------

using System;
using System.IO;

using UnityEngine;
using UnityEditor;

using SplineArchitect.Ui;

namespace SplineArchitect
{
#if UNITY_6000_5_OR_NEWER
    [Unity.Scripting.LifecycleManagement.NoAutoStaticsCleanup]
#endif
    public class EHandleFolder
    {
        private const string coreFolderGuid = "3476e448dc177304eb9ac191871aeeb9";
        private const string defaultMainFolderPath = "Assets/SplineArchitect";
        internal static long tempFolderSize { get; private set; }
        internal static long harddiskSpaceLeft { get; private set; }

        private static string mainFolderPath;

        public static string GetMainFolderPath()
        {
            if (string.IsNullOrEmpty(mainFolderPath))
            {
                mainFolderPath = RetrieveMainFolderPathMethod1();

                if (string.IsNullOrEmpty(mainFolderPath))
                {
                    Debug.LogWarning($"[Spline Architect] Could not retrieve {WindowInfo.toolName} main folder! Trying retrieve folder path using method 2.");
                    mainFolderPath = RetrieveMainFolderPathMethod2();
                }

                if (string.IsNullOrEmpty(mainFolderPath))
                {
                    Debug.LogError($"[Spline Architect] Could not retrieve {WindowInfo.toolName} main folder! Using default asset path: {defaultMainFolderPath}");
                    mainFolderPath = defaultMainFolderPath;
                }
            }

            return mainFolderPath;
        }

        private static string RetrieveMainFolderPathMethod1()
        {
            string path = AssetDatabase.GUIDToAssetPath(coreFolderGuid);

            if (!string.IsNullOrEmpty(path) && AssetDatabase.IsValidFolder(path))
            {
                return path.Substring(0, path.Length - 5);
            }

            return path;
        }

        private static string RetrieveMainFolderPathMethod2()
        {
            string[] guids = AssetDatabase.FindAssets($"{typeof(Spline).Name} t:MonoScript");
            string f = null;

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                MonoScript script = AssetDatabase.LoadAssetAtPath<MonoScript>(path);
                if (script != null && script.GetClass() == typeof(Spline))
                {
                    f = path;
                }
                else if (path.Contains("SplineArchitect/Core/Scripts/Spline.cs"))
                {
                    f = path;
                }
            }

            if (f == null)
                return null;

            string[] split = f.Split('/');

            string f2 = "";

            for (int i = 0; i < split.Length - 3; i++)
            {
                if(i == split.Length - 4)
                    f2 += split[i];
                else
                    f2 += split[i] + "/";
            }

            return f2;
        }

        internal static void UpdateTempFolderSize()
        {
            string tempPath = $"{Application.dataPath.Substring(0, Application.dataPath.Length - 7)}/Temp";
            long size = 0;
            if (Directory.Exists(tempPath))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(tempPath);
                FileInfo[] files = dirInfo.GetFiles("d_*.bin", SearchOption.TopDirectoryOnly);
                foreach (FileInfo file in files)
                    size += file.Length;
            }

            tempFolderSize = size;
        }

        internal static void UpdateHarddiskSpaceLeft()
        {
            DriveInfo drive = new DriveInfo(Path.GetPathRoot(Environment.CurrentDirectory));
            if (drive.IsReady) harddiskSpaceLeft = drive.AvailableFreeSpace;
        }
    }
}