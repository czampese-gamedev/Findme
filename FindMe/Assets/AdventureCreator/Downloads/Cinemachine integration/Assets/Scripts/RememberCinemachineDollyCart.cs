using UnityEngine;
using Cinemachine;

namespace AC.Downloads.CinemachineIntegration
{

	[AddComponentMenu ("Adventure Creator/Save system/Remember Cinemachine Dolly Cart")]
	[RequireComponent (typeof (CinemachineDollyCart))]
	public class RememberCinemachineDollyCart : Remember
	{

		public override string SaveData ()
		{
			CinemachineDollyCartData data = new CinemachineDollyCartData ();
			data.objectID = constantID;
			data.savePrevented = savePrevented;

			CinemachineDollyCart dollyCart = GetComponent<CinemachineDollyCart> ();
			data.speed = dollyCart.m_Speed;
			data.position = dollyCart.m_Position;

			return Serializer.SaveScriptData<CinemachineDollyCartData> (data);
		}


		public override void LoadData (string stringData)
		{
			CinemachineDollyCartData data = Serializer.LoadScriptData<CinemachineDollyCartData> (stringData);
			if (data == null) return;
			SavePrevented = data.savePrevented; if (savePrevented) return;

			CinemachineDollyCart dollyCart = GetComponent<CinemachineDollyCart> ();
			dollyCart.m_Speed = data.speed;
			dollyCart.m_Position = data.position;
		}

	}


	[System.Serializable]
	public class CinemachineDollyCartData : RememberData
	{

		public float speed;
		public float position;

		public CinemachineDollyCartData () { }

	}

}