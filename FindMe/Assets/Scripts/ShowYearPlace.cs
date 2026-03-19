using UnityEngine;
using UnityEngine.UI;
using TMPro; 
using System.Collections;
using UnityEngine.SceneManagement;
using AC;
public class ShowYearPlace : MonoBehaviour
{

    public TextMeshProUGUI numberText; // Reference to UI Text
    public TextMeshProUGUI locationText; // Reference to UI Text
    public int minNumber = 1;
    public int maxNumber = 100;
    public float tickSpeed = 0.05f; // Time between changes
    public int totalTicks = 20; // How long it ticks
    public AudioClip ticksound;
    
    private string sceneYear;


    public AC.Sound sound;


    private void OnEnable()
    {
        AC.EventManager.OnMenuTurnOn += OnMenuTurnOn;

    }


    IEnumerator RollNumbers()
    {
        for (int i = 0; i < totalTicks; i++)
        {
            // Generate random number (max is exclusive, so +1 for inclusive) [1, 2]
            int randomNumber = Random.Range(minNumber, maxNumber + 1);
            numberText.text = randomNumber.ToString();
            sound.Play();
            yield return new WaitForSeconds(tickSpeed); // Wait before next tick
        }
        // Final number
        numberText.text = sceneYear;
        yield return new WaitForSeconds(2);
        TurnOffMyMenu();

    }


    private void OnMenuTurnOn(Menu menu, bool isInstant)
    {
        // Check if the opened menu is the one you are interested in
        if (menu.title == "ShowYear")
        {
            sceneYear = AC.LocalVariables.GetVariable("Year").GetValue();
            locationText.text = AC.LocalVariables.GetVariable("Location").GetValue();
            StartCoroutine(RollNumbers());
        }
    }

    public void TurnOffMyMenu()
    {
        Menu menu = PlayerMenus.GetMenuWithName("ShowYear");
        if (menu != null)
        {
            menu.TurnOff();
        }
        else
        {
            Debug.LogWarning("ShowYear Menu not found!");
        }
    }

    private void OnDisable()
    {
        // Subscribe to the event
        AC.EventManager.OnMenuTurnOn -= OnMenuTurnOn;
    }

}
