using System.Collections.Generic;
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

      // Content Panel
      private TMP_Text ContentTitleText { get { return TitleText01; }}
      private TMP_Text ContentBodyText { get { return BodyText01; }}
      
      // Inventory Panel 
      private TMP_Text InventoryTitleText { get { return TitleText02; }}
      private TMP_Text InventoryBodyText { get { return BodyText02; }}
   
      // Menu Panel
      private TMP_Text MenuTitleText { get { return TitleText03; }}
      private Button Add1ItemButton { get { return Button01;}}
      private Button Delete1ItemButton { get { return Button02;}}
      private Button RefreshAllButton { get { return Button03;}}
      
      
      //  Unity Methods  --------------------------------
      protected void Start()
      {
         ContentTitleText.text = "Content";
         InventoryTitleText.text = "Inventory";
         MenuTitleText.text = "Menu";
         
         _inventoryServiceExample.OnRefreshed.AddListener(InventoryServiceExample_OnRefreshed);
         Add1ItemButton.onClick.AddListener(Add1ItemButton_OnClicked);
         Delete1ItemButton.onClick.AddListener(Delete1ItemButton_OnClicked);
         RefreshAllButton.onClick.AddListener(RefreshAllButton_OnClicked);
      }

      //  Event Handlers  -------------------------------
      
      private void Add1ItemButton_OnClicked()
      {
         _inventoryServiceExample.AddOneItem();
      }

      private void Delete1ItemButton_OnClicked()
      {
         _inventoryServiceExample.DeleteOneItem();
      }

      private void RefreshAllButton_OnClicked()
      {
         _inventoryServiceExample.Refresh();
      }
      
      private void InventoryServiceExample_OnRefreshed(List<string> clientContentObjectNames, 
         List<string> playerInventoryItemNames, string itemToAddName, string itemToDeleteName)
      {
         // Show UI: Game Content
         StringBuilder clientContentObjectStringBuilder = new StringBuilder();
         foreach (string clientContentObjectName in clientContentObjectNames)
         {
            clientContentObjectStringBuilder.Append(clientContentObjectName).AppendLine();
         }
         ContentBodyText.text = clientContentObjectStringBuilder.ToString();

         // Show UI: Player Inventory
         StringBuilder playerInventoryStringBuilder = new StringBuilder();
         foreach (string playerInventoryItemName in playerInventoryItemNames)
         {
            playerInventoryStringBuilder.Append(playerInventoryItemName).AppendLine();
         }
         InventoryBodyText.text = playerInventoryStringBuilder.ToString();

         // Show UI: Button Content Names
         Add1ItemButton.GetComponentInChildren<TMP_Text>().text = $"Add 1 Item\n({itemToAddName})";
         Delete1ItemButton.GetComponentInChildren<TMP_Text>().text = $"Delete 1 Item\n({itemToDeleteName})";
      }
   }
}


