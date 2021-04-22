namespace Beamable.Examples.Features.StoreFlow.MyCustomStore2
{
   /// <summary>
   /// Implementation of custom store constants for Beamable purchasing
   /// </summary>
   public class CustomStoreConstants
   {
      public class AppleAppStore
      {
         public const string Name = "AppleAppStore";
      }
      public class GooglePlay
      {
         public const string Name = "GooglePlay";
      }
      
      public enum ProcessingResult
      {
         Complete,
         Pending
      }
   }
}
