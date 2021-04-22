using System.Collections.Generic;
using Beamable.Api.Payments;

namespace Beamable.Examples.Features.StoreFlow.MyCustomStore2
{
   /// <summary>
   /// Implementation of custom store product for Beamable purchasing.
   /// </summary>
   public class CustomStoreProduct
   {
      //TODO: Implement Receipt
      public string Receipt { get; internal set; }
      
      public bool HasReceipt { get { return !string.IsNullOrEmpty(Receipt); }  }

      public SKU SKU { get; private set; }
      public ProductType ProductType { get; private set; }
      public Dictionary<string, string> Ids { get; private set; }
      public CustomStoreProduct(SKU sku, ProductType productType, Dictionary<string, string> iIds)
      {
         SKU = sku;
         ProductType = productType;
         Ids = iIds;
      }
   }
}
