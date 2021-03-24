using Beamable.Common.Tournaments;
using UnityEngine;

namespace Beamable.Examples.Services.ContentService
{
    /// <summary>
    /// Demonstrates <see cref="ContentService"/>.
    /// </summary>
    public class ContentServiceExample : MonoBehaviour
    {
        //  Fields  ---------------------------------------
        [SerializeField] private TournamentRef _tournamentRef;
        [SerializeField] private TournamentLink _tournamentLink;

        private TournamentContent _tournamentContentFromRef = null;
        private TournamentContent _tournamentContentFromLink = null;
        
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
      
            await _tournamentRef.Resolve()
                .Then(content =>
                {
                    _tournamentContentFromRef = content; 
                    Debug.Log($"_tournamentRef.Resolve() Success! Id = {_tournamentContentFromRef.Id}");
                    
                }).Error(ex =>
                {
                    Debug.LogError($"_tournamentRef.Resolve() Error!"); 
                });
            
            await _tournamentLink.Resolve()
                .Then(content =>
                {
                    _tournamentContentFromLink = content; 
                    Debug.Log($"_tournamentLink.Resolve() Success! Id = {_tournamentContentFromLink.Id}");
                })
                .Error(ex =>
                {
                    Debug.LogError($"_tournamentLink.Resolve() Error!"); 
                });
        }
    }
}