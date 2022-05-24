using System;
using System.Collections;
using Beamable.Api;
using Beamable.Api.Payments;
using Beamable.Common;
using Beamable.Common.Spew;
using Beamable.Service;
using UnityEngine;

namespace Beamable.Examples.Prefabs.StoreFlow.MyCustomPurchaser
{
   //  Supporting Types  -----------------------------
   // Todo Your Implementation: Set your own receipt...
   [Serializable]
   public class CustomPurchaseReceipt
   {
      public string Store;
      public string TransactionID;
      public string Payload;
   }
   
   // Todo Your Implementation: Set your own result...
   public enum ProcessingResult
   {
      Complete,
      Pending
   }
      
   // Todo Your Implementation: Set your own types...
   public enum ProductType
   {
      Consumable,
      NonConsumable,
      Subscription
   }

   /// <summary>
   /// Implementation of base for custom Beamable purchasing.
   /// </summary>
   public class BasePurchaser
   {
      //  Events  ---------------------------------------
      protected Action<CompletedTransaction> _onBeginPurchaseSuccess;
      protected Action<ErrorCode> _onBeginPurchaseError;
      protected Action _onBeginPurchaseCancelled;

      //  Constants  ------------------------------------
      private static readonly int[] RetryDelays = {1, 2, 5, 10, 20};
      protected const string AppleAppStore = "AppleAppStore";
      protected const string GooglePlay = "GooglePlay";

      //  Fields  ---------------------------------------
      protected long _transactionId = 0;
      protected readonly Promise<Unit> _initializePromise = new Promise<Unit>();
      protected PaymentService _paymentService = null;
      protected BeamContext _beamContext;

      //  Methods  --------------------------------------
      public async Promise<Unit> Initialize()
      {
         _beamContext = BeamContext.Default;
         await _beamContext.OnReady;
         _paymentService = _beamContext.Api.PaymentService;

         return await _initializePromise;
      }

      /// <summary>
      /// Determines if <see cref="ErrorCode"/> can be retried again
      /// </summary>
      /// <param name="errorCode"></param>
      /// <returns></returns>
      protected bool IsRetryable(ErrorCode errorCode)
      {
         // Server error or rate limiting or network error
         return errorCode.Code >= 500 || errorCode.Code == 429 || errorCode.Code == 0;
      }

      /// <summary>
      /// Clear all the callbacks and zero out the transaction ID.
      /// </summary>
      protected void ClearCallbacks()
      {
         _onBeginPurchaseSuccess = null;
         _onBeginPurchaseError = null;
         _onBeginPurchaseCancelled = null;
         _transactionId = 0;
      }

      /// <summary>
      /// If fulfillment failed, retry fulfillment with a backoff, as a coroutine.
      /// </summary>
      /// <param name="transaction"></param>
      /// <param name="purchasedCustomStoreProduct"></param>
      /// <returns></returns>
      protected IEnumerator RetryTransaction(CompletedTransaction transaction,
         CustomStoreProduct purchasedCustomStoreProduct)
      {
         // This block should only be hit when the error returned from the request is retryable. This lives down here
         // because C# doesn't allow you to yield return from inside a try..catch block.
         var waitTime = RetryDelays[Math.Min(transaction.Retries, RetryDelays.Length - 1)];
         InAppPurchaseLogger.Log(
            $"Got a retryable error from platform. Retrying complete purchase request in {waitTime} seconds.");

         // Avoid incrementing the backoff if the device is definitely not connected to the network at all.
         // This is narrow, and would still increment if the device is connected, but the internet has other problems

         if (Application.internetReachability != NetworkReachability.NotReachable)
         {
            transaction.Retries += 1;
         }

         yield return new WaitForSeconds(waitTime);

         FulfillTransaction(transaction, purchasedCustomStoreProduct);
      }

      /// <summary>
      /// Fulfill a completed transaction by completing the purchase in the
      /// payments service and informing Unity IAP of completion.
      /// </summary>
      /// <param name="transaction"></param>
      /// <param name="product"></param>
      protected virtual void FulfillTransaction(CompletedTransaction transaction,
         CustomStoreProduct product)
      {
         // Todo: Your Implementation
         throw new NotImplementedException("Override this by subclass and implement.");
      }
   }

}
