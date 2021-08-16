using System.Text;
using Beamable.Examples.Shared;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace Beamable.Examples.Services.CommerceService
{
   /// <summary>
   /// The UI for the <see cref="CommerceServiceExample"/>.
   /// </summary>
   public class CommerceServiceExampleUI : ExampleCanvasUI
   {
      //  Fields  ---------------------------------------
      [SerializeField] 
      private CommerceServiceExample _commerceServiceExample = null;
      
      [SerializeField] 
      private Image _iconImage01 = null;
      
      [SerializeField] 
      private Image _iconImage02 = null;
      
      // Menu Panel
      private TMP_Text MenuTitleText { get { return TitleText01; }}
      private Button BuyButton { get { return Button01;}}
      
      // Groups Panel
      private TMP_Text MainTitleText { get { return TitleText02; }}
      private TMP_Text MainBodyText { get { return BodyText02; }}
      
      // Messages Panel 
      private TMP_Text LogsTitleText { get { return TitleText03; }}
      private TMP_Text LogsBodyText { get { return BodyText03; }}
      
      // Icons Panel 
      private TMP_Text IconsTitleText { get { return _examplePanelUIs[3].TitleText; }}
      
      //  Unity Methods  --------------------------------
      protected override void Start()
      {
         base.Start();

         BuyButton.onClick.AddListener(BuyButton_OnClicked);
         
         //
         _commerceServiceExample.OnRefreshed.AddListener(CommerceServiceExample_OnRefreshed);
         
         // Populate default UI
         _commerceServiceExample.Refresh();
      }

      //  Methods  --------------------------------------
      
      
      //  Event Handlers  -------------------------------
      private void BuyButton_OnClicked()
      {
         _commerceServiceExample.Buy();
      }

      private void CommerceServiceExample_OnRefreshed(CommerceServiceExampleData
         commerceServiceExampleData)
      {
         // Show UI: Main
         StringBuilder stringBuilder01 = new StringBuilder();
         stringBuilder01.AppendLine();
         stringBuilder01.Append("CONTENT").AppendLine();
         foreach (ItemData storeItemData in commerceServiceExampleData.StoreItemDatas)
         {
            stringBuilder01.Append($" • {storeItemData.Title}").AppendLine();
         }

         stringBuilder01.AppendLine();
         stringBuilder01.Append("CURRENCY").AppendLine();
         foreach (string currencyLog in commerceServiceExampleData.CurrencyLogs)
         {
            stringBuilder01.Append($" • {currencyLog}").AppendLine();
         }
         
         stringBuilder01.AppendLine();
         stringBuilder01.Append("INVENTORY").AppendLine();
         foreach (ItemData inventoryStoreItemData in commerceServiceExampleData.InventoryItemDatas)
         {
            stringBuilder01.Append($" • {inventoryStoreItemData.Title}").AppendLine();
         }

         MainBodyText.text = stringBuilder01.ToString();

         // Show UI: Instructions
         StringBuilder stringBuilder02 = new StringBuilder();
         stringBuilder02.AppendLine();
         stringBuilder02.Append("INSTRUCTIONS").AppendLine();
         foreach (string instructionLog in commerceServiceExampleData.InstructionLogs)
         {
            stringBuilder02.Append($" • {instructionLog}").AppendLine();
         }

         LogsBodyText.text = stringBuilder02.ToString();

         // Show UI: Other 
         MenuTitleText.text = "Commerce Service Example";
         MainTitleText.text = "Main";
         LogsTitleText.text = "Instructions";
         IconsTitleText.text = "Icon Images";

         // Button Interactable
         BuyButton.interactable = commerceServiceExampleData.CanAffordSelectedStoreItemData;

         // Icon images
         if (commerceServiceExampleData.SelectedItemData != null)
         {
            AssetReferenceSprite assetReferenceSprite01 = 
               commerceServiceExampleData.SelectedItemData.ItemContent.icon;
            
            ExampleProjectHelper.AddressablesLoadAssetAsync<Image>(assetReferenceSprite01, _iconImage01);
            
         }
         
         if (commerceServiceExampleData.CurrencyContent != null)
         {
            AssetReferenceSprite assetReferenceSprite02 = 
               commerceServiceExampleData.CurrencyContent.icon;
            
            ExampleProjectHelper.AddressablesLoadAssetAsync<Image>(assetReferenceSprite02, _iconImage02);
         }
         
         
         // Button Text
         string itemName = "";
         if (commerceServiceExampleData.CanAffordSelectedStoreItemData)
         {
            itemName = commerceServiceExampleData.SelectedItemData.Title;
         }
         
         BuyButton.GetComponentInChildren<TMP_Text>().text =
               $"Buy\n{itemName}";
      }
   }
}


