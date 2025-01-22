#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.InputSystem;
using UnityEngine;

namespace AC
{

#if UNITY_EDITOR
	[InitializeOnLoad]
#endif
	public class ACCursorPositionProcessor : InputProcessor<Vector2>
	{

#if UNITY_EDITOR
		static ACCursorPositionProcessor ()
		{
			Initialize ();
		}
#endif

		public override Vector2 Process (Vector2 value, InputControl control)
		{
			if (KickStarter.settingsManager.inputMethod == InputMethod.KeyboardOrController)
			{
				return KickStarter.playerInput.GetMousePosition ();
			}
			return value;
		}

		[RuntimeInitializeOnLoadMethod (RuntimeInitializeLoadType.BeforeSceneLoad)]
		static void Initialize ()
		{
			InputSystem.RegisterProcessor<ACCursorPositionProcessor> ();
		}

	}

}