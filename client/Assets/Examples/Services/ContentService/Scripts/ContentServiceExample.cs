using Beamable.Common.Content;
using Beamable.Common.Tournaments;
using UnityEngine;

namespace Beamable.Examples.Services.ContentService
{
    [ContentType("samples")]
    public class SampleContent : ContentObject
    {
        public int x;
        public int y;
        public TournamentRef TournamentRef;
    }

    public class SampleRef : ContentRef<SampleContent> {}
    public class SampleLink : ContentLink<SampleContent>{}
    
    /// <summary>
    /// Demonstrates <see cref="ContentService"/>.
    /// </summary>
    public class ContentServiceExample : MonoBehaviour
    {
        [SerializeField] private TournamentRef _tournamentRef;
        [SerializeField] private TournamentLink _tournamentLink;

        private TournamentContent _tournamentContentFromRef = null;
        private TournamentContent _tournamentContentFromLink = null;
        
        protected void Start()
        {
            Debug.Log("Start()");
            
            _tournamentRef.Resolve()
                .Then(content =>
                {
                    _tournamentContentFromRef = content; 
                    Debug.Log("_tournamentRef.Resolve() Success!");
                    
                }).Error(ex =>
                {
                    Debug.LogError("_tournamentRef.Resolve() Error!"); 
                });

            _tournamentLink.Resolve()
                .Then(content =>
                {
                    _tournamentContentFromLink = content; 
                    Debug.Log("_tournamentLink.Resolve() Success!");
                })
                .Error(ex =>
                {
                    Debug.LogError("_tournamentLink.Resolve() Error!"); 
                });
        }
    }
}