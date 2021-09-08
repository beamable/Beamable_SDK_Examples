using Beamable.Common.Leaderboards;
using Beamable.Examples.Shared;
using UnityEngine;

namespace Beamable.Examples.Services.LeaderboardService.LeaderboardServiceMockDataExample
{
    /// <summary>
    /// Demonstrates <see cref="LeaderboardService"/>.
    /// </summary>
    public class LeaderboardServiceMockDataExample : MonoBehaviour
    {
        //  Fields  ---------------------------------------
        [SerializeField] private LeaderboardRef _leaderboardRef = null;
        [SerializeField] private double _score = 100;
        private IBeamableAPI beamableAPI = null;
        
        //  Unity Methods  --------------------------------
        protected void Start()
        {
            Debug.Log($"Start()");
 
            SetupBeamable();
        }

        //  Methods  --------------------------------------
        private async void SetupBeamable()
        {
            beamableAPI = await Beamable.API.Instance;
            Debug.Log($"beamableAPI.User.id = {beamableAPI.User.id}");
            
            LeaderboardContent leaderboardContent = await _leaderboardRef.Resolve();

            Debug.Log($"PopulateLeaderboard Starting. Wait < 30 seconds... ");
            int leaderboardRowCountMin = 10;
            int leaderboardScoreMin = 1000;
            int leaderboardScoreMax = 9999;
            
            // Populates mock "alias" and "score" for each leaderboard row
            string loggingResult = await MockDataCreator.PopulateLeaderboardWithMockData(
                beamableAPI, 
                leaderboardContent,
                leaderboardRowCountMin,
                leaderboardScoreMin,
                leaderboardScoreMax);
            
            Debug.Log($"PopulateLeaderboard Finish. Result = {loggingResult}");
        }
        
    }
}









