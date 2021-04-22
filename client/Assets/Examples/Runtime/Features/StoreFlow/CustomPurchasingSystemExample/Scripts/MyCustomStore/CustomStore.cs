
using System.Collections.Generic;
using UnityEngine;

namespace Beamable.Examples.Features.StoreFlow.MyCustomStore
{
   /// <summary>
   /// Implementation of custom store for Beamable purchasing.
   /// </summary>
   public class CustomStore
   {
      public static void Initialize(CustomPurchaser customPurchaser, List<CustomStoreProduct> customStoreProducts)
      {
         Debug.Log("CustomStore.Initialize()");
      }
   }
}
