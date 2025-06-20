using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateTint : MonoBehaviour
{
    private float tintduration;
    private Color tintcolor;

    public Color initialcolor;
    public float initialstrength;

    private IEnumerator RunTint;
    private IEnumerator StopTint;
    private void Start()
    {
        initialcolor = gameObject.GetComponent<Renderer>().material.GetColor("_TintColor");
        initialstrength = gameObject.GetComponent<Renderer>().material.GetFloat("_TintStrength");
    }


    public void AddTint(Color c, float t, float d, bool setinit)
    {
       
        if (setinit)
        {
            gameObject.GetComponent<Renderer>().material.SetFloat("_TintStrength", t);
            gameObject.GetComponent<Renderer>().material.SetColor("_TintColor", c);
            initialcolor = gameObject.GetComponent<Renderer>().material.GetColor("_TintColor");
            initialstrength = gameObject.GetComponent<Renderer>().material.GetFloat("_TintStrength");
        }
        //  gameObject.GetComponent<Renderer>().material.SetFloat("_TintStrength", 0f);
        tintcolor = c;
        tintduration = d;

        if (StopTint != null)
        {
            StopCoroutine(StopTint);
        }

        RunTint = RunTintChange(t);
        StartCoroutine(RunTint);
    }


    IEnumerator RunTintChange (float t)
    {

        float time = 0;
        while (time < tintduration)
        {

            gameObject.GetComponent<Renderer>().material.SetFloat("_TintStrength", Mathf.Lerp(gameObject.GetComponent<Renderer>().material.GetFloat("_TintStrength"), t, time / tintduration));
            gameObject.GetComponent<Renderer>().material.SetColor("_TintColor", Color.Lerp(gameObject.GetComponent<Renderer>().material.GetColor("_TintColor"), tintcolor, time / tintduration));
            time += Time.deltaTime;
            yield return null;


        }
        gameObject.GetComponent<Renderer>().material.SetFloat("_TintStrength", t);
        RunTint = null;

        //while (gameObject.GetComponent<Renderer>().material.GetFloat("_TintStrength") < t)
        //{
        //    float newstrength = gameObject.GetComponent<Renderer>().material.GetFloat("_TintStrength") +  (tintduration);
        //    gameObject.GetComponent<Renderer>().material.SetFloat("_TintStrength", newstrength);
        //    //yield return new WaitForSeconds(tintduration);
        //    yield return null;

        //}

    }


    IEnumerator RemoveTintChange()
    {

        float time = 0;
        while (time < tintduration)
        {

            gameObject.GetComponent<Renderer>().material.SetFloat("_TintStrength",Mathf.Lerp(gameObject.GetComponent<Renderer>().material.GetFloat("_TintStrength"), initialstrength, time / tintduration));
            gameObject.GetComponent<Renderer>().material.SetColor("_TintColor", Color.Lerp(gameObject.GetComponent<Renderer>().material.GetColor("_TintColor"), initialcolor, time / tintduration));
            time += Time.deltaTime;
            yield return null;
        }

        //while (gameObject.GetComponent<Renderer>().material.GetFloat("_TintStrength") > 0)
        //{
        //    float newstrength = gameObject.GetComponent<Renderer>().material.GetFloat("_TintStrength") - (tintduration);
        //    gameObject.GetComponent<Renderer>().material.SetFloat("_TintStrength", newstrength);
        //    //yield return new WaitForSeconds(tintduration);
        //    yield return null;

        //}
        StopTint = null;
        gameObject.GetComponent<Renderer>().material.SetFloat("_TintStrength", initialstrength);
    }


    public void RemoveTint()
    {
        if (RunTint == null)
        {
            // StopCoroutine(RunTint);
            StopTint = RemoveTintChange();
            StartCoroutine(StopTint);
        }
        //StopTint = RemoveTintChange();
        //StartCoroutine(StopTint);
       // gameObject.GetComponent<Renderer>().material.SetFloat("_TintStrength", 0);
    }
 
}
