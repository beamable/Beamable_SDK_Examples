using System.Text;
using Beamable.Examples.Shared;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Beamable.Examples.Services.MailService
{
   /// <summary>
   /// The UI for the <see cref="EventsServiceExample"/>.
   /// </summary>
   public class MailServiceExampleUI : ExampleCanvasUI
   {
      //  Fields  ---------------------------------------
      [SerializeField] private MailServiceExample _mailServiceExample = null;

      // Menu Panel
      private TMP_Text MenuTitleText { get { return TitleText01; }}
      private Button SendMailMessageButton { get { return Button01;}}
      private Button UpdateMailMessagesButton { get { return Button02;}}
      private Button RefreshButton { get { return Button03;}}
      
      // Events Panel
      private TMP_Text OverviewTitleText { get { return TitleText02; }}
      private TMP_Text OverviewBodyText { get { return BodyText02; }}
      
      // Claims Panel 
      private TMP_Text MailMessagesTitleText { get { return TitleText03; }}
      private TMP_Text MailMessagesBodyText { get { return BodyText03; }}
   
      //  Unity Methods  --------------------------------
      protected void Start()
      {
         _mailServiceExample.OnRefreshed.AddListener(EventsServiceExample_OnRefreshed);
         SendMailMessageButton.onClick.AddListener(SendMailMessageButton_OnClicked);
         UpdateMailMessagesButton.onClick.AddListener(UpdateMailMessagesButton_OnClicked);
         RefreshButton.onClick.AddListener(RefreshButton_OnClicked);
         
         // Populate default UI
         RefreshButton_OnClicked();
      }
      
      //  Event Handlers  -------------------------------
      private void SendMailMessageButton_OnClicked()
      {
         _mailServiceExample.SendMailMessage();
      }

      private void UpdateMailMessagesButton_OnClicked()
      {
         _mailServiceExample.UpdateMail();
      }

      private void RefreshButton_OnClicked()
      {
         _mailServiceExample.Refresh();
      }
      
      private void EventsServiceExample_OnRefreshed(MailServiceExampleData 
         mailServiceExampleData)
      {
         StringBuilder overviewStringBuilder = new StringBuilder();
         
         // Show UI: Overview
         overviewStringBuilder.Append("UNREAD MAIL").AppendLine();
         foreach (string unreadMailLog in mailServiceExampleData.UnreadMailLogs)
         {
            overviewStringBuilder.Append(unreadMailLog).AppendLine();
         }
         
         // Show UI: Updates
         overviewStringBuilder.AppendLine();
         overviewStringBuilder.Append("UPDATE MAIL").AppendLine();
         foreach (string updateMailLog in mailServiceExampleData.UpdateMailLogs)
         {
            overviewStringBuilder.Append(updateMailLog).AppendLine();
         }
         
         // Show UI: Send
         overviewStringBuilder.AppendLine();
         overviewStringBuilder.Append("SEND MAIL").AppendLine();
         foreach (string sendMailMessageLog in mailServiceExampleData.SendMailMessageLogs)
         {
            overviewStringBuilder.Append(sendMailMessageLog).AppendLine();
         }
         
         OverviewBodyText.text = overviewStringBuilder.ToString();
         StringBuilder mailMessagesStringBuilder = new StringBuilder();
         
         // Show UI: Messages
         mailMessagesStringBuilder.AppendLine();
         mailMessagesStringBuilder.Append("MAIL MESSAGES").AppendLine();
         foreach (string mailMessageLog in mailServiceExampleData.MailMessageLogs)
         {
            mailMessagesStringBuilder.Append(mailMessageLog).AppendLine();
         }
         MailMessagesBodyText.text = mailMessagesStringBuilder.ToString();

         // Show UI: Other
         MenuTitleText.text = "MailService Example";
         OverviewTitleText.text = "Overview";
         MailMessagesTitleText.text = "Mail";

         int nextMailIndex = mailServiceExampleData.MailMessageLogs.Count + 1;
         SendMailMessageButton.GetComponentInChildren<TMP_Text>().text = 
            $"Send Mail\n#{nextMailIndex}";

         UpdateMailMessagesButton.GetComponentInChildren<TMP_Text>().text =
            $"Update\nMail";
         
         RefreshButton.GetComponentInChildren<TMP_Text>().text = 
            $"Refresh\nUI";
      }
   }
}


