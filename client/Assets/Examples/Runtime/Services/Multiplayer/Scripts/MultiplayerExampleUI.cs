using System.Text;
using Beamable.Examples.Shared;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Beamable.Examples.Services.Multiplayer
{
   /// <summary>
   /// The UI for the <see cref="Sim"/>.
   /// </summary>
   public class MultiplayerExampleUI : ExampleCanvasUI
   {
      //  Fields  ---------------------------------------
      [SerializeField] private MultiplayerExample _multiplayerExample = null;

      // Menu Panel
      private TMP_Text MenuTitleText { get { return TitleText01; }}
      private Button StartButton { get { return Button01;}}
      private Button StopButton { get { return Button02;}}
      private Button SendPlayerMoveButton { get { return Button03;}}
      
      // Content Panel
      private TMP_Text ContentTitleText { get { return TitleText02; }}
      private TMP_Text ContentBodyText { get { return BodyText02; }}
      
      // Inventory Panel 
      private TMP_Text InventoryTitleText { get { return TitleText03; }}
      private TMP_Text InventoryBodyText { get { return BodyText03; }}
   
      //  Unity Methods  --------------------------------
      protected override void Start()
      {
         base.Start();

         _multiplayerExample.OnRefreshed.AddListener(InventoryServiceExample_OnRefreshed);
         StartButton.onClick.AddListener(StartButton_OnClicked);
         StopButton.onClick.AddListener(StopButton_OnClicked);
         SendPlayerMoveButton.onClick.AddListener(SendPlayerMoveButton_OnClicked);
         
         // Populate default UI
         _multiplayerExample.Refresh();
      }

      //  Event Handlers  -------------------------------
      private void StartButton_OnClicked()
      {
         _multiplayerExample.StartMultiplayer();
      }

      private void StopButton_OnClicked()
      {
         _multiplayerExample.StopMultiplayer();
      }

      private void SendPlayerMoveButton_OnClicked()
      {
         _multiplayerExample.SendPlayerMoveButton();
      }
      
      private void InventoryServiceExample_OnRefreshed(MultiplayerExampleData 
         multiplayerExampleData)
      {
         // Show UI: Main
         StringBuilder sb01 = new StringBuilder();
         sb01.Append($" • SessionState = {multiplayerExampleData.SessionState}").AppendLine();
         if (multiplayerExampleData.IsSessionConnected)
         {
            sb01.Append($" • MatchId = {multiplayerExampleData.MatchId}").AppendLine();
            sb01.Append($" • SessionSeed = {multiplayerExampleData.SessionSeed}").AppendLine();
            sb01.Append($" • PlayerDbids = {string.Join(",", multiplayerExampleData.PlayerDbids)}").AppendLine();
            sb01.Append($" • LocalPlayerDbid = {multiplayerExampleData.LocalPlayerDbid}").AppendLine();
            sb01.Append($" • CurrentFrame = {multiplayerExampleData.CurrentFrame}").AppendLine();
         }
         ContentBodyText.text = sb01.ToString();

         // Show UI: Player Moves
         StringBuilder sb02 = new StringBuilder();
         foreach (string playerInventoryItemName in multiplayerExampleData.PlayerMoveLogs)
         {
            sb02.Append($" • {playerInventoryItemName}").AppendLine();
         }
         InventoryBodyText.text = sb02.ToString();

         // Show UI: Other
         MenuTitleText.text = "Multiplayer Example";
         ContentTitleText.text = "Session";
         InventoryTitleText.text = "Moves";

         StartButton.interactable = !multiplayerExampleData.IsSessionConnected;
         StopButton.interactable = multiplayerExampleData.IsSessionConnected;
         SendPlayerMoveButton.interactable = multiplayerExampleData.IsSessionConnected;

         StartButton.GetComponentInChildren<TMP_Text>().text =
            $"Start\nMultiplayer";

         StopButton.GetComponentInChildren<TMP_Text>().text =
            $"Cancel\nMultiplayer";

         SendPlayerMoveButton.GetComponentInChildren<TMP_Text>().text =
            $"Send\nPlayer Move";
      }
   }
}


