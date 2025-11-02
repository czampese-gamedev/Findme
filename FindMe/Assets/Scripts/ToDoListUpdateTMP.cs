using UnityEngine;
using AC;
using TMPro;
using UnityEngine.UI;
using System.Text;
using System.Collections.Generic;

public class ToDoListUpdateTMP : MonoBehaviour
{

    public TMP_Text ToDoListTextObject; //UI that contains title and main grid

    public GameObject previousPageButton;
    public GameObject nextPageButton;

    public string MainObjectiveHeadingStyle;
    public string MainObjectiveHeadingStyleEnd;
    public string ObjectiveStyle;
    public string ObjectiveStyleEnd;
    public string SubCategoryActiveStyle;
    public string SubCategoryActiveStyleEnd;
    public string SubCategoryCompleteStyle;
    public string SubCategoryCompleteStyleEnd;

    

    public List<string> CategoriesToDisplay = new List<string>();

    private ObjectiveInstance[] mainObjectiveInstances;

    private StringBuilder ToDoText = new StringBuilder();

    private string currentCategory;

    private void OnEnable()
    {
        //subscribe to the onmenuturnon event
        AC.EventManager.OnMenuTurnOn += OnMenuTurnOn;
    }

    private void OnDisable()
    {
        AC.EventManager.OnMenuTurnOn -= OnMenuTurnOn;
    }

    private void OnMenuTurnOn(AC.Menu menu, bool isInstant)
    {
        //When a menu turns on, check if its the 'to do' menu, and run the update if it is
        if (menu.title == "ToDoList")
        {
            UpdateToDoListMenu();
        }
    }
    


    public void UpdateToDoListMenu()
    {
        //Clear the stringbuilder each time the menu is activated
        ToDoText.Clear();
        currentCategory = "blank";


            previousPageButton.SetActive(false);
            nextPageButton.SetActive(false);
   
      


        string currentLevelObjective = AC.LocalVariables.GetVariable("Objective").TextValue;

        //Get a full list of objectives
        mainObjectiveInstances = KickStarter.runtimeObjectives.GetObjectives();

        //Display the current level objectives first:
        foreach (ObjectiveInstance mo in mainObjectiveInstances)
        {
            string catName = KickStarter.inventoryManager.bins[mo.Objective.binID].label;
            if (catName == currentLevelObjective && CategoriesToDisplay.Contains(catName) && mo.CurrentState.Label != "Completed")
            {
                DisplayObjectives(mo, catName);
            }

        }

        //Display the Global objectives 2nd:
        foreach (ObjectiveInstance mo in mainObjectiveInstances)
        {
            string catName = KickStarter.inventoryManager.bins[mo.Objective.binID].label;
            if (catName == "GlobalObjective" && CategoriesToDisplay.Contains(catName) && mo.CurrentState.Label != "Completed")
            {
                DisplayObjectives(mo, catName);
            }
            
        }     

        //Iterate through all the other objectives and set up the UI for each
        foreach (ObjectiveInstance mo in mainObjectiveInstances)
        {
            string catName = KickStarter.inventoryManager.bins[mo.Objective.binID].label;

            if (catName != currentLevelObjective && catName !="GlobalObjective" && CategoriesToDisplay.Contains(catName) && mo.CurrentState.Label != "Completed")
            {
                //If there are any current level objectives or global objectives
                //active then add a page break
             
                DisplayObjectives(mo, catName);                      
            }
        }

        ToDoListTextObject.text = ToDoText.ToString();
    }


private void DisplayObjectives(ObjectiveInstance o, string c)
    {
        if (currentCategory != c)
        {
            if (c == "GlobalObjective")
            {
                ToDoText.AppendLine(MainObjectiveHeadingStyle + "Timeless" + MainObjectiveHeadingStyleEnd);
            }
            else
            {
                if (c != AC.LocalVariables.GetVariable("Objective").TextValue)
                {
                    ToDoText.AppendLine("<page>");
                    previousPageButton.SetActive(true);
                    nextPageButton.SetActive(true);
                }
                ToDoText.AppendLine(MainObjectiveHeadingStyle + c + MainObjectiveHeadingStyleEnd);
            }
            currentCategory = c;
        }

        ToDoText.AppendLine(ObjectiveStyle + o.Objective.description + ObjectiveStyleEnd);

        //Get a list of sub-objectives for this master objective
        ObjectiveInstance[] Subojectives = o.GetSubObjectives();

        //Iterate through sub-objectives and instantiate a UI for each
        foreach (ObjectiveInstance inst in Subojectives)
        {
            string subState = inst.CurrentState.Label;
            string subDescrition = inst.Objective.description;

            //If the sub-objective is completed, then turn on the 'cross out' image
            if (subState == "Completed")
            {
                ToDoText.AppendLine(SubCategoryCompleteStyle + subDescrition + SubCategoryCompleteStyleEnd);
                // newtext.GetComponentInChildren<Image>().enabled = true;
            }
            else
            {
                ToDoText.AppendLine(SubCategoryActiveStyle + subDescrition + SubCategoryActiveStyleEnd);
            }

        }
    }


public void ChangePage(int pagechange)
    {
        if (pagechange < 0)
        {
            if (ToDoListTextObject.pageToDisplay > 1)
            {
                ToDoListTextObject.pageToDisplay = ToDoListTextObject.pageToDisplay + pagechange;
            }

        }

        if (pagechange > 0)
        {
            if (ToDoListTextObject.pageToDisplay < ToDoListTextObject.textInfo.pageCount)
            {
                ToDoListTextObject.pageToDisplay = ToDoListTextObject.pageToDisplay + pagechange;
            }

        }

    }






}
