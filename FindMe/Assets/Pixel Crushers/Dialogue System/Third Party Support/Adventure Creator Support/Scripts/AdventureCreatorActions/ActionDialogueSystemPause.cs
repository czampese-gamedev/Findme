using UnityEngine;
using PixelCrushers.DialogueSystem;
using PixelCrushers;
using UnityEngine.EventSystems;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AC
{
    /// <summary>
    /// This custom Adventure Creator action pauses or unpauses the Dialogue System.
    /// </summary>
    [System.Serializable]
    public class ActionDialogueSystemPause : Action
    {
        public enum Mode { Pause, Unpause }

        public Mode mode;

        public bool deselect;

        protected static bool previousAutoFocus;

        public override ActionCategory Category { get { return ActionCategory.ThirdParty; } }
        public override string Title { get { return "Dialogue System Pause"; } }
        public override string Description { get { return "Pauses or unpauses the Dialogue System."; } }

        public ActionDialogueSystemPause()
        {
            this.isDisplayed = true;
            category = ActionCategory.ThirdParty;
            title = "Dialogue System Pause";
            description = "Pauses or unpauses the Dialogue System.";
        }

        override public float Run()
        {
            switch (mode)
            {
                case Mode.Pause:
                    previousAutoFocus = InputDeviceManager.instance.alwaysAutoFocus;
                    InputDeviceManager.instance.alwaysAutoFocus = false;
                    DialogueManager.Pause();
                    PixelCrushers.UIPanel.monitorSelection = false;
                    PixelCrushers.UIButtonKeyTrigger.monitorInput = false;
                    SetGraphicRaycaster(false);
                    if (deselect && EventSystem.current != null) EventSystem.current.SetSelectedGameObject(null);
                    break;
                case Mode.Unpause:
                    InputDeviceManager.instance.alwaysAutoFocus = previousAutoFocus;
                    DialogueManager.Unpause();
                    PixelCrushers.UIPanel.monitorSelection = true;
                    PixelCrushers.UIButtonKeyTrigger.monitorInput = true;
                    SetGraphicRaycaster(true);
                    break;
            }
            return 0;
        }

        protected void SetGraphicRaycaster(bool value)
        {
            if (DialogueManager.standardDialogueUI == null) return;
            var graphicRaycaster = DialogueManager.standardDialogueUI.GetComponentInParent<UnityEngine.UI.GraphicRaycaster>();
            if (graphicRaycaster == null) return;
            graphicRaycaster.enabled = value;
        }


#if UNITY_EDITOR

        override public void ShowGUI()
        {
            // Action-specific Inspector GUI code here
            mode = (Mode)EditorGUILayout.EnumPopup(new GUIContent("Mode:", "Pause or unpause the Dialogue System?"), mode);

            if (mode == Mode.Pause) deselect = EditorGUILayout.Toggle(new GUIContent("Deselect current?", "Deselect the currently-selected UI button."), deselect);
            AfterRunningOption();
        }

        public override string SetLabel()
        {
            return " (" + mode + ")";
        }

#endif

    }

}
