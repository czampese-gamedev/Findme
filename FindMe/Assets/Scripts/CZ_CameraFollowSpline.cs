using UnityEngine;
using SplineArchitect;
using AC;

public class CZ_CameraFollowSpline : MonoBehaviour
{
    public SplineArchitect.Spline SplineToFollow;
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    // Update is called once per frame
    void LateUpdate()
    {
       Vector3 splinepos = SplineToFollow.GetNearestPoint(KickStarter.player.transform.position);
        Vector3 newpos = Vector3.Lerp(this.transform.position, splinepos, Time.deltaTime * 2f);


        this.transform.position = newpos;

    }
}
