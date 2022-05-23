using System;
using System.Collections.Generic;
using Beamable.Common.Leaderboards;
using Beamable.Examples.Shared;
using Beamable.Leaderboards;
using UnityEngine;

namespace Beamable.Examples.Prefabs.LeaderboardFlow.LeaderboardFlowInterpolationExample
{
    /// <summary>
    /// Demonstrates <see cref="LeaderboardFlow"/>.
    /// </summary>
    public class LeaderboardFlowInterpolationExample : MonoBehaviour
    {
        [Header("Customization")]
        [SerializeField] 
        private LeaderboardMainMenu _leaderboardMainMenu = null;

        [SerializeField] 
        private LeaderboardItemCustom _leaderboardItemCustom = null;
        
        [Header("Cosmetics")]
        [SerializeField] 
        private List<GameObject> _gameObjectsToHide = null;

        private LeaderboardMainMenuCustom _leaderboardMainMenuCustom = null;
        private BeamContext _beamContext;
        
        //  Unity Methods  --------------------------------
        protected void Awake()
        {
            SetupLeaderboard();
        }
        
        protected void Start()
        {
            foreach (GameObject go in _gameObjectsToHide)
            {
                //Hide back button - not needed for this demo
                go.SetActive(false);
            }
            
            Debug.Log($"Start() Instructions...\n\n" +
                      " * Run The Scene\n" +
                      " * See UI representing leaderboard on-screen\n" +
                      " * No leaderboard results? Clear related leaderboard via Toolbox -> Portal\n" +
                      " * Leaderboard results do not rapidly increment in UI? Clear related leaderboard via via Toolbox -> Portal\n" +
                      "");

            SetupBeamable();
        }
        
        //  Methods  --------------------------------------
        private void SetupLeaderboard()
        {
            // Use "LeaderboardMainMenuCustom" instead of "LeaderboardMainMenu"
            _leaderboardMainMenuCustom = _leaderboardMainMenu.gameObject.AddComponent<LeaderboardMainMenuCustom>();
            
            // Use "LeaderboardItemCustom" instead of "LeaderboardItem"
            _leaderboardMainMenuCustom.LeaderboardItemPrefab = _leaderboardItemCustom;
            _leaderboardMainMenuCustom.LeaderboardBehavior = _leaderboardMainMenu.LeaderboardBehavior;
            _leaderboardMainMenuCustom.LeaderboardEntries = _leaderboardMainMenu.LeaderboardEntries;
            
            // Remove old
            Destroy(_leaderboardMainMenu);
        }
        

        private async void SetupBeamable()
        {
            _beamContext = BeamContext.Default;
            await _beamContext.OnReady;
            Debug.Log($"_beamContext.PlayerId = {_beamContext.PlayerId}");

            LeaderboardContent leaderboardContent = 
                await _leaderboardMainMenuCustom.LeaderboardBehavior.Leaderboard.Resolve();

            Debug.Log($"PopulateLeaderboard Starting. Wait < 30 seconds... ");
            int leaderboardRowCountMin = 10;
            int leaderboardScoreMin = 99;
            int leaderboardScoreMax = 99999;

            // Populate with custom values 
            Dictionary<string, object> leaderboardStats = new Dictionary<string, object>();
            leaderboardStats.Add("leaderboard_score_timestamp", new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds());
            leaderboardStats.Add("leaderboard_score_velocity", 99); // 99 score delta per second
            
            // Populates mock "alias" and "score" for each leaderboard row
            string loggingResult = await MockDataCreator.PopulateLeaderboardWithMockData(
                _beamContext, 
                leaderboardContent,
                leaderboardRowCountMin,
                leaderboardScoreMin,
                leaderboardScoreMax,
                leaderboardStats);
            
            Debug.Log($"PopulateLeaderboard Finish. Result = {loggingResult}");
        }
        
    }
}























