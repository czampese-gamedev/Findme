using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CustomScriptPersistent : MonoBehaviour
{

    private Volume PP_Volume;
    private DepthOfField PP_DOF;
    private GameObject GlobalVolume;

    private VolumeProfile _volumeProfile;
    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    
    // Update is called once per frame
    void Update()
    {
        
    }

    void ToggleDepthField()
    {
        PP_Volume = FindObjectOfType<Volume>();
        _volumeProfile = PP_Volume.profile;
        _volumeProfile.TryGet(out PP_DOF);
        if (PP_DOF.focalLength == 1)
        {
            PP_DOF.focalLength.value = 55;
        }
        else
        {
            PP_DOF.focalLength.value = 1;
        }



    }
}
