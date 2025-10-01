using PixelCrushers;
using PixelCrushers.DialogueSystem;
using UnityEngine;

public class ContinueFastForward : MonoBehaviour
{
    public KeyCode[] continueKeys = new KeyCode[] { KeyCode.Space, KeyCode.Return };

    void Update()
    {
        foreach (var key in continueKeys)
        {
            if (InputDeviceManager.IsKeyDown(key))
            {
                FastForward();
                return;
            }
        }

        if(Input.GetMouseButtonDown(0))
        {
            FastForward();
            return;
        }
    }

    void FastForward()
    {
      
        var typewriterEffect = GetComponent<AbstractTypewriterEffect>();
        if ((typewriterEffect != null) && typewriterEffect.isPlaying)
        {
            typewriterEffect.Stop();
        }
        else
        {
            GetComponentInParent<AbstractDialogueUI>().OnContinue();
        }
    }
}