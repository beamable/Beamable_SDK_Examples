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
            var beamContext = BeamContext.Default;
            await beamContext.OnReady;
            Debug.Log($"1 beamContext.PlayerId = {beamContext.PlayerId}");
        }
    
        private void MyMethodViaCallback()
        {
            var beamContext = BeamContext.Default;
            beamContext.OnReady.Then(_ =>
            {
                Debug.Log($"2 beamContext.PlayerId = {beamContext.PlayerId}");
            });
        }
    
        private IEnumerator MyMethodViaCoroutine()
        {
            var beamContext = BeamContext.Default;
            var promise = beamContext.OnReady;
            yield return promise.ToYielder();
            var result = promise.GetResult();
    
            Debug.Log($"3 beamContext.PlayerId = {beamContext.PlayerId}");
        }
    }
}