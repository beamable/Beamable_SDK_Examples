#if UNITY_PURCHASING
using UnityEngine;
using System;
using System.Collections;
using Beamable.Api;
using Beamable.Api.Payments;
using Beamable.Common;
using Beamable.Coroutines;
using Beamable.Service;
using Beamable.Spew;
using UnityEngine.Purchasing;
namespace Beamable.Examples.Features.StoreFlow
{
  /// <summary>
  /// Purchase receipt data structure corresponding to UnityIAP receipt JSON.
  /// </summary>
  [Serializable]
  public class UnityReceipt
  {
    // ReSharper disable InconsistentNaming
    public string Store;
    public string TransactionID;
    public string Payload;
    // ReSharper restore InconsistentNaming
  }


  /// <summary>
  /// Payment delegate that connects UnityIAP to Beamable purchasing. This class
  /// implements IStoreListener so it can receive UnityIAP signals and inherits
  /// from PaymentDelegate to handle integration with Beamable commerce.
  /// </summary>
  internal class PaymentDelegateExampleImpl : IStoreListener, PaymentDelegate,
    IServiceResolver<PaymentDelegate>
  {
    // Geometric backoff times, in seconds, for retries.
    private static readonly int[] RetryDelays = {1, 2, 4, 8, 16, 32};

    // Transaction ID to use during fulfillment.
    private long _txid;

    // Callbacks for success, failure, or cancellation by the user.
    private Action<CompletedTransaction> _successCallback;
    private Action<ErrorCode> _failureCallback;
    private Action _cancelCallback;

    // Promise to be completed when UnityIAP initialization is complete.
    private readonly Promise<Unit> _initializationPromise = new Promise<Unit>();

    // UnityIAP store controller to inform when purchases go through.
    private IStoreController StoreController { get; set; }

    // UnityIAP store extension provider for device-specific features.
    private IExtensionProvider ExtensionProvider { get; set; }

    #region IStoreListener

    /// <summary>
    /// Handle failed IAP initialization by logging the error.
    /// </summary>
    /// <param name="error">The error that occurred</param>
    public void OnInitializeFailed(InitializationFailureReason error)
    {
      InAppPurchaseLogger.LogFormat("OnInitializeFailed reason = {0}", error);
    }

    /// <summary>
    /// Process a successful purchase by fulfilling the transaction through the
    /// Beamable payments service.
    ///
    /// When fulfilling the transaction, use the transaction ID from when the
    /// transaction was initiated.
    /// </summary>
    /// <param name="args">Event arguments including purchase receipt.</param>
    /// <returns>Purchase processing result, in this case always Pending</returns>
    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
      var receipt = args.purchasedProduct.receipt;
      if (args.purchasedProduct.hasReceipt)
      {
        // hasReceipt indicates a rich receipt object in JSON format
        var receiptData = JsonUtility.FromJson<UnityReceipt>(args.purchasedProduct.receipt);
        receipt = receiptData.Payload;
      }

      var price = args.purchasedProduct.metadata.localizedPriceString;
      var isoCurrency = args.purchasedProduct.metadata.isoCurrencyCode;
      var listing = ""; // Listing left blank in this example.
      var sku = ""; // SKU left blank in this example.
      var tx = new CompletedTransaction(_txid, receipt, price, isoCurrency, listing, sku);
      FulfillTransaction(tx, args.purchasedProduct);

      return PurchaseProcessingResult.Pending;
    }

    /// <summary>
    /// Handle a failed purchase by invoking the appropriate callbacks and
    /// informing Beamable platform of the failure. Player-cancelled purchases
    /// need not inform the platform.
    /// </summary>
    /// <param name="product">The product for which the purchase was initiated</param>
    /// <param name="failureReason">Reason for failure, which may include cancellation by the user</param>
    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
      InAppPurchaseLogger.LogFormat("OnPurchaseFailed: FAIL. product = {0}, reason = {1}", product, failureReason);
      var platform = ServiceManager.Resolve<PlatformService>();
      if (failureReason == PurchaseFailureReason.UserCancelled)
      {
        platform.Payments.CancelPurchase(_txid);
        _cancelCallback?.Invoke();
      }
      else
      {
        var message = $"{product.definition.storeSpecificId}:{failureReason}";
        platform.Payments.FailPurchase(_txid, message);
        var errorCode = new ErrorCode((int) failureReason, GameSystem.GAME_CLIENT, message);
        _failureCallback?.Invoke(errorCode);
      }

