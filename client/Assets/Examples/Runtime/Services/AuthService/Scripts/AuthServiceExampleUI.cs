using System.Text;
using Beamable.Examples.Shared;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Beamable.Examples.Services.AuthService
{
   /// <summary>
   /// The UI for the <see cref="AuthServiceExample"/>.
   /// </summary>
   public class AuthServiceExampleUI: ExampleCanvasUI
   {
      //  Fields  ---------------------------------------
      [SerializeField] private AuthServiceExample _authServiceExample = null;

      // Menu Panel
      private TMP_Text MenuTitleText { get { return TitleText01; }}
      private Button UpdateCurrentUserButton { get { return Button01;}}
      private Button SwitchCurrentUserButton { get { return Button02;}}
      
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
         
         _authServiceExample.OnRefreshed.AddListener(AuthServiceExample_OnRefreshed);
         UpdateCurrentUserButton.onClick.AddListener(UpdateCurrentUserButton_OnClicked);
         SwitchCurrentUserButton.onClick.AddListener(SwitchCurrentUserButton_OnClicked);
         
         // Populate default UI
         _authServiceExample.Refresh();
      }

      //  Event Handlers  -------------------------------
      private async void UpdateCurrentUserButton_OnClicked()
      {
         await _authServiceExample.UpdateCurrentUser();
      }

      
      private async void SwitchCurrentUserButton_OnClicked()
      {
         await _authServiceExample.SwitchCurrentUser();
      }
      
      
      private void AuthServiceExample_OnRefreshed(AuthServiceExampleData 
         authServiceExampleData)
      {
         // Show UI: Main
         StringBuilder mainStringBuilder = new StringBuilder();
         foreach (string mainText in authServiceExampleData.MainTexts)
         {
            mainStringBuilder.Append(mainText).AppendLine();
         }
         MainBodyText.text = mainStringBuilder.ToString();

         // Show UI: Detail
         StringBuilder detailStringBuilder = new StringBuilder();
         foreach (string detailText in authServiceExampleData.DetailTexts)
         {
            detailStringBuilder.Append(detailText).AppendLine();
         }
         DetailBodyText.text = detailStringBuilder.ToString();

         // Show UI: Other
         UpdateCurrentUserButton.interactable = authServiceExampleData.IsBeamableSetup;
         SwitchCurrentUserButton.interactable = authServiceExampleData.IsBeamableSetup;
         _resetPlayerButton.interactable = authServiceExampleData.IsBeamableSetup;

         MenuTitleText.text = "AuthService Example";
         MainTitleText.text = "Main";
         DetailTitleText.text = "Detail";

         UpdateCurrentUserButton.GetComponentInChildren<TMP_Text>().text = 
            $"Update\nCurrent User";
         
         SwitchCurrentUserButton.GetComponentInChildren<TMP_Text>().text = 
            $"Switch\nCurrent User";
      }
   }
}


