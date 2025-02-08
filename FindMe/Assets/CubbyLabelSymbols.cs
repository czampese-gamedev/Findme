using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubbyLabelSymbols : MonoBehaviour
{

    MeshRenderer rend;
    public float xoffset;
    public float yoffset; 
    // Start is called before the first frame update
    void Start()
    {
        xoffset = xoffset * 0.14f;
        yoffset = yoffset * -0.14f;
        rend = GetComponent<MeshRenderer>();
       rend.material.mainTextureOffset = new Vector2(xoffset, yoffset);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
