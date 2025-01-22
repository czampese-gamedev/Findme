using UnityEngine;
using Cinemachine;

namespace AC.Downloads.CinemachineIntegration
{

	[AddComponentMenu ("Adventure Creator/Save system/Remember Cinemachine VCam")]
	[RequireComponent (typeof (CinemachineVirtualCameraBase))]
	public class RememberCinemachineVCam : Remember
	{

		public override string SaveData ()
		{
			CinemachineVCamData data = new CinemachineVCamData ();
			data.objectID = constantID;
			data.savePrevented = savePrevented;

			CinemachineVirtualCameraBase cinemachineVirtualCameraBase = GetComponent<CinemachineVirtualCameraBase> ();

			#if UNITY_2022_3_OR_NEWER
			CinemachineBrain cinemachineBrain = Object.FindFirstObjectByType <CinemachineBrain> ();
			#else
			CinemachineBrain cinemachineBrain = Object.FindObjectOfType <CinemachineBrain> ();
			#endif

			data.priority = cinemachineVirtualCameraBase.Priority;
			data.isActive = cinemachineBrain != null && cinemachineBrain.ActiveVirtualCamera != null && cinemachineBrain.ActiveVirtualCamera.VirtualCameraGameObject == cinemachineVirtualCameraBase.gameObject;
			data.positionX = cinemachineVirtualCameraBase.transform.position.x;
			data.positionY = cinemachineVirtualCameraBase.transform.position.y;
			data.positionZ = cinemachineVirtualCameraBase.transform.position.z;
			data.rotationX = cinemachineVirtualCameraBase.transform.rotation.x;
			data.rotationY = cinemachineVirtualCameraBase.transform.rotation.y;
			data.rotationZ = cinemachineVirtualCameraBase.transform.rotation.z;
			data.rotationW = cinemachineVirtualCameraBase.transform.rotation.w;

			CinemachineMixingCamera cinemachineMixingCamera = cinemachineVirtualCameraBase as CinemachineMixingCamera;
			if (cinemachineMixingCamera)
			{
				data.mixingWeights = new float[cinemachineMixingCamera.ChildCameras.Length];
				for (int i = 0; i < data.mixingWeights.Length; i++)
				{
					data.mixingWeights[i] = cinemachineMixingCamera.GetWeight (i);
				}
			}

			return Serializer.SaveScriptData<CinemachineVCamData> (data);
		}


		public override void LoadData (string stringData)
		{
			CinemachineVCamData data = Serializer.LoadScriptData<CinemachineVCamData> (stringData);
			if (data == null) return;
			SavePrevented = data.savePrevented; if (savePrevented) return;

			CinemachineVirtualCameraBase cinemachineVirtualCameraBase = GetComponent<CinemachineVirtualCameraBase> ();
			cinemachineVirtualCameraBase.Priority = data.priority;
			if (data.isActive)
			{
				cinemachineVirtualCameraBase.MoveToTopOfPrioritySubqueue ();
			}

			CinemachineMixingCamera cinemachineMixingCamera = cinemachineVirtualCameraBase as CinemachineMixingCamera;
			if (cinemachineMixingCamera)
			{
				for (int i = 0; i < data.mixingWeights.Length; i++)
				{
					cinemachineMixingCamera.SetWeight (i, data.mixingWeights[i]);
				}
			}
			else
			{
				Vector3 position = new Vector3 (data.positionX, data.positionY, data.positionZ);
				Quaternion rotation = new Quaternion (data.rotationX, data.rotationY, data.rotationZ, data.rotationW);
				cinemachineVirtualCameraBase.ForceCameraPosition (position, rotation);
			}
		}

	}


	[System.Serializable]
	public class CinemachineVCamData : RememberData
	{

		public int priority;
		public bool isActive;
		public float positionX, positionY, positionZ;
		public float rotationX, rotationY, rotationZ, rotationW;

		public float[] mixingWeights;

		public CinemachineVCamData () { }

	}

}