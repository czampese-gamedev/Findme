using UnityEngine;
using UnityEngine.SceneManagement;
using AC;
public class costume : MonoBehaviour
{
    private string sceneYear;

    public GameObject[] clothesBase;
    public GameObject[] clothing2016Uniform;
    public GameObject[] clothes2026;
    public GameObject[] clothesDefault;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
    
        UnityEngine.SceneManagement.SceneManager.activeSceneChanged += ChangedActiveScene;

       
    }

    private void ChangedActiveScene(Scene current, Scene next)
    {
  
        sceneYear = AC.LocalVariables.GetVariable("SceneClothes").GetValue();

       

        foreach (GameObject go in clothing2016Uniform)
        { go.SetActive(false); }

        foreach (GameObject go in clothes2026)
        { go.SetActive(false); }

        foreach (GameObject go in clothesDefault)
        { go.SetActive(false); }


        switch (sceneYear)
        {
            case "2016Uniform":
                foreach (GameObject go in clothing2016Uniform)
                { go.SetActive(true); }
                break;
            case "1526":
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
