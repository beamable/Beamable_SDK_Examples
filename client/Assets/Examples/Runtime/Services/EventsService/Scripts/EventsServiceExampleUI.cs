using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Beamable.Examples.Services.EventsService
{
   /// <summary>
   /// The UI for the <see cref="EventsServiceExample"/>.
   /// </summary>
   public class EventsServiceExampleUI : MonoBehaviour
   {
      //  Fields  ---------------------------------------
      
      [SerializeField] private EventsServiceExample _eventsServiceExample = null;

      [SerializeField] private TMP_Text _text01 = null;
      [SerializeField] private TMP_Text _text02 = null;

      [SerializeField] private Button _setScoreButton = null;
      [SerializeField] private Button _claimButton = null;
      [SerializeField] private Button _refreshAllButton = null;
      
      //  Unity Methods  --------------------------------
      protected void Start()
      {
         _eventsServiceExample.OnRefreshed.AddListener(EventsServiceExample_OnRefreshed);
         _setScoreButton.onClick.AddListener(SetScoreButton_OnClicked);
         _claimButton.onClick.AddListener(ClaimButton_OnClicked);
         _refreshAllButton.onClick.AddListener(RefreshAllButton_OnClicked);
      }

      //  Methods  --------------------------------------
      
      //  Event Handlers  -------------------------------
      private void SetScoreButton_OnClicked()
      {
         _eventsServiceExample.SetScoreInEvents();
      }

      private void ClaimButton_OnClicked()
      {
         _eventsServiceExample.ClaimRewardsInEvents();
      }

      private void RefreshAllButton_OnClicked()
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
         _text01.text = stringBuilder01.ToString();
         
         StringBuilder stringBuilder02 = new StringBuilder();
         
         // Show UI: Scores
         stringBuilder02.Append("SCORES").AppendLine();
         foreach (string setScoreLog in eventsServiceExampleData.SetScoreLogs)
         {
            stringBuilder02.Append(setScoreLog).AppendLine();
         }
         
         // Show UI: Claims
         stringBuilder02.Append("CLAIMS").AppendLine();
         foreach (string claimLog in eventsServiceExampleData.ClaimLogs)
         {
            stringBuilder02.Append(claimLog).AppendLine();
         }
         _text02.text = stringBuilder02.ToString();

         // Show UI: Button Content Names
         _setScoreButton.GetComponentInChildren<TMP_Text>().text = $"SetScore\n({eventsServiceExampleData.Score})";
         _claimButton.GetComponentInChildren<TMP_Text>().text = $"Claim";
      }
   }
}


