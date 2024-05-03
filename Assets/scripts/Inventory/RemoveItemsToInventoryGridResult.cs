namespace Inventory
{
    public readonly struct RemoveItemsToInventoryGridResult
    {
        public readonly string InventoryOwnerId;
        public readonly int ItemsToRemoveAmount;
        public readonly bool Success;

        
        public RemoveItemsToInventoryGridResult(
            string inventoryOwnerId, int itemsToRemoveAmount, bool success)
        {
            InventoryOwnerId = inventoryOwnerId;
            ItemsToRemoveAmount = itemsToRemoveAmount;
            Success = success;
        }
    }
}