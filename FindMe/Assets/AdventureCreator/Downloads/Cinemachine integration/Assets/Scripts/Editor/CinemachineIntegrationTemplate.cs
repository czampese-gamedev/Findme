#if UNITY_EDITOR

using System;
using UnityEngine;
using UnityEditor;

namespace AC.Downloads.CinemachineIntegration
{
	
	public class CinemachineIntegrationTemplate : Template
	{

		#region Variables

		[SerializeField] private UnityEngine.Object actionsFolderObj = null;
		[SerializeField] private Texture2D cameraIcon = null;
		[SerializeField] private _Camera brainPrefab = null;
		[SerializeField] private CinemachineMixerController mixerPrefab = null;
		[SerializeField] private RememberCinemachineVCam vCamPrefab = null;

		#endregion


		#region PublicFunctions

		public override bool CanInstall (ref string errorText)
		{
			if (KickStarter.sceneManager == null)
			{
				errorText = "No Scene Manager assigned";
				return false;
			}

			if (KickStarter.actionsManager == null)
			{
				errorText = "No Actions Manager assigned";
				return false;
			}

			if (brainPrefab == null || mixerPrefab == null || vCamPrefab == null)
			{
				errorText = "No prefab assigned";
				return false;
			}

			if (actionsFolderObj == null)
			{
				errorText = "No Actions folder assigned";
				return false;
			}

			return true;
		}

		#endregion


		#region ProtectedFunctions

		protected override void MakeChanges (string installPath, bool canDeleteOldAssets, System.Action onComplete, System.Action<string> onFail)
		{
			Undo.RecordObjects (new UnityEngine.Object[] {KickStarter.sceneManager, KickStarter.actionsManager }, "");

			// Actions
			InstallActionsFolder (actionsFolderObj);

			// Prefabs
			_Camera newBrainPrefab = CopyAsset<_Camera> (installPath, brainPrefab, ".prefab");
			if (newBrainPrefab == null)
			{
				onFail.Invoke ("Prefab copy failed.");
				return;
			}

			CinemachineMixerController newMixerPrefab = CopyAsset<CinemachineMixerController> (installPath, mixerPrefab, ".prefab");
			if (newMixerPrefab == null)
			{
				onFail.Invoke ("Prefab copy failed.");
				return;
			}

			RememberCinemachineVCam newVCamPrefab = CopyAsset<RememberCinemachineVCam> (installPath, vCamPrefab, ".prefab");
			if (newVCamPrefab == null)
			{
				onFail.Invoke ("Prefab copy failed.");
				return;
			}

			// Scene
			KickStarter.sceneManager.AddPrefab (new SceneManagerPrefabData ("Camera", "Cinemachine Brain", "A Cinemachine Brain camera that can be switched to like a regular AC GameCamera.", cameraIcon, newBrainPrefab.gameObject));
			KickStarter.sceneManager.AddPrefab (new SceneManagerPrefabData ("Camera", "Cinemachine Mixer", "A group of Cinemachine cameras that can be switched between using the Camera: Cinemachine Action.", cameraIcon, newMixerPrefab.gameObject));
			KickStarter.sceneManager.AddPrefab (new SceneManagerPrefabData ("Camera", "Cinemachine vCam", "A Cinemachine Virtual Camera whose position and rotation is saved within AC save-game files.", cameraIcon, newVCamPrefab.gameObject));

			onComplete.Invoke ();
		}

		#endregion


		#region GetSet

		public override string Label { get { return "Cinemachine integration"; }}
		public override string PreviewText { get { return "A collection of Actions, prefabs and components to integrate AC's camera system with Cinemachine"; }}
		public override Type[] AffectedManagerTypes { get { return new Type[] { typeof (SceneManager), typeof (ActionsManager) }; }}
		public override string FolderName { get { return "CinemachineIntegration"; }}

		#endregion

	}


	[CustomEditor (typeof (CinemachineIntegrationTemplate))]
	public class CinemachineIntegrationTemplateEditor : TemplateEditor
	{}

}

#endif