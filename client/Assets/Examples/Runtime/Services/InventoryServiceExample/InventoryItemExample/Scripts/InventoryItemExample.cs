using System.Collections.Generic;
using System.Threading.Tasks;
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

      private BeamContext _beamContext;
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
   
         _beamContext = BeamContext.Default;
         await _beamContext.OnReady;
            
         Debug.Log($"beamContext.PlayerId = {_beamContext.PlayerId}");

         Armor armorToAdd = await _itemToAdd.Resolve();
         _inventoryItemExampleData.ItemToAddName = armorToAdd.Name;

         Armor armorToDelete = await _itemToDelete.Resolve();
         _inventoryItemExampleData.ItemToDeleteName = armorToDelete.Name;

         // All items (Available in game)
         _beamContext.Api.ContentService.Subscribe(ContentService_OnChanged);

         // Filtered items (Owned by current player)
         _beamContext.Api.InventoryService.Subscribe(ContentType, InventoryService_OnChanged);

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
         await _beamContext.Api.InventoryService.AddItem(_itemToAdd.Id, new Dictionary<string, string>()).Then(obj =>
         {
            Debug.Log($"#3. PLAYER - InventoryService, AddOneItem = {_itemToAdd.Id}");
            
         });
      }
      
      public async void GetItems()
      {
         var allOwnedItems = await _beamContext.Api.InventoryService.GetCurrent();

         foreach (var inventoryItem in allOwnedItems.items)
         {
            foreach (var itemView in inventoryItem.Value)
            {
               Debug.Log("inventoryObject: " + itemView.id);
            }
         }
      }

      public async void DeleteOneItem()
      {
         string contentId = _itemToDelete.Id;

         var allOwnedItems = await _beamContext.Api.InventoryService.GetCurrent();

         if (allOwnedItems.items == null || allOwnedItems.items.Count == 0)
         {
            Debug.Log($"#4. PLAYER DeleteOneItem() failed. Player has no items yet.");
            return;
         }
   
         long itemIdToDelete = 0;
         foreach (var item in allOwnedItems.items)
         {
            //Checks the CONTENT id
            if (item.Key == _itemToDelete.Id)
            {
               //Stores the INVENTORY id
               itemIdToDelete = item.Value[0].id;
               break;
            }
         }
         
         if (itemIdToDelete == 0)
         {
            Debug.Log($"#4. PLAYER DeleteOneItem() failed. Player has no items of that type yet.");
            return;
         }
         
         await _beamContext.Api.InventoryService.DeleteItem(contentId, itemIdToDelete).Then(obj =>
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
      
      private async Task UpdateWithBuilder()
      {
         //Create a new instance of an InventoryUpdateBuilder
         var updateBuilder = new InventoryUpdateBuilder();
         //Add some properties to our new item
         var properties = new Dictionary<string, string>
         {
            {"rarity", "common"},
            {"damage", "10"}
         };
         var contentId = "items.DefaultSword";
         //Add a new update to the builder, using the variables we declared earlier
         updateBuilder.AddItem(contentId, properties);

         var currencyId = "currency.Gold";
         var amount = 100;
         //Currency updates can be added as well
         updateBuilder.CurrencyChange(currencyId, amount);
         //Apply the changes, also sending the data to the server
         await _beamContext.Api.InventoryService.Update(updateBuilder);
      }
   }
}

