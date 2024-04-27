﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
        private readonly Dictionary<Vector2Int, InventorySlot> _slotsMap;

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
            throw new NotImplementedException();
        }

        public bool Has(string itemId, int amount)
        {
            throw new NotImplementedException();
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

        public AddItemsToInventoryGridResult AddItems(string itemId, int amount = 1)
        {
            throw new NotImplementedException();
        }

        public AddItemsToInventoryGridResult AddItems(Vectior2Int slotCoords, string itemId, int amount = 1)
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

        public AddItemsToInventoryGridResult RemoveItems(Vectior2Int slotCoords, string itemId, int amount = 1)
        {
            throw new NotImplementedException();
        }

        public int GetItemSlotCapacity(string itemId)
        {
            return 64;
        }
    }
}