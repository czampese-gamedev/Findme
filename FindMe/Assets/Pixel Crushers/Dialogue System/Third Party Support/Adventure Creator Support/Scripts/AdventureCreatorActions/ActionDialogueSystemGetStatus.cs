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
	/// Lua function GetStatus(Asset1, Asset2).
	/// </summary>
	[System.Serializable]
	public class ActionDialogueSystemGetStatus : Action
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

		public int variableParameterID = -1;
		public int variableID;
		public VariableLocationType variableLocationType;

		protected GVar runtimeVariable;
		protected LocalVariables localVariables;
		protected VariableType varType = VariableType.Boolean;

		public override ActionCategory Category { get { return ActionCategory.ThirdParty; } }
		public override string Title { get { return "Dialogue System GetStatus"; } }
		public override string Description { get { return "Runs GetStatus() Lua function."; } }

		public ActionDialogueSystemGetStatus()
		{
			this.isDisplayed = true;
			category = ActionCategory.ThirdParty;
			title = "Dialogue System GetStatus";
			description = "Runs GetStatus() Lua function.";
		}

		override public void AssignValues(List<ActionParameter> parameters)
		{
			if (asset1FromParameter) asset1 = AssignString(parameters, asset1ParameterID, asset1);
			if (asset2FromParameter) asset2 = AssignString(parameters, asset2ParameterID, asset2);

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
			var code = $"return GetStatus({asset1Type}[\"{DialogueLua.StringToTableIndex(asset1)}\"], {asset2Type}[\"{DialogueLua.StringToTableIndex(asset2)}\"])";
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
			if (asset1FromParameter && asset2FromParameter)
			{
				labelAdd = $"GetStatus({asset1Type}[(parameter)], {asset2Type}[(parameter)])";
			}
			else if (asset1FromParameter)
			{
				labelAdd = $"GetStatus({asset1Type}[(parameter)], {asset2Type}[{asset2}])";
			}
			else if (asset1FromParameter)
			{
				labelAdd = $"GetStatus({asset1Type}[{asset1}], {asset2Type}[(parameter)])";
			}
			else
			{
				labelAdd = $"GetStatus({asset1Type}[{asset1}], {asset2Type}[{asset2}])";
			}
			return labelAdd;
		}

#endif

	}

}