using UnityEngine;
using AC;
using TMPro;
using UnityEngine.UI;

public class ToDoListUpdate : MonoBehaviour
{

    public GameObject pagepanel;
    public GameObject detailspanel;
    public Transform mainpanel;
    public TMP_Text subMissionText;

    private void OnEnable() // new
    {
        AC.EventManager.OnMenuTurnOn +=  OnMenuTurnOn; 
    }

    private void OnDisable() // new
    {
        AC.EventManager.OnMenuTurnOn -= OnMenuTurnOn;
    }

    private void OnMenuTurnOn(AC.Menu menu, bool isInstant) // changed
    {
        if (menu.title == "ToDoList")
        {
            UpdateToDoListMenu();
        }
    }
    private bool isIntialised = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
     
    }

    //private void OnEnable()
    //{
    //    if (!isIntialised)
    //    {
    //        Debug.Log("calling on enable");
    //       // UpdateToDoListMenu();
    //        isIntialised = true;
    //    }
    //}


    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateToDoListMenu()
    {
        GameObject checkMissionPanel = GameObject.Find("MissionPagePanel(Clone)");
       
        if (checkMissionPanel != null)
        {
            Destroy(checkMissionPanel);
        }

    
        ObjectiveInstance[] mainObjectiveInstances = KickStarter.runtimeObjectives.GetObjectives();

        GameObject newPanel = Instantiate(pagepanel);
        newPanel.transform.SetParent(mainpanel, false);

 

        TMP_Text pagetitle = newPanel.transform.Find("PageTitle").GetComponent<TMP_Text>();
        Transform displaygrid = newPanel.transform.Find("grid");

        foreach (ObjectiveInstance mo in mainObjectiveInstances)
        {
            string catName = KickStarter.inventoryManager.bins[mo.Objective.binID].label;


            if (catName == "Objective1526")
            {
                pagetitle.text = "1526";
           
                ObjectiveInstance[] Subojectives = mo.GetSubObjectives();

                GameObject mainitem = Instantiate(detailspanel);
                Transform mainitemContainer = mainitem.transform.Find("MainMissionContainer");
                mainitemContainer.Find("MainMissionText").GetComponent<TMP_Text>().text = mo.Objective.description;
                Toggle mainitemtoggle = mainitemContainer.Find("Toggle").GetComponent<Toggle>();

                Debug.Log("Current state objective is " + mo.CurrentState.Label);

                if (mo.CurrentState.Label == "Completed")
                {
                    Debug.Log("Main Objective state is Completed");
                    mainitemtoggle.isOn = true;
                }
                else { mainitemtoggle.isOn = false; }


                mainitem.transform.SetParent(displaygrid, false);

                Transform submissionpanel = mainitem.transform.Find("SubMissionPanel");

                foreach (ObjectiveInstance inst in Subojectives)
                {
                    string subState = inst.CurrentState.Label;
                    if (subState != "Completed")
                    {
                        TMP_Text newtext = Instantiate(subMissionText);
                        newtext.transform.SetParent(submissionpanel, false);
                        newtext.text = "- " + inst.Objective.description;
                    }
;                   
                    

                }
            }
        }


    }
}
