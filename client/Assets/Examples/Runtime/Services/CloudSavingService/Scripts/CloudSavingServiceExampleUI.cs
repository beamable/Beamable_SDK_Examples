using System.Text;
using Beamable.Examples.Shared;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Beamable.Examples.Services.CloudSavingService
{
   /// <summary>
   /// The UI for the <see cref="CloudSavingServiceExample"/>.
   /// </summary>
   public class CloudSavingServiceExampleUI : ExampleCanvasUI
   {
      //  Fields  ---------------------------------------
      [SerializeField] 
      private CloudSavingServiceExample _cloudSavingServiceExample = null;
      
      // Menu Panel
      private TMP_Text MenuTitleText { get { return TitleText01; }}
      private Button LoadDataButton { get { return Button01;}}
      private Button RandomizeDataButton { get { return Button02;}}
      private Button SaveDataButton { get { return Button03;}}
      
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

         LoadDataButton.onClick.AddListener(LoadDataButton_OnClicked);
         RandomizeDataButton.onClick.AddListener(RandomizeDataButton_OnClicked);
         SaveDataButton.onClick.AddListener(SaveDataButton_OnClicked);
         
         //
         _cloudSavingServiceExample.OnRefreshed.AddListener(CloudSavingServiceExample_OnRefreshed);
         
         // Populate default UI
         _cloudSavingServiceExample.Refresh();
      }

      //  Methods  --------------------------------------
      
      
      //  Event Handlers  -------------------------------
      private void LoadDataButton_OnClicked()
      {
         _cloudSavingServiceExample.LoadData();
      }
      
      
      private void RandomizeDataButton_OnClicked()
      {
         _cloudSavingServiceExample.RandomizeData();
      }
      
      
      private void SaveDataButton_OnClicked()
      {
         _cloudSavingServiceExample.SaveData();
      }

      
      private void CloudSavingServiceExample_OnRefreshed(CloudSavingServiceExampleData
         cloudSavingServiceExampleData)
      {
         // Show UI: Main
         StringBuilder stringBuilder01 = new StringBuilder();
         stringBuilder01.AppendLine();
         stringBuilder01.Append("HISTORY").AppendLine();
         string isDataFoundFirstFrame = cloudSavingServiceExampleData.IsDataFoundFirstFrame.ToString();
         if (cloudSavingServiceExampleData.DataState == DataState.Initializing)
         {
            isDataFoundFirstFrame = "Pending...";
         }
         stringBuilder01.Append($" • Cloud data found on scene load? {isDataFoundFirstFrame}").AppendLine();

         stringBuilder01.AppendLine();
         stringBuilder01.Append("STATE").AppendLine();
         stringBuilder01.Append($" • DataState = {cloudSavingServiceExampleData.DataState}").AppendLine();

         stringBuilder01.AppendLine();
         stringBuilder01.Append("LOCAL").AppendLine();
         stringBuilder01.Append($" • {cloudSavingServiceExampleData.MyCustomDataLocal}").AppendLine();
         
         stringBuilder01.AppendLine();
         stringBuilder01.Append("CLOUD").AppendLine();
         stringBuilder01.Append($" • {cloudSavingServiceExampleData.MyCustomDataCloud}").AppendLine();

         MainBodyText.text = stringBuilder01.ToString();

         // Show UI: Instructions
         StringBuilder stringBuilder02 = new StringBuilder();
         stringBuilder02.AppendLine();
         stringBuilder02.Append("INSTRUCTIONS").AppendLine();
         foreach (string instructionLog in cloudSavingServiceExampleData.InstructionLogs)
         {
            stringBuilder02.Append($" • {instructionLog}").AppendLine();
         }

         LogsBodyText.text = stringBuilder02.ToString();

         // Show UI: Other 
         MenuTitleText.text = "Cloud Saving Service Example";
         MainTitleText.text = "Main";
         LogsTitleText.text = "Instructions";

         // Button Interactable
         RandomizeDataButton.interactable = cloudSavingServiceExampleData.DataState == DataState.Synced;
         SaveDataButton.interactable = cloudSavingServiceExampleData.DataState == DataState.Unsynced;
         LoadDataButton.interactable = cloudSavingServiceExampleData.DataState == DataState.Pending ||
                                       cloudSavingServiceExampleData.DataState == DataState.Unsynced;
         
         LoadDataButton.GetComponentInChildren<TMP_Text>().text = 
            $"Load Data\n(From Cloud)";
         
         RandomizeDataButton.GetComponentInChildren<TMP_Text>().text =
            $"Randomize Data\n(Local Only)";
         
         SaveDataButton.GetComponentInChildren<TMP_Text>().text =
            $"Save Data\n(To Cloud)";
         
      }
   }
}


