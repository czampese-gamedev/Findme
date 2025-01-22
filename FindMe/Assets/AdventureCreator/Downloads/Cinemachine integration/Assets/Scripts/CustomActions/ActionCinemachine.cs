using UnityEngine;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Cinemachine;
using AC.Downloads.CinemachineIntegration;

namespace AC
{

	[System.Serializable]
	public class ActionCinemachine : Action
	{
		
		public override ActionCategory Category { get { return ActionCategory.Camera; }}
		public override string Title { get { return "Cinemachine"; }}
		public override string Description { get { return "Methods to control a Cinemachine Virtual Camera."; }}


		public enum CinemachineActionMethod { SetVCamPriority, ControlMixer }
		public CinemachineActionMethod cinemachineActionMethod = CinemachineActionMethod.SetVCamPriority;

		public CinemachineVirtualCameraBase virtualCamera;
		public int constantID = 0;
		public int parameterID = -1;
		protected CinemachineVirtualCameraBase runtimeVirtualCamera;

		public int priority;
		public int priorityParameterID = -1;

		public CinemachineMixerController mixerController;
		protected CinemachineMixerController runtimeMixerController;
		public int channelIndex;
		public float transitiomTime;


		public override void AssignValues (List<ActionParameter> parameters)
		{
			runtimeVirtualCamera = AssignFile (parameters, parameterID, constantID, virtualCamera);
			runtimeMixerController = AssignFile (parameters, parameterID, constantID, mixerController);
			priority = AssignInteger (parameters, priorityParameterID, priority);
		}


		public override float Run ()
		{
			switch (cinemachineActionMethod)
			{
				case CinemachineActionMethod.SetVCamPriority:
					if (runtimeVirtualCamera)
					{
						runtimeVirtualCamera.Priority = priority;
						runtimeVirtualCamera.MoveToTopOfPrioritySubqueue ();
					}
					break;

				case CinemachineActionMethod.ControlMixer:
					if (runtimeMixerController)
					{
						float duration = runtimeMixerController.SwitchCamera (channelIndex, transitiomTime);
						if (willWait && duration > 0f)
						{
							isRunning = true;
							return duration;
						}
					}
					break;
			}

			isRunning = false;
			return 0f;
		}

		
		#if UNITY_EDITOR

		public override void ShowGUI (List<ActionParameter> parameters)
		{
			cinemachineActionMethod = (CinemachineActionMethod) EditorGUILayout.EnumPopup ("Method", cinemachineActionMethod);

			if (cinemachineActionMethod == CinemachineActionMethod.SetVCamPriority)
			{
				parameterID = ChooseParameterGUI ("Virtual Camera:", parameters, parameterID, ParameterType.GameObject);
				if (parameterID >= 0)
				{
					constantID = 0;
					virtualCamera = null;
				}
				else
				{
					virtualCamera = (CinemachineVirtualCameraBase) EditorGUILayout.ObjectField ("Virtual Camera:", virtualCamera, typeof (CinemachineVirtualCameraBase), true);

					constantID = FieldToID (virtualCamera, constantID);
					virtualCamera = IDToField (virtualCamera, constantID, true);
				}

				priorityParameterID = ChooseParameterGUI ("Priority:", parameters, priorityParameterID, ParameterType.Integer);
				if (priorityParameterID < 0)
				{
					priority = EditorGUILayout.IntField ("Priority:", priority);
				}
			}
			else if (cinemachineActionMethod == CinemachineActionMethod.ControlMixer)
			{
				parameterID = ChooseParameterGUI ("Mixer controller:", parameters, parameterID, ParameterType.GameObject);
				if (parameterID >= 0)
				{
					constantID = 0;
					mixerController = null;
				}
				else
				{
					mixerController = (CinemachineMixerController) EditorGUILayout.ObjectField ("Mixer controller:", mixerController, typeof (CinemachineMixerController), true);

					constantID = FieldToID (mixerController, constantID);
					mixerController = IDToField (mixerController, constantID, true);
				}

				if (mixerController && mixerController.GetComponent<CinemachineMixingCamera> ())
				{
					CinemachineMixingCamera cinemachineMixingCamera = mixerController.GetComponent<CinemachineMixingCamera> ();
					string[] cameraNames = new string[cinemachineMixingCamera.ChildCameras.Length];
					for (int i = 0; i < cameraNames.Length; i++)
					{
						cameraNames[i] = cinemachineMixingCamera.ChildCameras[i].gameObject ? cinemachineMixingCamera.ChildCameras[i].gameObject.name : ("Channel " + i);
					}
					channelIndex = EditorGUILayout.Popup ("New camera:", channelIndex, cameraNames);
				}
				else
				{
					channelIndex = EditorGUILayout.IntField ("Channel index:", channelIndex);
				}

				transitiomTime = EditorGUILayout.FloatField ("Transition time (s):", transitiomTime);
				if (transitiomTime > 0f)
				{
					willWait = EditorGUILayout.Toggle ("Wait until finish?", willWait);
				}
			}
		}

		#endif
		
	}

}