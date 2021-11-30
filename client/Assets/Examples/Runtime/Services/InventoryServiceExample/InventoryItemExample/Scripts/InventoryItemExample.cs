using System.Collections.Generic;
using System.Linq;
using Beamable.Common.Api.Inventory;
using Beamable.Common.Content;
using Beamable.Common.Inventory;
using Beamable.Examples.Shared;
using UnityEngine;
using UnityEngine.Events;

namespace Beamable.Examples.Services.InventoryService.InventoryItemExample
{
   /// <summary>
   /// Holds data for use in the <see cref="InventoryItemExampleUI"/>.
   /// </summary>
   [System.Serializable]
   public class InventoryItemExampleData
   {
      public bool IsInteractable { get { return IsChangedContentService && IsChangedInventoryService;}}
      public bool IsChangedContentService = false;
      public bool IsChangedInventoryService = false;
      public string ItemToAddName = "";
      public string ItemToDeleteName = "";
      public List<string> ContentObjectNames = new List<string>();
      public List<string> InventoryItemNames = new List<string>();
   }
   
   [System.Serializable]
   public class InventoryItemExampleEvent : UnityEvent<InventoryItemExampleData> { }

   /// <summary>
   /// Demonstrates <see cref="InventoryService"/>.
   /// </summary>
   public class InventoryItemExample : MonoBehaviour
   {
      //  Events  ---------------------------------------
      [HideInInspector]
      public InventoryItemExampleEvent OnRefreshed = new InventoryItemExampleEvent();
      
      //  Fields  ---------------------------------------
      [SerializeField] private ArmorRef _itemToAdd = null;
      [SerializeField] private ArmorRef _itemToDelete = null;
      
      private IBeamableAPI _beamableAPI;
      private const string ContentType = "items";
      private InventoryItemExampleData _inventoryItemExampleData = new InventoryItemExampleData();

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
         _inventoryItemExampleData.ItemToAddName = armorToAdd.Name;

         Armor armorToDelete = await _itemToDelete.Resolve();
         _inventoryItemExampleData.ItemToDeleteName = armorToDelete.Name;

         // All items (Available in game)
         _beamableAPI.ContentService.Subscribe(ContentService_OnChanged);
         
         // Filtered items (Owned by current player)
         _beamableAPI.InventoryService.Subscribe(ContentType, InventoryService_OnChanged);

      }

      
      public void Refresh()
      {
         string refreshLog = $"Refresh() ...\n" +
                             $"\n * ContentType = {ContentType}" +
                             $"\n * GameContent.Count = {_inventoryItemExampleData.ContentObjectNames.Count}" +
                             $"\n * PlayerInventory.Count = {_inventoryItemExampleData.InventoryItemNames.Count}\n\n";
            
         //Debug.Log(refreshLog);

         OnRefreshed?.Invoke(_inventoryItemExampleData);
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

         List<InventoryObject<ItemContent>> allOwnedItems =
            _beamableAPI.InventoryService.GetItems<ItemContent>().GetResult();

         if (allOwnedItems == null || allOwnedItems.Count == 0)
         {
            Debug.Log($"#4. PLAYER DeleteOneItem() failed. Player has no items yet.");
            return;
         }

         long itemIdToDelete = 0;
         foreach (var x in allOwnedItems)
         {
            //Checks the CONTENT id
            if (x.ItemContent.Id == _itemToDelete.Id)
            {
               //Stores the INVENTORY id
               itemIdToDelete = x.Id;
               break;
            }
         }
         
         if (itemIdToDelete == 0)
         {
            Debug.Log($"#4. PLAYER DeleteOneItem() failed. Player has no items of that type yet.");
            return;
         }
         
         await _beamableAPI.InventoryService.DeleteItem(contentId, itemIdToDelete).Then(obj =>
         {
            Debug.Log($"#4. PLAYER DeleteOneItem() success. 1 player item for {contentId} is deleted.");
         });
      }
      
      //  Event Handlers  -------------------------------
      private void ContentService_OnChanged(ClientManifest clientManifest)
      {
         //Debug.Log($"#1. GAME - ContentService, count = {clientManifest.entries.Count}");
         
         _inventoryItemExampleData.ContentObjectNames.Clear();
         foreach (ClientContentInfo clientContentInfo in clientManifest.entries)
         {
            string contentName = ExampleProjectHelper.GetDisplayNameFromContentId(clientContentInfo.contentId);
            string contentItemName = $"{contentName} x 1";
            _inventoryItemExampleData.ContentObjectNames.Add(contentItemName);
         }

         _inventoryItemExampleData.IsChangedContentService = true;
         
         Refresh();
      }
      
      private void InventoryService_OnChanged(InventoryView inventoryView)
      {
         //Debug.Log($"#2. PLAYER - InventoryService, count = {inventoryView.items.Count}");

         _inventoryItemExampleData.InventoryItemNames.Clear();
         foreach (KeyValuePair<string, List<ItemView>> kvp in inventoryView.items)
         {
            string inventoryItemName = $"{kvp.Key} x {kvp.Value.Count}";
            _inventoryItemExampleData.InventoryItemNames.Add(inventoryItemName);
         }
         
         _inventoryItemExampleData.IsChangedInventoryService = true;
         
         Refresh();
      }
   }
}

