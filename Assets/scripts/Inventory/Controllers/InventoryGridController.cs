using System;
using System.Collections.Generic;

namespace Inventory
{
    public class InventoryGridController
    {
        private readonly List<InventorySlotController> _slotControllers = new();

        public InventoryGridController(ReadOnly.IReadOnlyInventoryGrid Inventory, InventoryView view)
        {
            var size = Inventory.Size;
            var slots = Inventory.GetSlots();
            var lineLength = size.y;

            for (var i = 0; i < size.x; i++)
            {
                for (var j = 0; j < size.y; j++)
                {
                    var index = i * lineLength + j;
                    var slotView = view.GetInventorySlotView(index);
                    var slot = slots[i, j];
                    _slotControllers.Add(new InventorySlotController(slot, slotView));
                }
            }
        }
    }
}
