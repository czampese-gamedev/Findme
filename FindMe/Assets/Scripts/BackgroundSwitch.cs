using UnityEngine;
using AC;

public class BackgroundSwitch : MonoBehaviour
{

    public GameObject[] bg;

    public int currentbg = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach(GameObject go in bg)
        {
            if (go == bg[currentbg])
            { go.SetActive(true); }
            else { go.SetActive(false); }
        }

        foreach (GameObject go in bg)
        {
            if (go.activeSelf)
            {
                Debug.Log("Go " + go.name + "is currently active");
            }
            else { Debug.Log("Go " + go.name + "is NOT active"); }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            SwitchBackground();

        }
    }

    public void SwitchBackground()
    {

        bool SwitchNext = false;

        foreach (GameObject go in bg)
        {
            

            if (go.activeSelf)
            {
                SwitchNext = true;
                go.SetActive(false);
                Debug.Log("Go " + go.name + "is being switched off");
            }
            else if (!go.activeSelf && SwitchNext)
                {
                    Debug.Log("Go " + go.name + "is being switched on");
                    go.SetActive(true);
                    SwitchNext = false;
                }

        }

    }
}
