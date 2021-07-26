using Beamable.Api.Payments;
using Beamable.Common.Inventory;

namespace Beamable.Examples.Services.CommerceService
{
    /// <summary>
    /// Renders one item row of the store.
    /// </summary>
    public class ItemData
    {
        //  Properties -----------------------------------
        public string Title { get { return _title; } }
        public PlayerListingView PlayerListingView { get { return _playerListingView; } }
        public ItemContent ItemContent { get { return _itemContent; } }

        //  Fields ---------------------------------------
        private ItemContent _itemContent = null;
        private string _title = null;
        private PlayerListingView _playerListingView = null;
        
        //  Constructor ---------------------------------
        public ItemData (string title, ItemContent itemContent, PlayerListingView playerListingView) 
        {
            _title = title;
            _itemContent = itemContent;
            
            //This is only for STORE items not for INVENTORY items
            _playerListingView = playerListingView;
            
        }
    }
}