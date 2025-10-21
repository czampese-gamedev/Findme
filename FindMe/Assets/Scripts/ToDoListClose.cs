using UnityEngine;
using AC;

public class ToDoListClose : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
   public void CloseToDoMenu()
    {
        PlayerMenus.GetMenuWithName("ToDoList").TurnOff();


    }
}
