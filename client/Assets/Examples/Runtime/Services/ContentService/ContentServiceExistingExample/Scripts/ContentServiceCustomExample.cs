using Beamable.Examples.Shared;
using UnityEngine;

namespace Beamable.Examples.Services.ContentService
{
    /// <summary>
    /// Demonstrates <see cref="ContentService"/>.
    /// </summary>
    public class ContentServiceCustomExample : MonoBehaviour
    {
        //  Fields  ---------------------------------------
        [SerializeField] private ArmorLink _armorLink;
        [SerializeField] private ArmorRef _armorRef;

        private Armor _armorFromLink = null;
        private Armor _armorFromRef = null;
        
        //  Unity Methods  --------------------------------
        protected void Start()
        {
            Debug.Log($"Start()");
            
            SetupBeamable();
        }

        //  Methods  --------------------------------------
        private async void SetupBeamable()
        {
            var beamContext = BeamContext.Default;
            await beamContext.OnReady;
      
            Debug.Log($"beamContext.PlayerId = {beamContext.PlayerId}");
            
            await _armorLink.Resolve()
                .Then(content =>
                {
                    _armorFromLink = content; 
                    Debug.Log($"_armorFromLink.Resolve() Success! " +
                              $"Id = {_armorFromLink.Id}");
                })
                .Error(ex =>
                {
                    Debug.LogError($"_armorFromLink.Resolve() Error!"); 
                });
            
            await _armorRef.Resolve()
                .Then(content =>
                {
                    _armorFromRef = content; 
                    Debug.Log($"_armorFromRef.Resolve() Success! " +
                              $"Id = {_armorFromRef.Id}");
                    
                }).Error(ex =>
                {
                    Debug.LogError($"_armorFromRef.Resolve() Error!"); 
                });
            

        }
    }
}