using UnityEngine;
using System.Collections;

public class playerstuff : MonoBehaviour
{

    public Animator playerAnimController;

   


    void PickRandomSitAnimation()
    {
        int randomIndex = Random.Range(1, 4);
  
        StartCoroutine(AdjustFloat( randomIndex, 1));

    }

    private IEnumerator AdjustFloat(float targetValue, float duration)
    {
        float currVal = playerAnimController.GetFloat("idleSit");
        float startValue = playerAnimController.GetFloat("idleSit");
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            // Calculate progress from 0 to 1
            float percentage = elapsedTime / duration;

            // Apply interpolation
            playerAnimController.SetFloat("idleSit", Mathf.Lerp(startValue, targetValue, percentage));

            yield return null; // Wait for next frame
        }

        // Ensure final value is exact
        playerAnimController.SetFloat("idleSit", targetValue);
    }
}
