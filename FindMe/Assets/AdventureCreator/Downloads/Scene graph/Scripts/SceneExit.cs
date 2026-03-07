using UnityEngine;

namespace AC.Downloads.SceneGraphTool
{

	public class SceneExit : PlayerStart
	{

		#region Variables

		[SerializeField] private Hotspot autoHotspot = null;
		private static int connectedConstantID;

		#endregion


		#region UnityStandards

		private void OnEnable ()
		{
			chooseSceneBy = ChooseSceneBy.Name;
			previousSceneName = string.Empty;

			if (connectedConstantID != 0)
			{
				ConstantID constantID = GetComponent<ConstantID> ();
				if (constantID)
				{
					if (constantID.constantID == connectedConstantID)
					{
						previousSceneName = KickStarter.sceneChanger.PreviousSceneName;
					}
				}
			}

			EventManager.OnHotspotInteract += OnHotspotInteract;
		}

			
		private void OnDisable ()
		{
			EventManager.OnHotspotInteract -= OnHotspotInteract;
		}


		private void Start ()
		{
			connectedConstantID = 0;
		}

		#endregion


		#region PublicFunctions

#if UNITY_EDITOR

		public void ShowGUI()
		{
			CustomGUILayout.Header("Camera settings");
			CustomGUILayout.BeginVertical();
			cameraOnStart = (_Camera)CustomGUILayout.ObjectField<_Camera>("Camera on start:", cameraOnStart, true, "", "The AC _Camera that should be made active when the Player starts the scene from this point");
			fadeInOnStart = CustomGUILayout.Toggle("Fade in on activate?", fadeInOnStart, "", "If True, then the MainCamera will fade in when the Player starts the scene from this point");
			if (fadeInOnStart)
			{
				fadeSpeed = CustomGUILayout.FloatField("Fade speed:", fadeSpeed, "", "The speed of the fade");
			}
			CustomGUILayout.EndVertical();

			CustomGUILayout.Header("Activation settings");
			CustomGUILayout.BeginVertical();
			autoHotspot = (Hotspot)CustomGUILayout.ObjectField<Hotspot>("Auto-Hotspot:", autoHotspot, true, "", "If set, the exit will be used when this Hotspot is interacted with.");
			CustomGUILayout.EndVertical ();
		}

#endif

		public void Interact ()
		{
			if (KickStarter.settingsManager.referenceScenesInSave != ChooseSceneBy.Name)
			{
				ACDebug.LogWarning ("For the Scene Graph to work, 'Reference scenes by' must be set to Name in the Settings Manager");
				return;
			}

			SceneGraph sceneGraph = SceneGraph.GetSceneGraph ();
			if (sceneGraph)
			{
				SceneExitNode sceneExitNode = sceneGraph.GraphData.GetSceneExitNode (this);
				if (sceneExitNode == null)
				{
					ACDebug.LogWarning ("Cannot find corresponding scene for exit " + this + " in scene graph " + sceneGraph, sceneGraph);
					return;
				}

				connectedConstantID = sceneExitNode.ConnectedConstantID;
				KickStarter.sceneChanger.ChangeScene (sceneExitNode.ConnectedSceneName, true);
			}
			else
			{
				ACDebug.LogWarning ("Cannot interact with scene exit - no scene graph found in Resources");
			}
		}

		#endregion


		#region CustomEvents

		private void OnHotspotInteract (Hotspot hotspot, AC.Button button)
		{
			if (hotspot == autoHotspot)
			{
				Interact ();
			}
		}

		#endregion

	}

}