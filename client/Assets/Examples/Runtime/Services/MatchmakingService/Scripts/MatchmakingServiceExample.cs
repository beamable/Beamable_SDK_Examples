using System.Collections.Generic;
using Beamable.Common.Content;
using UnityEngine;
using UnityEngine.Events;

namespace Beamable.Examples.Services.MatchmakingService
{
    public enum SessionState
    {
        None,
        Connecting,
        Connected,
        Disconnecting,
        Disconnected

    }

    
    /// <summary>
    /// Holds data for use in the <see cref="MatchmakingServiceExampleUI"/>.
    /// </summary>
    [System.Serializable]
    public class MatchmakingServiceExampleData
    {
        public SessionState SessionState = SessionState.None;
        public bool CanStart { get { return SessionState == SessionState.Disconnected;}} 
        public bool CanCancel { get { return SessionState == SessionState.Connected;}} 
        public List<string> MainLogs = new List<string>();
        public List<string> MatchmakingLogs = new List<string>();
        public List<string> InstructionsLogs = new List<string>();
    }
   
    [System.Serializable]
    public class RefreshedUnityEvent : UnityEvent<MatchmakingServiceExampleData> { }
    
    /// <summary>
    /// Demonstrates the creation of and joining to a
    /// Multiplayer game match with Beamable Multiplayer.
    /// </summary>
    public class MatchmakingServiceExample : MonoBehaviour
    {
        //  Events  ---------------------------------------
        [HideInInspector]
        public RefreshedUnityEvent OnRefreshed = new RefreshedUnityEvent();

        
        //  Fields  ---------------------------------------

        /// <summary>
        /// This defines the matchmaking criteria including "NumberOfPlayers"
        /// </summary>
        [SerializeField] private SimGameTypeRef _simGameTypeRef;
        private BeamContext _beamContext;
        private MyMatchmaking _myMatchmaking = null;
        private SimGameType _simGameType = null;
        private MatchmakingServiceExampleData _data = new MatchmakingServiceExampleData();

        //  Unity Methods  --------------------------------
        protected void Start()
        {
            string startLog = $"Start() Instructions..\n" +
                              $"\n * Play Scene" +
                              $"\n * View UI" +
                              $"\n * Press 'Start Matchmaking' Button \n\n";
         
            Debug.Log(startLog);
            
            _data.InstructionsLogs.Add("View UI");
            _data.InstructionsLogs.Add("Press 'Start Matchmaking' Button");
            
            SetupBeamable();
        }


        //  Methods  --------------------------------------
        private async void SetupBeamable()
        {
            _beamContext = BeamContext.Default;
            await _beamContext.OnReady;
            Debug.Log($"beamContext.PlayerId = {_beamContext.PlayerId}\n\n");

            _data.SessionState = SessionState.Disconnected;
            
            _simGameType = await _simGameTypeRef.Resolve();
            _data.MainLogs.Add($"beamContext.PlayerId = {_beamContext.PlayerId}");
            _data.MainLogs.Add($"SimGameType.Teams.Count = {_simGameType.teams.Count}");

            _myMatchmaking = new MyMatchmaking(
                _beamContext.Api.Experimental.MatchmakingService,
                _simGameType,
                _beamContext.PlayerId);

            _myMatchmaking.OnProgress.AddListener(MyMatchmaking_OnProgress);
            _myMatchmaking.OnComplete.AddListener(MyMatchmaking_OnComplete);
            _myMatchmaking.OnError.AddListener(MyMatchmaking_OnError);

            Refresh();
        }

        public async void StartMatchmaking()
        {
            string log = $"StartMatchmaking()";
            
            //Debug.Log(log);
            _data.SessionState = SessionState.Connecting;
            _data.MatchmakingLogs.Add(log);
            Refresh();
            
            await _myMatchmaking.StartMatchmaking();
        }
        
        public async void CancelMatchmaking()
        {
            string log = $"CancelMatchmaking()";
            //Debug.Log(log);
            
            _data.SessionState = SessionState.Disconnecting;
            _data.MatchmakingLogs.Add(log);
            Refresh();
            
            await _myMatchmaking.CancelMatchmaking();
            
            _data.SessionState = SessionState.Disconnected;
            Refresh();
        }
        

        public void Refresh()
        {
            string refreshLog = $"Refresh() ...\n" +
                                $"\n * MainLogs.Count = {_data.MainLogs.Count}" +
                                $"\n * MatchmakingLogs.Count = {_data.MatchmakingLogs.Count}\n\n";
         
            //Debug.Log(refreshLog);
         
            // Send relevant data to the UI for rendering
            OnRefreshed?.Invoke(_data);
        }
        
        //  Event Handlers  -------------------------------
        private void MyMatchmaking_OnProgress(MyMatchmakingResult myMatchmakingResult)
        {
            string log = $"OnProgress(), Players = {myMatchmakingResult.Players.Count} / {myMatchmakingResult.PlayerCountMax}";
            _data.MatchmakingLogs.Add(log);
            Refresh();
        }


        private void MyMatchmaking_OnComplete(MyMatchmakingResult myMatchmakingResult)
        {
            string log = $"OnComplete()...\n" + 
                         $"\tMatchId = {myMatchmakingResult.MatchId}\n " +
                         $"\tLocalPlayer = {myMatchmakingResult.LocalPlayer}\n" +
                         $"\tPlayers = {string.Join(",", myMatchmakingResult.Players)}\n";
            
            //Debug.Log(log);
            _data.SessionState = SessionState.Connected;
            _data.MatchmakingLogs.Add(log);
            Refresh();
        }


        private void MyMatchmaking_OnError(MyMatchmakingResult myMatchmakingResult)
        {
            _data.SessionState = SessionState.Disconnected;
            string log = $"OnError(), ErrorMessage = {myMatchmakingResult.ErrorMessage}\n";
            
            //Debug.Log(log);
            _data.MatchmakingLogs.Add(log);
            Refresh();
        }
    }
}