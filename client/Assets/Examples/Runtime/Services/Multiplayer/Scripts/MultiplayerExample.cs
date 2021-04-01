using System;
using System.Collections.Generic;
using Beamable.Experimental.Api.Sim;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Beamable.Examples.Services.Multiplayer
{
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
   }
   
    /// <summary>
    /// Demonstrates <see cref="SimClient"/>.
    /// </summary>
    public class MultiplayerExample : MonoBehaviour
    {
        //  Fields  ---------------------------------------
        private const long FramesPerSecond = 20;
        private const long TargetNetworkLead = 4;
        private const string RoomId = "MyCustomRoomId";

        private SimClient _simClient;
        private string _sessionSeed;
        private long _currentFrame;
        private List<string> _sessionPlayerDbids = new List<string>();
        private long _localPlayerDbid;
        
        
        //  Unity Methods  --------------------------------
        protected void Start()
        {
           Debug.Log($"Start() Instructions...\n" + 
                     " * Run The Scene\n" + 
                     " * Press 'Spacebar' in Unity Game Window to send custom event\n" + 
                     " * See results in the Unity Console Window\n");

            SetupBeamable();
        }
        
        
        protected void Update()
        {
           if (_simClient != null) 
           { 
              _simClient.Update(); 
           }

           // Send Custom Events
           if (Input.GetKeyDown(KeyCode.Space))
           {
              // Mock a random player position
              Vector3 position = new Vector3().normalized * Random.Range(1, 10);
              
              Debug.Log($"_simClient.SendEvent() Position = {position}");

              _simClient.SendEvent(MyPlayerMoveEvent.Name,
                 new MyPlayerMoveEvent(_localPlayerDbid, position));
           }

           string message = "";
           message += $"Room: {RoomId}\n";
           message += $"Seed: {_sessionSeed}\n";
           message += $"Frame: {_currentFrame}\n";
           message += $"Dbids:";
           foreach (var dbid in _sessionPlayerDbids)
           {
              message += $"{dbid},";
           }

           //Debug.Log($"message:{message}");
        }

        
        protected void OnDestroy()
        {
           if (_simClient != null)
           {
              //TODO: Manually leave session. Needed?
           }
        }
        
        
        //  Methods  --------------------------------------
        private async void SetupBeamable()
        {
            var beamableAPI = await Beamable.API.Instance;

            Debug.Log($"beamableAPI.User.id = {beamableAPI.User.id}");
            
            // Access Local Player Information
            _localPlayerDbid = beamableAPI.User.id;
            
            // Create Multiplayer Session
            _simClient = new SimClient(new SimNetworkEventStream(RoomId), 
               FramesPerSecond, TargetNetworkLead);
        
            // Handle Common Events
            _simClient.OnInit(SimClient_OnInit);
            _simClient.OnConnect(SimClient_OnConnect);
            _simClient.OnDisconnect(SimClient_OnDisconnect);
            _simClient.OnTick(SimClient_OnTick);
            
        }
        
        
        //  Event Handlers  -------------------------------
        private void SimClient_OnInit(string sessionSeed)
        {
           _sessionSeed = sessionSeed;
           Debug.Log($"SimClient_OnInit(): RoomId = {RoomId}, " +
                     $"sessionSeed = {sessionSeed}");
        }

        
        private void SimClient_OnConnect(string dbid)
        {
           _sessionPlayerDbids.Add(dbid);
        
           // Handle Custom Events for EACH dbid
           _simClient.On<MyPlayerMoveEvent>(MyPlayerMoveEvent.Name, dbid,
              SimClient_OnMyPlayerMoveEvent);
        
           Debug.Log($"SimClient_OnConnect() dbid = {dbid}");
        }

        
        private void SimClient_OnDisconnect(string dbid)
        {
           _sessionPlayerDbids.Remove(dbid);
           Debug.Log($"SimClient_OnDisconnect() dbid = {dbid}");
        }

        
        private void SimClient_OnTick(long currentFrame)
        {
           _currentFrame = currentFrame;
        }

        
        private void SimClient_OnMyPlayerMoveEvent(MyPlayerMoveEvent myPlayerMoveEvent)
        {
           Debug.Log($"SimClient_OnMyPlayerMoveEvent() Position = {myPlayerMoveEvent.Position}");
        }
    }
}