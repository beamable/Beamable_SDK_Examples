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


      protected async void Start()
      {
         await Beamable.API.Instance.Then(beamableAPI =>
         {
            _beamableAPI = beamableAPI;

            Debug.Log("Start()");
            Debug.Log("User.id: " + _beamableAPI.User.id);
            Refresh();
         });
      }


      public void Refresh()
      {
         Debug.Log("Refresh()");
         Debug.Log("_contentTypeGeneral: " + _contentTypeGeneral);
         Debug.Log("ItemToAdd: " + ItemToAdd.Id);
         Debug.Log("ItemToDelete: " + ItemToDelete.Id);
         

         // All items (Available in game)
         _beamableAPI.ContentService.Subscribe(clientManifest =>
         {
            Debug.Log("1 GAME - ContentService, all items count: " + clientManifest.entries.Count);

            _clientContentObjectNames.Clear();
            foreach (ClientContentInfo clientContentInfo in clientManifest.entries)
            {
               Debug.Log("\tcontentId : " + clientContentInfo.contentId);
               _clientContentObjectNames.Add(clientContentInfo.contentId);
            }

            OnRefreshedInvokeSafe();
         });

         // Filtered items (Available in game)
         _beamableAPI.ContentService.Subscribe(_contentTypeGeneral, clientManifest =>
         {
            Debug.Log($"2 GAME -  ContentService, '{_contentTypeGeneral}' items count: " + clientManifest.entries.Count);

            foreach (ClientContentInfo clientContentInfo in clientManifest.entries)
            {
               Debug.Log("\tcontentId : " + clientContentInfo.contentId);
            }
         });


         // All items (Owned by current player)
         _beamableAPI.InventoryService.Subscribe(view =>
         {
            Debug.Log($"5 PLAYER - InventoryService, all items count: " + view.items.Count);

            _playerInventoryItemNames.Clear();
            foreach (KeyValuePair<string, List<ItemView>> kvp in view.items)
            {
               Debug.Log("\tkey: " + kvp.Key);
               foreach (ItemView itemView in kvp.Value)
               {
                  Debug.Log("\t\tvalue: " + itemView.id);
               }
               _playerInventoryItemNames.Add($"{kvp.Key} ({kvp.Value.Count})");
            }

            OnRefreshedInvokeSafe();
         });

         // Filtered items (Owned by current player)
         _beamableAPI.InventoryService.Subscribe(_contentTypeGeneral, view =>
         {
            Debug.Log($"6 PLAYER - InventoryService, '{_contentTypeGeneral}' items count: " + view.items.Count);

            if (view.items.TryGetValue(_contentTypeGeneral, out List<ItemView> itemViews))
            {
               Debug.Log("unlockItems2: " + itemViews.Count);
               if (itemViews.Count > 0)
               {
               }
            }
         });

         //   foreach (var kvp in inventory.items)
         //   {
         //      if (!kvp.Key.StartsWith("items.car")) continue; // TODO: This is a bug with the inventory subscription. We only subscribed to items.car, but are getting back more itmes...
         //      Debug.Log(kvp.Key);
         //      var carKey = new CarRef();
         //      carKey.Id = kvp.Key;
         //      var content = await de.ContentService.GetContent(carKey);
         //      foreach (var val in kvp.Value)
         //      {
         //         var carValue = await CarContent.Generate(content, val.id, val.properties);
         //         carValue.SetIdAndVersion(content.Id, content.Version);
         //         carList.Add(carValue);
         //      }
         //   }

         //});
            
      }



      private void OnRefreshedInvokeSafe()
      {
         OnRefreshed?.Invoke(_clientContentObjectNames, _playerInventoryItemNames);
      }


      public async void Add1Item()
      {
         // Give an item (To current player from items available in game)
         await _beamableAPI.InventoryService.AddItem(ItemToAdd.Id, new Dictionary<string, string>()).Then(obj =>
         {
            Debug.Log($"3 PLAYER - InventoryService, AddItem: " + ItemToAdd.Id);
         });

      }


      public void Delete1Item()
      {
         string contentId = ItemToDelete.Id;
         // 
         _beamableAPI.InventoryService.Subscribe(contentId, async view =>
         {
            if (view.items.TryGetValue(contentId, out List<ItemView> itemViews))
            {
               if (itemViews.Count == 0)
               {
                  Debug.Log($"DeleteItem() failed. Player has no item for {contentId}.");
               }
               else
               {
                  // Delete an item (From current player)
                  long itemIdToDelete = itemViews[0].id;

                  await _beamableAPI.InventoryService.DeleteItem(contentId, itemIdToDelete).Then(obj =>
                  {
                     Debug.Log($"DeleteItem() success. 1 player item for {contentId} is deleted.");
                  });
               }
            }
            else
            {
               Debug.Log($"DeleteItem() failed... Player has no item for {contentId}.");
            }
         });
      }

      public void DeleteAllItem()
      {
         throw new NotImplementedException();
      }
   }
}

