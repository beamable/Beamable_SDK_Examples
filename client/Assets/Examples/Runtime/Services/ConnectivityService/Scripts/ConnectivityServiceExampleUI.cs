using System.Text;
using Beamable.Examples.Shared;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Beamable.Examples.Services.ConnectivityService
{
   /// <summary>
   /// The UI for the <see cref="ConnectivityServiceExample"/>.
   /// </summary>
   public class ConnectivityServiceExampleUI : ExampleCanvasUI
   {
      //  Fields  ---------------------------------------
      [SerializeField] private ConnectivityServiceExample _connectivityServiceExample = null;

      // Menu Panel
      private TMP_Text MenuTitleText { get { return TitleText01; }}
      private Button SetHasInternetButton { get { return Button01;}}
      
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

         _connectivityServiceExample.OnRefreshed.AddListener(ConnectivityServiceExample_OnRefreshed);
         SetHasInternetButton.onClick.AddListener(SetHasInternetButton_OnClicked);
         
         // Populate default UI
         _connectivityServiceExample.Refresh();
      }

      //  Methods  --------------------------------------
      
      
      //  Event Handlers  -------------------------------
      private void SetHasInternetButton_OnClicked()
      {
         _connectivityServiceExample.ToggleHasInternet();
      }

      
      private void ConnectivityServiceExample_OnRefreshed(ConnectivityServiceExampleData 
         connectivityServiceExampleData)
      {
         // Show UI: HasInternet
         StringBuilder stringBuilder01 = new StringBuilder();
         stringBuilder01.Append("HAS CONNECTIVITY").AppendLine();
         stringBuilder01.Append($" • HasConnectivity = {connectivityServiceExampleData.HasConnectivity}").AppendLine();
         MainBodyText.text = stringBuilder01.ToString();
         
         // Show UI: Group Players
         StringBuilder stringBuilder02 = new StringBuilder();
         stringBuilder02.Append("LOGS").AppendLine();
         foreach (string log in connectivityServiceExampleData.OutputLogs)
         {
            stringBuilder02.Append($" • {log}").AppendLine();
         }
         LogsBodyText.text = stringBuilder02.ToString();
         
         // Show UI: Other 
         MenuTitleText.text = "Connectivity Service Example";
         MainTitleText.text = "Main";
         LogsTitleText.text = "Logs";
         
         SetHasInternetButton.GetComponentInChildren<TMP_Text>().text =
            $"Debug\nSet HasConnectivity = {!connectivityServiceExampleData.HasConnectivity}";
      }
   }
}


