using AC;
using UnityEngine;

public class CZ_followobject : MonoBehaviour
{

    public Transform followobject;
    public float speed = 100f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, followobject.position, speed * Time.deltaTime);
    }
}
