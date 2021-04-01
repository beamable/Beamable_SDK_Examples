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
        
        //  Unity Methods  --------------------------------

        protected void Start()
        {
            Debug.Log("Start()");

            SetupBeamable();
        }
        
        //  Methods  --------------------------------------
        private async void SetupBeamable()
        {
            var beamableAPI = await Beamable.API.Instance;

            Debug.Log($"beamableAPI.User.id = {beamableAPI.User.id}");
            
            var simGameType = await _simGameTypeRef.Resolve();

            Debug.Log($"simGameType.maxPlayers = {simGameType.maxPlayers}");
            Debug.Log($"simGameType.minPlayersToStart = {simGameType.minPlayersToStart.Value}");
            
            var myMatchmaking = new MyMatchmaking(beamableAPI.Experimental.MatchmakingService, simGameType, beamableAPI.User.id);
            myMatchmaking.OnProgress += MyMatchmaking_OnProgress;
            myMatchmaking.OnComplete += MyMatchmaking_OnComplete;
            await myMatchmaking.Start();
        }
        
        
        //  Event Handlers  -------------------------------
        private void MyMatchmaking_OnProgress(MyMatchmakingResult myMatchmakingResult)
        {
            Debug.Log($"MyMatchmaking_OnProgress() " +
                      $"Players = {myMatchmakingResult.Players.Count}/{myMatchmakingResult.TargetPlayerCount} " +
                      $"RoomId = {myMatchmakingResult.RoomId}");
        }

        private void MyMatchmaking_OnComplete(MyMatchmakingResult myMatchmakingResult)
        {
            if (string.IsNullOrEmpty(myMatchmakingResult.ErrorMessage))
            {
                Debug.Log($"MyMatchmaking_OnComplete() Success! " +
                          $"RoomId = {myMatchmakingResult.RoomId}");
            }
            else
            {
                Debug.Log($"MyMatchmaking_OnComplete() Failed. " +
                          $"Error = {myMatchmakingResult.ErrorMessage}");
                
            }
        }
    }
}
