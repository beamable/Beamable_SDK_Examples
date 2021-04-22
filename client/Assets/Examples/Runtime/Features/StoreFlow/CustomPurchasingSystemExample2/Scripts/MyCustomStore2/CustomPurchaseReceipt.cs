using System;

namespace Beamable.Examples.Features.StoreFlow.MyCustomStore2
{
   [Serializable]
   public class CustomPurchaseReceipt
   {
      public string Store;
      public string TransactionID;
      public string Payload;
   }
}
