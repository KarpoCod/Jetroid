using UnityEngine;

public class InventoryOpening : MonoBehaviour
{
    public GameObject inventory;

    void OnValidate()
    {
        if (inventory == null) inventory = GameObject.Find("Inventory");
        inventory.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("i"))
        {
            inventory.SetActive(inventory.activeSelf);
        }
    }
}
