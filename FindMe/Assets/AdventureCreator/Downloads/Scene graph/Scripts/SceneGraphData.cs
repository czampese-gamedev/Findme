using UnityEngine;

namespace AC.Downloads.SceneGraphTool
{

	[System.Serializable]
	public class SceneGraphData
	{

		#region Variables

		[SerializeField] private SceneData[] sceneDatas = new SceneData[0];

		#endregion


		#region Constructors
		
		public SceneGraphData ()
		{
			sceneDatas = new SceneData[0];
		}


		public SceneGraphData (SceneData[] _sceneDatas)
		{
			sceneDatas = _sceneDatas;
		}

		#endregion


		#region PublicFunctions

		public SceneExitNode GetSceneExitNode (SceneExit sceneExit)
		{
			ConstantID constantID = sceneExit.GetComponent<ConstantID> ();
			if (constantID == null)
			{
				ACDebug.LogWarning ("Cannot find corresponding scene for Scene Exit " + sceneExit + ", as it has no ConstantID value", sceneExit);
				return null;
			}

			string startingSceneName = sceneExit.gameObject.scene.name;
			foreach (SceneData sceneData in sceneDatas)
			{
				if (sceneData.SceneName != startingSceneName)
				{
					continue;
				}

				foreach (SceneExitNode exitNode in sceneData.ExitNodes)
				{
					if (exitNode.ConstantID != constantID.constantID)
					{
						continue;
					}

					return exitNode;
				}

				return null;
			}

			return null;
		}

		#endregion


		#region GetSet

		public SceneData[] SceneDatas => sceneDatas;

		#endregion

	}

}