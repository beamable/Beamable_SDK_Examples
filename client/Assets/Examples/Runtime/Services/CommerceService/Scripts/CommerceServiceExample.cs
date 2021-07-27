using System.Collections.Generic;
using Beamable.Examples.Shared;
using UnityEngine;
using UnityEngine.Events;
using Beamable.Api.Payments;
using Beamable.Common.Api.Inventory;
using Beamable.Common.Inventory;
using Beamable.Common.Shop;

namespace Beamable.Examples.Services.CommerceService
{
    [System.Serializable]
    public class RefreshedUnityEvent : UnityEvent<CommerceServiceExampleData> { }
    
    /// <summary>
    /// Demonstrates <see cref="CommerceService"/>.
    /// </summary>
    public class CommerceServiceExample : MonoBehaviour
    {
        //  Events  ---------------------------------------
        [HideInInspector]
        public RefreshedUnityEvent OnRefreshed = new RefreshedUnityEvent();
        
        //  Fields  ---------------------------------------
        private const string ItemContentType = "items";
        private const string CurrencyContentType = "currency";
        private const string CurrencyType = "currency.Coin";
        private const string EmptyDisplayName = "[Empty]";
        private const string CurrencyDisplayName = "Coin";
        
        [SerializeField]
        private StoreRef _storeRef = null;
        private StoreContent _storeContent = null;
        private IBeamableAPI _beamableAPI = null;
  
        private CommerceServiceExampleData _data = new CommerceServiceExampleData();
    
        //  Unity Methods  --------------------------------
        protected void Start()
        {
            Debug.Log($"Start() Instructions...\n" +
                      " * Ensure Computer's Internet Is Active\n" +
                      " * Run The Scene\n" +
                      " * See Onscreen UI Show HasInternet = true\n" +
                      " * Ensure Computer's Internet Is NOT Active\n" +
                      " * See Onscreen UI Show HasInternet = false\n");

            SetupBeamable();
        }
        
        //  Methods  --------------------------------------
        private async void SetupBeamable()
        {
            _beamableAPI = await Beamable.API.Instance;

            Debug.Log($"beamableAPI.User.id = {_beamableAPI.User.id}");

            _storeContent = await _storeRef.Resolve();

            // Observe Changes
            _beamableAPI.InventoryService.Subscribe(ItemContentType, Inventory_OnChanged);
            _beamableAPI.InventoryService.Subscribe(CurrencyContentType, Currency_OnChanged);
            _beamableAPI.CommerceService.Subscribe(_storeContent.Id, CommerceService_OnChanged);
            
            // Update UI Immediately
            Refresh();
        }
        
        public async void Buy()
        {
            if (_data.SelectedItemData == null)
            {
                Debug.LogError($"BuySelectedStoreItem() failed because _selectedItemData = {_data.SelectedItemData}.");
                return;
            }

            if (!_data.CanAffordSelectedStoreItemData)
            {
                Debug.LogError($"BuySelectedStoreItem() failed because CanAffordSelectedStoreItemData = {_data.CanAffordSelectedStoreItemData}.");
                return;
            }

            // Buy!
            string storeSymbol = _storeContent.Id;
            string listingSymbol = _data.SelectedItemData.PlayerListingView.symbol;
            await _beamableAPI.CommerceService.Purchase(storeSymbol, listingSymbol);
            
        }

        public void ResetPlayer()
        {
            ExampleProjectHacks.ClearDeviceUsersAndReloadScene();
        }

        public void Refresh()
        {
            string refreshLog = $"Refresh() ..." +
                                $"\n * StoreItemDatas.Count = {_data.StoreItemDatas.Count}\n\n" +
                                $"\n * InventoryItemDatas.Count = {_data.InventoryItemDatas.Count}\n\n" +
                                $"\n * CurrencyAmount.Count = {_data.CurrencyAmount}\n\n";
            
            _data.InstructionLogs.Clear();
            _data.InstructionLogs.Add("Click `Buy` to add 1 item to Inventory");
            _data.InstructionLogs.Add("or Click `Reset` to delete and create a new player");
                               
            //Debug.Log(refreshLog);
            
            // Send relevant data to the UI for rendering
            OnRefreshed?.Invoke(_data);
        }
        
        //  Event Handlers  -------------------------------
        private async void Inventory_OnChanged(InventoryView inventoryView)
        {
            _data.InventoryItemDatas.Clear();
            foreach (KeyValuePair<string, List<ItemView>> kvp in inventoryView.items)
            {
                string itemName = ExampleProjectHelper.GetDisplayNameFromContentId(kvp.Key);
                ItemContent itemContent = await CommerceServiceHelper.GetItemContentById(_beamableAPI, kvp.Key);

                string title = $"{itemName} x {kvp.Value.Count}";
                ItemData itemData = new ItemData(title, itemContent, null);
                _data.InventoryItemDatas.Add(itemData);
            }

            if (_data.InventoryItemDatas.Count == 0)
            {
                // Show "Empty"
                _data.InventoryItemDatas.Add(new ItemData(EmptyDisplayName, null, null));
            }

            Refresh();
        }

        private async void Currency_OnChanged(InventoryView inventoryViewForCurrencies)
        {
            _data.CurrencyAmount = 0;
            _data.CurrencyLogs.Clear();
            foreach (KeyValuePair<string, long> kvp in inventoryViewForCurrencies.currencies)
            {
                if (kvp.Key == CurrencyType)
                {
                    _data.CurrencyAmount = (int)kvp.Value;
                }
                string itemName = ExampleProjectHelper.GetDisplayNameFromContentId(kvp.Key);
                CurrencyContent currencyContent = await CommerceServiceHelper.GetCurrencyContentById(_beamableAPI, kvp.Key);
                _data.CurrencyContent = currencyContent;
                _data.CurrencyLogs.Add($"{itemName} x {kvp.Value}");
                break;
            }
            
            if (_data.CurrencyLogs.Count == 0)
            {
                _data.CurrencyLogs.Add(EmptyDisplayName);
            }
            
            Refresh();
        }

        private async void CommerceService_OnChanged(PlayerStoreView playerStoreView)
        {
            _data.StoreItemDatas.Clear();

            foreach (PlayerListingView playerListingView in playerStoreView.listings)
            {
                int price = playerListingView.offer.price.amount;   
                string contentId = playerListingView.offer.obtainItems[0].contentId;
                string itemName = ExampleProjectHelper.GetDisplayNameFromContentId(contentId);
                ItemContent itemContent = await CommerceServiceHelper.GetItemContentById(_beamableAPI, contentId);

                string title = $"{itemName} ({price} {CurrencyDisplayName})";
                ItemData itemData = new ItemData(title, itemContent, playerListingView);
                _data.StoreItemDatas.Add(itemData);
            }
            
            if (_data.StoreItemDatas.Count == 0)
            {
                // Show "Empty"
                _data.StoreItemDatas.Add(new ItemData(EmptyDisplayName, null, null));
            }
            
            Refresh();
        }
    }
}
