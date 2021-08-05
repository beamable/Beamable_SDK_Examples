using System.Collections.Generic;
using Beamable.Common.Inventory;

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
}

