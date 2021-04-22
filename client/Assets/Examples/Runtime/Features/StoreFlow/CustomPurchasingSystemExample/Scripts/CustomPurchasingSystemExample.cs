using Beamable.Api.Payments;
using Beamable.Examples.Features.StoreFlow.MyCustomStore;
using Beamable.Service;
using UnityEngine;

namespace Beamable.Examples.Features.StoreFlow
{
    /// <summary>
    /// Demonstrates custom implementation of <see cref="IBeamablePurchaser"/>
    /// for In-App Purchasing using Beamable.
    /// </summary>
    public class CustomPurchasingSystemExample : MonoBehaviour
    {
        //  Properties  -----------------------------------
        //  Fields  ---------------------------------------
        //  Unity Methods  --------------------------------

        protected void Start()
        {
            Debug.Log("Start()");

            SetupBeamable();
        }
        
        //  Methods  --------------------------------------
        private async void SetupBeamable()
        {
            // Order is important
            
            // Order #1 - Call for Instance
            var beamableAPI = await Beamable.API.Instance;

            Debug.Log($"beamableAPI.User.id = {beamableAPI.User.id}");
            
            // Order #2 - Register
            ServiceManager.Provide(new CustomPurchaserResolver());
            
            // Order #3 - Now use the StoreFlow feature prefab at runtime
            // and 'Buy' an item. Watch the Unity Console Window for logging.
            
            // Note: This "CustomPurchaser" implementation is partially functional. 
            // Use it as a reference to create your own client/server purchasing system
            // which is compatible with Beamable.
        }
        
        
        //  Event Handlers  -------------------------------
    }
}