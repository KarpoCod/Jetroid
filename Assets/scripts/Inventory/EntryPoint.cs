using System;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
    public class EntryPoint : MonoBehaviour
    {
        [SerializeField] private ScreenView _screenView;

        private InventoryService _inventoryService;
        private ScreenController _screenController;
        private string _cachedOwnerId;

        void Start()
        {
            _inventoryService = new InventoryService();

            var inventoryData1 = CreateTestInventory("inventory1");
            _inventoryService.RegisterInventory(inventoryData1);

            var inventoryData2 = CreateTestInventory("inventory2");
            _inventoryService.RegisterInventory(inventoryData2);

            _screenController = new ScreenController(_inventoryService, _screenView);
            _screenController.OpenInventory("inventory1");
            _cachedOwnerId = "inventory1";
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                _screenController.OpenInventory("inventory1");
                _cachedOwnerId = "inventory1";
            }

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                _screenController.OpenInventory("inventory2");
                _cachedOwnerId = "inventory2";
            }
        }

        private InventoryGridData CreateTestInventory(string ownerId)
        {
            var size = new Vector2Int(3, 4);
            var createdInventorySlots = new List<InventorySlotData>();
            var length = size.x * size.y;
            for (var i = 0; i < length; i++)
            {
                createdInventorySlots.Add(new InventorySlotData());
            }

            var createdInventoryData = new InventoryGridData
            {
                OwnerId = ownerId,
                Size = size,
                Slots = createdInventorySlots
            };

            return createdInventoryData;
        }
    }
}

