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
      
      [SerializeField] 
      private Image _iconImage01 = null;
      
      [SerializeField] 
      private Image _iconImage02 = null;

      // Menu Panel
      private TMP_Text MenuTitleText { get { return TitleText01; }}
      private Button AddPrimaryCurrencyButton { get { return Button01;}}
      private Button RemovePrimaryCurrencyButton { get { return Button02;}}
      private Button TradePrimaryToSecondaryButton { get { return Button03;}}
      private Button TradeSecondaryToPrimaryButton { get { return _examplePanelUIs[0].Buttons[3];}}
      
      // Content Panel
      private TMP_Text ContentTitleText { get { return TitleText02; }}
      private TMP_Text ContentBodyText { get { return BodyText02; }}
      
      // Inventory Panel 
      private TMP_Text InventoryTitleText { get { return TitleText03; }}
      private TMP_Text InventoryBodyText { get { return BodyText03; }}
      
      // Icons Panel 
      private TMP_Text IconsTitleText { get { return _examplePanelUIs[3].TitleText; }}
   
      //  Unity Methods  --------------------------------
      protected override void Start()
      {
         base.Start();

         _inventoryCurrencyExample.OnRefreshed.AddListener(InventoryServiceExample_OnRefreshed);
         AddPrimaryCurrencyButton.onClick.AddListener(AddPrimaryCurrencyButton_OnClicked);
         RemovePrimaryCurrencyButton.onClick.AddListener(RemovePrimaryCurrencyButton_OnClicked);
         TradePrimaryToSecondaryButton.onClick.AddListener(TradePrimaryToSecondaryButton_OnClicked); 
         TradeSecondaryToPrimaryButton.onClick.AddListener(TradeSecondaryToPrimaryButton_OnClicked);
         
         // Populate default UI
         _inventoryCurrencyExample.Refresh();
      }

      //  Event Handlers  -------------------------------
      private void AddPrimaryCurrencyButton_OnClicked()
      {
         _inventoryCurrencyExample.AddPrimaryCurrency();
      }

      private void RemovePrimaryCurrencyButton_OnClicked()
      {
         _inventoryCurrencyExample.RemovePrimaryCurrency();
      }

      private void TradePrimaryToSecondaryButton_OnClicked()
      {
         _inventoryCurrencyExample.TradePrimaryToSecondary();
      }
      
      private void TradeSecondaryToPrimaryButton_OnClicked()
      {
         _inventoryCurrencyExample.TradeSecondaryToPrimary();
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
         
         // Show UI: Icon images
         if (inventoryItemExampleData.IsInteractable)
         {
            ExampleProjectHelper.AddressablesLoadAssetAsync<Image>(
               inventoryItemExampleData.CurrencyContentPrimary.icon, 
               _iconImage01);
            
            ExampleProjectHelper.AddressablesLoadAssetAsync<Image>(
               inventoryItemExampleData.CurrencyContentSecondary.icon, 
               _iconImage02);
         }

         // Show UI: Other
         MenuTitleText.text = "Inventory Currency Example";
         ContentTitleText.text = "Game - All Currencies";
         InventoryTitleText.text = "Player - Owned Currencies";
         IconsTitleText.text = "Icon Images";
         
         // Show UI: Button
         AddPrimaryCurrencyButton.interactable = inventoryItemExampleData.IsInteractable;
         RemovePrimaryCurrencyButton.interactable = inventoryItemExampleData.IsInteractable;
         TradePrimaryToSecondaryButton.interactable = inventoryItemExampleData.IsInteractable;
         TradeSecondaryToPrimaryButton.interactable = inventoryItemExampleData.IsInteractable;
         _resetPlayerButton.interactable = inventoryItemExampleData.IsInteractable;
         
         // Get shorter names
         string currencyContentPrimaryName = "";
         string currencyContentSecondaryName = "";
         if (inventoryItemExampleData.IsInteractable)
         {
            currencyContentPrimaryName = ExampleProjectHelper.GetDisplayNameFromContentId(
               inventoryItemExampleData.CurrencyContentPrimary.ContentName);
            
            currencyContentSecondaryName = ExampleProjectHelper.GetDisplayNameFromContentId(
               inventoryItemExampleData.CurrencyContentSecondary.ContentName);
         }
         
         // Show UI: Button
         AddPrimaryCurrencyButton.GetComponentInChildren<TMP_Text>().text = 
            $"Add\n({currencyContentPrimaryName})";
         
         RemovePrimaryCurrencyButton.GetComponentInChildren<TMP_Text>().text = 
            $"Remove\n({currencyContentPrimaryName})";
         
         TradePrimaryToSecondaryButton.GetComponentInChildren<TMP_Text>().text = 
            $"Trade\n({currencyContentPrimaryName}→{currencyContentSecondaryName})";
         
         TradeSecondaryToPrimaryButton.GetComponentInChildren<TMP_Text>().text = 
            $"Trade\n({currencyContentSecondaryName}→{currencyContentPrimaryName})";

      }
   }
}


