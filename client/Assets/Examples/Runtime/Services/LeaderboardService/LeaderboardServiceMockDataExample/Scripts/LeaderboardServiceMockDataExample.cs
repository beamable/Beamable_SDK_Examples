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
        private BeamContext _beamContext;
        
        //  Unity Methods  --------------------------------
        protected void Start()
        {
            Debug.Log($"Start()");
 
            SetupBeamable();
        }

        //  Methods  --------------------------------------
        private async void SetupBeamable()
        {
            _beamContext = BeamContext.Default;
            await _beamContext.OnReady;
            Debug.Log($"_beamContext.PlayerId = {_beamContext.PlayerId}");
            
            LeaderboardContent leaderboardContent = await _leaderboardRef.Resolve();

            Debug.Log($"PopulateLeaderboard Starting. Wait < 30 seconds... ");
            int leaderboardRowCountMin = 10;
            int leaderboardScoreMin = 1000;
            int leaderboardScoreMax = 9999;
            
            // Populates mock "alias" and "score" for each leaderboard row
            string loggingResult = await MockDataCreator.PopulateLeaderboardWithMockData(
                _beamContext, 
                leaderboardContent,
                leaderboardRowCountMin,
                leaderboardScoreMin,
                leaderboardScoreMax);
            
            Debug.Log($"PopulateLeaderboard Finish. Result = {loggingResult}");
        }
    }
}









