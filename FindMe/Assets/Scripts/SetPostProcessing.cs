using UnityEngine;
using System.Collections.Generic;
using static UnityEngine.LowLevelPhysics2D.PhysicsLayers;
using AC;
using UnityEngine.Rendering.Universal;



#if UNITY_EDITOR
using UnityEditor;
#endif


    [System.Serializable]
    public class SetPostProcessing : Action
    {

        public override ActionCategory Category { get { return ActionCategory.Camera; } }
        public override string Title { get { return "Set Post Processing"; } }
        public override string Description { get { return "Set Post Processing Layer Mask on Main Camera."; } }

        public string PostProcessingLayer;
  

        public override float Run()
        {
   
        if (!string.IsNullOrEmpty(PostProcessingLayer))
            {
            //DO STUFF HERE
            Camera unityCamera = AC.KickStarter.mainCamera.GetComponent<Camera>();
            unityCamera.GetUniversalAdditionalCameraData().volumeLayerMask = LayerMask.GetMask(PostProcessingLayer);
        }
            return 0f;
        }


#if UNITY_EDITOR

        public override void ShowGUI()
        {
        PostProcessingLayer = EditorGUILayout.TextField("Post Processing Layer:", PostProcessingLayer);

        }

#endif
    }
