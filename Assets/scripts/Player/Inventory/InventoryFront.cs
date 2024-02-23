using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryFront : MonoBehaviour
{
    public int capacity = 10;
    public item gear;
    public string dfg;
    private InventoryBack InventoryBack;

    void Start()
    {
        InventoryBack = new InventoryBack(capacity);
    }

    public void Add_item(item it)
    {
        bool can_add = InventoryBack.Add_item(it);
        if (!can_add) { Debug.Log(("невозможно добавить", it.count, it.title, "в выбранный инвентарь")); }
        Add_Graphics();
    }

    public void Remove_item(item it)
    {
        bool can_rem = InventoryBack.Remove_item(it);
        if (!can_rem) { Debug.Log(("невозможно убрать", it.count, it.title, "из выбранного инвентаря")); }
        Add_Graphics();
    }

    public void Add_air()
    {
        item it = new item(gear);
        it.count = 2;
        Add_item(it);
    }

    void Add_Graphics()
    {
        dfg = InventoryBack.graf;
    }
}
