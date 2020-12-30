using Core.Platform.SDK.Inventory;
using DisruptorBeam;
using DisruptorBeam.Content;
using System.Collections.Generic;
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

   public class InventoryServiceExample : MonoBehaviour
   {
      public ArmorContentRef armorContentRef;

      protected async void Start()
      {
         Debug.Log("armorContentRef.Id: " + armorContentRef.Id);

         string contentTypeGeneral = "items";
         string contentIdSpecific = "items.is_armor.GoldArmor01";

         Debug.Log("contentTypeGeneral: " + contentTypeGeneral);
         Debug.Log("contentIdSpecific: " + contentIdSpecific);

         await DisruptorEngine.Instance.Then(async de =>
         {

            // All items (Available in game)
            de.ContentService.Subscribe(clientManifest =>
            {
               Debug.Log("1 GAME - ContentService, all items count: " + clientManifest.entries.Count);

               foreach (ClientContentInfo clientContentInfo in clientManifest.entries)
               {
                  Debug.Log("\tcontentId : " + clientContentInfo.contentId);
               }
            });

            // Filtered items (Available in game)
            de.ContentService.Subscribe(contentTypeGeneral, clientManifest =>
            {
               Debug.Log($"2 GAME -  ContentService, '{contentTypeGeneral}' items count: " + clientManifest.entries.Count);

               foreach (ClientContentInfo clientContentInfo in clientManifest.entries)
               {
                  Debug.Log("\tcontentId : " + clientContentInfo.contentId);
               }
            });

            // Give an item (To current player from items available in game)
            await de.InventoryService.AddItem(contentIdSpecific, new Dictionary<string, string>()).Then(obj =>
            {
               Debug.Log($"3 PLAYER - InventoryService, AddItem: " + contentIdSpecific);
            });

            // Remote an item (From current player)
            long itemId = 1;
            await de.InventoryService.DeleteItem(contentIdSpecific, itemId).Then(obj =>
            {
               Debug.Log($"4 PLAYER - InventoryService, RemoveItem: " + contentIdSpecific);
            });

            // All items (Owned by current player)
            de.InventoryService.Subscribe(view =>
            {
               Debug.Log($"5 PLAYER - InventoryService, all items count: " + view.items.Count);

               foreach (KeyValuePair<string, List<ItemView>> kvp in view.items)
               {
                  Debug.Log("\tkey: " + kvp.Key);
                  foreach (ItemView itemView in kvp.Value)
                  {
                     Debug.Log("\t\tvalue: " + itemView.id);
                  }
               }
            });

            // Filtered items (Owned by current player)
            de.InventoryService.Subscribe(contentTypeGeneral, view =>
            {
               Debug.Log($"6 PLAYER - InventoryService, '{contentTypeGeneral}' items count: " + view.items.Count);

               if (view.items.TryGetValue(contentTypeGeneral, out List<ItemView> itemViews))
               {
                  Debug.Log("unlockItems2: " + itemViews.Count);
                  if (itemViews.Count > 0)
                  {
                  }
               }
            });


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
   }
}

