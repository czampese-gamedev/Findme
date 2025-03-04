using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace DecalSplines
{
    [Serializable]
    public static class RenderLayerManager   
    {
#if UNITY_EDITOR
        public static string[] GetRenderLayerNames()
        {
            return GraphicsSettings.defaultRenderPipeline.renderingLayerMaskNames;
        }

        public static uint DrawInspectorLayerGUI(uint renderLayerMask)
        {
            GUILayoutOption[] guiOptions = null;
            return (uint)EditorGUILayout.MaskField((int)renderLayerMask, GetRenderLayerNames(), guiOptions);
        }
#endif
    }
}