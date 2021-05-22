using System.Text;
using Beamable.Examples.Shared;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Beamable.Examples.Services.EventsService
{
   /// <summary>
   /// The UI for the <see cref="EventsServiceExample"/>.
   /// </summary>
   public class EventsServiceExampleUI : ExampleCanvasUI
   {
      //  Fields  ---------------------------------------
      [SerializeField] private EventsServiceExample _eventsServiceExample = null;

      // Menu Panel
      private TMP_Text MenuTitleText { get { return TitleText01; }}
      private Button SetScoreButton { get { return Button01;}}
      private Button ClaimButton { get { return Button02;}}
      private Button RefreshButton { get { return Button03;}}
      
      // Events Panel
      private TMP_Text EventsTitleText { get { return TitleText02; }}
      private TMP_Text EventsBodyText { get { return BodyText02; }}
      
      // Claims Panel 
      private TMP_Text ClaimsTitleText { get { return TitleText03; }}
      private TMP_Text ClaimsBodyText { get { return BodyText03; }}
   
      //  Unity Methods  --------------------------------
      protected void Start()
      {
         _eventsServiceExample.OnRefreshed.AddListener(EventsServiceExample_OnRefreshed);
         SetScoreButton.onClick.AddListener(SetScoreButton_OnClicked);
         ClaimButton.onClick.AddListener(ClaimButton_OnClicked);
         RefreshButton.onClick.AddListener(RefreshButton_OnClicked);
         
         // Populate default UI
         RefreshButton_OnClicked();
      }
      
      //  Event Handlers  -------------------------------
      private void SetScoreButton_OnClicked()
      {
         _eventsServiceExample.SetScoreInEvents();
      }

      private void ClaimButton_OnClicked()
      {
         _eventsServiceExample.ClaimRewardsInEvents();
      }

      private void RefreshButton_OnClicked()
      {
         _eventsServiceExample.Refresh();
      }
      
      private void EventsServiceExample_OnRefreshed(EventsServiceExampleData 
         eventsServiceExampleData)
      {
         StringBuilder stringBuilder01 = new StringBuilder();
         
         // Show UI: Events
         foreach (string runningEventsLog in eventsServiceExampleData.RunningEventsLogs)
         {
            stringBuilder01.Append(runningEventsLog).AppendLine();
         }
         EventsBodyText.text = stringBuilder01.ToString();
         
         StringBuilder stringBuilder02 = new StringBuilder();
         
         // Show UI: Scores
         stringBuilder02.AppendLine();
         stringBuilder02.Append("SCORES").AppendLine();
         foreach (string setScoreLog in eventsServiceExampleData.SetScoreLogs)
         {
            stringBuilder02.Append(setScoreLog).AppendLine();
         }
         
         // Show UI: Claims
         stringBuilder02.AppendLine();
         stringBuilder02.Append("CLAIMS").AppendLine();
         foreach (string claimLog in eventsServiceExampleData.ClaimLogs)
         {
            stringBuilder02.Append(claimLog).AppendLine();
         }
         ClaimsBodyText.text = stringBuilder02.ToString();

         // Show UI: Other
         MenuTitleText.text = "EventsService Example";
         EventsTitleText.text = "Events";
         ClaimsTitleText.text = "Claims";

         SetScoreButton.GetComponentInChildren<TMP_Text>().text = 
            $"SetScore\nTo {eventsServiceExampleData.Score}";
         
         ClaimButton.GetComponentInChildren<TMP_Text>().text = 
            $"Attempt\nClaim";
         
         RefreshButton.GetComponentInChildren<TMP_Text>().text = 
            $"Refresh\nUI";
      }
   }
}


