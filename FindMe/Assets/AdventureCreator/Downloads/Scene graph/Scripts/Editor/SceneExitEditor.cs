#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.Callbacks;

namespace AC.Downloads.SceneGraphTool
{

	[CustomEditor (typeof (SceneExit), true)]
	public class SceneExitEditor : Editor
	{

		public override void OnInspectorGUI ()
		{
			SceneExit _target = (SceneExit) target;
			_target.ShowGUI ();
		}

	}

}

#endif