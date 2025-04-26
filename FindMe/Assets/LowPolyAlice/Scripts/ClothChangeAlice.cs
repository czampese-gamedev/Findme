using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;

namespace SimpleLowPolyCharachterAlice
{

#if UNITY_EDITOR

    /// <summary>
    /// Unity script for the Alice clothes change
    /// </summary>

public class ClothChangeAlice : MonoBehaviour
{

    private int clothCurrentHair=0;
    private int clothCurrentExpression=0;
    private int clothCurrentShirt=0;
    private int clothCurrentPants=0;
    private int clothCurrentShoes=0;

    // Start is called before the first frame update
    void Start()
    {
       
    }

    //Pieces of clothing functions

    public void setHair(int cl) {
       
        transform.GetChild(0).transform.GetChild(clothCurrentHair).gameObject.SetActive(false);
        EditorUtility.SetDirty(transform.GetChild(0).transform.GetChild(clothCurrentHair).gameObject);
        transform.GetChild(0).transform.GetChild(cl).gameObject.SetActive(true);
        EditorUtility.SetDirty(transform.GetChild(0).transform.GetChild(cl).gameObject);
        clothCurrentHair = cl;
    }


    public void setExpression(int cl) {
       
        transform.GetChild(1).transform.GetChild(clothCurrentExpression).gameObject.SetActive(false);
        EditorUtility.SetDirty(transform.GetChild(1).transform.GetChild(clothCurrentExpression).gameObject);
        transform.GetChild(1).transform.GetChild(cl).gameObject.SetActive(true);
        EditorUtility.SetDirty(transform.GetChild(1).transform.GetChild(cl).gameObject);
        clothCurrentExpression = cl;
    }

    public void setShirt(int cl) {
       
        transform.GetChild(2).transform.GetChild(clothCurrentShirt).gameObject.SetActive(false);
        EditorUtility.SetDirty(transform.GetChild(2).transform.GetChild(clothCurrentShirt).gameObject);
        transform.GetChild(2).transform.GetChild(cl).gameObject.SetActive(true);
        EditorUtility.SetDirty(transform.GetChild(2).transform.GetChild(cl).gameObject);
        clothCurrentShirt = cl;
    }

    public void setPants(int cl) {
       
        transform.GetChild(3).transform.GetChild(clothCurrentPants).gameObject.SetActive(false);
        EditorUtility.SetDirty(transform.GetChild(3).transform.GetChild(clothCurrentPants).gameObject);
        transform.GetChild(3).transform.GetChild(cl).gameObject.SetActive(true);
        EditorUtility.SetDirty(transform.GetChild(3).transform.GetChild(cl).gameObject);
        clothCurrentPants = cl;
    }

    public void setShoes(int cl) {
       
        transform.GetChild(4).transform.GetChild(clothCurrentShoes).gameObject.SetActive(false);
        EditorUtility.SetDirty(transform.GetChild(4).transform.GetChild(clothCurrentShoes).gameObject);
        transform.GetChild(4).transform.GetChild(cl).gameObject.SetActive(true);
        EditorUtility.SetDirty(transform.GetChild(4).transform.GetChild(cl).gameObject);
        clothCurrentShoes = cl;
    }

   //Initial clothes setup
   public int clothCheck(int level){

    int count=childCount(level);
    int activeChild=0;
    int iter;
    for (iter=0;iter<count;iter++) {
        if (transform.GetChild(level).transform.GetChild(iter).gameObject.activeSelf==true){activeChild=iter;}
    }
    return activeChild;
    }

    //Children count
    public int childCount(int level){
    var children=0;
    children=transform.GetChild(level).transform.childCount;
    return children;
    
    }

}


#endif

}