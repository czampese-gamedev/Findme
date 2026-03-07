#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.Callbacks;

namespace AC.Downloads.SceneGraphTool
{

	[CustomEditor (typeof (SceneGraph), true)]
	public class SceneGraphEditor : Editor
	{

		public override void OnInspectorGUI ()
		{
			SceneGraph _target = (SceneGraph) target;
			_target.ShowGUI ();
		}


		[OnOpenAssetAttribute(10)]
		public static bool OnOpenAsset (int instanceID, int line)
		{
			if (Selection.activeObject is SceneGraph && instanceID == Selection.activeInstanceID)
			{
				SceneGraph sceneGraph = (SceneGraph) Selection.activeObject as SceneGraph;
				SceneGraphEditorWindow.Init (sceneGraph);
				return true;
			}
			return false;
		}

	}

}

#endif