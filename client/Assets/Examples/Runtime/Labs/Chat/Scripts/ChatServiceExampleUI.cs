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

      // Events Panel
      private TMP_Text GroupsTitleText { get { return TitleText01; }}
      private TMP_Text GroupsBodyText { get { return BodyText01; }}
      
      // Claims Panel 
      private TMP_Text MessagesTitleText { get { return TitleText02; }}
      private TMP_Text MessagesBodyText { get { return BodyText02; }}
   
      // Menu Panel
      private TMP_Text MenuTitleText { get { return TitleText03; }}
      private Button SendMessageButton { get { return Button01;}}
      private Button CreateGroupButton { get { return Button02;}}
      private Button LeaveGroupButton { get { return Button03;}}
      
      
      //  Unity Methods  --------------------------------
      protected void Start()
      {
         _chatServiceExample.OnRefreshed.AddListener(EventsServiceExample_OnRefreshed);
         SendMessageButton.onClick.AddListener(SendMessageButton_OnClicked);
         CreateGroupButton.onClick.AddListener(CreateGroupButton_OnClicked);
         LeaveGroupButton.onClick.AddListener(LeaveGroup_OnClicked);
         
         // Populate default UI
         _chatServiceExample.Refresh();
      }

      //  Methods  --------------------------------------
      
      //  Event Handlers  -------------------------------
      private void SendMessageButton_OnClicked()
      {
         _chatServiceExample.SendRoomMessage();
      }

      private void CreateGroupButton_OnClicked()
      {
         _chatServiceExample.CreateGroup();
      }

      private void LeaveGroup_OnClicked()
      {
         _chatServiceExample.LeaveGroup();
      }
      
      private void EventsServiceExample_OnRefreshed(ChatServiceExampleData 
         chatServiceExampleData)
      {
         // Show UI: Groups
         StringBuilder stringBuilder01 = new StringBuilder();
         stringBuilder01.Append("GROUPS").AppendLine();
         foreach (string groupName in chatServiceExampleData.GroupNames)
         {
            stringBuilder01.Append($"•{groupName}").AppendLine();
         }
         // Show UI: Rooms
         stringBuilder01.AppendLine();
         stringBuilder01.Append("ROOMS").AppendLine();
         foreach (string setScoreLog in chatServiceExampleData.RoomNames)
         {
            stringBuilder01.Append($"•{setScoreLog}").AppendLine();
         }
         
         // Show UI: Users
         stringBuilder01.AppendLine();
         stringBuilder01.Append("USERS").AppendLine();
         foreach (string roomUsername in chatServiceExampleData.RoomUsernames)
         {
            stringBuilder01.Append($"•{roomUsername}").AppendLine();
         }
         GroupsBodyText.text = stringBuilder01.ToString();
         
         // Show UI: Messages
         StringBuilder stringBuilder02 = new StringBuilder();
         foreach (string roomMessage in chatServiceExampleData.RoomMessages)
         {
            stringBuilder02.Append($"•{roomMessage}").AppendLine();
         }
         MessagesBodyText.text = stringBuilder02.ToString();
         
         // Show UI: Other 
         GroupsTitleText.text = "Chat";
         MessagesTitleText.text = "Messages";
         MenuTitleText.text = "Menu";
         
         CreateGroupButton.GetComponentInChildren<TMP_Text>().text = 
            $"Create Group\n({chatServiceExampleData.GroupToCreateName})";

         LeaveGroupButton.GetComponentInChildren<TMP_Text>().text = 
            $"Leave Group\n({chatServiceExampleData.GroupToLeaveName})";

         SendMessageButton.GetComponentInChildren<TMP_Text>().text =
            $"Send Message\n({chatServiceExampleData.MessageToSend})";
         
         bool isInGroup = chatServiceExampleData.GroupNames.Count > 1;
         CreateGroupButton.interactable = !isInGroup;
         LeaveGroupButton.interactable = isInGroup;

      }
   }
}


