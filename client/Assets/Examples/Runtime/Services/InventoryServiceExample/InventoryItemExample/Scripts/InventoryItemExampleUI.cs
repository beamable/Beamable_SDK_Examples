using System.Text;
using Beamable.Examples.Shared;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Beamable.Examples.Services.InventoryService.InventoryItemExample
{
   /// <summary>
   /// The UI for the <see cref="InventoryItemExample"/>.
   /// </summary>
   public class InventoryItemExampleUI : ExampleCanvasUI
   {
      //  Fields  ---------------------------------------
      [SerializeField] private InventoryItemExample _inventoryItemExample = null;

      // Menu Panel
      private TMP_Text MenuTitleText { get { return TitleText01; }}
      private Button AddItemButton { get { return Button01;}}
      private Button DeleteItemButton { get { return Button02;}}
      
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

         _inventoryItemExample.OnRefreshed.AddListener(InventoryServiceExample_OnRefreshed);
         AddItemButton.onClick.AddListener(AddItemButton_OnClicked);
         DeleteItemButton.onClick.AddListener(DeleteItemButton_OnClicked);
         
         // Populate default UI
         _inventoryItemExample.Refresh();
      }

      //  Event Handlers  -------------------------------
      private void AddItemButton_OnClicked()
      {
         _inventoryItemExample.AddOneItem();
      }

      private void DeleteItemButton_OnClicked()
      {
         _inventoryItemExample.DeleteOneItem();
      }

      private void InventoryServiceExample_OnRefreshed(InventoryItemExampleData 
         inventoryItemExampleData)
      {
         // Show UI: Game Content
         StringBuilder clientContentObjectStringBuilder = new StringBuilder();
         foreach (string clientContentObjectName in inventoryItemExampleData.ContentObjectNames)
         {
            clientContentObjectStringBuilder.Append($" • {clientContentObjectName}").AppendLine();
         }
         ContentBodyText.text = clientContentObjectStringBuilder.ToString();

         // Show UI: Player Inventory
         StringBuilder playerInventoryStringBuilder = new StringBuilder();
         foreach (string playerInventoryItemName in inventoryItemExampleData.InventoryItemNames)
         {
            playerInventoryStringBuilder.Append($" • {playerInventoryItemName}").AppendLine();
         }
         InventoryBodyText.text = playerInventoryStringBuilder.ToString();

         // Show UI: Other
         MenuTitleText.text = "InventoryService Example";
         ContentTitleText.text = "Game - All Content";
         InventoryTitleText.text = "Player - Current Inventory";

         AddItemButton.interactable = inventoryItemExampleData.IsInteractable;
         DeleteItemButton.interactable = inventoryItemExampleData.IsInteractable;
         _resetPlayerButton.interactable = inventoryItemExampleData.IsInteractable;
         
         AddItemButton.GetComponentInChildren<TMP_Text>().text = 
            $"Add 1 Item\n({inventoryItemExampleData.ItemToAddName})";
         
         DeleteItemButton.GetComponentInChildren<TMP_Text>().text = 
            $"Delete 1 Item\n({inventoryItemExampleData.ItemToDeleteName})";

      }
   }
}


