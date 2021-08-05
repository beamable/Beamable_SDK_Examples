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
      public bool IsInteractable { get { return IsChangedContentService && IsChangedInventoryService;}}
      public bool IsChangedContentService = false;
      public bool IsChangedInventoryService = false;
      public CurrencyContent CurrencyContentPrimary = null;
      public CurrencyContent CurrencyContentSecondary = null;
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
      [SerializeField] private CurrencyRef _currencyRefPrimary = null;
      [SerializeField] private CurrencyRef _currencyRefSecondary = null;
      
      private IBeamableAPI _beamableAPI;
      private const int CurrencyDeltaPerClick = 1;
      private const string ContentType = "currency";
      private readonly InventoryCurrencyExampleData _inventoryCurrencyExampleData = new InventoryCurrencyExampleData();
         
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

         _inventoryCurrencyExampleData.CurrencyContentPrimary = 
            await _currencyRefPrimary.Resolve();
         
         _inventoryCurrencyExampleData.CurrencyContentSecondary = 
            await _currencyRefSecondary.Resolve();

         // All currencies (Available in game)
         _beamableAPI.ContentService.Subscribe(ContentService_OnChanged);
         
         // Filtered currencies (Owned by current player)
         _beamableAPI.InventoryService.Subscribe(ContentType, InventoryService_OnChanged);

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
      
      


      
      public async void AddPrimaryCurrency()
      {
         InventoryUpdateBuilder inventoryUpdateBuilder = new InventoryUpdateBuilder();
         inventoryUpdateBuilder.CurrencyChange(_inventoryCurrencyExampleData.CurrencyContentPrimary.Id, 
            CurrencyDeltaPerClick);

         // Add
         await _beamableAPI.InventoryService.Update(inventoryUpdateBuilder).Then(obj =>
         {
            Debug.Log($"#1. PLAYER AddPrimaryCurrency2() success.");
                     
         });
      }

      
      public async void RemovePrimaryCurrency()
      {
         InventoryUpdateBuilder inventoryUpdateBuilder = new InventoryUpdateBuilder();
         
         // Remove
         inventoryUpdateBuilder.CurrencyChange(_inventoryCurrencyExampleData.CurrencyContentPrimary.Id, 
            -CurrencyDeltaPerClick);

         await _beamableAPI.InventoryService.Update(inventoryUpdateBuilder).Then(obj =>
         {
            Debug.Log($"#2. PLAYER RemovePrimaryCurrency() success.");
                     
         });
      }

      public async void TradePrimaryToSecondary()
      {
         InventoryUpdateBuilder inventoryUpdateBuilder = new InventoryUpdateBuilder();
         
         // Remove Primary
         inventoryUpdateBuilder.CurrencyChange(_inventoryCurrencyExampleData.CurrencyContentPrimary.Id, 
            -CurrencyDeltaPerClick);
         
         // Add Secondary
         inventoryUpdateBuilder.CurrencyChange(_inventoryCurrencyExampleData.CurrencyContentSecondary.Id, 
            CurrencyDeltaPerClick);

         await _beamableAPI.InventoryService.Update(inventoryUpdateBuilder).Then(obj =>
         {
            Debug.Log($"#3. PLAYER TradePrimaryToSecondary() success.");
                     
         });
      }
      
      public async void TradeSecondaryToPrimary()
      {
         InventoryUpdateBuilder inventoryUpdateBuilder = new InventoryUpdateBuilder();
         
         // Remove Secondary
         inventoryUpdateBuilder.CurrencyChange(_inventoryCurrencyExampleData.CurrencyContentSecondary.Id, 
            -CurrencyDeltaPerClick);
         
         // Add Primary
         inventoryUpdateBuilder.CurrencyChange(_inventoryCurrencyExampleData.CurrencyContentPrimary.Id, 
            CurrencyDeltaPerClick);

         await _beamableAPI.InventoryService.Update(inventoryUpdateBuilder).Then(obj =>
         {
            Debug.Log($"#4. PLAYER TradeSecondaryToPrimary() success.");
                     
         });
      }
      
      
      public void ResetPlayer()
      {
         ExampleProjectHacks.ClearDeviceUsersAndReloadScene();
         Debug.Log($"#5. ResetPlayer() success.");
      }


      
      //  Event Handlers  -------------------------------
      private void ContentService_OnChanged(ClientManifest clientManifest)
      {
         _inventoryCurrencyExampleData.IsChangedContentService = true;
         
         // Filter to ONLY CurrencyContent
         List<ClientContentInfo> clientContentInfos =  clientManifest.entries.Where((clientContentInfo, i) => 
               ExampleProjectHelper.IsMatchingClientContentInfo(clientContentInfo, ContentType)).ToList();
         
         Debug.Log($"GAME - ContentService_OnChanged, " +
                   $"currencies.Count = {clientContentInfos.Count}");
         
         _inventoryCurrencyExampleData.ContentCurrencyNames.Clear();
         foreach (ClientContentInfo clientContentInfo in clientContentInfos)
         {
            string currencyName = ExampleProjectHelper.GetDisplayNameFromContentId(clientContentInfo.contentId);
            string currencyNameFormatted = $"{currencyName}";
            _inventoryCurrencyExampleData.ContentCurrencyNames.Add(currencyNameFormatted);
         }
         
         // Alphabetize
         _inventoryCurrencyExampleData.ContentCurrencyNames.Sort();
         
         Refresh();
      }
      
      private void InventoryService_OnChanged(InventoryView inventoryView)
      {
         _inventoryCurrencyExampleData.IsChangedInventoryService = true;
         
         Debug.Log($"PLAYER - InventoryService_OnChanged, " +
                   $"currencies.Count = {inventoryView.currencies.Count}");

         _inventoryCurrencyExampleData.InventoryCurrencyNames.Clear();
         foreach (KeyValuePair<string, long> kvp in inventoryView.currencies)
         {
            string currencyName = ExampleProjectHelper.GetDisplayNameFromContentId(kvp.Key);
            string currencyNameFormatted = $"{currencyName} x {kvp.Value}";
            _inventoryCurrencyExampleData.InventoryCurrencyNames.Add(currencyNameFormatted);
         }
         
         // Alphabetize
         _inventoryCurrencyExampleData.InventoryCurrencyNames.Sort();
         
         Refresh();
      }
   }
}

