using UnityEngine;

namespace Beamable.Examples.Features.XXX
{
    /// <summary>
    /// Demonstrates <see cref="Example"/>.
    /// </summary>
    public class Example : MonoBehaviour
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
            var beamableAPI = await Beamable.API.Instance;

            Debug.Log($"beamableAPI.User.id = {beamableAPI.User.id}");
        }
        
        
        //  Event Handlers  -------------------------------
    }
}