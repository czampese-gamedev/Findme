using UnityEngine;

namespace AC
{

	public class InputRemapper : MonoBehaviour
	{

		#region Variables

		[SerializeField] private RemappableAction[] remappableActions = new RemappableAction[0];
		private RemappableAction activeRebinding;

		#endregion


		#region UnityStandards

		private void Start ()
		{
			foreach (RemappableAction remappableAction in remappableActions)
			{
				remappableAction.AddClickEvent (() => OnClickRemapButton (remappableAction));
			}
		}


		private void OnEnable ()
		{
			foreach (RemappableAction remappableAction in remappableActions)
			{
				remappableAction.EndRebinding ();
			}
		}


		private void OnDisable ()
		{
			foreach (RemappableAction remappableAction in remappableActions)
			{
				remappableAction.EndRebinding ();
			}
		}

		#endregion


		#region PublicFunctions

		public void OnClickRemapButton (RemappableAction remappableAction)
		{
			if (activeRebinding != null)
			{
				activeRebinding.EndRebinding ();

				if (remappableAction == activeRebinding)
				{
					activeRebinding = null;
					return;
				}
			}

			activeRebinding = remappableAction;
			activeRebinding.BeginRebinding ();
		}


		public void OnClickDefaultsButton ()
		{
			ControlsReader controlsReader = ControlsReader.Instance;
			if (controlsReader == null) return;

			if (activeRebinding != null)
			{
				activeRebinding.EndRebinding ();
				activeRebinding = null;
			}
			
			controlsReader.ClearRebindings ();

			foreach (RemappableAction remappableAction in remappableActions)
			{
				remappableAction.EndRebinding ();
			}
		}

		#endregion

	}

}