using UnityEngine;
using AC;
using TMPro;
using UnityEngine.UI;
using System.Text;
using System.Collections.Generic;

public class TestingOptions : MonoBehaviour
{
    public Transform InventoryListPanel;
    public GameObject InventoryItemTickboxPrefab;
    private void OnEnable()
    {
        GetInventoryItems();
    }

        void GetInventoryItems()
    {
        InvItem[] allDefinedItems = KickStarter.inventoryManager.items.ToArray();

        foreach (InvItem item in allDefinedItems)
        {

            // 1. Instantiate the Toggle as a child of the container
            GameObject newToggleGO = Instantiate(InventoryItemTickboxPrefab, InventoryListPanel);
            newToggleGO.name = item.id.ToString();  
            newToggleGO.GetComponentInChildren<Text>().text = item.label;
            // 2. Access the Toggle component to configure it
            Toggle toggleComponent = newToggleGO.GetComponent<Toggle>();

            if (KickStarter.runtimeInventory.PlayerInvCollection.Contains(item.id))
            {
                toggleComponent.isOn = true;    
            }else
            {
                toggleComponent.isOn = false;
            }
            toggleComponent.onValueChanged.AddListener(delegate {
                OnToggleValueChanged(toggleComponent);
            });
        }

        void OnToggleValueChanged(Toggle change)
        {
            if (change.isOn)
            {
                KickStarter.runtimeInventory.PlayerInvCollection.Add(new InvInstance(int.Parse(change.name)));
                Debug.Log("Toggle " + change.GetComponentInChildren<Text>().text + " is now ON");
                // Add your "ON" logic here
            }
            else
            {
                KickStarter.runtimeInventory.PlayerInvCollection.DeleteAllOfType(int.Parse(change.name));
                Debug.Log("Toggle " + change.GetComponentInChildren<Text>().text + " is now OFF");
                // Add your "OFF" logic here
            }
        }

    }




}
