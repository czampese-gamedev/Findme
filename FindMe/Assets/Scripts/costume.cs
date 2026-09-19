using UnityEngine;
using UnityEngine.SceneManagement;
using AC;
public class costume : MonoBehaviour
{
    private string sceneClothes;

    public GameObject[] clothesBase;
    public GameObject[] clothing2016Uniform;
    public GameObject[] clothes2026;
    public GameObject[] clothesDefault;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
       
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += ChangedActiveScene;
        if (AC.LocalVariables.GetVariable("SceneClothes") != null)
        {
            ChangeCostume();
        }



    }

    private void ChangedActiveScene(Scene current, Scene next)
    {

        Debug.Log("Got in Costume-ChangedActiveScene. Current scene is " + current.name + " and next scene is " + next.name);
        if (KickStarter.stateHandler != null)
        {
            if (AC.LocalVariables.GetVariable("SceneClothes") != null)
            {
                ChangeCostume();
            }

        }

    }

    private void ChangeCostume()
    {
        sceneClothes = AC.LocalVariables.GetVariable("SceneClothes").GetValue();

        foreach (GameObject go in clothing2016Uniform)
        { go.SetActive(false); }

        foreach (GameObject go in clothes2026)
        { go.SetActive(false); }

        foreach (GameObject go in clothesDefault)
        { go.SetActive(false); }


        switch (sceneClothes)
        {
            case "2016Uniform":
                foreach (GameObject go in clothing2016Uniform)
                { go.SetActive(true); }
                break;
            case "2026Uniform":
                foreach (GameObject go in clothes2026)
                { go.SetActive(true); }
                break;

            default:
                foreach (GameObject go in clothesDefault)
                { go.SetActive(true); }
                break;
        }
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDestroy()
    {
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged -= ChangedActiveScene;
    }
}
