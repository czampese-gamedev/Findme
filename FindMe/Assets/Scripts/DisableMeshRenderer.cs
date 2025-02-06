using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableMeshRenderer : MonoBehaviour
{

    public MeshRenderer m;
    // Start is called before the first frame update
    void Start()
    {

        m.enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
