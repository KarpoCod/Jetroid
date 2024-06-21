using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class ChooseWrldButt : MonoBehaviour
{
    public GameObject button;
    public GameObject EditName;
    public GameObject Delete;
    public GameObject Info;
    BoxCollider2D Collider;

    void Start()
    {
        Collider = button.GetComponent<BoxCollider2D>();
    }

    void OnMouseEnter()
    {
        //Debug.Log("asdasd");
        EditName.SetActive(true);
        Delete.SetActive(true);
        Info.SetActive(true);
        Collider.size = new Vector2(628, 100);
    }

    void OnMouseExit()
    {
        EditName.SetActive(false);
        Delete.SetActive(false);
        Info.SetActive(false);
        Collider.size = new Vector3(480, 100);
    }

    public void Edit()
    {
        Debug.Log("Edit");
    }

    public void DeleteWorld()
    {
        Debug.Log("DeleteWorld");
    }

    public void WorldInfo()
    {
        Debug.Log("WorldInfo");
    }
}
