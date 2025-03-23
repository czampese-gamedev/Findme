using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeGV : MonoBehaviour
{

    public GameObject gv1;
    public GameObject gv2;
    public GameObject gv3;
    public GameObject gv4;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

   public  void updategv(int g)
    {
        if (g == 1 )
        {
            gv1.SetActive(true);
            gv2.SetActive(false);
            gv3.SetActive(false);
            gv4.SetActive(false);
        }
        if (g == 2)
        {
            gv1.SetActive(false);
            gv2.SetActive(true);
            gv3.SetActive(false);
            gv4.SetActive(false);
        }
        if (g == 3)
        {
            gv1.SetActive(false);
            gv2.SetActive(false);
            gv3.SetActive(true);
            gv4.SetActive(false);
        }

        if (g == 4)
        {
            gv1.SetActive(false);
            gv2.SetActive(false);
            gv3.SetActive(false);
            gv4.SetActive(true);
        }

    }
}
