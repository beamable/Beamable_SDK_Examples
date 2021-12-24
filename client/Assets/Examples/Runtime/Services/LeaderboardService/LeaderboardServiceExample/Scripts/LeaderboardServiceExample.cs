using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Common;
using Beamable.Common.Api.Leaderboards;
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

            // Get Board - Log 100 results nearest to active user
            Debug.Log($"LeaderboardService.GetBoard({_leaderboardRef.Id},{_beamableAPI.User.id})");
            List<RankEntry> rankEntries = await LeaderboardServiceGetBoard(_leaderboardRef.Id, _beamableAPI.User.id);
            
            // Set Score
            Debug.Log($"LeaderboardService.SetScore({_leaderboardRef.Id},{_score})");
            await LeaderboardServiceSetScore(_leaderboardRef.Id, _score);
            
            // Get Score
            double score = await LeaderboardServiceGetScore(_leaderboardRef.Id);
            Debug.Log($"LeaderboardService.GetScore({_leaderboardRef.Id},{score})");
        }
        
        private async Task<List<RankEntry>> LeaderboardServiceGetBoard(string id, long userId)
        {
            LeaderBoardView leaderBoardView = await _beamableAPI.LeaderboardService.GetBoard(id, 0, 100, userId);

            foreach (RankEntry rankEntry in leaderBoardView.rankings)
            {
                // Get alias for userId of rankEntry
                long nextUserId = rankEntry.gt;
                var stats = 
                    await _beamableAPI.StatsService.GetStats("client", "public", "player", nextUserId );
                
                string alias = "";
                stats.TryGetValue(alias, out alias);
                if (string.IsNullOrEmpty(alias))
                {
                    alias = "Unknown Alias";
                }
                
                // Log
                Debug.Log($"Rank = {rankEntry.rank}, Alias = {alias}, Score = {rankEntry.score}");
            }

            return leaderBoardView.rankings;
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


