using System.Text;
using Beamable.Examples.Shared;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Beamable.Examples.Labs.ChatService
{
   /// <summary>
   /// The UI for the <see cref="ChatServiceExample"/>.
   /// </summary>
   public class ChatServiceExampleUI : ExampleCanvasUI
   {
      //  Fields  ---------------------------------------
      
      [SerializeField] private ChatServiceExample _chatServiceExample = null;

      // Menu Panel
      private TMP_Text MenuTitleText { get { return TitleText01; }}
      private Button SendMessageButton { get { return Button01;}}
      private Button CreateRoomButton { get { return Button02;}}
      private Button LeaveRoomButton { get { return Button03;}}
      
      // Rooms Panel
      private TMP_Text RoomsTitleText { get { return TitleText02; }}
      private TMP_Text RoomsBodyText { get { return BodyText02; }}
      
      // Messages Panel 
      private TMP_Text MessagesTitleText { get { return TitleText03; }}
      private TMP_Text MessagesBodyText { get { return BodyText03; }}
   
      
      //  Unity Methods  --------------------------------
      protected override void Start()
      {
         base.Start();

         _chatServiceExample.OnRefreshed.AddListener(EventsServiceExample_OnRefreshed);
         SendMessageButton.onClick.AddListener(SendMessageButton_OnClicked);
         CreateRoomButton.onClick.AddListener(CreateRoomButton_OnClicked);
         LeaveRoomButton.onClick.AddListener(LeaveRoom_OnClicked);
         
         // Populate default UI
         _chatServiceExample.Refresh();
      }

      //  Methods  --------------------------------------
      
      //  Event Handlers  -------------------------------
      private async void SendMessageButton_OnClicked()
      {
         await _chatServiceExample.SendRoomMessage();
      }

      private async void CreateRoomButton_OnClicked()
      {
         await _chatServiceExample.CreateRoom();
      }

      private async void LeaveRoom_OnClicked()
      {
         await _chatServiceExample.LeaveRoom();
      }
      
      private void EventsServiceExample_OnRefreshed(ChatServiceExampleData 
         chatServiceExampleData)
      {
         // Show UI: Rooms
         StringBuilder stringBuilder01 = new StringBuilder();
         // Show UI: Rooms
         stringBuilder01.AppendLine();
         stringBuilder01.Append("ROOMS").AppendLine();
         foreach (string roomName in chatServiceExampleData.RoomNames)
         {
            stringBuilder01.Append($" • {roomName}").AppendLine();
         }
         
         // Show UI: Players
         stringBuilder01.AppendLine();
         stringBuilder01.Append("PLAYERS").AppendLine();
         foreach (string roomPlayer in chatServiceExampleData.RoomPlayers)
         {
            stringBuilder01.Append($" • {roomPlayer}").AppendLine();
         }
         RoomsBodyText.text = stringBuilder01.ToString();
         
         // Show UI: Messages
         StringBuilder stringBuilder02 = new StringBuilder();
         foreach (string roomMessage in chatServiceExampleData.RoomMessages)
         {
            stringBuilder02.Append($" • {roomMessage}").AppendLine();
         }
         MessagesBodyText.text = stringBuilder02.ToString();
         
         // Show UI: Other 
         MenuTitleText.text = "ChatService Example";
         RoomsTitleText.text = "Rooms";
         MessagesTitleText.text = "Messages";
         
         CreateRoomButton.GetComponentInChildren<TMP_Text>().text = 
            $"Create Room\n({chatServiceExampleData.RoomToCreateName})";

         LeaveRoomButton.GetComponentInChildren<TMP_Text>().text = 
            $"Leave Room\n({chatServiceExampleData.RoomToLeaveName})";

         SendMessageButton.GetComponentInChildren<TMP_Text>().text =
            $"Send Message\n({chatServiceExampleData.MessageToSend})";

         bool isInRoom = chatServiceExampleData.IsInRoom;
         SendMessageButton.interactable = isInRoom;
         CreateRoomButton.interactable = !isInRoom;
         LeaveRoomButton.interactable = isInRoom;
      }
   }
}


