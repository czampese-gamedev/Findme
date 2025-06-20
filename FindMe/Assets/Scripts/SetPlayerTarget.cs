using UnityEngine;
using Cinemachine;
using AC;

public class SetPlayerTarget : MonoBehaviour
{

    private void OnEnable() { EventManager.OnSetPlayer += SetNewPlayer; }
    private void OnDisable() { EventManager.OnSetPlayer -= SetNewPlayer; }

    private void SetNewPlayer(Player player)
    {
       // GetComponent<CinemachineVirtualCam>().m_Follow = player.transform;
        GetComponent<CinemachineVirtualCamera>().m_LookAt = player.transform;
    }

}