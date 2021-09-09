using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Common.Leaderboards;
using Beamable.Examples.Shared;
using UnityEngine;

namespace Beamable.Examples.Services.LeaderboardService.LeaderboardServiceOfflineExample
{
    /// <summary>
    /// Demonstrates <see cref="LeaderboardService"/>.
    /// </summary>
    public class LeaderboardServiceOfflineExample : MonoBehaviour
    {
        //  Fields  ---------------------------------------
        [SerializeField] 
        private LeaderboardRef _leaderboardRef = null;
        
        [Tooltip("The Leaderboard score will change by this amount.")]
        [SerializeField] 
        private double _scoreDelta = 100;
        
        [Tooltip("Determines if internet will be manually disconnected.")]
        [SerializeField] 
        private bool _willDisconnectDuringSetScore = true;

        
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
            
            // Get
            double scoreInitial = await LeaderboardServiceGetScore(_leaderboardRef.Id);
            Debug.Log($"LeaderboardService.GetScore({_leaderboardRef.Id}) ={scoreInitial})");

            // Set
            double scoreNext = scoreInitial + _scoreDelta;
            await LeaderboardServiceSetScore(_leaderboardRef.Id, scoreNext);
            Debug.Log($"LeaderboardService.SetScore({_leaderboardRef.Id},{scoreNext})");
            
            // Get
            double scoreFinal = await LeaderboardServiceGetScore(_leaderboardRef.Id);
            Debug.Log($"LeaderboardService.GetScore({_leaderboardRef.Id}) = {scoreFinal})");
        }
        
        
        private async Task LeaderboardServiceSetScore(string id, double score)
        {
            // Set the user alias - This is likely not appropriate for a production project
            await MockDataCreator.SetCurrentUserAlias(_beamableAPI.StatsService, MockDataCreator.DefaultMockAlias);
            
            Dictionary<string, object> stats = new Dictionary<string, object>();
            
            // Maybe disconnect
            if (_willDisconnectDuringSetScore)
            {
                _beamableAPI.ConnectivityService.SetHasInternet(false);
                stats.Add("_final_score", score);
            }
            
            // Question: #1 Does SetScore work when offline?
            // Question: #1 If yes, why does it throw an error every time when offline?
            try
            {
                // Set the score
                await _beamableAPI.LeaderboardService.SetScore(id, score, stats);
            }
            catch (Exception e)
            {
                //offline, throws exception every time of...
                //  object/leaderboards/leaderboards.Leaderboard01/entry?id=1402161530324831&score=340&increment=False
                //  should not be cached and requires internet connectivity.
                Debug.LogWarning(e.Message);
            }
            
            
            // Maybe reconnect
            if (_willDisconnectDuringSetScore)
            {
                _beamableAPI.ConnectivityService.SetHasInternet(true);
            }
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


