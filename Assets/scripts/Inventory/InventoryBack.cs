using System.Collections;
using System.Collections.Generic;

public class InventoryBack
{
    public InventoryBack(int cap, List<item> inv)
    {
        this.capacity = cap;
        this.inventory = inv;
    }
    public InventoryBack(int cap)
    {
        this.capacity = cap;
        inventory = new List<item>(capacity);
    }

    public List<item> inventory;
    public int capacity = 10;
    public string graf;


    public bool Add_item(item it)
    {
        while (it.count > 0) {
            item adding_slot = inventory.Find(x => (x.count < x.maxCount && x.id == it.id));
            if (adding_slot != null) {
                int ind = inventory.IndexOf(adding_slot);
                if (adding_slot.maxCount >= adding_slot.count + it.count)
                {
                    adding_slot.count += it.count;
                    break;
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

    public void Add_air(item i, int count)
    {
        item it = new item(i);
        it.count = count;
        Add_item(it);
    }

    void Add_Graphics(){
        graf = "";
        int ind = 0;
        foreach (var item in inventory)
        {
            graf += ind + item.title + ", " + item.count + "\n";
            ind++;
        }
    }

}
