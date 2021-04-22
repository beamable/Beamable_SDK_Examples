using System.Collections.Generic;
using System.Linq;
using Beamable.Api;
using Beamable.Api.Payments;
using Beamable.Common;
using Beamable.Coroutines;
using Beamable.Service;
using UnityEngine;

namespace Beamable.Examples.Features.StoreFlow.MyCustomStore
{
   /// <summary>
   /// Implementation of custom Beamable purchasing.
   /// </summary>
   public class CustomPurchaser : BasePurchaser, ICustomStoreListener, IBeamablePurchaser
   {
      private ICustomStoreController _customStoreController;
      private readonly Promise<Unit> _initPromise = new Promise<Unit>();

      /// <summary>
      /// Begin initialization of Beamable purchasing.
      /// </summary>
      public Promise<Unit> Initialize()
      {
         ServiceManager.Resolve<PlatformService>().Payments.GetSKUs().Then(rsp =>
         {
            List<CustomStoreProduct> customStoreProducts = new List<CustomStoreProduct>();
            foreach (SKU sku in rsp.skus.definitions)
            {
               Dictionary<string, string> idDictionary = new Dictionary<string, string>
               {
                  {sku.productIds.itunes, CustomStoreConstants.AppleAppStore.Name},
                  {sku.productIds.googleplay, CustomStoreConstants.GooglePlay.Name}
               };

               customStoreProducts.Add(
                     new CustomStoreProduct(sku, ProductType.Consumable, idDictionary));
            }

            // Will respond with OnInitialized or OnInitializeFailed.
            CustomStore.Initialize(this, customStoreProducts);
         });

         return _initPromise;
      }

      #region "IBeamablePurchaser"
      
      /// <summary>
      /// Get the localized price string for a given SKU.
      /// </summary>
      public string GetLocalizedPrice(string skuSymbol)
      {
         var product =
            (_customStoreController?.CustomStoreProducts)?.FirstOrDefault(p => p.SKU.name == skuSymbol);
         
         return product?.SKU.realPrice.ToString() ?? "???";
      }

      
      /// <summary>
      /// Start a purchase for the given listing using the given SKU.
      /// </summary>
      /// <param name="listingSymbol">Store listing that should be purchased.</param>
      /// <param name="skuSymbol">Platform specific SKU of the item being purchased.</param>
      /// <returns>Promise containing completed transaction.</returns>
      public Promise<CompletedTransaction> StartPurchase(string listingSymbol, string skuSymbol)
      {
         var result = new Promise<CompletedTransaction>();
         _txid = 0;
         _onPurchaseSuccess = result.CompleteSuccess;
         _onPurchaseError = result.CompleteError;
         if (_cancelled == null) _cancelled = () =>
         { result.CompleteError(
            new ErrorCode(400, GameSystem.GAME_CLIENT, "Purchase Cancelled"));
         };

         ServiceManager.Resolve<PlatformService>().Payments.BeginPurchase(listingSymbol).Then(rsp =>
         {
            _txid = rsp.txid;
            _customStoreController.InitiatePurchase(skuSymbol, _txid.ToString());
         }).Error(err =>
         {
            Debug.LogError($"There was an exception making the begin purchase request: {err}");
            _onPurchaseError?.Invoke(err as ErrorCode);
         });

         return result;
      }
      #endregion

      #region "ICustomStoreListener"
      
      /// <summary>
      /// React to successful Unity IAP initialization.
      /// </summary>
      /// <param name="controller"></param>
      public void OnInitialized(ICustomStoreController controller)
      {
         Debug.Log($"OnInitialized() count = {controller.CustomStoreProducts.Count}");
         
         _customStoreController = controller;
         RestorePurchases();
         _initPromise.CompleteSuccess(PromiseBase.Unit);
      }

      
      /// <summary>
      /// Handle failed initialization by logging the error.
      /// </summary>
      public void OnInitializeFailed(string error)
      {
         Debug.Log($"OnInitializeFailed() error = {error}");
      }

      
      /// <summary>
      /// Process a completed purchase by fulfilling it.
      /// </summary>
      /// <param name="product"></param>
      /// <returns></returns>
      public CustomStoreConstants.ProcessingResult ProcessPurchase(CustomStoreProduct product)
      {
         Debug.Log($"ProcessPurchase() product = {product}");
         
         string rawReceipt;
         if (product.HasReceipt)
         {
            var receipt = JsonUtility.FromJson<CustomPurchaseReceipt>(product.Receipt);
            rawReceipt = receipt.Payload;
            
            Debug.Log($"receipt = {receipt}");
         }
         else
         {
            rawReceipt = product.Receipt;
         }

         var transaction = new CompletedTransaction(
            _txid,
            rawReceipt,
            GetLocalizedPrice(product.SKU.name),
            "tbdIsoCurrencyCode"
         );
         
         FulfillTransaction(transaction, product);

         return CustomStoreConstants.ProcessingResult.Pending;
      }

      /// <summary>
      /// Handle a purchase failure event from Unity IAP.
      /// </summary>
      /// <param name="product">The product whose purchase was attempted</param>
      /// <param name="failureReason">Information about why the purchase failed</param>
      public void OnPurchaseFailed(CustomStoreProduct product, string failureReason)
      {
         var platform = ServiceManager.Resolve<PlatformService>();
         
         if (failureReason == "tbdSomeReason")
         {
            platform.Payments.CancelPurchase(_txid);
            _cancelled?.Invoke();
         }
         else
         {
            int reasonInt = 1; //tbdReason
            string reason = product.SKU.name+ ":" + failureReason;
            platform.Payments.FailPurchase(_txid, reason);

            var errorCode = new ErrorCode(reasonInt, GameSystem.GAME_CLIENT, reason);
            _onPurchaseError?.Invoke(errorCode);
         }

         ClearCallbacks();
      }
      #endregion

      /// <summary>
      /// Initiate transaction restoration if needed.
      /// </summary>
      public void RestorePurchases()
      {
         Debug.Log($"RestorePurchases() platform = {Application.platform}");
         
         if (Application.platform == RuntimePlatform.IPhonePlayer ||
             Application.platform == RuntimePlatform.OSXPlayer)
         {
            // Todo: For iOS...
         }
         else
         {
            // Todo: For other platform(s)...
         }
      }
      
      /// <summary>
      /// Fulfill a completed transaction by completing the purchase in the
      /// payments service and informing Unity IAP of completion.
      /// </summary>
      /// <param name="transaction"></param>
      /// <param name="product"></param>
      protected override void FulfillTransaction(CompletedTransaction transaction, 
         CustomStoreProduct product)
      {
         base.FulfillTransaction(transaction, product);
         
         Debug.Log($"FulfillTransaction() SKUSymbol = {transaction.SKUSymbol}");
         
         ServiceManager.Resolve<PlatformService>().Payments.CompletePurchase(transaction).Then(_ =>
         {
            _customStoreController.ConfirmPendingPurchase(product);
            _onPurchaseSuccess?.Invoke(transaction);
            ClearCallbacks();
         }).Error(ex =>
         {
            Debug.Log($"error = {ex.Message}");
            var err = ex as ErrorCode;

            if (err == null)
            {
               return;
            }

            var retryable = err.Code >= 500 || err.Code == 429 || err.Code == 0;   // Server error or rate limiting or network error
            if (retryable)
            {
               ServiceManager.Resolve<CoroutineService>().StartCoroutine(
                  RetryTransaction(transaction, product));
            }
            else
            {
               _customStoreController.ConfirmPendingPurchase(product);
               _onPurchaseError?.Invoke(err);
               ClearCallbacks();
            }
         });
      }
   }
}
