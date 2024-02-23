using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryOpen : MonoBehaviour
{ 
    public GameObject[] Controlled_obj;

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.I))
        {
            foreach (GameObject go in Controlled_obj) { go.SetActive(!go.activeSelf); } 
        }
    }
}
