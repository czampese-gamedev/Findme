using UnityEngine;
using System.Collections.Generic;
using PixelCrushers.DialogueSystem;
using PixelCrushers.DialogueSystem.AdventureCreatorSupport;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AC
{

    /// <summary>
    /// This custom Adventure Creator action runs the 
    /// Lua function GetRelationship(Actor1, Actor2, Relationship).
    /// </summary>
    [System.Serializable]
    public class ActionDialogueSystemGetRelationship : Action
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

        public int variableParameterID = -1;
        public int variableID;
        public VariableLocationType variableLocationType;

        protected GVar runtimeVariable;
        protected LocalVariables localVariables;
        protected VariableType varType = VariableType.Boolean;

        public override ActionCategory Category { get { return ActionCategory.ThirdParty; } }
        public override string Title { get { return "Dialogue System GetRelationship"; } }
        public override string Description { get { return "Runs GetRelationship() Lua function."; } }

        public ActionDialogueSystemGetRelationship()
        {
            this.isDisplayed = true;
            category = ActionCategory.ThirdParty;
            title = "Dialogue System GetRelationship";
            description = "Runs GetRelationship() Lua function.";
        }

        override public void AssignValues(List<ActionParameter> parameters)
        {
            if (actor1FromParameter) actor1 = AssignString(parameters, actor1ParameterID, actor1);
            if (actor2FromParameter) actor2 = AssignString(parameters, actor2ParameterID, actor2);
            if (relationshipFromParameter) relationship = AssignString(parameters, relationshipParameterID, relationship);

            switch (variableLocationType)
            {
                case VariableLocationType.Global:
                    variableID = AssignVariableID(parameters, variableParameterID, variableID);
                    runtimeVariable = AC.GlobalVariables.GetVariable(variableID);
                    break;

                case VariableLocationType.Local:
                    if (!isAssetFile)
                    {
                        variableID = AssignVariableID(parameters, variableParameterID, variableID);
                        runtimeVariable = LocalVariables.GetVariable(variableID, localVariables);
                    }
                    break;
            }
        }

        public override void AssignParentList(ActionList actionList)
        {
            if (actionList != null)
            {
                localVariables = UnityVersionHandler.GetLocalVariablesOfGameObject(actionList.gameObject);
            }
            if (localVariables == null)
            {
                localVariables = KickStarter.localVariables;
            }

            base.AssignParentList(actionList);
        }

        override public float Run()
        {
            var code = $"return GetRelationship(Actor[\"{DialogueLua.StringToTableIndex(actor1)}\"], Actor[\"{DialogueLua.StringToTableIndex(actor2)}\"], \"{relationship}\")";
            var luaResult = Lua.Run(code, DialogueDebug.LogInfo);
            switch (variableLocationType)
            {
                case VariableLocationType.Global:
                    AC.GlobalVariables.SetFloatValue(variableID, luaResult.asFloat);
                    break;
                case VariableLocationType.Local:
                    LocalVariables.SetFloatValue(variableID, luaResult.asFloat);
                    break;
            }
            return 0;
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

            // Variable:
            variableLocationType = (VariableLocationType)EditorGUILayout.EnumPopup("Variable type:", variableLocationType);
            switch (variableLocationType)
            {
                case VariableLocationType.Global:
                    if (AdvGame.GetReferences().variablesManager != null)
                    {
                        variableID = ShowVarGUI(AdvGame.GetReferences().variablesManager.vars, parameters, ParameterType.GlobalVariable, variableID, variableParameterID);
                    }
                    break;

                case VariableLocationType.Local:
                    if (isAssetFile)
                    {
                        EditorGUILayout.HelpBox("Local variables cannot be accessed in ActionList assets.", MessageType.Info);
                    }
                    else if (localVariables != null)
                    {
                        variableID = ShowVarGUI(localVariables.localVars, parameters, ParameterType.LocalVariable, variableID, variableParameterID);
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("No 'Local Variables' component found in the scene. Please add an AC GameEngine object from the Scene Manager.", MessageType.Info);
                    }
                    break;

            }

            AfterRunningOption();
        }

        private int ShowVarGUI(List<GVar> vars, List<ActionParameter> parameters, ParameterType parameterType, int variableID, int parameterID)
        {
            // Create a string List of the field's names (for the PopUp box)
            List<string> labelList = new List<string>();

            int i = 0;
            int variableNumber = -1;

            if (vars.Count > 0)
            {
                foreach (GVar _var in vars)
                {
                    labelList.Add(_var.label);

                    // If a GlobalVar variable has been removed, make sure selected variable is still valid
                    if (_var.id == variableID)
                    {
                        variableNumber = i;
                    }

                    i++;
                }

                if (variableNumber == -1 && (parameters == null || parameters.Count == 0 || parameterID == -1))
                {
                    // Wasn't found (variable was deleted?), so revert to zero
                    if (variableID > 0) LogWarning("Previously chosen variable no longer exists!");
                    variableNumber = 0;
                    variableID = 0;
                }

                string label = "Variable:";

                parameterID = Action.ChooseParameterGUI(label, parameters, parameterID, parameterType);
                if (parameterID >= 0)
                {
                    //variableNumber = 0;
                    variableNumber = Mathf.Min(variableNumber, vars.Count - 1);
                    variableID = -1;
                }
                else
                {
                    variableNumber = EditorGUILayout.Popup(label, variableNumber, labelList.ToArray());
                    variableNumber = Mathf.Max(0, variableNumber);
                    variableID = vars[variableNumber].id;
                }
            }
            else
            {
                EditorGUILayout.HelpBox("No variables exist!", MessageType.Info);
                variableID = -1;
                variableNumber = -1;
            }

            variableParameterID = parameterID;

            if (variableNumber >= 0)
            {
                varType = vars[variableNumber].type;
            }

            return variableID;
        }

        public override string SetLabel()
        {
            // Return a string used to describe the specific action's job.
            string labelAdd = "";
            if (actor1FromParameter && actor2FromParameter)
            {
                labelAdd = $"GetRelationship(Actor[(parameter)], Actor[(parameter)])";
            }
            else if (actor1FromParameter)
            {
                labelAdd = $"GetRelationship(Actor[(parameter)], Actor[{actor2}])";
            }
            else if (actor1FromParameter)
            {
                labelAdd = $"GetRelationship(Actor[{actor1}], Actor[(parameter)])";
            }
            else
            {
                labelAdd = $"GetRelationship(Actor[{actor1}], Actor[{actor2}])";
            }
            return labelAdd;
        }

#endif

    }

}