using UnityEngine;

namespace Beamable.Examples.Services.ContentService
{
    /// <summary>
    /// Demonstrates content validation within <see cref="ComplexItem"/>.
    /// </summary>
    public class ContentValidationExample : MonoBehaviour
    {
        //  Fields  ---------------------------------------
        [SerializeField] 
        private ComplexItemLink _complexItemLink;
        
        private ComplexItem _complexItem = null;
        
        //  Unity Methods  --------------------------------
        protected void Start()
        {
            Debug.Log($"Start() Instructions...\n\n" + 
                      " * Run The Scene\n" + 
                      " * See output in Unity Console Window\n" + 
                      " * Open Beamable Content Manager Window, " +
                      "select one of the `ComplexItem`, " +
                      "and view it in the inspector to see the content validation \n");
            
            SetupBeamable();
        }

        //  Methods  --------------------------------------
        private async void SetupBeamable()
        {
            var beamContext = BeamContext.Default;
            await beamContext.OnReady;
      
            Debug.Log($"beamContext.PlayerId = {beamContext.PlayerId}");
            
            await _complexItemLink.Resolve()
                .Then(content =>
                {
                    _complexItem = content; 
                    Debug.Log($"_complexItemLink.Resolve() Success! " +
                              $"Id = {_complexItem.Id}");
                    
                }).Error(ex =>
                {
                    Debug.LogError($"_complexItemLink.Resolve() Error!"); 
                });
        }
    }
}