using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureOffset : MonoBehaviour
{

    MeshRenderer rend;
    public float divideby = 8;
    public float xoffset;
    public float yoffset; 
    // Start is called before the first frame update
    void Start()
    {
        
       
        rend = GetComponent<MeshRenderer>();
        Texture t = rend.material.mainTexture;
        float offsetvalue = t.width / divideby;
        xoffset = (xoffset * offsetvalue) / 10f;
        yoffset = (yoffset * -offsetvalue) / 10f;
        Debug.Log("Texture width is " + t.width + " offsetvalue is " + offsetvalue + " xoffset is " + xoffset + " yoffset is " + yoffset);
        rend.material.mainTextureOffset = new Vector2(xoffset, yoffset);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
