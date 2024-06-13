using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryOpening : MonoBehaviour
{
    public GameObject inventory;
    public bool InventoryOpened = false;
    void Start()
    {
        inventory = GameObject.Find("Inventory");
        inventory.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("i"))
        {
            if (InventoryOpened)
            {
                inventory.SetActive(false);
                InventoryOpened = false;
            }
            else
            {
                inventory.SetActive(true);
                InventoryOpened = true;
            }
        }
    }
}
