using UnityEngine;

namespace Beamable.Examples.Features.TournamentFlow
{
    /// <summary>
    /// Demonstrates <see cref="TournamentFlow"/>.
    /// </summary>
    public class TournamentFlowExample : MonoBehaviour
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