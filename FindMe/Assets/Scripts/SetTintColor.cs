using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTintColor : MonoBehaviour
{
    public Color tintColor;
    [Range(0f, 1f)]
    public float tintStrength;
    public float tintDuration;
    public bool setinitialcolor = false;
   
}
