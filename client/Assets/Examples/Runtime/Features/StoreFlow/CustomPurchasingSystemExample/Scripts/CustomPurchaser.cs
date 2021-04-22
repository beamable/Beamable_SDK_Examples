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
   ///
   /// 1. See this partially-functional <see cref="CustomPurchaser"/> as a template.
   ///   Copy it and complete your custom implementation.
   ///
   /// 2. See the fully-functional <see cref="UnityBeamablePurchaser"/> (Search
   ///   in Beamable SDK) for inspiration.
   /// 
   /// </summary>
   public class CustomPurchaser : BasePurchaser, IBeamablePurchaser
   {
      //  Fields  ---------------------------------------
      private List<CustomStoreProduct> _customStoreProducts = new List<CustomStoreProduct>();

      //  Methods  --------------------------------------
      #region "IBeamablePurchaser"
      
      /// <summary>
      /// Begin initialization of Beamable purchasing.
      /// </summary>
      public override Promise<Unit> Initialize()
      {
         base.Initialize();
         
         Debug.LogError($"Initialize()");
            
         _paymentService.GetSKUs().Then(rsp =>
         {
            _customStoreProducts = new List<CustomStoreProduct>();
            foreach (SKU sku in rsp.skus.definitions)
            {
               Dictionary<string, string> idDictionary = new Dictionary<string, string>
               {
                  {sku.productIds.itunes, AppleAppStore},
                  {sku.productIds.googleplay, GooglePlay}
               };

               _customStoreProducts.Add(
                     new CustomStoreProduct(sku, ProductType.Consumable, idDictionary));
            }

            // Todo Your Implementation: Determine initialization pass/fail
            OnInitialized();
            
            //or
            
            //OnInitializeFailed("tbd error");
            
         });

         return _initializePromise;
      }
      
      
      /// <summary>
      /// Get the localized price string for a given SKU.
      /// </summary>
      public string GetLocalizedPrice(string skuSymbol)
      {
         Debug.LogError($"GetLocalizedPrice() skuSymbol = {skuSymbol}");
         
         var product = _customStoreProducts.FirstOrDefault(
               p => p.SKU.name == skuSymbol);
         
         // Todo Your Implementation: Get the localized price. Not this.
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
         Debug.LogError($"StartPurchase() skuSymbol = {skuSymbol}");
         
         _transactionId = 0;
         var completedTransactionPromise = new Promise<CompletedTransaction>();
         
         _onBeginPurchaseSuccess = completedTransactionPromise.CompleteSuccess;
         _onBeginPurchaseError = completedTransactionPromise.CompleteError;

         if (_onBeginPurchaseCancelled == null)
         {
            _onBeginPurchaseCancelled = () =>
            { 
               completedTransactionPromise.CompleteError( 
                  new ErrorCode(400, GameSystem.GAME_CLIENT, "Purchase Cancelled"));
            };
         }

         _paymentService.BeginPurchase(listingSymbol).Then(rsp =>
         {
            _transactionId = rsp.txid;
            PaymentService_OnBeginPurchase(skuSymbol, _transactionId);
            
         }).Error(err =>
         {
            Debug.LogError($"OnBeginPurchaseError() error = {err}");
            _onBeginPurchaseError?.Invoke(err as ErrorCode);
         });

         return completedTransactionPromise;
      }

      #endregion



      
      /// <summary>
      /// Process a completed purchase by fulfilling it.
      /// </summary>
      /// <param name="product"></param>
      /// <returns></returns>
      public ProcessingResult ProcessPurchase(CustomStoreProduct product)
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
            _transactionId,
            rawReceipt,
            GetLocalizedPrice(product.SKU.name),
            "tbdIsoCurrencyCode"
         );
         
         FulfillTransaction(transaction, product);

         return ProcessingResult.Pending;
      }

      /// <summary>
      /// Handle a purchase failure event from Unity IAP.
      /// </summary>
      /// <param name="product">The product whose purchase was attempted</param>
      /// <param name="failureReason">Information about why the purchase failed</param>
      public void OnPurchaseFailed(CustomStoreProduct product, string failureReason)
      {
         Debug.LogError($"OnPurchaseFailed() product = {product.SKU.name}");
         
         // Todo Your Implementation: Setup custom reasons...
         if (failureReason == "tbdSomeReason")
         {
            // Maybe cancel...
            _paymentService.CancelPurchase(_transactionId);
            _onBeginPurchaseCancelled?.Invoke();
         }
         else
         {
            // Todo Your Implementation: Setup custom reasons...
            int reasonInt = 1;
            string reason = product.SKU.name+ ":" + failureReason;
            
            // Maybe fail...
            _paymentService.FailPurchase(_transactionId, reason);

            var errorCode = new ErrorCode(reasonInt, GameSystem.GAME_CLIENT, reason);
            _onBeginPurchaseError?.Invoke(errorCode);
         }

         ClearCallbacks();
      }

      /// <summary>
      /// Initiate transaction restoration if needed.
      /// </summary>
      public void RestorePurchases()
      {
         Debug.Log($"RestorePurchases() platform = {Application.platform}");
         
         if (Application.platform == RuntimePlatform.IPhonePlayer ||
             Application.platform == RuntimePlatform.OSXPlayer)
         {
            // Todo Your Implementation: For iOS...
         }
         else
         {
            // Todo Your Implementation: For other platform(s)...
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
         
         _paymentService.CompletePurchase(transaction).Then(_ =>
         {
            PaymentService_OnCompletePurchase(product);
            _onBeginPurchaseSuccess?.Invoke(transaction);
            ClearCallbacks();
         }).Error(ex =>
         {
            Debug.Log($"error = {ex.Message}");
            var errorCode = ex as ErrorCode;

            if (errorCode == null)
            {
               return;
            }

            var isRetryable = IsRetryable(errorCode);
            if (isRetryable)
            {
               ServiceManager.Resolve<CoroutineService>().StartCoroutine(
                  RetryTransaction(transaction, product));
            }
            else
            {
               PaymentService_OnCompletePurchase(product);
               _onBeginPurchaseError?.Invoke(errorCode);
               ClearCallbacks();
            }
         });
      }
      
      //  Event Handlers  -------------------------------
      
      private void OnInitialized()
      {
         Debug.Log($"OnInitialized() count = {_customStoreProducts.Count}");
         
         RestorePurchases();
         _initializePromise.CompleteSuccess(PromiseBase.Unit);
      }

      private void OnInitializeFailed(string error)
      {
         Debug.Log($"OnInitializeFailed() error = {error}");
         
         // Todo Your Implementation: handle failure
      }
      
      private void PaymentService_OnBeginPurchase(string skuSymbol, long transactionId)
      {
         Debug.Log($"OnBeginPurchase() skuSymbol = {skuSymbol}, " +
                   $"transactionId = {transactionId}");
         
         // Todo Your Implementation: implement purchase
      }
      
      private void PaymentService_OnCompletePurchase(CustomStoreProduct product)
      {
         Debug.Log($"OnCompletePurchase() product = {product.SKU.name}"); 
         
         // Todo Your Implementation: Complete the purchase
      }
   }
}
