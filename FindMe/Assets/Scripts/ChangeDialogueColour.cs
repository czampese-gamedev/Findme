using UnityEngine;
using PixelCrushers.DialogueSystem;
using UnityEngine.UI;
using DTT.UI.ProceduralUI;


public class ChangeDialogueColour : MonoBehaviour
    {


    void OnConversationLine(Subtitle subtitle)
        {
        Color color = Color.hotPink;
            // Get actor' node color:
            Actor actor = DialogueManager.masterDatabase.GetActor(subtitle.speakerInfo.id);
        if (!string.IsNullOrEmpty(actor.LookupValue("NodeColor")))
        {
            color = Tools.WebColor(actor.LookupValue("NodeColor"));
        }
        else
        {
            Debug.Log("No Color found - using the default");
        }
            // Get subtitle panel and set color:
            DialogueActor dialogueActor;
            var panel = DialogueManager.standardDialogueUI.conversationUIElements.standardSubtitleControls.GetPanel(subtitle, out dialogueActor);

        panel.portraitName.color = color;

        Transform border = panel.transform.Find("Border");
        if (border) {
            border.GetComponent<Image>().color = color;
        }


        Transform bg = panel.transform.Find("Background");

        if (bg)
        {

            Gradient myGradient = new Gradient();
            GradientColorKey[] colorKeys = new GradientColorKey[2];
            colorKeys[0] = new GradientColorKey(Color.black, 0.0f);
            colorKeys[1] = new GradientColorKey(color, 1.0f);

            GradientAlphaKey[] alphaKeys = new GradientAlphaKey[2];
            alphaKeys[0] = new GradientAlphaKey(1.0f, 0.0f); // Opaque at the start
            alphaKeys[1] = new GradientAlphaKey(1.0f, 1.0f); // Opaque at the end

            myGradient.SetKeys(colorKeys, alphaKeys);
            GradientEffect ge = bg.GetComponent<GradientEffect>();
            ge.Gradient = myGradient;
            ge.Rotation = -15;
            ge.Scale = 1;
            ge.Offset = new Vector2(0.75f, 0f);

        }
            // panel.panel.GetComponent().color = color;
        }
    }

