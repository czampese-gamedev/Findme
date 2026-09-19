using UnityEngine;
using DG.Tweening;
public class Dotween_PlayAll : MonoBehaviour
{

    public DOTweenAnimation[] tweens;

    public void PlayTweens()
    {     
        
        foreach (DOTweenAnimation t in tweens)
        {
           
            t.DORestart();
        }

    }

}
