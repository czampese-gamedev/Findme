using UnityEngine;
using Cinemachine;

namespace AC.Downloads.CinemachineIntegration
{

	[RequireComponent (typeof (CinemachineMixingCamera))]
	public class CinemachineMixerController : MonoBehaviour
	{

		#region Variables

		private float[] originalWeights = new float[0];
		private float[] targetWeights = new float[0];
		private float transitionTime = 0f;
		private float transitionDuration = 0f;
		private CinemachineMixingCamera cinemachineMixingCamera;

		#endregion


		#region UnityStandards

		private void Awake ()
		{
			cinemachineMixingCamera = GetComponent<CinemachineMixingCamera> ();
		}


		private void OnEnable ()
		{
			EventManager.OnFinishLoading += OnFinishLoading;
		}


		private void OnDisable ()
		{
			EventManager.OnFinishLoading -= OnFinishLoading;
		}


		private void Update ()
		{
			if (transitionTime > 0f)
			{
				float lerpAmount = 1f - (transitionTime / transitionDuration);

				int numCameras = targetWeights.Length;
				float[] actualBlends = new float[numCameras];
				for (int i = 0; i < numCameras; i++)
				{
					actualBlends[i] = Mathf.Lerp (originalWeights[i], targetWeights[i], lerpAmount);
					cinemachineMixingCamera.SetWeight (i, actualBlends[i]);
				}

				transitionTime -= Time.deltaTime;

				if (transitionTime <= 0f)
				{
					SnapToTarget ();
				}
			}
		}

		#endregion


		#region PublicFunctions

		public float SwitchCamera (int channel, float duration)
		{
			int numCameras = cinemachineMixingCamera.ChildCameras.Length;
			originalWeights = new float[numCameras];
			targetWeights = new float[numCameras];

			bool weightsAreSame = true;
			for (int i = 0; i < numCameras; i++)
			{
				originalWeights[i] = cinemachineMixingCamera.GetWeight (i);
				targetWeights[i] = (i == channel) ? 1f : 0f;

				if (originalWeights[i] != targetWeights[i])
				{
					weightsAreSame = false;
				}

				if (duration <= 0f)
				{
					cinemachineMixingCamera.SetWeight (i, targetWeights[i]);
				}
			}

			if (duration <= 0f || weightsAreSame)
			{
				SnapToTarget ();
				transitionTime = 0f;
				return 0f;
			}

			transitionTime = transitionDuration = Mathf.Max (0f, duration);
			return transitionTime;
		}

		#endregion


		#region CustomEvents

		private void OnFinishLoading (int saveID)
		{
			if (transitionTime > 0f)
			{
				transitionTime = 0f;
				SnapToTarget ();
			}
		}

		#endregion


		#region PrivateFunctions

		private void SnapToTarget ()
		{
			int numCameras = targetWeights.Length;
			for (int i = 0; i < numCameras; i++)
			{
				cinemachineMixingCamera.SetWeight (i, targetWeights[i]);
			}
		}

		#endregion

	}

}