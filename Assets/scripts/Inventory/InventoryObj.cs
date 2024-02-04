using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryObj : MonoBehaviour
{
    public List<item> inventory;
    public int capacity = 10;
    public item gear;
    public string graf;

    void Start()
    {
        inventory = new List<item>(capacity);
    }

    public bool Add_item(item it)
    {
        while (it.count > 0) {
            item adding_slot = inventory.Find(x => (x.count < x.maxCount && x.id == it.id));
            if (adding_slot != null) {
                int ind = inventory.IndexOf(adding_slot);
                if (adding_slot.maxCount - adding_slot.count >= it.count)
                {
                    adding_slot.count += it.count;
                    inventory[ind] = adding_slot;
                }
                else
                {
                    it.count -= adding_slot.maxCount - adding_slot.count;
                    adding_slot.count = adding_slot.maxCount;
                    inventory[ind] = adding_slot;
                }
            }
            else if (inventory.Count < capacity)
            {
                inventory.Add(it);
                break;
            }
            else { return false; }
        }
        Add_Graphics();
        return true;
    }

    public bool Remove_item(item it)
    {
        item removing_slot = inventory.Find(x => (x.count >= it.count && x.id == it.id));
        if (removing_slot == null) { return false; }
        else if (removing_slot.count == it.count)
        {
            inventory.Remove(removing_slot);
        }
        else
        {
            int ind = inventory.IndexOf(removing_slot);
            removing_slot.count -= it.count;
            inventory[ind] = removing_slot;
        }
        Add_Graphics();
        return true;
    }

    public void Add_air()
    {
        item it = gear;
        it.count = 1;
        Add_item(it);
    }

    void Add_Graphics(){
        graf = "";
        int ind = 0;
        foreach (var item in inventory)
        {
            graf += ind + item.name + ", " + item.count + "\n";
            ind++;
        }
    }

}
