using UnityEngine;
using AC;

public class CloseInventoryMenuMouseOff : MonoBehaviour
{
    Menu myInventoryMenu;
    bool HasHoveredOver = false;

    private void OnEnable()
    {
        EventManager.OnMenuTurnOff += OnMenuTurnOff;
    }

    private void OnDisable()
    {
        EventManager.OnMenuTurnOff -= OnMenuTurnOff;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myInventoryMenu = PlayerMenus.GetMenuWithName("Inventory");
    }

    private void OnMenuTurnOff(Menu menu, bool isInstant)
    {
        if (menu != null && menu.title == "Inventory")
        {
            HasHoveredOver = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePos = UnityEngine.InputSystem.Mouse.current.position.ReadValue();

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
