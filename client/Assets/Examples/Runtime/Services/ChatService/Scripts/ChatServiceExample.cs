using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Common.Api;
using UnityEngine;
using Beamable.Experimental.Api.Chat;
using UnityEngine.Events;

namespace Beamable.Examples.Labs.ChatService
{
    /// <summary>
    /// Holds data for use in the <see cref="ChatServiceExampleUI"/>.
    /// </summary>
    [System.Serializable]
    public class ChatServiceExampleData
    {
        public List<string> RoomNames = new List<string>();
        public List<string> RoomPlayers = new List<string>();
        public List<string> RoomMessages = new List<string>();
        public string RoomToCreateName = "";
        public string RoomToLeaveName = "";
        public bool IsInRoom = false;
        public string MessageToSend = "";
    }
   
    [System.Serializable]
    public class RefreshedUnityEvent : UnityEvent<ChatServiceExampleData> { }
    
    /// <summary>
    /// Demonstrates <see cref="ChatService"/>.
    /// </summary>
    public class ChatServiceExample : MonoBehaviour
    {
        //  Events  ---------------------------------------
        [HideInInspector]
        public RefreshedUnityEvent OnRefreshed = new RefreshedUnityEvent();
        
        //  Fields  ---------------------------------------
        private ChatView _chatView = null;
        private BeamContext _beamContext;
        private ChatServiceExampleData _data = new ChatServiceExampleData();
    
        //  Unity Methods  --------------------------------
        protected void Start()
        {
            Debug.Log("Start()");

            SetupBeamable();
        }
        
        //  Methods  --------------------------------------
        private async void SetupBeamable()
        {
            _beamContext = BeamContext.Default;
            await _beamContext.OnReady;

            Debug.Log($"beamContext.PlayerId = {_beamContext.PlayerId}");

            // Observe ChatService Changes
            _beamContext.Api.Experimental.ChatService.Subscribe(chatView =>
            {
                _chatView = chatView;
                
                // Clear data when ChatService changes
                _data.RoomNames.Clear();
                _data.RoomMessages.Clear();
                _data.RoomPlayers.Clear();
                
                foreach(RoomHandle room in chatView.roomHandles)
                {
                    room.OnRemoved += Room_OnRemoved;
                    
                    string roomName = $"{room.Name}";
                    _data.RoomNames.Add(roomName);
                    
                    room.Subscribe().Then(_ =>
                    {
                        // Clear data (again) when Room changes
                        _data.RoomMessages.Clear();
                        _data.RoomPlayers.Clear();
                        _data.RoomToLeaveName = room.Name;
                        
                        foreach(var message in room.Messages)
                        {
                            string roomMessage = $"{message.gamerTag}: {message.content}";
                            _data.RoomMessages.Add(roomMessage);
                        }
                        
                        
                        foreach(var player in room.Players)
                        {
                            string playerName = $"{player}";
                            _data.RoomPlayers.Add(playerName);
                        }
                        
                        Refresh();
                    });
                    room.OnMessageReceived += Room_OnMessageReceived;
                }
                Refresh();
            });
        }



        public async Task<bool> IsProfanity(string text)
        {
            bool isProfanityText = true;
            try
            {
                var result = await _beamContext.Api.Experimental.ChatService.ProfanityAssert(text);
                isProfanityText = false;
            } catch{}

            return isProfanityText;
        }
        
        public async Task<EmptyResponse> SendRoomMessage()
        {
            string messageToSend = _data.MessageToSend;
            
            bool isProfanity  = await IsProfanity(messageToSend);

            if (isProfanity)
            {
                // Disallow (or prompt Player to resubmit)
                messageToSend = "Message Not Allowed";
            }
            
            foreach(RoomHandle room in _chatView.roomHandles)
            {
                if (room.Players.Count > 0)
                {
                    await room.SendMessage(messageToSend);
                }
            }

            return new EmptyResponse();
        }
        
        public async Task<EmptyResponse> CreateRoom ()
        {
            string roomName = _data.RoomToCreateName;
            bool keepSubscribed = false;
            List<long> players = new List<long>{_beamContext.PlayerId};
            
            var result = await _beamContext.Api.Experimental.ChatService.CreateRoom(
                roomName, keepSubscribed, players);
            
            Refresh();
            
            return new EmptyResponse();
        }
        
        public async Task<EmptyResponse> LeaveRoom()
        {
            var roomInfos = await _beamContext.Api.Experimental.ChatService.GetMyRooms();
            
            foreach(var roomInfo in roomInfos)
            {
                var result = await _beamContext.Api.Experimental.ChatService.LeaveRoom(roomInfo.id);
            }

            Refresh();
            
            return new EmptyResponse();
        }
        
        public void Refresh()
        {
            _data.IsInRoom = _data.RoomPlayers.Count > 0;
            
            // Create new mock message 
            int messageIndex = _data.RoomMessages.Count;
            _data.MessageToSend = $"Hello {messageIndex:000}!";
            
            // Create new mock group name
            int groupIndex = _data.RoomNames.Count;
            _data.RoomToCreateName = $"Room{groupIndex:000}";
            
            // Create temp name for pretty UI
            if (string.IsNullOrEmpty(_data.RoomToLeaveName))
            {
                _data.RoomToLeaveName = _data.RoomToCreateName;
            }
            
            // Log
            string refreshLog = $"Refresh() ...\n" +
                                $"\n * RoomNames.Count = {_data.RoomNames.Count}" +
                                $"\n * RoomPlayers.Count = {_data.RoomPlayers.Count}" +
                                $"\n * RoomMessages.Count = {_data.RoomMessages.Count}" +
                                $"\n * IsInRoom = {_data.IsInRoom}\n\n";
            
            Debug.Log(refreshLog);
            
            // Send relevant data to the UI for rendering
            OnRefreshed?.Invoke(_data);
        }
        
        //  Event Handlers  -------------------------------
        private void Room_OnMessageReceived(Message message)
        {
            string roomMessage = $"{message.gamerTag}: {message.content}";
            Debug.Log($"Room_OnMessageReceived() roomMessage = {roomMessage}");
            _data.RoomMessages.Add(roomMessage);
            Refresh();
        }
        
        private void Room_OnRemoved()
        {
            Debug.Log($"Room_OnRemoved");
            Refresh();
        }
    }
}