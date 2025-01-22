/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013-2024
 *	
 *	"ActionTemplate.cs"
 * 
 *	This is a blank action template.
 * 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AC
{
	[System.Serializable]

	public class ActionCinemachineCamera : Action

	{



		public CinemachineVirtualCameraBase CM_cam;

		public int cmCamConstantID = 0;

		public int cmCamParameterID = -1;

		private CinemachineVirtualCameraBase runtimeCMCam;



		public int Priority;

		public int priorityParameterID = -1;





		public ActionCinemachineCamera()

		{

			this.isDisplayed = true;

			category = ActionCategory.Camera;

			title = "CM Priority";

			description = "Changes CM priority";

		}





		public override void AssignValues(List<ActionParameter> parameters)

		{

			runtimeCMCam = AssignFile<CinemachineVirtualCameraBase>(parameters, cmCamParameterID, cmCamConstantID, CM_cam);

			Priority = AssignInteger(parameters, priorityParameterID, Priority);

		}





		override public float Run()

		{

			if (runtimeCMCam)

			{

				runtimeCMCam.enabled = true;

				runtimeCMCam.MoveToTopOfPrioritySubqueue();

				runtimeCMCam.Priority = Priority;

			}

			return 0f;



		}





#if UNITY_EDITOR



		override public void ShowGUI(List<ActionParameter> parameters)

		{

			cmCamParameterID = Action.ChooseParameterGUI("CM camera:", parameters, cmCamParameterID, ParameterType.GameObject);

			if (cmCamParameterID >= 0)

			{

				cmCamConstantID = 0;

				CM_cam = null;

			}

			else

			{

				CM_cam = (CinemachineVirtualCameraBase)EditorGUILayout.ObjectField("CM Camera:", CM_cam, typeof(CinemachineVirtualCameraBase), true);




				cmCamConstantID = FieldToID<CinemachineVirtualCameraBase>(CM_cam, cmCamConstantID);

				CM_cam = IDToField<CinemachineVirtualCameraBase>(CM_cam, cmCamConstantID, true);

			}



			priorityParameterID = Action.ChooseParameterGUI("Priority:", parameters, priorityParameterID, ParameterType.Integer);

			if (priorityParameterID < 0)

			{

				Priority = EditorGUILayout.IntField("Priority:", Priority);

			}



			AfterRunningOption();



		}



#endif



	}


}