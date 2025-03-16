using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureOffset : MonoBehaviour
{

    MeshRenderer rend;
    public float divideby = 8;
    public float xoffset = 0;
    public float yoffset = 0; 
    // Start is called before the first frame update
    void Start()
    {
        
       if (xoffset != 0)
        {
            xoffset = xoffset - 1;
        }

        if (yoffset != 0)
        {
            yoffset = yoffset - 1;
        }

        rend = GetComponent<MeshRenderer>();
        float offsetvalue = 1 / divideby;
        xoffset = (xoffset * offsetvalue);
        yoffset = (yoffset * -offsetvalue); ;
        rend.material.mainTextureOffset = new Vector2(xoffset, yoffset);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
