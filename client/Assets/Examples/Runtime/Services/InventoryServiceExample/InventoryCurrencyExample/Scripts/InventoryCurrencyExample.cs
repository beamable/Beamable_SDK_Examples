using System.Collections.Generic;
using System.Linq;
using Beamable.Common.Api.Inventory;
using Beamable.Common.Content;
using Beamable.Common.Inventory;
using Beamable.Examples.Shared;
using UnityEngine;
using UnityEngine.Events;

namespace Beamable.Examples.Services.InventoryService.InventoryCurrencyExample
{
   /// <summary>
   /// Holds data for use in the <see cref="InventoryCurrencyExampleUI"/>.
   /// </summary>
   [System.Serializable]
   public class InventoryCurrencyExampleData
   {
      public string CurrencyToAddName = "";
      public string CurrencyToRemoveName = "";
      public List<string> ContentCurrencyNames = new List<string>();
      public List<string> InventoryCurrencyNames = new List<string>();
   }
   
   [System.Serializable]
   public class InventoryCurrencyExampleEvent : UnityEvent<InventoryCurrencyExampleData> { }

   /// <summary>
   /// Demonstrates <see cref="InventoryService"/>.
   /// </summary>
   public class InventoryCurrencyExample : MonoBehaviour
   {
      //  Events  ---------------------------------------
      [HideInInspector]
      public InventoryCurrencyExampleEvent OnRefreshed = new InventoryCurrencyExampleEvent();
      
      //  Fields  ---------------------------------------
      [SerializeField] private CurrencyRef _currencyPrimary = null;
      [SerializeField] private CurrencyRef _currencySecondary = null;
      
      private IBeamableAPI _beamableAPI;
      private const string ContentType = "currency";
      private InventoryCurrencyExampleData _inventoryCurrencyExampleData = new InventoryCurrencyExampleData();

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

         CurrencyContent currencyToAdd = await _currencyPrimary.Resolve();
         _inventoryCurrencyExampleData.CurrencyToAddName = currencyToAdd.ContentName;

         CurrencyContent currencyToRemove = await _currencySecondary.Resolve();
         _inventoryCurrencyExampleData.CurrencyToRemoveName = currencyToRemove.ContentName;

         // All currencies (Available in game)
         _beamableAPI.ContentService.Subscribe(ContentType, ContentService_OnChanged);
         
         // Filtered currencies (Owned by current player)
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
                             $"\n * ContentCurrencyNames.Count = {_inventoryCurrencyExampleData.ContentCurrencyNames.Count}" +
                             $"\n * InventoryCurrencyNames.Count = {_inventoryCurrencyExampleData.InventoryCurrencyNames.Count}\n\n";
            
         //Debug.Log(refreshLog);

         OnRefreshed?.Invoke(_inventoryCurrencyExampleData);
      }

      
      public async void AddCoin()
      {
         long ownedAmount =
            _beamableAPI.InventoryService.GetCurrency(_currencyPrimary).GetResult();
         
         await _beamableAPI.InventoryService.SetCurrency( _currencyPrimary, 
            ownedAmount+1).Then(obj =>
         {
            Debug.Log($"#3. PLAYER AddCoin() success.");
                     
         });
      }

      
      public async void RemoveCoin()
      {
         long ownedAmount =
            _beamableAPI.InventoryService.GetCurrency(_currencyPrimary).GetResult();

         if (ownedAmount == 0)
         {
            Debug.Log($"#4. PLAYER RemoveCoin() failed. Player has no such currency yet.");
            return;
         }
         
         await _beamableAPI.InventoryService.SetCurrency(_currencyPrimary, 
            ownedAmount-1).Then(obj =>
         {
            Debug.Log($"#4. PLAYER RemoveCoin() success.");
                     
         });
      }

      public async void TradeCoinToXPButton()
      {

      }
      
      public async void TradeXPToCoinButton()
      {

      }

      
      //  Event Handlers  -------------------------------
      private void ContentService_OnChanged(ClientManifest clientManifest)
      {
         Debug.Log($"#1. GAME - ContentService, count = {clientManifest.entries.Count}");
         
         _inventoryCurrencyExampleData.ContentCurrencyNames.Clear();
         foreach (ClientContentInfo clientContentInfo in clientManifest.entries)
         {
            string contentName = ExampleProjectHelper.GetDisplayNameFromContentId(clientContentInfo.contentId);
            string contentNameFormatted = $"{contentName} x 1";
            _inventoryCurrencyExampleData.ContentCurrencyNames.Add(contentNameFormatted);
         }
         
         Refresh();
      }
      
      private void InventoryService_OnChanged(InventoryView inventoryView)
      {
         Debug.Log($"#2. PLAYER - InventoryService, count = {inventoryView.currencies.Count}");

         _inventoryCurrencyExampleData.InventoryCurrencyNames.Clear();
         foreach (KeyValuePair<string, long> kvp in inventoryView.currencies)
         {
            string inventoryNameFormatted = $"{kvp.Key} x {kvp.Value}";
            _inventoryCurrencyExampleData.InventoryCurrencyNames.Add(inventoryNameFormatted);
         }
         
         Refresh();
      }
   }
}

