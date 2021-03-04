using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Beamable.Examples.Services.InventoryService
{
   /// <summary>
   /// The UI for the <see cref="InventoryServiceExample"/>.
   /// </summary>
   public class InventoryServiceExampleUI : MonoBehaviour
   {
      //  Fields  ---------------------------------------
      
      [SerializeField] private InventoryServiceExample _inventoryServiceExample = null;

      [SerializeField] private TMP_Text _gameContentBodyText = null;
      [SerializeField] private TMP_Text _playerInventoryBodyText = null;

      [SerializeField] private Button _add1ItemButton = null;
      [SerializeField] private Button _delete1ItemButton = null;
      [SerializeField] private Button _deleteAllItemsButton = null;
      [SerializeField] private Button _refreshAllButton = null;
      
      //  Unity Methods  --------------------------------
      protected void Start()
      {
         _inventoryServiceExample.OnRefreshed += InventoryServiceExample_OnRefreshed;
         _add1ItemButton.onClick.AddListener(Add1ItemButton_OnClicked);
         _delete1ItemButton.onClick.AddListener(Delete1ItemButton_OnClicked);
         _deleteAllItemsButton.onClick.AddListener(DeleteAllItemsButton_OnClicked);
         _refreshAllButton.onClick.AddListener(RefreshAllButton_OnClicked);
      }

      //  Methods  --------------------------------------
      
      private void Add1ItemButton_OnClicked()
      {
         _inventoryServiceExample.AddOneItem();
      }


      private void Delete1ItemButton_OnClicked()
      {
         _inventoryServiceExample.DeleteOneItem();
      }

      private void DeleteAllItemsButton_OnClicked()
      {
         _inventoryServiceExample.DeleteAllItems();
      }

      private void RefreshAllButton_OnClicked()
      {
         _inventoryServiceExample.Refresh();
      }
      
      //  Event Handlers  -------------------------------
      private void InventoryServiceExample_OnRefreshed(List<string> clientContentObjectNames, List<string> playerInventoryItemNames)
      {
         //Show UI: Game Content
         StringBuilder clientContentObjectStringBuilder = new StringBuilder();
         foreach (string clientContentObjectName in clientContentObjectNames)
         {
            clientContentObjectStringBuilder.Append(clientContentObjectName).AppendLine();
         }
         _gameContentBodyText.text = clientContentObjectStringBuilder.ToString();

         //Show UI: Player Inventory
         StringBuilder playerInventoryStringBuilder = new StringBuilder();
         foreach (string playerInventoryItemName in playerInventoryItemNames)
         {
            playerInventoryStringBuilder.Append(playerInventoryItemName).AppendLine();
         }
         _playerInventoryBodyText.text = playerInventoryStringBuilder.ToString();
      }
   }
}


