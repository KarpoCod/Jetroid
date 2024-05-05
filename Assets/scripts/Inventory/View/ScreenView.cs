using UnityEngine;


namespace Inventory
{
    public class ScreenView : MonoBehaviour
    {
        [SerializeField] private InventoryView _inventoryView;
        
        public InventoryView InventoryView => _inventoryView;
    }
}
