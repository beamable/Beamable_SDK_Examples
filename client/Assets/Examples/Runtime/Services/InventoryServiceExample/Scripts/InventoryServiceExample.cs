using System.Collections.Generic;
using System.Linq;
using Beamable.Common.Api.Inventory;
using Beamable.Common.Content;
using Beamable.Common.Inventory;
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

   var beamableAPI = await Beamable.API.Instance;
      
   // All items (Available in game)
   beamableAPI.ContentService.Subscribe(clientManifest =>
   {
      Debug.Log($"#1. ContentService, all object count = {clientManifest.entries.Count}");
   });

   // Filtered items (Available in game)
   beamableAPI.ContentService.Subscribe("items", clientManifest =>
   {
      Debug.Log($"#2. ContentService, filtered 'items' object count = {clientManifest.entries.Count}");
   });
         
         // Filtered items (Owned by current player)
         _beamableAPI.InventoryService.Subscribe(ContentType, view =>
         {
            Debug.Log($"#3. PLAYER - InventoryService, '{ContentType}' items count = {view.items.Count}");

            _inventoryServiceExampleData.InventoryItemNames.Clear();
            foreach (KeyValuePair<string, List<ItemView>> kvp in view.items)
            {
               string inventoryItemName = $"{kvp.Key} ({kvp.Value.Count})";
               Debug.Log($"\tinventoryItemName = {inventoryItemName}");
               
               foreach (ItemView itemView in kvp.Value)
               {
                  Debug.Log($"\t\tvalue = {itemView.id}");
               }

               _inventoryServiceExampleData.InventoryItemNames.Add(inventoryItemName);
            }
            
            Refresh();
         });

         // All items (Owned by current player)
         _beamableAPI.InventoryService.Subscribe(view =>
         {
            Debug.Log($"#4. PLAYER - InventoryService, all items count = {view.items.Count}");

            _inventoryServiceExampleData.InventoryItemNames.Clear();
            foreach (KeyValuePair<string, List<ItemView>> kvp in view.items)
            {
               string inventoryItemName = $"{kvp.Key} ({kvp.Value.Count})";
               Debug.Log($"\tinventoryItemName = {inventoryItemName}");
               
               foreach (ItemView itemView in kvp.Value)
               {
                  Debug.Log($"\t\tvalue = {itemView.id}");
               }
              
               _inventoryServiceExampleData.InventoryItemNames.Add(inventoryItemName);
            }

            Refresh();
         });

      }

      public void Refresh()
      {
         Debug.Log($"Refresh()");
         Debug.Log($"\tContentType = {ContentType}");
         Debug.Log($"\tGameContent.Count = {_inventoryServiceExampleData.ContentObjectNames.Count}");
         Debug.Log($"\tPlayerInventory.Count = {_inventoryServiceExampleData.InventoryItemNames.Count}");

         OnRefreshed?.Invoke(_inventoryServiceExampleData);
      }

      public async void AddOneItem()
      {
         // Give an item (To current player from items available in game)
         await _beamableAPI.InventoryService.AddItem(_itemToAdd.Id, new Dictionary<string, string>()).Then(obj =>
         {
            Debug.Log($"#5. PLAYER - InventoryService, AddOneItem = {_itemToAdd.Id}");
            
         });
      }

      public async void DeleteOneItem()
      {
         string contentId = _itemToDelete.Id;

         List<InventoryObject<ItemContent>> itemContents =
            _beamableAPI.InventoryService.GetItems<ItemContent>().GetResult();

         if (itemContents.Count == 0)
         {
            Debug.Log($"#6. PLAYER DeleteOneItem() failed. Player has no such item yet.");
            return;
         }
         
         long itemIdToDelete = itemContents.First().Id;

         await _beamableAPI.InventoryService.DeleteItem(contentId, itemIdToDelete).Then(obj =>
         {
            Debug.Log($"#6. PLAYER DeleteOneItem() success. 1 player item for {contentId} is deleted.");
                     
         });
      }
   }
}

