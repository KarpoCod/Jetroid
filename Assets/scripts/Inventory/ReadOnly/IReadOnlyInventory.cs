using System;

namespace Inventory.ReadOnly
{
    public interface IReadOnlyInventory
    {
        event Action<string, int> ItemsAdded;
        event Action<string, int> ItemsRemoved;

        string OwnerId { get; }

        int GetAmount(string ItemId);
        bool Has(string itemId, int amount);
    }
}