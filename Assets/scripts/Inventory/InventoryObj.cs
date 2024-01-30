using System.Collections;
using UnityEngine;

public class InventoryObj : ScriptableObject
{
    public item[,] inventory;
    public Vector2Int capacity;
    void Start()
    {
        inventory = new item[capacity.x, capacity.y];
    }
}
