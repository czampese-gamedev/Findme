using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TurnOffVSync : MonoBehaviour
{
    public TMP_Text thisistext;

    private int qs; 

    // Start is called before the first frame update
    void Start()
    {
        qs = QualitySettings.GetQualityLevel();
    }

    // Update is called once per frame
    void Update()
    {
        int v = QualitySettings.vSyncCount;
        thisistext.text = "Quality Setting is " + qs;
    }
}
