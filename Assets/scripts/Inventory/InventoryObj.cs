using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryObj : MonoBehaviour
{
    public List<item> inventory;
    public int capacity = 10;
    public item gear;

    void Start()
    {
        inventory = new List<item>(capacity);
    }

    public bool Add_item(item it)
    {
        while (it.count > 0) {
            item adding_slot = inventory.Find(x => (x.count < x.maxCount && x.id == it.id));
            if (adding_slot != null) {
                if (adding_slot.maxCount - adding_slot.count > it.count)
                {
                    adding_slot.count += it.count;
                }
                else
                {
                    it.count -= adding_slot.maxCount - adding_slot.count;
                    adding_slot.count = adding_slot.maxCount;
                }
            }
            else if (inventory.Count < capacity)
            {
                inventory.Add(it);
            }
            else { return false; }
        }
        return true;
    }
    public void Add_air()
    {
        gear.count = 1;
        Add_item(gear);
    }

    void Add_Graphics(){ }

}
