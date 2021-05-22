using System.Collections.Generic;
using Beamable.Api.Payments;

namespace Beamable.Examples.Prefabs.StoreFlow.MyCustomPurchaser
{
   /// <summary>
   /// Implementation of custom store product for Beamable purchasing.
   /// </summary>
   public class CustomStoreProduct
   {
      //  Properties  -----------------------------------
      public SKU SKU { get; private set; }
      public ProductType ProductType { get; private set; }
      public Dictionary<string, string> Ids { get; private set; }
      
      // Todo Your Implementation: Setup custom receipt...
      public string Receipt { get; internal set; }
      public bool HasReceipt { get { return !string.IsNullOrEmpty(Receipt); }  }
      
      //  Methods  --------------------------------------
      public CustomStoreProduct(SKU sku, ProductType productType, Dictionary<string, string> iIds)
      {
         SKU = sku;
         ProductType = productType;
         Ids = iIds;
      }
   }
}
