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
      private Button UpdateMailMessagesButton { get { return Button01;}}
      private Button RefreshButton { get { return Button02;}}
      
      // Events Panel
      private TMP_Text OverviewTitleText { get { return TitleText02; }}
      private TMP_Text OverviewBodyText { get { return BodyText02; }}
      
      // Claims Panel 
      private TMP_Text MailMessagesTitleText { get { return TitleText03; }}
      private TMP_Text MailMessagesBodyText { get { return BodyText03; }}
   
      //  Unity Methods  --------------------------------
      protected override void Start()
      {
         base.Start();

         _mailServiceExample.OnRefreshed.AddListener(EventsServiceExample_OnRefreshed);
         UpdateMailMessagesButton.onClick.AddListener(UpdateMailMessagesButton_OnClicked);
         RefreshButton.onClick.AddListener(RefreshButton_OnClicked);

         UpdateMailMessagesButton.interactable = false;
         RefreshButton.interactable = false;
         _resetPlayerButton.interactable = false;
      }
      
      //  Event Handlers  -------------------------------

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
         
         // Show UI: Overview
         StringBuilder overviewStringBuilder = new StringBuilder();
         overviewStringBuilder.Append("UNREAD MAIL").AppendLine().AppendLine();
         foreach (string unreadMailLog in mailServiceExampleData.UnreadMailLogs)
         {
            overviewStringBuilder.Append("\t" + unreadMailLog).AppendLine();
         }
         
         // Show UI: Updates
         overviewStringBuilder.AppendLine();
         overviewStringBuilder.Append("UPDATE MAIL").AppendLine().AppendLine();
         foreach (string updateMailLog in mailServiceExampleData.UpdateMailLogs)
         {
            overviewStringBuilder.Append("\t" + updateMailLog).AppendLine();
         }
         OverviewBodyText.text = overviewStringBuilder.ToString();
         
         
         // Show UI: Messages
         StringBuilder mailMessagesStringBuilder = new StringBuilder();
         mailMessagesStringBuilder.AppendLine();
         mailMessagesStringBuilder.Append("MAIL MESSAGES").AppendLine().AppendLine();
         foreach (string mailMessageLog in mailServiceExampleData.MailMessageLogs)
         {
            mailMessagesStringBuilder.Append("\t" + mailMessageLog).AppendLine();
         }
         MailMessagesBodyText.text = mailMessagesStringBuilder.ToString();

         // Show UI: Other
         UpdateMailMessagesButton.interactable = mailServiceExampleData.IsBeamableSetup;
         RefreshButton.interactable = mailServiceExampleData.IsBeamableSetup;
         _resetPlayerButton.interactable = mailServiceExampleData.IsBeamableSetup;
         
         MenuTitleText.text = "MailService Example";
         OverviewTitleText.text = "Overview";
         MailMessagesTitleText.text = "Mail";
         
         UpdateMailMessagesButton.GetComponentInChildren<TMP_Text>().text =
            $"Update\nMail";
         
         RefreshButton.GetComponentInChildren<TMP_Text>().text = 
            $"Refresh\nUI";
      }
   }
}