      ClearCallbacks();
    }

    /// <summary>
    /// Complete initialization of this payment delegate with information from
    /// UnityIAP. Also restore any unfulfilled purchases.
    /// </summary>
    /// <param name="controller">UnityIAP store controller</param>
    /// <param name="extensions">Store-specific extensions handler</param>
    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
      StoreController = controller;
      ExtensionProvider = extensions;

      _initializationPromise.CompleteSuccess(PromiseBase.Unit);

      RestorePurchases();
    }

    #endregion // IStoreListener

    /// <summary>
    /// Fulfill the transaction through the Beamable platform's payment service,
    /// also informing UnityIAP of the purchase completion through the store
    /// controller.
    ///
    /// This will invoke either the success callback or the failure callback, if
    /// they have been registered.
    /// </summary>
    /// <param name="transaction">The Beamable transaction object</param>
    /// <param name="purchasedProduct">Which product was purchased through UnityIAP</param>
    private void FulfillTransaction(CompletedTransaction transaction, Product purchasedProduct)
    {
      var platform = ServiceManager.Resolve<PlatformService>();
      platform.Payments.CompletePurchase(transaction).Then(_ =>
      {
        StoreController.ConfirmPendingPurchase(purchasedProduct);
        _successCallback?.Invoke(transaction);
        ClearCallbacks();
      }).Error(ex =>
      {
        Debug.LogError($"Exception while fulfilling purchase ex = {ex}");
        var err = ex as ErrorCode;

        if (err == null)
        {
          // Unknown error: bail out.
          return;
        }

        if (IsRetryable(err))
        {
          var coroutineService = ServiceManager.Resolve<CoroutineService>();
          coroutineService.StartCoroutine(
            RetryTransaction(transaction, purchasedProduct));
        }
        else
        {
          StoreController.ConfirmPendingPurchase(purchasedProduct);
          _failureCallback?.Invoke(err);
          ClearCallbacks();
        }
      });
    }

    /// <summary>
    /// Retry the transaction in the case of a network error, a server error, or
    /// rate limiting.
    /// </summary>
    private IEnumerator RetryTransaction(CompletedTransaction transaction, Product purchasedProduct)
    {
      // If we hit the end of the array, just keep using the maximum delay.
      var delay = RetryDelays[Math.Min(transaction.Retries, RetryDelays.Length - 1)];
      transaction.Retries += 1;
      yield return new WaitForSeconds(delay);
      FulfillTransaction(transaction, purchasedProduct);
    }

    /// <summary>
    /// Tell whether an error code is considered retryable. Retryable errors are
    /// internal server errors (5xx), rate limiting (429), and network errors (0).
    /// </summary>
    private static bool IsRetryable(ErrorCode err)
    {
      return err.Code >= 400 || err.Code == 429 || err.Code == 0;
    }

    /// <summary>
    /// Restore purchases previously made by this user. Behavior depends on
    /// platform: on Google, purchase restoration happens automatically out of
    /// our hands; on Apple, we must explicitly request restoration and that may
    /// trigger an Apple authentication dialog.
    ///
    /// This method could be made public if you wish to trigger it from a button
    /// press or the like.
    /// </summary>
    private void RestorePurchases()
    {
      if (Application.platform != RuntimePlatform.IPhonePlayer &&
          Application.platform != RuntimePlatform.OSXPlayer)
      {
        InAppPurchaseLogger.LogFormat("RestorePurchases not needed on {0}", Application.platform);
        return;
      }

      var apple = ExtensionProvider.GetExtension<IAppleExtensions>();
      apple.RestoreTransactions(result =>
      {
        InAppPurchaseLogger.LogFormat("Apple RestorePurchases result = {0}", result);
      });
    }

    /// <summary>
    /// Clear all callback actions and the transaction ID.
    /// </summary>
    private void ClearCallbacks()
    {
      _successCallback = null;
      _failureCallback = null;
      _cancelCallback = null;
      _txid = 0;
    }

    #region PaymentDelegate

    /// <summary>
    /// Initialize this payment delegate for use with Beamable commerce.
    /// </summary>
    /// <returns>Promise to be completed when initialization is complete</returns>
    public Promise<Unit> Initialize()
    {
      var platform = ServiceManager.Resolve<PlatformService>();
      platform.Payments.GetSKUs().Then(response =>
      {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
        foreach (var sku in response.skus.definitions)
        {
          builder.AddProduct(sku.name, ProductType.Consumable, new IDs
          {
            {sku.productIds.itunes, AppleAppStore.Name},
            {sku.productIds.googleplay, GooglePlay.Name}
          });
        }

        // Kick off UnityIAP initialization. This will eventually trigger
        // OnInitialized or OnInitializeFailed.
        UnityPurchasing.Initialize(this, builder);
      });

      return _initializationPromise;
    }

    /// <summary>
    /// Given a SKU, get its localized price.
    /// </summary>
    /// <param name="purchaseSymbol">The SKU to check</param>
    /// <returns>The localized price, or three question marks if unknown</returns>
    public string GetLocalizedPrice(string purchaseSymbol)
    {
      var product = StoreController?.products.WithID(purchaseSymbol);
      return product?.metadata.localizedPriceString ?? "???";
    }

    /// <summary>
    /// Lowercase startPurchase is a convenience method for invoking
    /// StartPurchase and attaching callbacks all in one call.
    /// </summary>
    /// <param name="listingSymbol">The Beamable listing being purchased</param>
    /// <param name="skuSymbol">The UnityIAP SKU being purchased</param>
    /// <param name="success">Callback for successful purchase</param>
    /// <param name="fail">Callback if the purchase failed</param>
    /// <param name="cancelled">Callback if the user cancelled the purchase</param>
    public void startPurchase(
      string listingSymbol,
      string skuSymbol,
      Action<CompletedTransaction> success,
      Action<ErrorCode> fail,
      Action cancelled
    )
    {
      StartPurchase(listingSymbol, skuSymbol)
        .Then(tx => success?.Invoke(tx))
        .Error(err => fail?.Invoke(err as ErrorCode));
      if (cancelled != null)
      {
        _cancelCallback += cancelled;
      }
    }

    /// <summary>
    /// Initiate the purchase of a particular SKU and its associated listing.
    /// </summary>
    /// <param name="listingSymbol">Beamable listing for this purchase</param>
    /// <param name="skuSymbol">UnityIAP SKU for this purchase</param>
    /// <returns>Promise to be completed upon success or failure</returns>
    public Promise<CompletedTransaction> StartPurchase(string listingSymbol, string skuSymbol)
    {
      var promise = new Promise<CompletedTransaction>();
      _txid = 0;
      _successCallback = promise.CompleteSuccess;
      _failureCallback = promise.CompleteError;
      if (_cancelCallback == null)
      {
        _cancelCallback = () =>
        {
          promise.CompleteError(new ErrorCode(400, GameSystem.GAME_CLIENT, "Purchase Cancelled"));
        };
      }

      var platform = ServiceManager.Resolve<PlatformService>();
      platform.Payments.BeginPurchase(listingSymbol).Then(response =>
      {
        _txid = response.txid;
        StoreController.InitiatePurchase(skuSymbol, _txid.ToString());
        // The success callback will be called by FulfillTransaction, which
        // is invoked by ProcessPurchase through UnityIAP.
      }).Error(err =>
      {
        Debug.LogError($"Error beginning purchase err = {err}");
        _failureCallback?.Invoke(err as ErrorCode);
      });

      return promise;
    }

    #endregion // PaymentDelegate

    #region IServiceResolver

    /// <summary>
    /// Tear down this service provider object.
    /// </summary>
    public void OnTeardown()
    {
      StoreController = null;
      ExtensionProvider = null;
    }

    /// <summary>
    /// Tell whether this service provider can be resolved by ServiceManager.
    /// </summary>
    public bool CanResolve()
    {
      return true;
    }

    /// <summary>
    /// Tell whether this service provider exists.
    /// </summary>
    public bool Exists()
    {
      return true;
    }

    /// <summary>
    /// Resolve this service provider to a concrete instance of PaymentDelegate.
    /// </summary>
    /// <returns>The PaymentDelegate instance</returns>
    public PaymentDelegate Resolve()
    {
      return this;
    }

    #endregion // IServiceResolver
  }
}
#endif // UNITY_PURCHASING