using System.Collections;
using UnityEngine;

namespace Beamable.Examples.LearningFundamentals.AsynchronousProgramming
{
    /// <summary>
    /// Demonstrates asynchronous programming.
    /// </summary>
    public class AsynchronousProgrammingExample : MonoBehaviour
    {
        //  Unity Methods  --------------------------------
        protected void Start()
        {
            Debug.Log("Start()");

            MyMethodViaAsyncAwait();
            MyMethodViaCallback();
            StartCoroutine(MyMethodViaCoroutine());
        }
        
        //  Methods  --------------------------------------
        private async void MyMethodViaAsyncAwait()
        {
            var beamableAPI = await Beamable.API.Instance;
            Debug.Log($"beamableAPI.User.id = {beamableAPI.User.id}");
        }
    
        private void MyMethodViaCallback()
        {
            Beamable.API.Instance.Then(beamableAPI =>
            {
                Debug.Log($"beamableAPI.User.id = {beamableAPI.User.id}");
            });
        }
    
        private IEnumerator MyMethodViaCoroutine()
        {
            var promise = Beamable.API.Instance;
            yield return promise.ToYielder();
            var beamableAPI = promise.GetResult();
    
            Debug.Log($"beamableAPI.User.id = {beamableAPI.User.id}");
        }
        
        //  Event Handlers  -------------------------------
    }
}