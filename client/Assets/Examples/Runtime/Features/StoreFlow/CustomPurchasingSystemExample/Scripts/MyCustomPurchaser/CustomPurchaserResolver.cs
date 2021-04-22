using Beamable.Api.Payments;
using Beamable.Service;

namespace Beamable.Examples.Features.StoreFlow.MyCustomPurchaser
{
   /// <summary>
   /// Beamable <see cref="IServiceResolver{IBeamablePurchaser}"/> for
   /// <see cref="CustomPurchaser"/>
   /// </summary>
   public class CustomPurchaserResolver : IServiceResolver<IBeamablePurchaser>
   {
      //  Fields  ---------------------------------------
      private CustomPurchaser _customPurchaser;

      //  Methods  --------------------------------------
      public void OnTeardown()
      {
         _customPurchaser = null;
      }

      public bool CanResolve() => true;

      public bool Exists() => _customPurchaser != null;

      public IBeamablePurchaser Resolve()
      {
         return _customPurchaser ?? (_customPurchaser = new CustomPurchaser());
      }
   }
}
