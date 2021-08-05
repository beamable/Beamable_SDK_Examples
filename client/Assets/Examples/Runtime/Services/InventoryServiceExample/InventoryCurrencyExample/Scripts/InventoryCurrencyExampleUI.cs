using System.Text;
using Beamable.Examples.Shared;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Beamable.Examples.Services.InventoryService.InventoryCurrencyExample
{
   /// <summary>
   /// The UI for the <see cref="InventoryCurrencyExample"/>.
   /// </summary>
   public class InventoryCurrencyExampleUI : ExampleCanvasUI
   {
      //  Fields  ---------------------------------------
      [SerializeField] 
      private InventoryCurrencyExample _inventoryCurrencyExample = null;

      // Menu Panel
      private TMP_Text MenuTitleText { get { return TitleText01; }}
      private Button AddCoinButton { get { return Button01;}}
      private Button RemoveCoinButton { get { return Button02;}}
      private Button TradeCoinToXPButton { get { return Button03;}}
      private Button TradeXPToCoinButton { get { return _examplePanelUIs[0].Buttons[3];}}
      private Button ResetPlayerButton { get { return _examplePanelUIs[0].Buttons[4];}}
      
      // Content Panel
      private TMP_Text ContentTitleText { get { return TitleText02; }}
      private TMP_Text ContentBodyText { get { return BodyText02; }}
      
      // Inventory Panel 
      private TMP_Text InventoryTitleText { get { return TitleText03; }}
      private TMP_Text InventoryBodyText { get { return BodyText03; }}
   
      //  Unity Methods  --------------------------------
      protected void Start()
      {
         _inventoryCurrencyExample.OnRefreshed.AddListener(InventoryServiceExample_OnRefreshed);
         AddCoinButton.onClick.AddListener(AddCoinButton_OnClicked);
         RemoveCoinButton.onClick.AddListener(RemoveCoinButton_OnClicked);
         TradeCoinToXPButton.onClick.AddListener(TradeCoinToXPButton_OnClicked); 
         TradeXPToCoinButton.onClick.AddListener(TradeXPToCoinButton_OnClicked);
         ResetPlayerButton.onClick.AddListener(ResetPlayerButton_OnClicked);
         
         // Populate default UI
         _inventoryCurrencyExample.Refresh();
      }

      //  Event Handlers  -------------------------------
      private void AddCoinButton_OnClicked()
      {
         _inventoryCurrencyExample.AddCoin();
      }

      private void RemoveCoinButton_OnClicked()
      {
         _inventoryCurrencyExample.RemoveCoin();
      }

      private void TradeCoinToXPButton_OnClicked()
      {
         _inventoryCurrencyExample.TradeCoinToXPButton();
      }
      
      private void TradeXPToCoinButton_OnClicked()
      {
         _inventoryCurrencyExample.TradeXPToCoinButton();
      }
      
      private void ResetPlayerButton_OnClicked()
      {
         _inventoryCurrencyExample.ResetPlayer();
      }
      
      private void InventoryServiceExample_OnRefreshed(InventoryCurrencyExampleData 
         inventoryItemExampleData)
      {
         // Show UI: Game Content
         StringBuilder clientContentObjectStringBuilder = new StringBuilder();
         foreach (string clientContentObjectName in inventoryItemExampleData.ContentCurrencyNames)
         {
            clientContentObjectStringBuilder.Append($" • {clientContentObjectName}").AppendLine();
         }
         ContentBodyText.text = clientContentObjectStringBuilder.ToString();

         // Show UI: Player Inventory
         StringBuilder playerInventoryStringBuilder = new StringBuilder();
         foreach (string playerInventoryItemName in inventoryItemExampleData.InventoryCurrencyNames)
         {
            playerInventoryStringBuilder.Append($" • {playerInventoryItemName}").AppendLine();
         }
         InventoryBodyText.text = playerInventoryStringBuilder.ToString();

         // Show UI: Other
         MenuTitleText.text = "Inventory Currency Example";
         ContentTitleText.text = "Game - All Content";
         InventoryTitleText.text = "Player - Current Inventory";
         
         AddCoinButton.GetComponentInChildren<TMP_Text>().text = 
            $"Add \n({inventoryItemExampleData.CurrencyToAddName})";
         
         RemoveCoinButton.GetComponentInChildren<TMP_Text>().text = 
            $"Remove\n({inventoryItemExampleData.CurrencyToRemoveName})";
         
         TradeCoinToXPButton.GetComponentInChildren<TMP_Text>().text = 
            $"Trade \n(Coin→XP)";
         
         TradeXPToCoinButton.GetComponentInChildren<TMP_Text>().text = 
            $"Trade \n(XP→Coin)";

         ResetPlayerButton.GetComponentInChildren<TMP_Text>().text =
            $"Debug\n(Reset Player)";
      }
   }
}


