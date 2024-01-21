using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryOpen : MonoBehaviour
{ 
    public GameObject InventoryRef;
    public GameObject Minimap;

    private bool InventoryOpened = false;


    void Start()
    {
        InventoryRef.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.F))
        {
            if (InventoryOpened)
            {
                CloseInventory();
            }
            else
            {
                OpenInventory();
            }
        }
    }

    public void OpenInventory()
    {
        InventoryRef.SetActive(true);
        InventoryOpened = true;
        Minimap.SetActive(false);
    }

    public void CloseInventory()
    {
        InventoryRef.SetActive(false);
        InventoryOpened = false;
        Minimap.SetActive(true);
    }
}
