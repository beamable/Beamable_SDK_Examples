
namespace Beamable.Examples.Features.StoreFlow.MyCustomStore
{
   /// <summary>
   /// Implemented by developers using <see cref="CustomStore"/>
   /// </summary>
   public interface ICustomStoreListener
   {
      void OnInitializeFailed(string error);
      CustomStoreConstants.ProcessingResult ProcessPurchase(CustomStoreProduct product);
      void OnPurchaseFailed(CustomStoreProduct customStoreProduct, string failureReason);
      void OnInitialized(ICustomStoreController customStoreController);
   }
}
