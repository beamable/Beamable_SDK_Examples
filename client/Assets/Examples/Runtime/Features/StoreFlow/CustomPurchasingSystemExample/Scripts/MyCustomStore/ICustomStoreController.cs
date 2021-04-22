using System;
using System.Collections.Generic;

namespace Beamable.Examples.Features.StoreFlow.MyCustomStore
{
   /// <summary>
   /// Used by developers to control the <see cref="CustomStore"/>
   /// </summary>
   public interface ICustomStoreController
    {
        List<CustomStoreProduct> CustomStoreProducts { get; }
        void InitiatePurchase(CustomStoreProduct customStoreProduct, string payload);
        void InitiatePurchase(string productId, string payload);
        void InitiatePurchase(CustomStoreProduct customStoreProduct);
        void InitiatePurchase(string productId);
        void FetchAdditionalProducts(List<CustomStoreProduct> additionalCustomStoreProducts, 
            Action successCallback, Action<string> failCallback);
        void ConfirmPendingPurchase(CustomStoreProduct customStoreProduct);
    }
}
