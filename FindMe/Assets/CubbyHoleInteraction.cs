using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubbyHoleInteraction : MonoBehaviour
{

    public GameObject[] drawers;
    public GameObject CurrentlySelectedDrawer; 


    // Start is called before the first frame update
    void Start()
    {
        foreach(GameObject drawer in drawers)
        {
            drawer.GetComponent<Animator>().SetInteger("Close0Open1", 1);
            Debug.Log(drawer.name + " is being opened");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
