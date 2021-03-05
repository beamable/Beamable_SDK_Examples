using System;
using System.Collections.Generic;
using System.Linq;
using Beamable.Common.Api.Inventory;
using Beamable.Common.Content;
using Beamable.Common.Inventory;
using UnityEngine;

namespace Beamable.Examples.Services.InventoryService
{
   [ContentType("armor")]
   public class Armor : ItemContent
   {
      public string Name = "";
      public int Defense = 0;
   }

   [System.Serializable]
   public class ArmorContentRef : ContentRef<Armor> { }

   /// <summary>
   /// Demonstrates <see cref="InventoryService"/>.
   /// </summary>
   public class InventoryServiceExample : MonoBehaviour
   {
      //  Fields  ---------------------------------------
      public event Action<List<string>,List<string>, string, string> OnRefreshed;
      
      [SerializeField] private ArmorContentRef _itemToAdd = null;
      [SerializeField] private ArmorContentRef _itemToDelete = null;
      
      private IBeamableAPI _beamableAPI;
      private const string ContentType = "items";
      private string _itemToAddName = "";
      private string _itemToDeleteName = "";
      
      private List<string> _clientContentObjectNames = new List<string>();
      private List<string> _playerInventoryItemNames = new List<string>();

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
         _itemToAddName = armorToAdd.Name;

         Armor armorToDelete = await _itemToDelete.Resolve();
         _itemToDeleteName = armorToDelete.Name;
         
         // All items (Available in game)
         _beamableAPI.ContentService.Subscribe(clientManifest =>
         {
            Debug.Log($"#1. GAME - ContentService, all items count = {clientManifest.entries.Count}");

            _clientContentObjectNames.Clear();
            foreach (ClientContentInfo clientContentInfo in clientManifest.entries)
            {
               Debug.Log($"\tcontentId = {clientContentInfo.contentId}");
               _clientContentObjectNames.Add(clientContentInfo.contentId);
            }

            Refresh();
         });

         // Filtered items (Available in game)
         _beamableAPI.ContentService.Subscribe(ContentType, clientManifest =>
         {
            Debug.Log($"#2. GAME - ContentService, '{ContentType}' items count = {clientManifest.entries.Count}");

            foreach (ClientContentInfo clientContentInfo in clientManifest.entries)
            {
               Debug.Log($"\tcontentId = {clientContentInfo.contentId}");
            }
         });
         
         // Filtered items (Owned by current player)
         _beamableAPI.InventoryService.Subscribe(ContentType, view =>
         {
            Debug.Log($"#3. PLAYER - InventoryService, '{ContentType}' items count = {view.items.Count}");

            _playerInventoryItemNames.Clear();
            foreach (KeyValuePair<string, List<ItemView>> kvp in view.items)
            {
               Debug.Log($"\tkey = {kvp.Key}");
               foreach (ItemView itemView in kvp.Value)
               {
                  Debug.Log($"\t\tvalue = {itemView.id}");
               }
               _playerInventoryItemNames.Add($"{kvp.Key} ({kvp.Value.Count})");
            }
            
            Refresh();
         });

         // All items (Owned by current player)
         _beamableAPI.InventoryService.Subscribe(view =>
         {
            Debug.Log($"#4. PLAYER - InventoryService, all items count = {view.items.Count}");

            _playerInventoryItemNames.Clear();
            foreach (KeyValuePair<string, List<ItemView>> kvp in view.items)
            {
               Debug.Log($"\tkey = {kvp.Key}");
               foreach (ItemView itemView in kvp.Value)
               {
                  Debug.Log($"\t\tvalue = {itemView.id}");
               }
               _playerInventoryItemNames.Add($"{kvp.Key} ({kvp.Value.Count})");
            }

            Refresh();
         });

      }

      public void Refresh()
      {
         Debug.Log($"Refresh()");
         Debug.Log($"\tContentType = {ContentType}");
         Debug.Log($"\tGameContent.Count = {_clientContentObjectNames.Count}");
         Debug.Log($"\tPlayerInventory.Count = {_playerInventoryItemNames.Count}");
         
         OnRefreshed?.Invoke(_clientContentObjectNames, _playerInventoryItemNames, _itemToAddName, _itemToDeleteName);
         
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

