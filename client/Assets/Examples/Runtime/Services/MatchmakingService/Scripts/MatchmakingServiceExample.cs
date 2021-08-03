using Beamable.Common.Content;
using UnityEngine;

namespace Beamable.Examples.Services.MatchmakingService
{
    /// <summary>
    /// Demonstrates the creation of and joining to a
    /// Multiplayer game room with Beamable Multiplayer.
    /// </summary>
    public class MatchmakingServiceExample : MonoBehaviour
    {
        //  Fields  ---------------------------------------
        
        /// <summary>
        /// This defines the matchmaking criteria including "NumberOfPlayers"
        /// </summary>
        [SerializeField]
        private SimGameTypeRef _simGameTypeRef;
        private IBeamableAPI _beamableAPI = null;
        
        
        //  Unity Methods  --------------------------------
        protected void Start()
        {
            Debug.Log("Start()\n\n");
            SetupBeamable();
        }
        
        
        //  Methods  --------------------------------------
        private async void SetupBeamable()
        {
            _beamableAPI = await Beamable.API.Instance;
            Debug.Log($"beamableAPI.User.id = {_beamableAPI.User.id}\n\n");
            
            var simGameType = await _simGameTypeRef.Resolve();
            
            var myMatchmaking = new MyMatchmaking(
                _beamableAPI.Experimental.MatchmakingService, 
                simGameType, 
                _beamableAPI.User.id);
            
            myMatchmaking.OnProgress.AddListener(MyMatchmaking_OnProgress);
            myMatchmaking.OnComplete.AddListener(MyMatchmaking_OnComplete);
            myMatchmaking.OnError.AddListener(MyMatchmaking_OnError);
            
            Debug.Log($"myMatchmaking.StartMatchmaking()\n\n");
            await myMatchmaking.StartMatchmaking();
        }
        
        
        //  Event Handlers  -------------------------------
        private void MyMatchmaking_OnProgress(MyMatchmakingResult myMatchmakingResult)
        {
            Debug.Log($"MyMatchmaking_OnProgress()...\n\n" +
                      $"MatchId = {myMatchmakingResult.MatchId}\n" +
                      $"Players = {myMatchmakingResult.Players.Count} of {myMatchmakingResult.PlayerCountMax}\n");

        }

        
        private void MyMatchmaking_OnComplete(MyMatchmakingResult myMatchmakingResult)
        {
            Debug.Log($"MyMatchmaking_OnComplete()...\n\n" +
                      $"GameId = {myMatchmakingResult.GameId}\n" +
                      $"MatchId = {myMatchmakingResult.MatchId}\n" +
                      $"LocalPlayer = {myMatchmakingResult.LocalPlayer}\n" +
                      $"Players = {string.Join(",", myMatchmakingResult.Players)}\n");
        }
        
        
        private void MyMatchmaking_OnError(MyMatchmakingResult myMatchmakingResult)
        {
            Debug.Log($"MyMatchmaking_OnError()...\n\n" +
                      $"ErrorMessage = {myMatchmakingResult.ErrorMessage}\n");
            
        }
    }
}
