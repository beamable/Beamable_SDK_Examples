namespace Beamable.Server.MyMicroservice
{
   [Microservice("MyMicroservice")]
   public class MyMicroservice : Microservice
   {
      [ClientCallable]
      public int AddMyValues(int a, int b)
      {
         return a + b * 2;
      }
   }
}