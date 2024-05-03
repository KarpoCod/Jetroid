using System;
using System.Collections.Generic;
using System.ComponentModel.Design;

namespace Inventory
{
    public class InventorySlotController
    {
        private readonly InventorySlotView _view;

        public InventorySlotController(ReadOnly.IReadOnlyInventorySlot slot, InventorySlotView view)
        {
            _view = view;

            slot.ItemIdChanged += OnSlotItemIdChanged;
            slot.ItemAmountChanged += OnSlotItemAmountChanged;

            view.Title = slot.ItemId;
            view.Amount = slot.Amount;
        }

        private void OnSlotItemIdChanged(string newItemId)
        {
            _view.Title = newItemId;
        }

        private void OnSlotItemAmountChanged(int newAmount) 
        {
            _view.Amount = newAmount;
        }
    } 
}
