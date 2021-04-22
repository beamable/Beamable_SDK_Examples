using System;
using System.Collections;
using Beamable.Api;
using Beamable.Api.Payments;
using Beamable.Spew;
using UnityEngine;

namespace Beamable.Examples.Features.StoreFlow.MyCustomStore2
{
   /// <summary>
   /// Implementation of base for custom Beamable purchasing.
   /// </summary>
   public class BasePurchaser 
   {
      static readonly int[] RETRY_DELAYS = { 1, 2, 5, 10, 20 }; 
      protected long _txid;
      protected Action<CompletedTransaction> _onPurchaseSuccess;
      protected Action<ErrorCode> _onPurchaseError;
      protected Action _cancelled;
      
      /// <summary>
      /// Clear all the callbacks and zero out the transaction ID.
      /// </summary>
      protected void ClearCallbacks()
      {
         _onPurchaseSuccess = null;
         _onPurchaseError = null;
         _cancelled = null;
         _txid = 0;
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
         var waitTime = RETRY_DELAYS[Math.Min(transaction.Retries, RETRY_DELAYS.Length - 1)];
         InAppPurchaseLogger.Log($"Got a retryable error from platform. Retrying complete purchase request in {waitTime} seconds.");

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
         // override this
      }
   }
}
