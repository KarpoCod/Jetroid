using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Inventory
{
    public class InventoryGrid : ReadOnly.IReadOnlyInventoryGrid
    {
        public event Action<string, int> ItemsAdded;
        public event Action<string, int> ItemsRemoved;
        public event Action<Vector2Int> SizeChanged;

        public string OwnerId => _data.OwnerId;

        public Vector2Int Size 
        { 
            get => _data.Size;
            set 
            {
                if(_data.Size != value)
                {
                    _data.Size = value;
                    SizeChanged?.Invoke(value);
                }
            }
        }

        private readonly InventoryGridData _data;
        private Dictionary<Vector2Int, InventorySlot> _slotsMap;

        public InventoryGrid(InventoryGridData data)
        {
            _data = data;

            var size = _data.Size;
            for (int i = 0; i < size.x; i++)
            {
                for (int j = 0; j < size.y; j++)
                {
                    var index = i *size.x + j;
                    var slotData = data.Slots[index];
                    var slot = new InventorySlot(slotData);
                    var position = new Vector2Int(i, j);
                    _slotsMap[position] = slot;
                }
            }
        }

        public int GetAmount(string itemId)
        {
            var amount = 0;
            var slots = _data.Slots;

            foreach (var slot in slots)
            {
                if (slot.ItemId == itemId)
                {
                    amount += slot.Amount;
                }
            }
            return amount;

            //throw new NotImplementedException();
        }

        public bool Has(string itemId, int amount)
        {
            var amountExists = GetAmount(itemId);
            return amountExists >= amount;
        }

        public ReadOnly.IReadOnlyInventorySlot[,] GetSlots()
        {
            var array = new ReadOnly.IReadOnlyInventorySlot[Size.x, Size.y];

            for (int i = 0; i < Size.x; i++)
            {
                for (int j = 0; j < Size.y; j++)
                {
                    var position = new Vector2Int(i, j);
                    array[i, j] = _slotsMap[position];
                }
            }

            return array;
        }

        public void SwitchSlots(Vector2Int SlotCordsA, Vector2Int SlotCordB)
        {
            var slotA = _slotsMap[SlotCordsA];
            var slotB = _slotsMap[SlotCordB];
            var tempSlotItemId = slotA.ItemId;
            var tempSlotAmount = slotA.Amount;
            slotA.ItemId = slotB.ItemId;
            slotA.Amount = slotB.Amount;
            slotB.ItemId = tempSlotItemId;
            slotB.Amount = tempSlotAmount;
        }

        public AddItemsToInventoryGridResult AddItems(string itemId, int amount = 1)
        {
            var remainingAmount = amount;
            var itemsAddedToSlotWithSameItemsAmount = AddToSlotsWithSameItems(itemId, remainingAmount, out remainingAmount);

            if (remainingAmount <= 0)
            {
                return new AddItemsToInventoryGridResult(OwnerId, amount, itemsAddedToSlotWithSameItemsAmount);
            }

            var itemsAddedToAvailableSlotsAmount = AddToFirstAvailableSlots(itemId, remainingAmount, out remainingAmount);
            var totalAddedItemsAmount = itemsAddedToSlotWithSameItemsAmount + itemsAddedToAvailableSlotsAmount;

            return new AddItemsToInventoryGridResult(OwnerId, amount, totalAddedItemsAmount);
        }

        public AddItemsToInventoryGridResult AddItems(Vector2Int slotCoords, string itemId, int amount = 1)
        {
            var slot = _slotsMap[slotCoords];
            var newValue = slot.Amount + amount;
            var itemsAddedAmount = 0;

            if (slot.IsEmpty)
            {
                slot.ItemId= itemId;
            }

            var itemSlotCapacity = GetItemSlotCapacity(itemId);

            if (newValue > itemSlotCapacity)
            {
                var remainingItems = newValue - itemSlotCapacity;
                var itemsToAddAmount = itemSlotCapacity - slot.Amount;
                itemsAddedAmount += itemsToAddAmount;
                slot.Amount = itemSlotCapacity;

                var result = AddItems(itemId, remainingItems);
                itemsAddedAmount += result.ItemsAddedAmount;
            }
            else
            {
                itemsAddedAmount = amount;
                slot.Amount = newValue;
            }

            return new AddItemsToInventoryGridResult(OwnerId, amount, itemsAddedAmount);
        }

        public RemoveItemsToInventoryGridResult RemoveItems(Vector2Int slotCoords, string itemId, int amount = 1)
        {
            var slot = _slotsMap[slotCoords];

            if (slot.IsEmpty || slot.ItemId != itemId || slot.Amount < amount)
            {
                return new RemoveItemsToInventoryGridResult(OwnerId, amount, false);
            }

            slot.Amount -= amount;

            if(slot.Amount == 0)
            {
                slot.ItemId = null;
            }

            return new RemoveItemsToInventoryGridResult(OwnerId, amount, true);
        }

        public RemoveItemsToInventoryGridResult RemoveItems(string itemId, int amount = 1)
        {
            if (!Has(itemId, amount))
            {
                return new RemoveItemsToInventoryGridResult(OwnerId, amount, false);
            }

            var amountToRemove = amount;

            for (var i = 0; i < Size.x;  i++)
            {
                for (var j = 0; j < Size.y; j++)
                {
                    var slotCoords = new Vector2Int(i, j);
                    var slot = _slotsMap[slotCoords];

                    if (slot.ItemId != itemId)
                    {
                        continue;
                    }

                    if (amountToRemove > slot.Amount)
                    {
                        amountToRemove -= slot.Amount;

                        RemoveItems(slotCoords, itemId, slot.Amount);

                        if (amountToRemove == 0)
                        {
                            return new RemoveItemsToInventoryGridResult(OwnerId, amount, true);
                        }
                    }
                    else
                    {
                        RemoveItems(slotCoords, itemId, amountToRemove);
                    }

                }
            }

            throw new Exception("couldn't remove some items...");
        }

        private int AddToSlotsWithSameItems(string itemId, int amount, out int remainingAmount)
        {
            var itemsAddedAmount = 0;
            remainingAmount = amount;

            for (var i = 0; i < Size.x; i++)
            {
                for (var j = 0; j < Size.y; j++)
                {
                    var coords = new Vector2Int(i, j);
                    var slot = _slotsMap[coords];

                    if (slot.IsEmpty)
                    {
                        continue;
                    }
                    var slotCapacity = GetItemSlotCapacity(slot.ItemId);

                    if (slot.ItemId != itemId) { continue; }

                    if (slotCapacity <= slot.Amount) { continue; }

                    var newValue = slot.Amount + remainingAmount;

                    if(newValue > slotCapacity)
                    {
                        remainingAmount = newValue - slotCapacity;
                        var itemsToAddAmount = slotCapacity - slot.Amount;
                        itemsAddedAmount += itemsToAddAmount;
                        slot.Amount = slotCapacity;

                        if (remainingAmount == 0)
                        {
                            return itemsAddedAmount;
                        }
                    }
                    else
                    {
                        itemsAddedAmount += remainingAmount;
                        slot.Amount = newValue;
                        remainingAmount = 0;

                        return itemsAddedAmount;
                    }
                }
            }
            return itemsAddedAmount;
        }

        private int AddToFirstAvailableSlots(string itemId, int amount, out int remainingAmount)
        {
            var itemsAddedAmount = 0;
            remainingAmount = amount;

            for (var i = 0; i < Size.x; i++)
            {
                for (var j = 0; j < Size.y; j++)
                {
                    var coords = new Vector2Int(i, j);
                    var slot = _slotsMap[coords];

                    if (!slot.IsEmpty)
                    {
                        continue;
                    }
                    var slotCapacity = GetItemSlotCapacity(slot.ItemId);

                    var newValue = remainingAmount;

                    if (newValue > slotCapacity)
                    {
                        remainingAmount = newValue - slotCapacity;
                        var itemsToAddAmount = slotCapacity;
                        itemsAddedAmount += itemsToAddAmount;
                        slot.Amount = slotCapacity;
                    }
                    else
                    {
                        itemsAddedAmount += remainingAmount;
                        slot.Amount = newValue;
                        remainingAmount = 0;

                        return itemsAddedAmount;
                    }
                }
            }
            return itemsAddedAmount;
        }

        public int GetItemSlotCapacity(string itemId)
        {
            return 64;
        }
    }
}
