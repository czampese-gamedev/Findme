using Unity.AI.Navigation;
using UnityEngine;

public class TurnOnOffNavMeshLink : MonoBehaviour
{
    protected NavMeshLink _navlink;

    public void TurnOn()
    {
        Switch(true);
    }


    public void TurnOff()
    {
        Switch(false);
    }


    protected void Switch(bool turnOn)
    {
        if (_navlink == null)
        {
            _navlink = GetComponent<NavMeshLink>();
        }
        if (_navlink)
        {
            _navlink.enabled = turnOn;
        }
    }
    }
