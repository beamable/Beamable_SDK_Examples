using System.Text;
using Beamable.Common.Api.CloudData;
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
      private Button LoadTrialDataButton { get { return Button01;}}
      
      // Groups Panel
      private TMP_Text MainTitleText { get { return TitleText02; }}
      private TMP_Text MainBodyText { get { return BodyText02; }}
      
      // Messages Panel 
      private TMP_Text LogsTitleText { get { return TitleText03; }}
      private TMP_Text LogsBodyText { get { return BodyText03; }}
      
      //  Unity Methods  --------------------------------
      protected override void Start()
      {
         base.Start();

         _trialDataServiceExample.OnRefreshed.AddListener(TrialDataServiceExample_OnRefreshed);
         LoadTrialDataButton.onClick.AddListener(LoadTrialDataButton_OnClicked);
         
         // Populate default UI
         _trialDataServiceExample.Refresh();
      }

      //  Methods  --------------------------------------
      
      
      //  Event Handlers  -------------------------------
      private async void LoadTrialDataButton_OnClicked()
      {
         LoadTrialDataButton.interactable = false;
         await _trialDataServiceExample.LoadTrialData();
      }
      
      private void TrialDataServiceExample_OnRefreshed(TrialDataServiceExampleData 
         trialDataServiceExampleData)
      {
         // Show UI: Main
         StringBuilder stringBuilder01 = new StringBuilder();
         
         stringBuilder01.Append($" • IsInABTest = " + 
                                $"{trialDataServiceExampleData.IsInABTest}").AppendLine();
         stringBuilder01.Append($" • CloudMetaDatas.Count = " + 
            $"{trialDataServiceExampleData.CloudMetaDatas.Count}").AppendLine();

         foreach (CloudMetaData cloudMetaData in trialDataServiceExampleData.CloudMetaDatas)
         {
            stringBuilder01.Append($"\tcohort = {cloudMetaData.cohort.cohort}").AppendLine();
            stringBuilder01.Append($"\ttrial = {cloudMetaData.cohort.trial}").AppendLine();
         }
         
         MainBodyText.text = stringBuilder01.ToString();
         
         // Show UI: Logs
         StringBuilder stringBuilder02 = new StringBuilder();
         MyPlayerProgression myPlayerProgression = trialDataServiceExampleData.MyPlayerProgression;
         if (myPlayerProgression != null)
         {
            // Show loaded data
            stringBuilder02.Append($"<b>Data</b>").AppendLine();
            stringBuilder02.Append($" • MyPlayerProgression").AppendLine();
            stringBuilder02.Append($"\tMaxHealth = {myPlayerProgression.MaxHealth}").AppendLine();
            stringBuilder02.Append($"\tMaxInventorySpace = {myPlayerProgression.MaxInventorySpace}").AppendLine();
            
            // Show summary of example
            stringBuilder02.AppendLine();
            stringBuilder02.Append($"<b>Summary</b>").AppendLine();
            string consequence = " has the same values for all users.";
            if (trialDataServiceExampleData.IsInABTest)
            {
               consequence = " has values driven by the AB Test.";
            }
         
            stringBuilder02.Append($" • Because IsInABTest = {trialDataServiceExampleData.IsInABTest}, " +
                                   $"{trialDataServiceExampleData.DataName}" +
                                   $"{consequence}").AppendLine();
         }
         LogsBodyText.text = stringBuilder02.ToString();
         
         // Show UI: Other 
         MenuTitleText.text = "TrialData Service Example";
         MainTitleText.text = "Main";
         LogsTitleText.text = "Logs";

         LoadTrialDataButton.interactable = trialDataServiceExampleData.IsUIInteractable;
         
         LoadTrialDataButton.GetComponentInChildren<TMP_Text>().text =
            $"Reload Trial";
      }
   }
}


