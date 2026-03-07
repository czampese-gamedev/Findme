#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace AC.Downloads.SceneGraphTool
{

	public class SceneGraphEditorWindow : EditorWindow
	{

		#region Variables

		private SceneGraphView sceneGraphView;
		private SceneGraph sceneGraph;

		#endregion


		#region Constructors

		public static void Init (SceneGraph _sceneGraph)
		{
			if (_sceneGraph == null)
			{
				return;
			}

			SceneGraphEditorWindow window = GetWindow<SceneGraphEditorWindow> ();
			window.titleContent = new GUIContent (_sceneGraph.name);
			window.position = new Rect (300, 200, 1100, 500);

			window.AssignGraph (_sceneGraph);
		}

		#endregion


		#region UnityStandards

		private void OnDisable ()
		{
			if (sceneGraphView != null)
			{
				rootVisualElement.Remove (sceneGraphView);
			}
		}

		#endregion


		#region PrivateFunctions

		private void AssignGraph (SceneGraph _sceneGraph)
		{
			sceneGraph = _sceneGraph;
			sceneGraphView = new SceneGraphView (_sceneGraph);
			sceneGraphView.StretchToParentSize ();
			rootVisualElement.Clear ();
			rootVisualElement.Add (sceneGraphView);
			
			GenerateToolbar ();
		}


		private void GenerateToolbar ()
		{
			Toolbar toolbar = new Toolbar ();

			UnityEngine.UIElements.Button gatherButton = new UnityEngine.UIElements.Button (() => { sceneGraph.GatherSceneExitData (); AssignGraph (sceneGraph); sceneGraphView.Refresh (); } );
			gatherButton.text = "Gather data";
			toolbar.Add (gatherButton);

			UnityEngine.UIElements.Button refreshButton = new UnityEngine.UIElements.Button (() => { sceneGraphView.Refresh (); } );
			refreshButton.text = "Refresh";
			toolbar.Add (refreshButton);

			UnityEngine.UIElements.Button saveButton = new UnityEngine.UIElements.Button (() => { sceneGraphView.Save (); } );
			saveButton.text = "Save";
			toolbar.Add (saveButton);

			rootVisualElement.Add (toolbar);
		}

		#endregion

	}

}

#endif