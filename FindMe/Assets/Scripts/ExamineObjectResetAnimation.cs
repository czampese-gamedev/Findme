using UnityEngine;

public class ExamineObjectResetAnimation : MonoBehaviour
{

    private Animator[] allChildAnimators;
    public void resetAnimation()
    {
        allChildAnimators = GetComponentsInChildren<Animator>();

        // Example of triggering a parameter on all of them
        foreach (Animator anim in allChildAnimators)
        {
            anim.SetInteger("Status",0);
        }
    }
}
