using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace AC
{

	[Serializable]
	public class RemappableAction
	{

		#region Variables

		[SerializeField] private InputActionReference inputActionReference = null;
		[SerializeField] private UnityEngine.UI.Button linkedUIButton = null;
		private InputActionRebindingExtensions.RebindingOperation activeRebindingOperation;

		#endregion


		#region PublicFunctions

		public void AddClickEvent (UnityAction onClick)
		{
			linkedUIButton.onClick.AddListener (onClick);
		}


		public void BeginRebinding ()
		{
			inputActionReference.action.Disable ();

			activeRebindingOperation = inputActionReference.action.PerformInteractiveRebinding ()
				.WithControlsExcluding ("Mouse")
				.OnMatchWaitForAnother (0.1f)
				.OnComplete (operation => OnRemapBinding ())
				.Start ();

			SetTextToPending ();
		}


		public void EndRebinding ()
		{
			inputActionReference.action.Enable ();

			SetTextToLabel ();

			if (activeRebindingOperation != null)
			{
				activeRebindingOperation.Dispose ();
				activeRebindingOperation = null;
			}
		}

		#endregion


		#region PrivateFunctions

		private void OnRemapBinding ()
		{
			EndRebinding ();
			ControlsReader.Instance.SaveRebindings ();
		}


		private void SetTextToLabel ()
		{
			if (inputActionReference == null) return;

			int bindingIndex = inputActionReference.action.GetBindingIndexForControl (inputActionReference.action.controls[0]);
			string inputKey = InputControlPath.ToHumanReadableString (inputActionReference.action.bindings[bindingIndex].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);

			SetButtonText (inputActionReference.action.name + " - " + inputKey);
		}


		private void SetTextToPending ()
		{
			if (inputActionReference == null) return;

			SetButtonText (inputActionReference.action.name + " - " + "(Press a key)");
		}


		private void SetButtonText (string text)
		{
			if (linkedUIButton == null) return;
			#if TextMeshProIsPresent
			TMPro.TextMeshProUGUI textBox = linkedUIButton.GetComponentInChildren<TMPro.TextMeshProUGUI> ();
			#else
			Text textBox = linkedUIButton.GetComponentInChildren<Text> ();
			#endif
			if (textBox == null) return;

			textBox.text = text;
		}

		#endregion

	}

}