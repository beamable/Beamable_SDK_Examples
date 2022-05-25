using UnityEngine;

namespace Beamable.Examples.Integrations.Facebook
{
    /// <summary>
    /// Demonstrates <see cref="Facebook"/> integration.
    /// </summary>
    public class FacebookExample : MonoBehaviour
    {
        //  Properties  -----------------------------------
        //  Fields  ---------------------------------------
        //  Unity Methods  --------------------------------

        protected void Start()
        {
            Debug.Log($"Start() Instructions...\n\n" + 
                      " * Complete steps: https://docs.beamable.com/docs/adding-facebook-sign-in\n" + 
                      " * Run The Scene\n" +
                      " * See Login Flow Feature Prefab - With functional Facebook Sign-In\n");

            SetupBeamable();
        }
        
        //  Methods  --------------------------------------
        private async void SetupBeamable()
        {
            var beamContext = BeamContext.Default;
            await beamContext.OnReady;

            Debug.Log($"beamContext.PlayerId = {beamContext.PlayerId}");
        }
    }
}