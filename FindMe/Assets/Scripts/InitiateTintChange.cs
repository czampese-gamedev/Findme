using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitiateTintChange : MonoBehaviour
{

    public Color tintColor;
    [Range(0f, 1f)]
    public float tintStrength;
    public float tintduration = 1f;

    public Transform p;

    private void OnTriggerEnter(Collider other)
    {


        if (other.tag == "ChangePlayerTint")
        {
            //Check if parent has updatetint component
            UpdateTint utparent = gameObject.GetComponent<UpdateTint>();
            if (utparent != null)
            {
                Color c = other.GetComponent<SetTintColor>().tintColor;
                float s = other.GetComponent<SetTintColor>().tintStrength;
                float d = other.GetComponent<SetTintColor>().tintDuration;
                bool setinit = other.GetComponent<SetTintColor>().setinitialcolor;
                utparent.AddTint(c, s, d, setinit);

            }
            //check if child objects have updatetint component
           p = transform.root;

            foreach (Transform child in p.GetComponentsInChildren<Transform>())
            {
                UpdateTint ut = child.gameObject.GetComponent<UpdateTint>();
                if (ut != null)
                {
                    Color c = other.GetComponent<SetTintColor>().tintColor;
                    float s = other.GetComponent<SetTintColor>().tintStrength;
                    float d = other.GetComponent<SetTintColor>().tintDuration;
                    bool setinit = other.GetComponent<SetTintColor>().setinitialcolor;
                    ut.AddTint(c, s, d, setinit);

                }
            }

        }
       
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "ChangePlayerTint")
        {
            UpdateTint utparent = gameObject.GetComponent<UpdateTint>();
            if (utparent != null)
            {
                utparent.RemoveTint();
            }
            p = transform.root;
            foreach (Transform child in p.GetComponentsInChildren<Transform>())
            {
                UpdateTint ut = child.gameObject.GetComponent<UpdateTint>();
                if (ut != null)
                {
                    ut.RemoveTint();
                }
            }
        }
       
    }
}
