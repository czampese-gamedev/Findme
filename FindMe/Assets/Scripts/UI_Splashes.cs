using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AC;

[System.Serializable]
public class ItemData
{
    public int Reference;
    public GameObject targetObject;
    public int duration;
}

public class UI_Splashes : MonoBehaviour
{

   // [SerializeField] private GameObject[] Splashes;
    [SerializeField] private List<ItemData> Splashes = new List<ItemData>();
    void Awake()
    {
        //foreach (GameObject go in Splashes)
        //{
        //    go.SetActive(false);
        //}

        foreach (ItemData data in Splashes)
        {
            data.targetObject.SetActive(false);
        }
    }


    public void ShowSplash(int UISlot)
    {

        foreach (ItemData data in Splashes)
        {
            if (data.Reference == UISlot)
            {
                data.targetObject.SetActive(true);
                StartCoroutine(WaitAndExecute(data.duration));
            }
        }
        
    }

    IEnumerator WaitAndExecute(float waittime)
    {
        yield return new WaitForSeconds(waittime);

        Menu menuToClose = PlayerMenus.GetMenuWithName("UI_Splashes");

        if (menuToClose != null)
        {
            foreach (ItemData data in Splashes)
            {
                data.targetObject.SetActive(false);
            }
            // Turn off the menu safely
            menuToClose.TurnOff();
        }
    }


}
