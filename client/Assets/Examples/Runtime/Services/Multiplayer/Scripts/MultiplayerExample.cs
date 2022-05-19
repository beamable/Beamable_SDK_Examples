using System.Collections.Generic;
using Beamable.Experimental.Api.Sim;
using UnityEngine;
using UnityEngine.Events;

namespace Beamable.Examples.Services.Multiplayer
{
   [System.Serializable]
   public class MultiplayerExampleDataEvent : UnityEvent<MultiplayerExampleData> { }

   public enum SessionState
   {
      None,
      Initializing,
      Initialized,
      Connected,
      Disconnected
   }
   
   /// <summary>
   /// Holds data for use in the <see cref="MultiplayerExample"/>.
   /// </summary>
   [System.Serializable]
   public class MultiplayerExampleData
   {
      public string MatchId = null;
      public string SessionSeed;
      public long CurrentFrame;
      public long LocalPlayerDbid;
      public bool IsSessionConnected { get { return SessionState == SessionState.Connected; }}
      public SessionState SessionState = SessionState.None;
      public List<string> PlayerMoveLogs = new List<string>();
      public List<string> PlayerDbids = new List<string>();
   }
   
   /// <summary>
   /// Defines a simple type of in-game "move" sent by a player
   /// </summary>
   public class MyPlayerMoveEvent
   {
      public static string Name = "MyPlayerMoveEvent";
      public long PlayerDbid;
      public Vector3 Position;

      public MyPlayerMoveEvent(long playerDbid, Vector3 position)
      {
         PlayerDbid = playerDbid;
         Position = position;
      }

      public override string ToString()
      {
         return $"[MyPlayerMoveEvent({Position})]";
      }
   }
   
    /// <summary>
    /// Demonstrates <see cref="SimClient"/>.
    /// </summary>
    public class MultiplayerExample : MonoBehaviour
    {
       //  Events  ---------------------------------------
       [HideInInspector]
       public MultiplayerExampleDataEvent OnRefreshed = new MultiplayerExampleDataEvent();
       
        //  Fields  ---------------------------------------
       
        private MultiplayerExampleData _multiplayerExampleData = new MultiplayerExampleData();
        private const long FramesPerSecond = 20;
        private const long TargetNetworkLead = 4;
        private SimClient _simClient;
        private BeamContext _beamContext;
        
        //  Unity Methods  --------------------------------
        protected void Start()
        {
           Debug.Log($"Start() Instructions...\n\n" + 
                     " * Run The Scene\n" + 
                     " * Press 'Start Multiplayer'\n" + 
                     " * Press 'Send Player Move'\n" + 
                     " * See results in the in-game UI\n");

            SetupBeamable();
        }
        
        
        protected void Update()
        {
           if (_simClient != null) 
           { 
              _simClient.Update(); 
           }

           string refreshString = "";
           refreshString += $"MatchId: {_multiplayerExampleData.MatchId}\n";
           refreshString += $"Seed: {_multiplayerExampleData.SessionSeed}\n";
           refreshString += $"Frame: {_multiplayerExampleData.CurrentFrame}\n";
           refreshString += $"Dbids:";
           foreach (var dbid in _multiplayerExampleData.PlayerDbids)
           {
              refreshString += $"{dbid},";
           }

           //Debug.Log($"message:{refreshString}");
        }

        
        protected void OnDestroy()
        {
           if (_simClient != null)
           {
              StopMultiplayer();
           }
        }
        
        
        //  Methods  --------------------------------------
        private async void SetupBeamable()
        {
           _beamContext = BeamContext.Default;
           await _beamContext.OnReady;

            Debug.Log($"_beamContext.PlayerId = {_beamContext.PlayerId}");
            
            // Access Local Player Information
            _multiplayerExampleData.LocalPlayerDbid = _beamContext.PlayerId;

        }
        
