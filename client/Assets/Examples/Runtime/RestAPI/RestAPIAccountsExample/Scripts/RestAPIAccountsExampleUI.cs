using System.Text;
using Beamable.Examples.Shared;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Beamable.Examples.RestAPI.RestAPIAccountsExample
{
   /// <summary>
   /// The UI for the <see cref="RestAPIAccountsExample"/>.
   /// </summary>
   public class RestAPIAccountsExampleUI: ExampleCanvasUI
   {
      //  Fields  ---------------------------------------
      [SerializeField] private RestAPIAccountsExample _restAPIAccountsExample = null;

      // Menu Panel
      private TMP_Text MenuTitleText { get { return TitleText01; }}
      private Button BasicAccountsMeButton { get { return Button01;}}
      private Button BasicAccountsRegisterButton { get { return Button02;}}
      
      // Content Panel
      private TMP_Text MainTitleText { get { return TitleText02; }}
      private TMP_Text MainBodyText { get { return BodyText02; }}
      
      // Inventory Panel 
      private TMP_Text DetailTitleText { get { return TitleText03; }}
      private TMP_Text DetailBodyText { get { return BodyText03; }}
   
      //  Unity Methods  --------------------------------
      protected override void Start()
      {
         base.Start();
         
         _restAPIAccountsExample.OnRefreshed.AddListener(AuthServiceExample_OnRefreshed);
         BasicAccountsMeButton.onClick.AddListener(BasicAccountsMeButton_OnClicked);
         BasicAccountsRegisterButton.onClick.AddListener(BasicAccountsRegisterButton_OnClicked);
         
         // Populate default UI
         _restAPIAccountsExample.Refresh();
      }

      //  Event Handlers  -------------------------------
      private async void BasicAccountsMeButton_OnClicked()
      {
         await _restAPIAccountsExample.Call_BasicAccountsMe();
      }

      
      private async void BasicAccountsRegisterButton_OnClicked()
      {
         await _restAPIAccountsExample.Call_BasicAccountsRegister();
      }
      
      
      private void AuthServiceExample_OnRefreshed(RestAPIAccountsExampleData 
         restAPIAccountsExampleData)
      {
         // Show UI: Main
         StringBuilder mainStringBuilder = new StringBuilder();
         foreach (string mainText in restAPIAccountsExampleData.MainTexts)
         {
            mainStringBuilder.Append(mainText).AppendLine();
         }
         MainBodyText.text = mainStringBuilder.ToString();

         // Show UI: Detail
         StringBuilder detailStringBuilder = new StringBuilder();
         foreach (string detailText in restAPIAccountsExampleData.DetailTexts)
         {
            detailStringBuilder.Append(detailText).AppendLine().AppendLine();
         }
         DetailBodyText.text = detailStringBuilder.ToString();

         // Show UI: Other
         BasicAccountsMeButton.interactable = restAPIAccountsExampleData.IsBeamableSetup;
         BasicAccountsRegisterButton.interactable = restAPIAccountsExampleData.IsBeamableSetup;
         _resetPlayerButton.interactable = restAPIAccountsExampleData.IsBeamableSetup;

         MenuTitleText.text = "RestAPI Accounts Example";
         MainTitleText.text = "Instructions";
         DetailTitleText.text = "Responses";

         BasicAccountsMeButton.GetComponentInChildren<TMP_Text>().text = 
            $"Call\n/basic/accounts/me";
         
         BasicAccountsRegisterButton.GetComponentInChildren<TMP_Text>().text = 
            $"Call\n/basic/accounts/register";
      }
   }
}


