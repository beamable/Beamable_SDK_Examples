using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Common.Api;
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
        public bool IsInGroup = false;
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
        private ChatView _chatView = null;
        private GroupsView _groupsView = null;
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
                _groupsView = groupsView;
                _data.GroupNames.Clear();
                
                _data.IsInGroup = groupsView.Groups.Count > 0;
                Debug.Log("GroupsService.Subscribe() count: " + groupsView.Groups.Count + " isinGroup: " + _data.IsInGroup);
                
                foreach(var groupView in groupsView.Groups)
                {
                    string groupName = $"Name = {groupView.Group.name}, Members = {groupView.Group.members.Count}";
                    _data.GroupNames.Add(groupName);

                    // Create a new chat room for the group
                    string roomName = $"Room For Group {groupView.Group.name}";
                    await _beamableAPI.Experimental.ChatService.CreateRoom(groupName, false,
                        new List<long> {_beamableAPI.User.id});
                    
                    // Store, so user can leave if/when desired
                    _data.GroupToLeaveName = groupView.Group.name;
                }
                
                Refresh();
            });
            
            // Observe ChatService Changes
            _beamableAPI.Experimental.ChatService.Subscribe(chatView =>
            {
                _chatView = chatView;
                _data.RoomNames.Clear();
                
                foreach(RoomHandle room in chatView.roomHandles)
                {
                    // Optional: Only setup non-empty rooms
                    if (room.Players.Count > 0)
                    {
                        string roomName = $"Name = {room.Name}, Players = {room.Players.Count}";
                        _data.RoomNames.Add(roomName);

                        room.Subscribe().Then(_ =>
                        {
                            Debug.Log($"Subscribed to {room.Name}");

                            _data.RoomMessages.Clear();

                            foreach (var message in room.Messages)
                            {
                                string roomMessage = $"{message.gamerTag}: {message.content}";
                                _data.RoomMessages.Add(roomMessage);
                            }

                            room.OnMessageReceived += RoomHandle_OnMessageReceived;
                            Refresh();
                        });
                    }
                }
                Refresh();
            });
        }
        
        public async Task<EmptyResponse> SendGroupMessage()
        {
            foreach(RoomHandle room in _chatView.roomHandles)
            {
                await room.SendMessage(_data.MessageToSend);
            }
            return new EmptyResponse();
        }
        
        public async Task<EmptyResponse> CreateGroup ()
        {
            // Leave any existing group
            await LeaveGroup();
            
            string groupName = _data.GroupToCreateName;
            string groupTag = "t01";
            string enrollmentType = "open";

            // Search existing group
            var groupSearchResponse = await _beamableAPI.GroupsService.Search(groupName, 
                new List<string> {enrollmentType});
            
            Debug.Log("x: " + groupSearchResponse.groups.Count);

            // Join or Create new group
            if (groupSearchResponse.groups.Count > 0)
            {
                foreach (var group in groupSearchResponse.groups)
                {
                    var groupMembershipResponse = await _beamableAPI.GroupsService.JoinGroup(group.id);

                    Debug.Log( ("groupMembershipResponse: " + groupMembershipResponse.member));
         
                }
            }
            else
            {
                var groupCreateRequest = new GroupCreateRequest(groupName, groupTag, enrollmentType, 0, 50);
                var groupCreateResponse = await _beamableAPI.GroupsService.CreateGroup(groupCreateRequest);
                var createdGroup = groupCreateResponse.group;

                // Join new group
                await _beamableAPI.GroupsService.JoinGroup(createdGroup.id);
    
            }
   
            Refresh();

            return new EmptyResponse();
        }
        
        public async Task<EmptyResponse> LeaveGroup()
        {
            Debug.Log("TRY 1 -----------------------------");
            Debug.Log("Before LeaveGroup: c = " + _groupsView.Groups.Count);
            
            foreach(var group in _groupsView.Groups)
            {
                Debug.Log("Before LeaveGroup: j = " + group.Joined + ", dbid = " + _beamableAPI.User.id);
                var result = await _beamableAPI.GroupsService.LeaveGroup(group.Group.id);
                Debug.Log("After LeaveGroup: m = " + result.member);
            }

            var groupsViewAfter = await _beamableAPI.GroupsService.GetCurrent();
            Debug.Log("After LeaveGroup: c = " + groupsViewAfter.Groups.Count);
            
            Debug.Log("TRY 2 -----------------------------");
            Debug.Log("Before LeaveGroup: c = " + groupsViewAfter.Groups.Count);
            
            foreach(var groupAfter in groupsViewAfter.Groups)
            {
                Debug.Log("Before LeaveGroup: j = " + groupAfter.Joined + ", dbid = " + _beamableAPI.User.id);
                var result = await _beamableAPI.GroupsService.LeaveGroup(groupAfter.Group.id);
                Debug.Log("After LeaveGroup: m = " + result.member);
            }

            var groupsView3 = await _beamableAPI.GroupsService.GetCurrent();
            Debug.Log("After LeaveGroup: c = " + groupsView3.Groups.Count);

            Refresh();
            
            return new EmptyResponse();
        }
        
        public void Refresh()
        {
            Debug.Log($"Refresh()");
            Debug.Log($"\tGroupNames.Count = {_data.GroupNames.Count}");
            Debug.Log($"\tRoomNames.Count = {_data.RoomNames.Count}");
            Debug.Log($"\tUserNames.Count = {_data.RoomUsernames.Count}");
            Debug.Log($"\tIsInGroup = {_data.IsInGroup}");
            
            // Create new mock message 
            int messageIndex = _data.RoomMessages.Count;
            _data.MessageToSend = $"Hello World {messageIndex:000}!";
            
            // Create new mock group name
            int groupIndex = _data.GroupNames.Count;
            _data.GroupToCreateName = $"Group{groupIndex:000}";

            // Create temp name for pretty UI
            if (string.IsNullOrEmpty(_data.GroupToLeaveName))
            {
                _data.GroupToLeaveName = _data.GroupToCreateName;
            }
         
            // Send relevant data to the UI for rendering
            OnRefreshed?.Invoke(_data);
        }
        
        //  Event Handlers  -------------------------------
        private async void RoomHandle_OnMessageReceived(Message message)
        {
            string roomMessage = $"{message.gamerTag}: {message.content}";
            Debug.Log($"Received Message = {roomMessage} in room = {message.roomId}");
            _data.RoomMessages.Add(roomMessage);
            Refresh();
        }
    }
}