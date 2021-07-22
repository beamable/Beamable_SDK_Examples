using System.Text;
using Beamable.Examples.Shared;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Beamable.Examples.Services.TrialDataService
{
   /// <summary>
   /// The UI for the <see cref="TrialDataServiceExample"/>.
   /// </summary>
   public class TrialDataServiceExampleUI : ExampleCanvasUI
   {
      //  Fields  ---------------------------------------
      [SerializeField] private TrialDataServiceExample _trialDataServiceExample = null;

      // Menu Panel
      private TMP_Text MenuTitleText { get { return TitleText01; }}
      private Button TogglePlayerLevelButton { get { return Button01;}}
      private Button RefreshButton { get { return Button02;}}
      
      // Groups Panel
      private TMP_Text MainTitleText { get { return TitleText02; }}
      private TMP_Text MainBodyText { get { return BodyText02; }}
      
      // Messages Panel 
      private TMP_Text LogsTitleText { get { return TitleText03; }}
      private TMP_Text LogsBodyText { get { return BodyText03; }}
      
      //  Unity Methods  --------------------------------
      protected void Start()
      {
         _trialDataServiceExample.OnRefreshed.AddListener(TrialDataServiceExample_OnRefreshed);
         TogglePlayerLevelButton.onClick.AddListener(TogglePlayerLevelButton_OnClicked);
         RefreshButton.onClick.AddListener(RefreshButton_OnClicked);
         
         // Populate default UI
         _trialDataServiceExample.Refresh();
      }



      //  Methods  --------------------------------------
      
      
      //  Event Handlers  -------------------------------
      private async void TogglePlayerLevelButton_OnClicked()
      {
         await _trialDataServiceExample.TogglePlayerlevel();
      }
      
      private void RefreshButton_OnClicked()
      {
         _trialDataServiceExample.Refresh();
      }
      
      private void TrialDataServiceExample_OnRefreshed(TrialDataServiceExampleData 
         trialDataServiceExampleData)
      {
         // Show UI: HasInternet
         StringBuilder stringBuilder01 = new StringBuilder();
         
         stringBuilder01.Append($" • IsDataExistingOnFirstFrame = " + 
            $"{trialDataServiceExampleData.IsDataExistingOnFirstFrame}").AppendLine();
         stringBuilder01.Append($" • PlayerLevel = " + 
                                $"{trialDataServiceExampleData.PlayerLevel}").AppendLine();
         stringBuilder01.Append($" • IsInABTest = " + 
                                $"{trialDataServiceExampleData.IsInABTest}").AppendLine();
         stringBuilder01.Append($" • CloudMetaDatas.Count = " + 
            $"{trialDataServiceExampleData.CloudMetaDatas.Count}").AppendLine();
         
         MainBodyText.text = stringBuilder01.ToString();
         
         // Show UI: Logs
         StringBuilder stringBuilder02 = new StringBuilder();
         if (trialDataServiceExampleData.OutputLogs.Count > 0)
         {
            foreach (string log in trialDataServiceExampleData.OutputLogs)
            {
               stringBuilder02.Append($" • {log}").AppendLine();
            }
            stringBuilder02.AppendLine();

            stringBuilder02.Append($"<b>Summary</b>").AppendLine();
            string consequence = " is the same values for all users.";
            if (trialDataServiceExampleData.IsInABTest)
            {
               consequence = " has values driven by the AB Test.";
            }
         
            stringBuilder02.Append($"Because IsInABTest = {trialDataServiceExampleData.IsInABTest}, " +
                                   $"{trialDataServiceExampleData.DataName}" +
                                   $"{consequence}").AppendLine();
         }
         LogsBodyText.text = stringBuilder02.ToString();
         
         // Show UI: Other 
         MenuTitleText.text = "TrialData Service Example";
         MainTitleText.text = "Main";
         LogsTitleText.text = "Logs";
         
         // Toggle Value
         int nextPlayerLevel = 2;
         if (trialDataServiceExampleData.PlayerLevel != 1)
         {
            nextPlayerLevel = 1;
         }
         
         TogglePlayerLevelButton.GetComponentInChildren<TMP_Text>().text =
            $"Toggle\n(PlayerLevel to {nextPlayerLevel})";
         RefreshButton.GetComponentInChildren<TMP_Text>().text =
            $"Refresh\n(Debug)";
      }
   }
}


