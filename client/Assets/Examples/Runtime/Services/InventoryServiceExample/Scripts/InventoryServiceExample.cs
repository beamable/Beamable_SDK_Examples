using System.Collections.Generic;
using System.Linq;
using Beamable.Common.Api.Inventory;
using Beamable.Common.Content;
using Beamable.Common.Inventory;
using Beamable.Examples.Services.CommerceService;
using Beamable.Examples.Shared;
using UnityEngine;
using UnityEngine.Events;

namespace Beamable.Examples.Services.InventoryService
{
   /// <summary>
   /// Holds data for use in the <see cref="InventoryServiceExampleUI"/>.
   /// </summary>
   [System.Serializable]
   public class InventoryServiceExampleData
   {
      public string ItemToAddName = "";
      public string ItemToDeleteName = "";
      public List<string> ContentObjectNames = new List<string>();
      public List<string> InventoryItemNames = new List<string>();
   }
   
   [System.Serializable]
   public class InventoryServiceExampleEvent : UnityEvent<InventoryServiceExampleData> { }

   [System.Serializable]
   public class ArmorContentRef : ContentRef<Armor> { }
   
   /// <summary>
   /// Demonstrates <see cref="InventoryService"/>.
   /// </summary>
   public class InventoryServiceExample : MonoBehaviour
   {
      //  Events  ---------------------------------------
      [HideInInspector]
      public InventoryServiceExampleEvent OnRefreshed = new InventoryServiceExampleEvent();
      
      //  Fields  ---------------------------------------
      [SerializeField] private ArmorContentRef _itemToAdd = null;
      [SerializeField] private ArmorContentRef _itemToDelete = null;
      
      private IBeamableAPI _beamableAPI;
      private const string ContentType = "items";
      private InventoryServiceExampleData _inventoryServiceExampleData = new InventoryServiceExampleData();

      //  Unity Methods  --------------------------------
      protected void Start()
      {
         Debug.Log($"Start()");

         SetupBeamable();
      }

      //  Methods  --------------------------------------
      private async void SetupBeamable()
      { 
   
         _beamableAPI = await Beamable.API.Instance;
            
         Debug.Log($"beamableAPI.User.id = {_beamableAPI.User.id}");

         Armor armorToAdd = await _itemToAdd.Resolve();
         _inventoryServiceExampleData.ItemToAddName = armorToAdd.Name;

         Armor armorToDelete = await _itemToDelete.Resolve();
         _inventoryServiceExampleData.ItemToDeleteName = armorToDelete.Name;

         // All items (Available in game)
         _beamableAPI.ContentService.Subscribe(ContentService_OnChanged);
         
         // Filtered items (Owned by current player)
         _beamableAPI.InventoryService.Subscribe(ContentType, InventoryService_OnChanged);

      }

      public void ResetPlayer()
      {
         ExampleProjectHacks.ClearDeviceUsersAndReloadScene();
      }
      
      
      public void Refresh()
      {
         string refreshLog = $"Refresh() ...\n" +
                             $"\n * ContentType = {ContentType}" +
                             $"\n * GameContent.Count = {_inventoryServiceExampleData.ContentObjectNames.Count}" +
                             $"\n * PlayerInventory.Count = {_inventoryServiceExampleData.InventoryItemNames.Count}\n\n";
            
         //Debug.Log(refreshLog);

         OnRefreshed?.Invoke(_inventoryServiceExampleData);
      }

      
      public async void AddOneItem()
      {
         // Give an item (To current player from items available in game)
         await _beamableAPI.InventoryService.AddItem(_itemToAdd.Id, new Dictionary<string, string>()).Then(obj =>
         {
            Debug.Log($"#3. PLAYER - InventoryService, AddOneItem = {_itemToAdd.Id}");
            
         });
      }

      
      public async void DeleteOneItem()
      {
         string contentId = _itemToDelete.Id;

         List<InventoryObject<ItemContent>> itemContents =
            _beamableAPI.InventoryService.GetItems<ItemContent>().GetResult();

         if (itemContents.Count == 0)
         {
            Debug.Log($"#4. PLAYER DeleteOneItem() failed. Player has no such item yet.");
            return;
         }
         
         long itemIdToDelete = itemContents.First().Id;

         await _beamableAPI.InventoryService.DeleteItem(contentId, itemIdToDelete).Then(obj =>
         {
            Debug.Log($"#4. PLAYER DeleteOneItem() success. 1 player item for {contentId} is deleted.");
                     
         });
      }
      
      //  Event Handlers  -------------------------------
      private void ContentService_OnChanged(ClientManifest clientManifest)
      {
         Debug.Log($"#1. GAME - ContentService, count = {clientManifest.entries.Count}");
         
         _inventoryServiceExampleData.ContentObjectNames.Clear();
         foreach (ClientContentInfo clientContentInfo in clientManifest.entries)
         {
            string contentName = ExampleProjectHelper.GetDisplayNameFromContentId(clientContentInfo.contentId);
            string contentItemName = $"{contentName} x 1";
            _inventoryServiceExampleData.ContentObjectNames.Add(contentItemName);
         }
         
         Refresh();
      }
      
      private void InventoryService_OnChanged(InventoryView inventoryView)
      {
         Debug.Log($"#2. PLAYER - InventoryService, count = {inventoryView.items.Count}");

         _inventoryServiceExampleData.InventoryItemNames.Clear();
         foreach (KeyValuePair<string, List<ItemView>> kvp in inventoryView.items)
         {
            string inventoryItemName = $"{kvp.Key} x {kvp.Value.Count}";
            _inventoryServiceExampleData.InventoryItemNames.Add(inventoryItemName);
         }
         
         Refresh();
      }
   }
}

