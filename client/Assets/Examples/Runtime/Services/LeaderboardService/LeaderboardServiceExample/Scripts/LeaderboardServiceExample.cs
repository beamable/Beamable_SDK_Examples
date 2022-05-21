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

            // Get Board - Log 100 results nearest to active user
            Debug.Log($"LeaderboardService.GetBoard({_leaderboardRef.Id},{_beamContext.PlayerId})");
            List<RankEntry> rankEntries = await LeaderboardServiceGetBoard(_leaderboardRef.Id, _beamContext.PlayerId);
            
            // Set Score
            Debug.Log($"LeaderboardService.SetScore({_leaderboardRef.Id},{_score})");
            await LeaderboardServiceSetScore(_leaderboardRef.Id, _score);
            
            // Get Score
            double score = await LeaderboardServiceGetScore(_leaderboardRef.Id);
            Debug.Log($"LeaderboardService.GetScore({_leaderboardRef.Id},{score})");
        }
        
        private async Task<List<RankEntry>> LeaderboardServiceGetBoard(string id, long userId)
        {
            LeaderBoardView leaderBoardView = await _beamContext.Api.LeaderboardService.GetBoard(id, 0, 100, userId);

            foreach (RankEntry rankEntry in leaderBoardView.rankings)
            {
                // Get alias for userId of rankEntry
                long nextUserId = rankEntry.gt;
                var stats = 
                    await _beamContext.Api.StatsService.GetStats("client", "public", "player", nextUserId );
                
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
            await MockDataCreator.SetCurrentUserAlias(_beamContext.Api.StatsService, MockDataCreator.DefaultMockAlias);
            
            // Set the score
            await _beamContext.Api.LeaderboardService.SetScore(id, score);
        }

        
        private async Task<double> LeaderboardServiceGetScore(string id)
        {
            var score = await _beamContext.Api.LeaderboardService
                .GetUser(id, _beamContext.PlayerId)
                .Map(entry => entry.score);

            return score;
        }
    }
}


