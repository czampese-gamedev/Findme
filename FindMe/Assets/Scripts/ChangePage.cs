using TMPro;
using UnityEngine;

public class ChangePage : MonoBehaviour
{
    public TMP_Text tmpText; // Drag your TextMesh Pro component here

    public void NextPage()
    {
        // Check if we aren't on the last page
        if (tmpText.pageToDisplay < tmpText.textInfo.pageCount)
        {
            tmpText.pageToDisplay++;
           // tmpText.maxVisibleCharacters = 0;

        }
    }

    public void PreviousPage()
    {
        // Check if we are past page 1
        if (tmpText.pageToDisplay > 1)
        {
            tmpText.pageToDisplay--;
           // tmpText.maxVisibleCharacters = 0;
        }
    }
}
