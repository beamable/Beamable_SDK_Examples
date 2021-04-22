﻿using Beamable.Api.Payments;
using Beamable.Service;
using UnityEngine;

namespace Beamable.Examples.Features.StoreFlow.MyCustomPurchaser
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
        //  Unity Methods  --------------------------------
        protected void Start()
        {
            Debug.Log("Start()");
            SetupBeamable();
        }
        
        //  Methods  --------------------------------------
        private async void SetupBeamable()
        {
            // Order is important here...
            
            // Order #1 - Call for Instance
            var beamableAPI = await Beamable.API.Instance;
            Debug.Log($"beamableAPI.User.id = {beamableAPI.User.id}");
            
            // Order #2 - Register the purchaser
            ServiceManager.Provide(new CustomPurchaserResolver());
            var customPurchaser = ServiceManager.Resolve<IBeamablePurchaser>();
            await customPurchaser.Initialize();
            beamableAPI.BeamableIAP.CompleteSuccess(customPurchaser);
                
            // Order #3 - Now use the StoreFlow feature prefab at runtime
            // and 'Buy' an item. Watch the Unity Console Window for logging
            // to verify success
        }
    }
}