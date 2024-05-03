using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
    public class InventoryService
    {

        private readonly Dictionary<string, InventoryGrid> _inventoriesMap = new();

        public InventoryGrid RegisterInventory(InventoryGridData inventoryData)
        {
            var inventory = new InventoryGrid(inventoryData);
            _inventoriesMap[inventory.OwnerId] = inventory;

            return inventory;
        }

        public AddItemsToInventoryGridResult AddItemsToInventory(string OwnerId, string itemId, int amount = 1)
        {
            var inventory = _inventoriesMap[OwnerId];
            return inventory.AddItems(itemId, amount);
        }

        public AddItemsToInventoryGridResult AddItemsToInventorySlot(string OwnerId, Vector2Int slotCords, string itemId, int amount = 1)
        {
            var inventory = _inventoriesMap[OwnerId];
            return inventory.AddItems(slotCords, itemId, amount);
        }

        public RemoveItemsToInventoryGridResult RemoveItems(string OwnerId, string itemId, int amount = 1)
        {
            var inventory = _inventoriesMap[OwnerId];
            return inventory.RemoveItems(itemId, amount);
        }

        public RemoveItemsToInventoryGridResult RemoveItems(string OwnerId, Vector2Int slotCords, string itemId, int amount = 1)
        {
            var inventory = _inventoriesMap[OwnerId];
            return inventory.RemoveItems(slotCords, itemId, amount);
        }

        public bool Has(string OwnerId, string itemId, int amount = 1)
        {
            var inventory = _inventoriesMap[OwnerId];
            return inventory.Has(itemId, amount);
        }

        public ReadOnly.IReadOnlyInventoryGrid GetInventory(string ownerId)
        {
            return _inventoriesMap[ownerId];
        }
    }
}