using UnityEngine;
#if UNITY_PURCHASING
using Beamable.Service;
#endif // UNITY_PURCHASING


namespace Beamable.Examples.Features.StoreFlow
{
  /// <summary>
  /// Payment delegate injection MonoBehaviour. When an object with this behaviour
  /// exists in the scene, it will register ExamplePaymentDelegate as the payment
  /// delegate if no other payment delegate was already registered.
  /// </summary>
  public class PaymentDelegateExample : MonoBehaviour
  {
    protected void Awake()
    {
        RegisterPaymentDelegate();
    }

    /// <summary>
    /// Register the payment delegate upon awakening, if needed.
    /// </summary>
    private void RegisterPaymentDelegate()
    {
      
      
#if UNITY_PURCHASING
      if (!ServiceManager.Exists<PaymentDelegateExample>())
      {
        ServiceManager.ProvideWithDefaultContainer<PaymentDelegate>(
          new PaymentDelegateExampleImpl());
      }
        Debug.Log(($"RegisterPaymentDelegate found required UNITY_PURCHASING."));
#else
        Debug.Log($"RegisterPaymentDelegate did not find required UNITY_PURCHASING.");
#endif // UNITY_PURCHASING
      
      
    }
  }
}
