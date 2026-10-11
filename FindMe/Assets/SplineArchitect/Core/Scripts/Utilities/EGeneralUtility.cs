// -----------------------------------------------------------------------------
// SplineArchitect
// Filename: EGeneralUtility.cs
//
// Author: Mikael Danielsson
// Date Created: 26-04-2025
// (C) 2023 Mikael Danielsson. All rights reserved.
// -----------------------------------------------------------------------------

#if UNITY_EDITOR
using System.IO;

using UnityEngine;
using UnityEditor;

namespace SplineArchitect.Utility
{
    internal class EGeneralUtility
    {
        internal static UnityEngine.Object GetObjectFromSessionState(string key)
        {
#if UNITY_6000_5_OR_NEWER
            EntityId entityId = SessionState.GetEntityId(key, EntityId.None);

            if (entityId == EntityId.None)
                return null;

            return EditorUtility.EntityIdToObject(entityId);
#else
            int instanceId = SessionState.GetInt(key, 0);

            if (instanceId == 0)
                return null;

    #if UNITY_6000_3_OR_NEWER
        #pragma warning disable CS0618
            return EditorUtility.EntityIdToObject(instanceId);
        #pragma warning restore CS0618
    #else
            return EditorUtility.InstanceIDToObject(instanceId);
    #endif
#endif
        }

        internal static void SetObjectSessionKey(string key, UnityEngine.Object obj)
        {
#if UNITY_6000_5_OR_NEWER
            SessionState.SetEntityId(key, obj.GetEntityId());
#else
    #pragma warning disable CS0618
            SessionState.SetInt(key, obj.GetInstanceID());
    #pragma warning restore CS0618
#endif
        }

        internal static string GetObjectSessionKey(string prefix, UnityEngine.Object obj)
        {
#if UNITY_6000_5_OR_NEWER
            return $"{prefix}{EntityId.ToULong(obj.GetEntityId())}";
#else
    #pragma warning disable CS0618
            return $"{prefix}{obj.GetInstanceID()}";
    #pragma warning restore CS0618
#endif
        }

        internal static string GetTempFolderPath()
        {
            return Application.dataPath.Substring(0, Application.dataPath.Length - "Asset/".Length) + "Temp";
        }

        internal static string GetAssetPathOnlyEditor(Object o, bool full = false)
        {
            string path = UnityEditor.AssetDatabase.GetAssetPath(o);
            if (path.Length > 0 && full)
            {
                path = path.Substring("Assets/".Length);
                path = Path.Combine(Application.dataPath, path);
            }

            return path.Replace("\\", "/");
        }
    }
}
#endif