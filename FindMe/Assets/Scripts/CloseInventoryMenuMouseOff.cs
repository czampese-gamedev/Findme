using UnityEngine;
using AC;

public class CloseInventoryMenuMouseOff : MonoBehaviour
{
    Menu myInventoryMenu;
    bool HasHoveredOver = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myInventoryMenu = PlayerMenus.GetMenuWithName("Inventory");
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePos = Input.mousePosition;

        if(myInventoryMenu.IsPointInside(mousePos) == true && myInventoryMenu.IsEnabled() == true && HasHoveredOver == false)
        {
            HasHoveredOver = true;
        }

        if (myInventoryMenu.IsPointInside(mousePos)==false && myInventoryMenu.IsEnabled() == true && HasHoveredOver == true)
        {
            HasHoveredOver = false;
            myInventoryMenu.TurnOff();
        }
    }
}
