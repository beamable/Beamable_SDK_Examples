using System.Collections.Generic;
using Beamable.Common.Inventory;
using UnityEngine;

namespace Beamable.Examples.Services.CommerceService
{
    /// <summary>
    /// Holds data for use in the <see cref="CommerceServiceExampleUI"/>.
    /// </summary>
    [System.Serializable]
    public class CommerceServiceExampleData
    {
        public ItemData SelectedItemData
        {
            get
            {
                // Arbitrarily consider the first index as 'selected'
                // Optional: Add UI to allow user to click to select instead
                if (StoreItemDatas.Count < 1)
                {
                    return null;
                }
                return StoreItemDatas[0];
            }
        }
        
        public bool CanAffordSelectedStoreItemData
        {
            get
            {
                if (SelectedItemData == null)
                {
                    return false;
                }
                else
                {
                    Debug.Log($"{CurrencyAmount}  >= {SelectedItemData.PlayerListingView.offer.price.amount}");
                    return CurrencyAmount >= SelectedItemData.PlayerListingView.offer.price.amount;
                }
            }
        }

        public List<ItemData> StoreItemDatas = new List<ItemData>();
        public List<ItemData> InventoryItemDatas = new List<ItemData>();
        public List<string> InstructionLogs = new List<string>();
        public List<string> CurrencyLogs = new List<string>();
        public CurrencyContent CurrencyContent = null;
        public int CurrencyAmount = 0;
    }
}
