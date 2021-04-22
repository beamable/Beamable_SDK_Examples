using Beamable.Api.Payments;
using Beamable.Service;

namespace Beamable.Examples.Features.StoreFlow.MyCustomStore
{
   /// <summary>
   /// Beamable service resolver.
   /// </summary>
   public class CustomPurchaserResolver : IServiceResolver<IBeamablePurchaser>
   {
      private CustomPurchaser _customPurchaser;

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
