using UnityEngine;

namespace Beamable.Examples.Prefabs.XXX
{
    /// <summary>
    /// Demonstrates <see cref="Example"/>.
    /// </summary>
    public class TemplateExample : MonoBehaviour
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
            var beamContext = BeamContext.Default;
            await beamContext.OnReady;

            Debug.Log($"beamContext.PlayerId = {beamContext.PlayerId}");
        }
        
        
        //  Event Handlers  -------------------------------
    }
}