using System;
using System.Collections.Generic;
using Beamable.Common.Api.Inventory;
using Beamable.Common.Content;
using Beamable.Common.Inventory;
using UnityEngine;

namespace Beamable.Examples.Services.InventoryService
{
   [ContentType("is_armor")]
   public class Armor : ItemContent
   {
      public string Name;
      public string Defense;
   }

   [System.Serializable]
   public class ArmorContentRef : ContentRef<Armor> { }

   /// <summary>
   /// Demonstrates <see cref="InventoryService"/>.
   /// </summary>
   public class InventoryServiceExample : MonoBehaviour
   {
      public event Action<List<string>,List<string>> OnRefreshed;
      public ArmorContentRef ItemToAdd;
      public ArmorContentRef ItemToDelete;
      //
      private IBeamableAPI _beamableAPI;
      private string _contentTypeGeneral = "items";
      private List<string> _clientContentObjectNames = new List<string>();
      private List<string> _playerInventoryItemNames = new List<string>();


      protected void Start()
      {
         Debug.Log($"Start()");

         SetupBeamable();
      }

      private async void SetupBeamable()
      {
         await Beamable.API.Instance.Then(beamableAPI =>
         {
            _beamableAPI = beamableAPI;
            
            Debug.Log($"beamableAPI.User.id = {_beamableAPI.User.id}");
            
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
            _beamableAPI.ContentService.Subscribe(_contentTypeGeneral, clientManifest =>
            {
               Debug.Log($"#2. GAME - ContentService, '{_contentTypeGeneral}' items count = {clientManifest.entries.Count}");

               foreach (ClientContentInfo clientContentInfo in clientManifest.entries)
               {
                  Debug.Log($"\tcontentId = {clientContentInfo.contentId}");
               }
            });
            
            // Filtered items (Owned by current player)
            _beamableAPI.InventoryService.Subscribe(_contentTypeGeneral, view =>
            {
               Debug.Log($"#3. PLAYER - InventoryService, '{_contentTypeGeneral}' items count = {view.items.Count}");

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


         });
      }

      public void Refresh()
      {
         Debug.Log($"Refresh()");
         Debug.Log($"\t_contentTypeGeneral = {_contentTypeGeneral}");
         Debug.Log($"\t_clientContentObjectNames.Count = {_clientContentObjectNames.Count}");
         Debug.Log($"\t_playerInventoryItemNames.Count = {_playerInventoryItemNames.Count}");
         
         OnRefreshed?.Invoke(_clientContentObjectNames, _playerInventoryItemNames);
         
      }

      public async void Add1Item()
      {
         // Give an item (To current player from items available in game)
         await _beamableAPI.InventoryService.AddItem(ItemToAdd.Id, new Dictionary<string, string>()).Then(obj =>
         {
            Debug.Log($"#5. PLAYER - InventoryService, AddItem = {ItemToAdd.Id}");
         });

      }

      public async void Delete1Item()
      {
         string contentId = ItemToDelete.Id;
         
         //TODO: How do I find this? Afterwards remove the if below - srivello
         long itemIdToDelete = -1;

         if (itemIdToDelete != -1) 
         {
            Debug.Log($"#6. PLAYER DeleteItem() failed. Must have a itemIdToDelete value.");
            return;
         }

         await _beamableAPI.InventoryService.DeleteItem(contentId, itemIdToDelete).Then(obj =>
         {
            Debug.Log($"#6. PLAYER DeleteItem() success. 1 player item for {contentId} is deleted.");
                     
         });
      }

      public void DeleteAllItem()
      {
         throw new NotImplementedException();
      }
   }
}

