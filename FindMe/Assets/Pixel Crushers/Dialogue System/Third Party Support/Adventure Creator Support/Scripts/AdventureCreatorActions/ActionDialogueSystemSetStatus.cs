using UnityEngine;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using PixelCrushers.DialogueSystem.AdventureCreatorSupport;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AC
{

    public enum DialogueDatabaseAssetType { Actor, Item, Location }

    public enum VariableLocationType { Global, Local }

    /// <summary>
    /// This custom Adventure Creator action runs the
    /// Lua function SetStatus().
    /// </summary>
    [System.Serializable]
    public class ActionDialogueSystemSetStatus : Action
    {

        public int constantID = 0;

        public DialogueDatabaseAssetType asset1Type = DialogueDatabaseAssetType.Actor;
        public int asset1ParameterID = -1;
        public bool asset1FromParameter = false;
        public string asset1 = string.Empty;

        public DialogueDatabaseAssetType asset2Type = DialogueDatabaseAssetType.Actor;
        public int asset2ParameterID = -1;
        public bool asset2FromParameter = false;
        public string asset2 = string.Empty;

        public float value;

        public override ActionCategory Category { get { return ActionCategory.ThirdParty; } }
        public override string Title { get { return "Dialogue System SetStatus"; } }
        public override string Description { get { return "Runs Dialogue System SetStatus()."; } }

        public ActionDialogueSystemSetStatus()
        {
            this.isDisplayed = true;
            category = ActionCategory.ThirdParty;
            title = "Dialogue System SetStatus";
            description = "Runs SetStatus() Lua function.";
        }

        override public float Run()
        {
            var code = $"return SetStatus({asset1Type}[\"{DialogueLua.StringToTableIndex(asset1)}\"], {asset2Type}[\"{DialogueLua.StringToTableIndex(asset2)}\"], {value})";
            Lua.Run(code, DialogueDebug.LogInfo);
            return 0;
        }

#if UNITY_EDITOR

        override public void ShowGUI(List<ActionParameter> parameters)
        {
            // Asset1:
            asset1Type = (DialogueDatabaseAssetType)EditorGUILayout.EnumPopup("Asset1 Type:", asset1Type);
            asset1FromParameter = EditorGUILayout.Toggle(new GUIContent("Asset1 is parameter?", "Tick to use a parameter value for Asset1."), asset1FromParameter);
            if (asset1FromParameter)
            {
                asset1ParameterID = Action.ChooseParameterGUI("Asset1:", parameters, asset1ParameterID, ParameterType.String);
            }
            else
            {
                asset1 = EditorGUILayout.TextField(new GUIContent("Asset1:", "The name of the " + asset1Type), asset1);
            }

            // Asset2:
            asset2Type = (DialogueDatabaseAssetType)EditorGUILayout.EnumPopup("Asset2 Type:", asset2Type);
            asset2FromParameter = EditorGUILayout.Toggle(new GUIContent("Asset2 is parameter?", "Tick to use a parameter value for Asset2."), asset2FromParameter);
            if (asset2FromParameter)
            {
                asset2ParameterID = Action.ChooseParameterGUI("Asset2:", parameters, asset2ParameterID, ParameterType.String);
            }
            else
            {
                asset2 = EditorGUILayout.TextField(new GUIContent("Asset2:", "The name of the " + asset2Type), asset2);
            }

            // Etc:
            value = EditorGUILayout.FloatField(new GUIContent("Value", "Status value to set"), value);

            AfterRunningOption();
        }

        public override string SetLabel()
        {
            string labelAdd = "";
            if (asset1FromParameter && asset2FromParameter)
            {
                labelAdd = $"SetStatus({asset1Type}[(parameter)], {asset2Type}[(parameter)], {value})";
            }
            else if (asset1FromParameter)
            {
                labelAdd = $"SetStatus({asset1Type}[(parameter)], {asset2Type}[{asset2}], {value})";
            }
            else if (asset1FromParameter)
            {
                labelAdd = $"SetStatus({asset1Type}[{asset1}], {asset2Type}[(parameter)], {value})";
            }
            else
            {
                labelAdd = $"SetStatus({asset1Type}[{asset1}], {asset2Type}[{asset2}], {value})";
            }
            return labelAdd;
        }

#endif

    }

}
