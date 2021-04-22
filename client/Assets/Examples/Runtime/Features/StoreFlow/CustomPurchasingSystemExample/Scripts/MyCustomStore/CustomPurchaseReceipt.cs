using System;

namespace Beamable.Examples.Features.StoreFlow.MyCustomStore
{
   [Serializable]
   public class CustomPurchaseReceipt
   {
      public string Store;
      public string TransactionID;
      public string Payload;
   }
}
