#if UNITY_EDITOR

namespace AC.Downloads.SceneGraphTool
{

	public class SceneGraphNode : UnityEditor.Experimental.GraphView.Node
	{

		#region Variables

		private string sceneName;

		#endregion


		#region Constructors

		public SceneGraphNode (string _sceneName)
		{
			title = _sceneName;
			sceneName = _sceneName;
		}

		#endregion


		#region GetSet

		public string SceneName => sceneName;

		#endregion

	}

}

#endif
