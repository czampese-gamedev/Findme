// -----------------------------------------------------------------------------
// SplineArchitect
// Filename: MeshContainer_Editor.cs
//
// Author: Mikael Danielsson
// Date Created: 13-01-2026
// (C) 2023 Mikael Danielsson. All rights reserved.
// -----------------------------------------------------------------------------

using System;

using UnityEngine;

using SplineArchitect.Utility;

namespace SplineArchitect
{
    public partial class MeshContainer
    {
#if UNITY_EDITOR
        public static bool disableVersionHashMsg;

        //Runtime data
        [NonSerialized]
        private int meshData = 0;
        [NonSerialized]
        private string monitorMeshKey = null;

        internal void UpdateCachedSceneName(string newSceneName)
        {
            cachedSceneName = newSceneName;
        }

        internal bool HasReadabilityDif()
        {
            if (originMesh.isReadable != GetInstanceMesh().isReadable)
                return true;

            return false;
        }

        internal bool TryUpdateVersionHash()
        {
            if (originMesh == null)
                return false;

            string path = EGeneralUtility.GetAssetPathOnlyEditor(originMesh);

            if (path == "")
            {
                Debug.LogError($"[Spline Architect] Can't set version hash! {originMesh.name} does not have an asset path!");
                return false;
            }

            Hash128 newHash = UnityEditor.AssetDatabase.GetAssetDependencyHash(path);

            if (!newHash.isValid)
                return false;

            // Create new version hash
            const ulong offset = 14695981039346656037UL;
            const ulong prime = 1099511628211UL;
            string value = newHash.ToString();
            ulong newVersionHash = offset;
            for (int i = 0; i < value.Length; i++)
            {
                newVersionHash ^= value[i];
                newVersionHash *= prime;
            }

            ulong oldVersionHash = versionHash;
            versionHash = newVersionHash;

            if (newVersionHash != oldVersionHash)
            {
                if (!Application.isPlaying && !disableVersionHashMsg)
                {
                    int messageId = unchecked((int)(newVersionHash ^ (newVersionHash >> 32)));
                    EActionDelayed.Add(() =>
                    {
                        if (originMesh == null || path == null)
                            return;

                        if (oldVersionHash != 0)
                        {
                            Debug.Log($"[Spline Architect] Mesh change detected for: {originMesh.name}\n\n" +
                                      $"Old hash: {oldVersionHash}\n" +
                                      $"New hash: {newVersionHash}\n" +
                                      $"Asset: {path}\n" +
                                      $"Info: This message appears when Spline Architect detects that a mesh may have changed externally, for example after being modified in software such as Blender. " +
                                      $"The mesh data will be updated to reflect the latest version of the asset. " +
                                      $"This can also happen when moving a project between Unity versions. " +
                                      $"In that case, the mesh itself may not have changed, but Unity's dependency hash has. This is expected and nothing to worry about.\n",
                                      originMesh);
                        }
                        else
                        {
                            Debug.Log($"[Spline Architect] Mesh version data upgraded for: {originMesh.name}\n\n" +
                                      $"Previous format: Legacy timestamp (Spline Architect 2.9.3 and earlier)\n" +
                                      $"New hash: {newVersionHash}\n" +
                                      $"Asset: {path}\n" +
                                      $"Info: Spline Architect now uses a dependency hash instead of file timestamps for mesh change detection. " +
                                      $"This is more reliable across different computers and version control systems. " +
                                      $"This upgrade only happens once. Save the scene to store the new mesh version data, otherwise this message will appear again.\n",
                                      originMesh);
                        }
                    }, 0, 0, EActionDelayed.ActionFlag.FRAMES | EActionDelayed.ActionFlag.LATE, messageId);
                }

                return true;
            }

            return false;
        }

        internal void EnsureValidOriginMesh()
        {
            string path = EGeneralUtility.GetAssetPathOnlyEditor(originMesh, true);

            if (path == "")
            {
                Mesh mesh = ESplineObjectUtility.GetOriginMeshFromMeshNameId(originMesh);
                originMesh = mesh;
            }
        }

        internal float GetDataUsage()
        {
            if (!string.IsNullOrEmpty(meshKey) && monitorMeshKey == GetMeshKey())
                return meshData;

            Mesh mesh = GetInstanceMesh();

            if (resolution > 0)
                mesh = HandleCachedResources.FetchModifiedOriginalMesh(this);

            if (mesh == null)
                return 0;

            meshData = 0;
            meshData += mesh.vertexCount * 12;
            meshData += mesh.triangles.Length * (mesh.indexFormat == UnityEngine.Rendering.IndexFormat.UInt16 ? 2 : 4);
            meshData += mesh.normals.Length * 12;
            meshData += mesh.tangents.Length * 16;
            meshData += mesh.uv.Length * 8;
            meshData += mesh.uv2.Length * 8;
            meshData += mesh.uv3.Length * 8;
            meshData += mesh.uv4.Length * 8;
            meshData += mesh.uv5.Length * 8;
            meshData += mesh.uv6.Length * 8;
            meshData += mesh.uv7.Length * 8;
            meshData += mesh.uv8.Length * 8;
            meshData += mesh.colors.Length * 16;
            meshData += mesh.name.Length * 2;
            meshData += 24;

            monitorMeshKey = GetMeshKey();

            return meshData;
        }
#endif
    }
}