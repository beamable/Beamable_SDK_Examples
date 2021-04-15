using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Common.Api.Groups;
using UnityEngine;
using Beamable.Experimental.Api.Chat;
using UnityEngine.Events;

namespace Beamable.Examples.Services.GroupsService
{
    /// <summary>
    /// Holds data for use in the <see cref="GroupsServiceExampleUI"/>.
    /// </summary>
    [System.Serializable]
    public class GroupsServiceExampleData
    {
        public List<string> GroupNames = new List<string>();
        public List<string> RoomNames = new List<string>();
        public List<string> RoomUsernames = new List<string>();
        public List<string> RoomMessages = new List<string>();
        public string GroupToCreateName = "";
        public string GroupToLeaveName = "";

        public bool IsInGroup = true;
        public string MessageToSend = "";
    }
   
    [System.Serializable]
    public class RefreshedUnityEvent : UnityEvent<GroupsServiceExampleData> { }
    
    /// <summary>
    /// Demonstrates <see cref="GroupsService"/>.
    /// </summary>
    public class GroupsServiceExample : MonoBehaviour
    {
        //  Events  ---------------------------------------
        [HideInInspector]
        public RefreshedUnityEvent OnRefreshed = new RefreshedUnityEvent();
        
        //  Fields  ---------------------------------------
        [SerializeField] private ChatView _chatView;
        private IBeamableAPI _beamableAPI = null;
        private GroupsServiceExampleData _data = new GroupsServiceExampleData();
    
        //  Unity Methods  --------------------------------
        protected void Start()
        {
            Debug.Log("Start()");

            SetupBeamable();
        }
        
        //  Methods  --------------------------------------
        private async void SetupBeamable()
        {
            _beamableAPI = await Beamable.API.Instance;

            Debug.Log($"beamableAPI.User.id = {_beamableAPI.User.id}");

            // Observe GroupsService Changes
            _beamableAPI.GroupsService.Subscribe(async groupsView =>
            {
                _data.GroupNames.Clear();
                foreach(var group in groupsView.Groups)
                {
                    string groupName = $"Name = {group.Group.name}, Members = {group.Group.members.Count}";
                    _data.GroupNames.Add(groupName);
                }
            });
            
            // Observe ChatService Changes
            _beamableAPI.Experimental.ChatService.Subscribe(chatView =>
            {
                _chatView = chatView;
                
                _data.RoomNames.Clear();
                foreach(RoomHandle room in chatView.roomHandles)
                {
                    string roomName = $"Name = {room.Name}, Players = {room.Players.Count}";
                    _data.RoomNames.Add(roomName);
                    
                    room.Subscribe().Then(_ =>
                    {
                        _data.RoomMessages.Clear();
                        Debug.Log($"Subscribed to {room.Id}");
                        foreach(var message in room.Messages)
                        {
                            string roomMessage = $"U={message.gamerTag}, R={message.roomId}: {message.content}";
                            Debug.Log($"Message: {roomMessage}");
                            
                            _data.RoomMessages.Add(roomMessage);
                       
                        }
                        Refresh();
                    });
                    room.OnMessageReceived += RoomHandle_OnMessageReceived;
                }
            });
        }
        
        public async Task<bool> IsProfanity(string text)
        {
            bool isProfanityText = true;
            try
            {
                var result = await _beamableAPI.Experimental.ChatService.ProfanityAssert(text);
                isProfanityText = false;
            } catch{}

            return isProfanityText;
        }
        
        public async void SendRoomMessage()
        {
            string messageToSend = _data.MessageToSend;
            
            bool isProfanity  = await IsProfanity(messageToSend);

            if (isProfanity)
            {
                // Disallow (or prompt user to resubmit)
                messageToSend = "Message Not Allowed";
            }
            
            foreach(RoomHandle room in _chatView.roomHandles)
            {
                room.SendMessage(messageToSend);
            }
        }
        
        public async void CreateGroup ()
        {
            string groupName = _data.GroupToCreateName;
            string groupTag = "ali";
            var group = new GroupCreateRequest(groupName, groupTag,
                "open", 0, 50);
            
            var result1 = await _beamableAPI.GroupsService.CreateGroup(group);
            var result2 = await _beamableAPI.GroupsService.JoinGroup(result1.group.id);

            // Store, so user can leave if/when desired
            _data.GroupToLeaveName = result1.group.name;
            
            _data.IsInGroup = true;
            Refresh();
        }
        
        public async void LeaveGroup()
        {
            var groupsView = await _beamableAPI.GroupsService.GetCurrent();
            
            foreach(var group in groupsView.Groups)
            {
                var result = await _beamableAPI.GroupsService.LeaveGroup(group.Group.id);
            }

            _data.IsInGroup = false;
            
            Refresh();
        }
        
        public void Refresh()
        {
            Debug.Log($"Refresh()");
            Debug.Log($"\tGroupNames.Count = {_data.GroupNames.Count}");
            Debug.Log($"\tRoomNames.Count = {_data.RoomNames.Count}");
            Debug.Log($"\tUserNames.Count = {_data.RoomUsernames.Count}");
            
            // Create new mock message 
            int messageIndex = _data.RoomMessages.Count;
            _data.MessageToSend = $"Hello World {messageIndex:000}!";
            
            // Create new mock group name
            int groupIndex = _data.GroupNames.Count;
            _data.GroupToCreateName = $"Group {groupIndex:000}";
         
            // Send relevant data to the UI for rendering
            OnRefreshed?.Invoke(_data);
        }
        
        //  Event Handlers  -------------------------------
        private void RoomHandle_OnMessageReceived(Message message)
        {
            string roomMessage = $"{message.gamerTag} in {message.roomId}: {message.content}";
            Debug.Log($"Received Message = {roomMessage}");
            _data.RoomMessages.Add(roomMessage);
            Refresh();
        }
    }
}