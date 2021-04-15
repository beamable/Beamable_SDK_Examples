using System.Text;
using Beamable.Examples.Shared;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Beamable.Examples.Services.InventoryService
{
   /// <summary>
   /// The UI for the <see cref="InventoryServiceExample"/>.
   /// </summary>
   public class InventoryServiceExampleUI : ExampleCanvasUI
   {
      //  Fields  ---------------------------------------
      [SerializeField] private InventoryServiceExample _inventoryServiceExample = null;

      // Menu Panel
      private TMP_Text MenuTitleText { get { return TitleText01; }}
      private Button AddItemButton { get { return Button01;}}
      private Button DeleteItemButton { get { return Button02;}}
      private Button RefreshButton { get { return Button03;}}
      
      // Content Panel
      private TMP_Text ContentTitleText { get { return TitleText02; }}
      private TMP_Text ContentBodyText { get { return BodyText02; }}
      
      // Inventory Panel 
      private TMP_Text InventoryTitleText { get { return TitleText03; }}
      private TMP_Text InventoryBodyText { get { return BodyText03; }}
   
      //  Unity Methods  --------------------------------
      protected void Start()
      {
         _inventoryServiceExample.OnRefreshed.AddListener(InventoryServiceExample_OnRefreshed);
         AddItemButton.onClick.AddListener(AddItemButton_OnClicked);
         DeleteItemButton.onClick.AddListener(DeleteItemButton_OnClicked);
         RefreshButton.onClick.AddListener(RefreshButton_OnClicked);
         
         // Populate default UI
         RefreshButton_OnClicked();
      }

      //  Event Handlers  -------------------------------
      private void AddItemButton_OnClicked()
      {
         _inventoryServiceExample.AddOneItem();
      }

      private void DeleteItemButton_OnClicked()
      {
         _inventoryServiceExample.DeleteOneItem();
      }

      private void RefreshButton_OnClicked()
      {
         _inventoryServiceExample.Refresh();
      }
      
      private void InventoryServiceExample_OnRefreshed(InventoryServiceExampleData 
         inventoryServiceExampleData)
      {
         // Show UI: Game Content
         StringBuilder clientContentObjectStringBuilder = new StringBuilder();
         foreach (string clientContentObjectName in inventoryServiceExampleData.ContentObjectNames)
         {
            clientContentObjectStringBuilder.Append(clientContentObjectName).AppendLine();
         }
         ContentBodyText.text = clientContentObjectStringBuilder.ToString();

         // Show UI: Player Inventory
         StringBuilder playerInventoryStringBuilder = new StringBuilder();
         foreach (string playerInventoryItemName in inventoryServiceExampleData.InventoryItemNames)
         {
            playerInventoryStringBuilder.Append(playerInventoryItemName).AppendLine();
         }
         InventoryBodyText.text = playerInventoryStringBuilder.ToString();

         // Show UI: Other
         ContentTitleText.text = "Content";
         InventoryTitleText.text = "Inventory";
         MenuTitleText.text = "Menu";
         
         AddItemButton.GetComponentInChildren<TMP_Text>().text = 
            $"Add 1 Item\n({inventoryServiceExampleData.ItemToAddName})";
         
         DeleteItemButton.GetComponentInChildren<TMP_Text>().text = 
            $"Delete 1 Item\n({inventoryServiceExampleData.ItemToDeleteName})";

         RefreshButton.GetComponentInChildren<TMP_Text>().text =
            $"Refresh";
      }
   }
}


