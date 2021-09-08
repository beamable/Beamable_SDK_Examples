using System.Threading.Tasks;
using Beamable.Common.Leaderboards;
using Beamable.Examples.Shared;
using UnityEngine;

namespace Beamable.Examples.Services.LeaderboardService.LeaderboardServiceExample
{
    /// <summary>
    /// Demonstrates <see cref="LeaderboardService"/>.
    /// </summary>
    public class LeaderboardServiceExample : MonoBehaviour
    {
        //  Fields  ---------------------------------------
        [SerializeField] private LeaderboardRef _leaderboardRef = null;
        [SerializeField] private double _score = 100;
        private IBeamableAPI _beamableAPI = null;
        
        //  Unity Methods  --------------------------------
        protected void Start()
        {
            Debug.Log($"Start()");
 
            SetupBeamable();
        }

        //  Methods  --------------------------------------
        private async void SetupBeamable()
        {
            _beamableAPI = await Beamable.API.Instance;
            
            Debug.Log($"_beamableAPI.User.id = {_beamableAPI.User.id}");
            
            // Set
            Debug.Log($"LeaderboardService.SetScore({_leaderboardRef.Id},{_score})");
            await LeaderboardServiceSetScore(_leaderboardRef.Id, _score);
            
            // Get
            double score = await LeaderboardServiceGetScore(_leaderboardRef.Id);
            Debug.Log($"LeaderboardService.GetScore({_leaderboardRef.Id},{score})");
        }
        
        
        private async Task LeaderboardServiceSetScore(string id, double score)
        {
            // Set the user alias - This is likely not appropriate for a production project
            await MockDataCreator.SetCurrentUserAlias(_beamableAPI.StatsService, MockDataCreator.DefaultMockAlias);
            
            // Set the score
            await _beamableAPI.LeaderboardService.SetScore(id, score);
        }

        
        private async Task<double> LeaderboardServiceGetScore(string id)
        {
            var score = await _beamableAPI.LeaderboardService
                .GetUser(id, _beamableAPI.User.id)
                .Map(entry => entry.score);

            return score;
        }
    }
}


