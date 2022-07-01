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
        public List<string> GroupPlayerNames = new List<string>();
        public List<string> RoomNames = new List<string>();
        public List<string> RoomPlayerNames = new List<string>();
        public List<string> RoomMessages = new List<string>();
        public string GroupToCreateName = "";
        public bool IsInGroup = false;
        public bool IsInRoom = false;
        public string MessageToSend = "";
        public bool IsBeamableSetUp = false;
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
        private BeamContext _beamContext;
        private ChatService _chatService;
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
            _beamContext = BeamContext.Default;
            await _beamContext.OnReady;
            _chatService = _beamContext.Api.Experimental.ChatService;

            Debug.Log($"beamContext.PlayerId = {_beamContext.PlayerId}");

            // Observe GroupsService Changes
            _beamContext.Api.GroupsService.Subscribe(async groupsView =>
            {
                _groupsView = groupsView;
                _data.IsInGroup = _groupsView.Groups.Count > 0;

                Debug.Log("GroupsService.Subscribe 1: " + _groupsView.Groups.Count);

                _data.GroupNames.Clear();
                _data.GroupPlayerNames.Clear();
                foreach(var groupView in groupsView.Groups)
                {
                    string groupName = $"Name = {groupView.Group.name}, Players = {groupView.Group.members.Count}";
                    _data.GroupNames.Add(groupName);

                    foreach (var member in groupView.Group.members)
                    {
                        string groupPlayerName = $"Name = {member.gamerTag}";
                        _data.GroupPlayerNames.Add(groupPlayerName);
                    }

                    // Create a new chat room for the group
                    string roomName = $"Room For {groupView.Group.name}";
                    await _chatService.CreateRoom(roomName, false,
                        new List<long> {_beamContext.PlayerId});

                }
                Refresh();
            });

            // Observe ChatService Changes
            _chatService.Subscribe(chatView =>
            {
                _chatView = chatView;

                int roomsWithPlayers = 0;
                _data.RoomNames.Clear();
                _data.RoomPlayerNames.Clear();
                foreach(RoomHandle room in chatView.roomHandles)
                {
                    // Optional: Only setup non-empty rooms
                    if (room.Players.Count > 0)
                    {
                        roomsWithPlayers++;

                        string roomName = $"Name = {room.Name}, Players = {room.Players.Count}";
                        _data.RoomNames.Add(roomName);

                        foreach (var roomPlayerDbid in room.Players)
                        {
                            string roomPlayerName = $"Player = {roomPlayerDbid}";
                            _data.RoomPlayerNames.Add(roomPlayerName);
                        }

                        room.Subscribe().Then(_ =>
                        {
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

                _data.IsInRoom = roomsWithPlayers > 0;
                Debug.Log("ChatService.Subscribe 1: " + roomsWithPlayers);

                _data.IsBeamableSetUp = _beamContext != null;

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
            await LeaveGroups();

            string groupName = _data.GroupToCreateName;
            string groupTag = "t01";
            string enrollmentType = "open";

            // Search existing group
            var groupSearchResponse = await _beamContext.Api.GroupsService.Search(groupName,
                new List<string> {enrollmentType});

            // Join or Create new group
            if (groupSearchResponse.groups.Count > 0)
            {
                foreach (var group in groupSearchResponse.groups)
                {
                    var groupMembershipResponse = await _beamContext.Api.GroupsService.JoinGroup(group.id);
                }
            }
            else
            {
                var groupCreateRequest = new GroupCreateRequest(groupName, groupTag, enrollmentType, 0, 50);
                var groupCreateResponse = await _beamContext.Api.GroupsService.CreateGroup(groupCreateRequest);
                var createdGroup = groupCreateResponse.group;

                // Join new group
                await _beamContext.Api.GroupsService.JoinGroup(createdGroup.id);
            }

            await _beamContext.Api.GroupsService.Subscribable.Refresh();

            Refresh();

            return new EmptyResponse();
        }

        public async Task<EmptyResponse> LeaveGroups()
        {
            // Leave any existing room
            await LeaveRooms();

            // Leave any existing groups
            foreach(var group in _groupsView.Groups)
            {
                var result = await _beamContext.Api.GroupsService.LeaveGroup(group.Group.id);
            }

            // HACK: Force refresh here (0.10.1)
            // Wait (arbitrary milliseconds) for refresh to complete
            _beamContext.Api.GroupsService.Subscribable.ForceRefresh();
            await Task.Delay(300);

            Refresh();

            return new EmptyResponse();
        }

        public async Task<EmptyResponse> LeaveRooms()
        {
            Debug.Log("_chatView 1: " + _chatView.roomHandles.Count);
            foreach(var room in _chatView.roomHandles)
            {
                var result = await _chatService.LeaveRoom(room.Id);
            }

            Debug.Log("_chatView 2: " + _chatView.roomHandles.Count);

            _data.RoomMessages.Clear();
            Refresh();
            return new EmptyResponse();
        }

        public void Refresh()
        {
            // Create new mock message
            int messageIndex = _data.RoomMessages.Count + 1;
            _data.MessageToSend = $"Hello {messageIndex:000}!";

            // Create new mock group name
            int groupIndex = _data.GroupNames.Count + 1;
            _data.GroupToCreateName = $"Group{groupIndex:000}";

            string refreshLog = $"Refresh() ...\n" +
                                $"\n * GroupNames.Count = {_data.GroupNames.Count}" +
                                $"\n * GroupPlayerNames.Count = {_data.GroupPlayerNames.Count}" +
                                $"\n * RoomNames.Count = {_data.RoomNames.Count}" +
                                $"\n * RoomUserNames.Count = {_data.RoomPlayerNames.Count}" +
                                $"\n * IsInGroup = {_data.IsInGroup}" +
                                $"\n * IsInRoom = {_data.IsInRoom}\n\n";
            //Debug.Log(refreshLog);

            // Send relevant data to the UI for rendering
            OnRefreshed?.Invoke(_data);
        }

        //  Event Handlers  -------------------------------
        private void RoomHandle_OnMessageReceived(Message message)
        {
            string roomMessage = $"{message.gamerTag}: {message.content}";
            _data.RoomMessages.Add(roomMessage);
            Refresh();
        }
    }
}