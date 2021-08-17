using System.Text;
using Beamable.Examples.Shared;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Beamable.Examples.Services.MatchmakingService
{
   /// <summary>
   /// The UI for the <see cref="MatchmakingServiceExample"/>.
   /// </summary>
   public class MatchmakingServiceExampleUI : ExampleCanvasUI
   {
     
      //  Fields  ---------------------------------------
      [SerializeField] 
      private MatchmakingServiceExample _matchmakingServiceExample = null;

      // Menu Panel
      private TMP_Text MenuTitleText { get { return TitleText01; }}
      private Button StartMatchmakingButton { get { return Button01;}}
      private Button CancelMatchmakingButton { get { return Button02;}}
      
      // Events Panel
      private TMP_Text MainTitleText { get { return TitleText02; }}
      private TMP_Text MainBodyText { get { return BodyText02; }}
      
      // Claims Panel 
      private TMP_Text InstructionsTitleText { get { return TitleText03; }}
      private TMP_Text InstructionsBodyText { get { return BodyText03; }}
   
      //  Unity Methods  --------------------------------
      protected override void Start()
      {
         base.Start();

         _matchmakingServiceExample.OnRefreshed.AddListener(EventsServiceExample_OnRefreshed);
         StartMatchmakingButton.onClick.AddListener(StartMatchmakingButton_OnClicked);
         CancelMatchmakingButton.onClick.AddListener(CancelMatchmakingButton_OnClicked);
         
         _matchmakingServiceExample.Refresh();
      }
      
      //  Event Handlers  -------------------------------

      private void StartMatchmakingButton_OnClicked()
      {
         _matchmakingServiceExample.StartMatchmaking();
      }

      
      private void CancelMatchmakingButton_OnClicked()
      {
         _matchmakingServiceExample.CancelMatchmaking();
      }
      
      
      private void EventsServiceExample_OnRefreshed(MatchmakingServiceExampleData 
         matchmakingServiceExampleData)
      {
         
         // Show UI: Overview
         StringBuilder mainStringBuilder = new StringBuilder();
         mainStringBuilder.Append("MAIN").AppendLine().AppendLine();
         mainStringBuilder.Append($" • SessionState = {matchmakingServiceExampleData.SessionState}").AppendLine();
         foreach (string mainLog in matchmakingServiceExampleData.MainLogs)
         {
            mainStringBuilder.Append($" • {mainLog}").AppendLine();
         }
         
         // Show UI: Updates
         mainStringBuilder.AppendLine();
         mainStringBuilder.Append("MATCHMAKING").AppendLine().AppendLine();
         foreach (string matchmakingLog in matchmakingServiceExampleData.MatchmakingLogs)
         {
            mainStringBuilder.Append($" • {matchmakingLog}").AppendLine();
         }
         MainBodyText.text = mainStringBuilder.ToString();
         
         
         // Show UI: Messages
         StringBuilder instructionsStringBuilder = new StringBuilder();
         foreach (string instructionsLog in matchmakingServiceExampleData.InstructionsLogs)
         {
            instructionsStringBuilder.Append($" • {instructionsLog}").AppendLine();
         }
         InstructionsBodyText.text = instructionsStringBuilder.ToString();

         // Show UI: Other
         MenuTitleText.text = "Matchmaking Service Example";
         MainTitleText.text = "Main";
         InstructionsTitleText.text = "Instructions";

         StartMatchmakingButton.interactable = matchmakingServiceExampleData.CanStart;
         CancelMatchmakingButton.interactable = matchmakingServiceExampleData.CanCancel;

         StartMatchmakingButton.GetComponentInChildren<TMP_Text>().text =
            $"Start\nMatchmaking";
         
         CancelMatchmakingButton.GetComponentInChildren<TMP_Text>().text =
            $"Cancel\nMatchmaking";
         
      }
   }
}


