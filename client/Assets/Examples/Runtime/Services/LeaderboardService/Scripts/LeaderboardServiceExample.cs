using System.Threading.Tasks;
using Beamable.Common.Leaderboards;
using UnityEngine;

namespace Beamable.Examples.Services.LeaderboardService
{
    /// <summary>
    /// Demonstrates <see cref="LeaderboardService"/>.
    /// </summary>
    public class LeaderboardServiceExample : MonoBehaviour
    {
        //  Fields  ---------------------------------------
        [SerializeField] private LeaderboardRef _leaderboardRef = null;
        [SerializeField] private double _score = 100;
        private IBeamableAPI beamableAPI = null;
        
        //  Unity Methods  --------------------------------
        protected async void Start()
        {
            Debug.Log($"Start()");

 
            SetupBeamable();
        }

        //  Methods  --------------------------------------
        private async void SetupBeamable()
        {
            beamableAPI = await Beamable.API.Instance;
            
            Debug.Log($"beamableAPI.User.id = {beamableAPI.User.id}");
            
            // Set
            Debug.Log($"LeaderboardService.SetScore({_leaderboardRef.Id},{_score})");
            await LeaderboardServiceSetScore(_leaderboardRef.Id, _score);
            
            // Get
            double score = await LeaderboardServiceGetScore(_leaderboardRef.Id);
            Debug.Log($"LeaderboardService.GetScore({_leaderboardRef.Id},{score})");
        }
        
        
        private async Task LeaderboardServiceSetScore(string id, double score)
        {
            await beamableAPI.LeaderboardService.SetScore(id, score);
        }

        
        private async Task<double> LeaderboardServiceGetScore(string id)
        {
            var score = await beamableAPI.LeaderboardService
                .GetUser(id, beamableAPI.User.id)
                .Map(entry => entry.score);

            return score;
        }
    }
}























