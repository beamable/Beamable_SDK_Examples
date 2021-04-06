using UnityEngine;
using Beamable.Server.Clients;

namespace Beamable.Examples.Services.Microservices
{
    /// <summary>
    /// Demonstrates <see cref="Microservices"/>.
    /// </summary>
    public class MicroservicesExample : MonoBehaviour
    {
        //  Properties  -----------------------------------
        
        //  Fields  ---------------------------------------
        private MyMicroserviceClient  _myMicroserviceClient;
        
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
            
            _myMicroserviceClient = new MyMicroserviceClient();
            
            // AddMyValues(): 10 + 5
            await _myMicroserviceClient.AddMyValues(10, 5)
                .Then(MyMicroserviceClient_OnServerCallCompleted);
        }

        //  Event Handlers  -------------------------------
        private void MyMicroserviceClient_OnServerCallCompleted(int result)
        {
            // Result:15
            Debug.Log ($"Result:{result}");
        }
    }
}