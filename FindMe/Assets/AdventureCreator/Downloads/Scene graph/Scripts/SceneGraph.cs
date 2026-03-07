using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AC.Downloads.SceneGraphTool
{

	[CreateAssetMenu (menuName = "Adventure Creator/Scene graph")]
	public class SceneGraph : ScriptableObject
	{

		#region Variables

		[SerializeField] private SceneGraphData sceneGraphData = new SceneGraphData ();

		#endregion


		#region PublicFunctions

		public static SceneGraph GetSceneGraph ()
		{
			Object[] sceneGraphsAssets = Resources.LoadAll (string.Empty, typeof (SceneGraph));
			if (sceneGraphsAssets != null && sceneGraphsAssets.Length > 0)
			{
				if (sceneGraphsAssets.Length == 1 || KickStarter.settingsManager == null)
				{
					return sceneGraphsAssets[0] as SceneGraph;
				}

				// Multiple, locate most appropriate
				string gameName = KickStarter.settingsManager.saveFileName.ToLower ();
				for (int i = 0; i < sceneGraphsAssets.Length; i++)
				{
					if (sceneGraphsAssets[i].name.ToLower ().Contains (gameName))
					{
						return sceneGraphsAssets[i] as SceneGraph;
					}
				}

				return sceneGraphsAssets[0] as SceneGraph;
			}
			return null;
		}

		
		#if UNITY_EDITOR

		public void ShowGUI ()
		{
			GUILayout.Label (sceneGraphData.SceneDatas.Length.ToString () + " scenes recorded");

			GUILayout.BeginHorizontal ();

			if (GUILayout.Button ("Gather data"))
			{
				GatherSceneExitData ();
			}

			if (GUILayout.Button ("Open graph"))
			{
				OpenGraphEditor ();
			}

			GUILayout.EndHorizontal ();
		}

		#endif

		#endregion


		#region PrivateFunctions

		#if UNITY_EDITOR

		[ContextMenu ("Gather scene exit data")]
		public void GatherSceneExitData ()
		{
			if (!UnityEditor.SceneManagement.EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo ())
			{
				return;
			}

			Undo.RecordObject (this, "Update scene graph");
			SceneData[] backupSceneDatas = sceneGraphData.SceneDatas;

			List<SceneData> sceneDataList = new List<SceneData> ();

			string originalScene = UnityVersionHandler.GetCurrentSceneFilepath (); 
			string[] sceneFiles = AdvGame.GetSceneFiles ();
			for (int j = 0; j < sceneFiles.Length; j++)
			{
				string sceneFile = sceneFiles[j];

				if (string.IsNullOrEmpty (sceneFile))
				{
					continue;
				}

				UnityVersionHandler.OpenScene (sceneFile);

				string sceneName = ExtractSceneName (sceneFile);

				#if UNITY_2022_3_OR_NEWER
				SceneExit[] exits = UnityEngine.Object.FindObjectsByType <SceneExit> (FindObjectsSortMode.None);
				#else
				SceneExit[] exits = UnityEngine.Object.FindObjectsOfType<SceneExit> ();
				#endif
				SceneExitNode[] sceneExitNodes = new SceneExitNode [exits.Length];
				
				for (int i = 0; i < exits.Length; i++)
				{
					SceneExit exit = exits[i];

					ConstantID constantID = exit.GetComponent<ConstantID> ();
					if (constantID == null)
					{
						constantID = exit.gameObject.AddComponent<ConstantID> ();
						EditorUtility.SetDirty (exit.gameObject);
						constantID.constantID = constantID.GetInstanceID ();
					}

					Vector3 transformPosition = exit.transform.position;

					SceneExitNode sceneExitNode = new SceneExitNode (exit.gameObject.name, constantID.constantID);

					// Got existing connectedSceneName?
					foreach (SceneData backupSceneData in backupSceneDatas)
					{
						if (backupSceneData.SceneName != sceneName)
						{
							continue;
						}

						foreach (SceneExitNode backupExitNode in backupSceneData.ExitNodes)
						{
							if (backupExitNode.ConstantID != constantID.constantID)
							{
								continue;
							}

							// Found match
							sceneExitNode.RestoreBackup (backupExitNode);
						}
					}

					sceneExitNodes[i] = sceneExitNode;
				}

				Vector2 defaultPosition = new Vector2 (20, 20 + (j * 50));
				SceneData thisSceneData = new SceneData (sceneName, defaultPosition, sceneExitNodes);

				// Got existing position?
				foreach (SceneData backupSceneData in backupSceneDatas)
				{
					if (backupSceneData.SceneName != sceneName)
					{
						continue;
					}

					thisSceneData.UpdatePosition (backupSceneData.NodePosition);
				}

				sceneDataList.Add (thisSceneData);

				UnityVersionHandler.SaveScene ();
				EditorUtility.SetDirty (this);
			}

			sceneGraphData = new SceneGraphData (sceneDataList.ToArray ());

			UnityVersionHandler.OpenScene (originalScene);

			OpenGraphEditor ();
		}


		[ContextMenu ("Open graph editor")]
		private void OpenGraphEditor ()
		{
			SceneGraphEditorWindow.Init (this);
		}


		private string ExtractSceneName (string sceneFile)
		{
			int lastSlashIndex = sceneFile.LastIndexOf ("/"[0]);
			string sceneNamePlusExtension = sceneFile.Substring (lastSlashIndex + 1);
			int extensionIndex = sceneNamePlusExtension.LastIndexOf ("."[0]);
			string sceneName = sceneNamePlusExtension.Substring (0, extensionIndex);
			return sceneName;
		}

		#endif

		#endregion


		#region GetSet

		public SceneGraphData GraphData => sceneGraphData;

		#endregion

	}

}