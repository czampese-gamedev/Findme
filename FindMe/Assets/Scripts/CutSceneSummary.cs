using AC;
using System.Collections.Generic;
using UnityEngine;
public class CutSceneSummary : MonoBehaviour
{

    public GameObject[] Summary;


    private void OnEnable()
    {

        GVar myVar = LocalVariables.GetVariable("Summary");

        if (myVar != null)
        {
            foreach (GameObject g in Summary)
            {
                if (myVar.TextValue == g.name)
                {
                    g.SetActive(true);
                }
                else
                {
                    g.SetActive(false);
                }
            }
        }
        else
        {
            foreach (GameObject g in Summary)
            {
                g.SetActive(false);
            }


        }



    }

    
}
