using UnityEngine;

namespace AC.Downloads.CinemachineIntegration
{

	public class ParentToSpawnedPlayer : MonoBehaviour
	{

		[SerializeField] private Vector3 localPosition;

		private void OnEnable () { EventManager.OnPlayerSpawn += OnPlayerSpawn; }
		private void OnDisable () { EventManager.OnPlayerSpawn -= OnPlayerSpawn; }

		private void OnPlayerSpawn (Player player)
		{
			transform.SetParent (player.transform);
			transform.localPosition = localPosition;
		}

	}

}