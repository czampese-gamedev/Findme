using AC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;


public class CustomScriptPersistent : MonoBehaviour
{
    private Volume[] PP_Volumes;
    private DepthOfField PP_DOF;
    private GameObject GlobalVolume;
    private VolumeStack volumeStack;

    private VolumeProfile _volumeProfile;
    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        EventManager.OnChangeLanguage += OnChangeLanguage;
        SetLanguageOnLoad();
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        UnityEngine.SceneManagement.SceneManager.sceneUnloaded += OnSceneUnLoaded;
        PP_Volumes = Object.FindObjectsByType<Volume>(FindObjectsSortMode.None);

    }

    private void OnEnable()
    {
        // 2. Subscribe to the scene loaded event
        
    }

  

    // Update is called once per frame
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PP_Volumes = Object.FindObjectsByType<Volume>(FindObjectsSortMode.None);
        GlobalVariables.SetStringValue(25, scene.name);
        GlobalVariables.SetIntegerValue(26, scene.buildIndex);
    }

    private void OnSceneUnLoaded(Scene scene)
    {
        if (scene.name != "Loading") {
            GlobalVariables.SetStringValue(26, scene.name);
            GlobalVariables.SetIntegerValue(27, scene.buildIndex);
        }
    }

    public void OnChangeLanguage(int Language)
    {
        string currlang = AC.Options.GetLanguageName();
        SetLanguage(currlang);

    }

    void ToggleDepthField()
    {
        /*Used when the pause menu starts 
         This is called via an Event (WhenPauseMenuTurnedOn)
        Which calls an action list ToggleDOF
         */


        foreach (Volume v in PP_Volumes)
        {
            _volumeProfile = v.profile;
            if (_volumeProfile.TryGet(out PP_DOF))
            {
                if (PP_DOF != null)
                {
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
        }
    }

    void SetLanguageOnLoad()
    {
        string currlang = AC.Options.GetLanguageName();
        SetLanguage(currlang);
    }

    void SetLanguage(string lang)
    {
        //Sets dialogue system language to match the AC language. Using this because 
        //AC seems to set the language weirdly if you just let the integration handle it
        //and AC uses index rather than name
        PixelCrushers.DialogueSystem.DialogueManager.SetLanguage(lang);
    }




    private void OnDisable()
    {
        // Unsubscribe
        EventManager.OnChangeLanguage -= OnChangeLanguage;
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
        UnityEngine.SceneManagement.SceneManager.sceneUnloaded -= OnSceneUnLoaded;


    }
}