        public void StartMultiplayer()
        {
           if (_simClient != null)
           {
              StopMultiplayer();
           }
           
           _multiplayerExampleData.SessionState = SessionState.Initializing;
           Refresh();
           
           // Generates a specific MatchId
           // (Otherwise use Beamable's MatchmakingService)
           _multiplayerExampleData.MatchId = "MyCustomMatchId_" + UnityEngine.Random.Range(0,99999);
           
           // Create Multiplayer Session
           _simClient = new SimClient(
              new SimNetworkEventStream(_multiplayerExampleData.MatchId, _beamContext.ServiceProvider), 
              FramesPerSecond, TargetNetworkLead);
        
           // Handle Common Events
           _simClient.OnInit(SimClient_OnInit);
           _simClient.OnConnect(SimClient_OnConnect);
           _simClient.OnDisconnect(SimClient_OnDisconnect);
           _simClient.OnTick(SimClient_OnTick);
        }

        public void StopMultiplayer()
        {
           if (_simClient != null)
           {
              // TODO: Manually Disconnect/close?
              _simClient = null;
           }

           _multiplayerExampleData.SessionState = SessionState.Disconnected;
           Refresh();
        }

        public void SendPlayerMoveButton()
        {
           if (_simClient == null)
           {
              return;
           }
           
           // Create a mock  player position
           Vector3 position = new Vector3(
              Random.Range(1, 10),
              Random.Range(1, 10),
              Random.Range(1, 10)
              );
           
           _simClient.SendEvent(MyPlayerMoveEvent.Name,
              new MyPlayerMoveEvent(_multiplayerExampleData.LocalPlayerDbid, position));
        }
        
        public void Refresh()
        {
           string refreshLog = $"Refresh() ...\n" +
                               $"\n * LocalPlayerDbid = {_multiplayerExampleData.LocalPlayerDbid}\n\n" +
                               $"\n * PlayerDbids.Count = {_multiplayerExampleData.PlayerDbids.Count}\n\n" +
                               $"\n * PlayerMoveLogs.Count = {_multiplayerExampleData.PlayerMoveLogs.Count}\n\n";
            
           //Debug.Log(refreshLog);

           OnRefreshed?.Invoke(_multiplayerExampleData);
        }

        
        //  Event Handlers  -------------------------------
        private void SimClient_OnInit(string sessionSeed)
        {
           _multiplayerExampleData.SessionState = SessionState.Initialized;
           _multiplayerExampleData.SessionSeed = sessionSeed;
           Refresh();
           
           Debug.Log($"SimClient_OnInit()...\n" + 
                     $"MatchId = {_multiplayerExampleData.MatchId}, " +
                     $"sessionSeed = {sessionSeed}");
        }

        
        private void SimClient_OnConnect(string dbid)
        {
           _multiplayerExampleData.SessionState = SessionState.Connected;
           _multiplayerExampleData.PlayerDbids.Add(dbid);
           Refresh();
        
           // Handle Custom Events for EACH dbid
           _simClient.On<MyPlayerMoveEvent>(MyPlayerMoveEvent.Name, dbid,
              SimClient_OnMyPlayerMoveEvent);
        
           Debug.Log($"SimClient_OnConnect() dbid = {dbid}");
        }

        
        private void SimClient_OnDisconnect(string dbid)
        {
           if (long.Parse(dbid) == _multiplayerExampleData.LocalPlayerDbid)
           {
              StopMultiplayer();
           }
           
           _multiplayerExampleData.PlayerDbids.Remove(dbid);
           Refresh();
           
           Debug.Log($"SimClient_OnDisconnect() dbid = {dbid}");
        }

        
        private void SimClient_OnTick(long currentFrame)
        {
           _multiplayerExampleData.CurrentFrame = currentFrame;
           Refresh();
        }

        
        private void SimClient_OnMyPlayerMoveEvent(MyPlayerMoveEvent myPlayerMoveEvent)
        {
           string playerMoveLog = $"{myPlayerMoveEvent}";
           _multiplayerExampleData.PlayerMoveLogs.Add(playerMoveLog);
           Refresh();
        }
    }
}