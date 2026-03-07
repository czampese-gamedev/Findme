using System;
using UnityEngine;

namespace AC.Downloads.SceneGraphTool
{

	[Serializable]
	public class SceneData
	{

		#region Variables

		[SerializeField] private string sceneName;
		[SerializeField] private Vector2 nodePosition;
		[SerializeField] private SceneExitNode[] exitNodes = new SceneExitNode[0];

		#endregion


		#region Constructors

		public SceneData (string _sceneName, Vector2 _nodePosition, SceneExitNode[] _exitNodes)
		{
			sceneName = _sceneName;
			nodePosition = _nodePosition;
			exitNodes = _exitNodes;
		}

		#endregion


		#region PublicFunctions

		public void UpdatePosition (Vector2 _nodePosition)
		{
			nodePosition = _nodePosition;
		}

		#endregion


		#region GetSet

		public string SceneName { get { return sceneName; } }
		public Vector2 NodePosition { get { return nodePosition; } }
		public SceneExitNode[] ExitNodes { get { return exitNodes; } }

		#endregion

	}

}