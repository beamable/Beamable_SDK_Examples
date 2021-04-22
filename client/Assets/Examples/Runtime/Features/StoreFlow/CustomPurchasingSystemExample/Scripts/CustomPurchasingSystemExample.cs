using Beamable.Service;
using UnityEngine;

namespace Beamable.Examples.Features.StoreFlow.MyCustomStore
{
    /// <summary>
    /// Implementation of custom Beamable purchasing.
    ///
    /// 1. See the partially-functional <see cref="CustomPurchaser"/> as a template.
    ///   Copy it and complete your custom implementation.
    ///
    /// 2. See the fully-functional <see cref="UnityBeamablePurchaser"/> (Search
    ///   in Beamable SDK) for inspiration.
    /// 
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
            
            // Order #2 - Register the resolver
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