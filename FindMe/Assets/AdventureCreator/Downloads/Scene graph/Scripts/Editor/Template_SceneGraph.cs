#if UNITY_EDITOR

using System;
using UnityEngine;
using UnityEditor;

namespace AC.Downloads.SceneGraphTool
{
	
	public class SceneGraphTemplate : Template
	{

		#region Variables

		[SerializeField] private SceneExit sceneExit3DPrefab = null;
		[SerializeField] private SceneExit sceneExit2DPrefab = null;
		[SerializeField] private Texture2D playerStartIcon = null;

		#endregion


		#region PublicFunctions

		public override bool CanInstall(ref string errorText)
		{
			if (sceneExit3DPrefab == null || sceneExit2DPrefab == null)
			{
				errorText = "No Scene Exit prefab assigned";
				return false;
			}

			if (KickStarter.sceneManager == null)
			{
				errorText = "No Scene Manager assigned";
				return false;
			}

			return true;
		}

		#endregion


		#region ProtectedFunctions

		protected override void MakeChanges(string installPath, bool canDeleteOldAssets, System.Action onComplete, System.Action<string> onFail)
		{
			Undo.RecordObjects(new UnityEngine.Object[] { KickStarter.settingsManager, KickStarter.sceneManager, KickStarter.menuManager }, "");

			// Prefabs
			SceneExit newSceneExit2DPrefab = CopyAsset<SceneExit>(installPath, sceneExit2DPrefab, ".prefab");
			if (newSceneExit2DPrefab == null)
			{
				onFail.Invoke("Prefab copy failed.");
				return;
			}

			SceneExit newSceneExit3DPrefab = CopyAsset<SceneExit>(installPath, sceneExit3DPrefab, ".prefab");
			if (newSceneExit3DPrefab == null)
			{
				onFail.Invoke("Prefab copy failed.");
				return;
			}

			// Scene
			if (SceneSettings.IsUnity2D())
			{
				KickStarter.sceneManager.AddPrefab(new SceneManagerPrefabData("Navigation", "Scene Exit", "An exit referenced by the Scene Graph.", playerStartIcon, newSceneExit2DPrefab.gameObject));
			}
			else
			{
				KickStarter.sceneManager.AddPrefab(new SceneManagerPrefabData("Navigation", "Scene Exit", "An exit referenced by the Scene Graph.", playerStartIcon, newSceneExit3DPrefab.gameObject));
			}

			onComplete.Invoke();
		}

		#endregion


		#region GetSet

		public override string Label { get { return "Scene Graph"; } }
		public override string PreviewText { get { return "Adds the ability to connect scenes using a graph."; } }
		public override Type[] AffectedManagerTypes { get { return new Type[] { typeof(SceneManager) }; } }
		public override bool RequiresInstallPath { get { return true; } }
		public override string FolderName { get { return "SceneGraph"; } }

		#endregion

	}


	[CustomEditor (typeof (SceneGraphTemplate))]
	public class SceneGraphTemplateEditor : TemplateEditor
	{}

}

#endif