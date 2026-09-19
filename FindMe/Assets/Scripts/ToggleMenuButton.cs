using UnityEngine;
using System.Collections.Generic;
using static UnityEngine.LowLevelPhysics2D.PhysicsLayers;
using AC;


#if UNITY_EDITOR
using UnityEditor;
#endif


    [System.Serializable]
    public class ToggleMenuButton : Action
    {

        public override ActionCategory Category { get { return ActionCategory.Object; } }
        public override string Title { get { return "Toggle Menu Button"; } }
        public override string Description { get { return "Toggle a menu button."; } }

        public string menuName;
        public string buttonName;

        public override float Run()
        {
   
        if (!string.IsNullOrEmpty(menuName) && !string.IsNullOrEmpty(buttonName))
            {    
            MenuElement button = PlayerMenus.GetElementWithName(menuName, buttonName);
            CanvasGroup canvasGroup = button.GetObjectToSelect().transform.GetComponent<CanvasGroup>();


            canvasGroup.interactable = !canvasGroup.interactable;
            canvasGroup.blocksRaycasts = !canvasGroup.blocksRaycasts;
        }
            return 0f;
        }


#if UNITY_EDITOR

        public override void ShowGUI()
        {
            menuName = EditorGUILayout.TextField("menuName:", menuName);
            buttonName = EditorGUILayout.TextField("buttonName:", buttonName);
        }

#endif
    }
