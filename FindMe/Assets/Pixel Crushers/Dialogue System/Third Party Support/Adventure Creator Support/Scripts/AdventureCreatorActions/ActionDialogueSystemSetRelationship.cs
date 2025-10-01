using UnityEngine;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using PixelCrushers.DialogueSystem.AdventureCreatorSupport;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AC
{

    public enum LuaFunctionSetMode { Set, Increment, Decrement }

    /// <summary>
    /// This custom Adventure Creator action runs the
    /// Lua function SetStatus().
    /// </summary>
    [System.Serializable]
    public class ActionDialogueSystemSetRelationship : Action
    {

        public int constantID = 0;

        public int actor1ParameterID = -1;
        public bool actor1FromParameter = false;
        public string actor1 = string.Empty;

        public int actor2ParameterID = -1;
        public bool actor2FromParameter = false;
        public string actor2 = string.Empty;

        public int relationshipParameterID = -1;
        public bool relationshipFromParameter = false;
        public string relationship = string.Empty;

        public LuaFunctionSetMode mode = LuaFunctionSetMode.Set;
        public float value;

        public override ActionCategory Category { get { return ActionCategory.ThirdParty; } }
        public override string Title { get { return "Dialogue System SetRelationship"; } }
        public override string Description { get { return "Runs Dialogue System SetRelationship()."; } }

        public ActionDialogueSystemSetRelationship()
        {
            this.isDisplayed = true;
            category = ActionCategory.ThirdParty;
            title = "Dialogue System SetRelationship";
            description = "Runs SetRelationship() Lua function.";
        }

        override public float Run()
        {
            var functionName = GetFunctionName();
            var relationshipString = relationship.Replace("\"", "");
            var code = $"return {functionName}(Actor[\"{DialogueLua.StringToTableIndex(actor1)}\"], Actor[\"{DialogueLua.StringToTableIndex(actor2)}\"], \"{relationshipString}\", {value})";
            Lua.Run(code, DialogueDebug.LogInfo);
            return 0;
        }

        protected string GetFunctionName()
        { 
            switch (mode)
            {
                case LuaFunctionSetMode.Increment:
                    return "IncRelationship";
                case LuaFunctionSetMode.Decrement:
                    return "DecRelationship";
                default:
                    return "SetRelationship";
            }
        }

#if UNITY_EDITOR

        override public void ShowGUI(List<ActionParameter> parameters)
        {
            // Actor1:
            actor1FromParameter = EditorGUILayout.Toggle(new GUIContent("Actor1 is parameter?", "Tick to use a parameter value for Actor1."), actor1FromParameter);
            if (actor1FromParameter)
            {
                actor1ParameterID = Action.ChooseParameterGUI("Actor1:", parameters, actor1ParameterID, ParameterType.String);
            }
            else
            {
                actor1 = EditorGUILayout.TextField(new GUIContent("Actor1:", "The name of the actor"), actor1);
            }

            // Actor2:
            actor2FromParameter = EditorGUILayout.Toggle(new GUIContent("Actor2 is parameter?", "Tick to use a parameter value for Actor2."), actor2FromParameter);
            if (actor2FromParameter)
            {
                actor2ParameterID = Action.ChooseParameterGUI("Actor2:", parameters, actor2ParameterID, ParameterType.String);
            }
            else
            {
                actor2 = EditorGUILayout.TextField(new GUIContent("Actor2:", "The name of the actor"), actor2);
            }

            // Relationship:
            relationshipFromParameter = EditorGUILayout.Toggle(new GUIContent("Relationship type is parameter?", "Tick to use a parameter value for relationship type."), relationshipFromParameter);
            if (relationshipFromParameter)
            {
                relationshipParameterID = Action.ChooseParameterGUI("Relationship type:", parameters, relationshipParameterID, ParameterType.String);
            }
            else
            {
                relationship = EditorGUILayout.TextField(new GUIContent("Relationship type:", "The name of the relationship type (your choice of string)"), relationship);
            }

            // Etc:
            mode = (LuaFunctionSetMode)EditorGUILayout.EnumPopup("Mode", mode);
            value = EditorGUILayout.FloatField(new GUIContent("Value", "Relationship value"), value);

            AfterRunningOption();
        }

        public override string SetLabel()
        {
            string labelAdd = "";
            var functionName = GetFunctionName();
            if (actor1FromParameter && actor2FromParameter)
            {
                labelAdd = $"{functionName}(Actor[(parameter)], Actor[(parameter)], {mode}, {value})";
            }
            else if (actor1FromParameter)
            {
                labelAdd = $"{functionName}(Actor[(parameter)], Actor[{actor2}], {mode}, {value})";
            }
            else if (actor1FromParameter)
            {
                labelAdd = $"{functionName}(Actor[{actor1}], Actor[(parameter)], {mode}, {value})";
            }
            else
            {
                labelAdd = $"{functionName}(Actor[{actor1}], Actor[{actor2}], {mode}, {value})";
            }
            return labelAdd;
        }

#endif

    }

}
