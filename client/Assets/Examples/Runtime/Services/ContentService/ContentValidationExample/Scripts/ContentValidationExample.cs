using UnityEngine;

namespace Beamable.Examples.Services.ContentService
{
    /// <summary>
    /// Demonstrates content validation within <see cref="ComplexItem"/>.
    /// </summary>
    public class ContentValidationExample : MonoBehaviour
    {
        //  Fields  ---------------------------------------
        [SerializeField] private ComplexItemLink _complexItemLink;
        private ComplexItem _complexItem = null;
        
        //  Unity Methods  --------------------------------
        protected void Start()
        {
            Debug.Log($"Start()");
            
            SetupBeamable();
        }

        //  Methods  --------------------------------------
        private async void SetupBeamable()
        {
            IBeamableAPI beamableAPI = await Beamable.API.Instance;
      
            Debug.Log($"beamableAPI.User.id = {beamableAPI.User.id}");
            
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