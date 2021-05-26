using Beamable.Examples.Services.MailService;
using UnityEditor;

namespace Beamable.Examples.Shared
{ 
    /// <summary>
    /// Consolidate all Example-related Unity menu items
    /// </summary>
    public static class SharedMenuItems 
    {
        //  Fields  ---------------------------------------
        private const int MENU_ITEM_PRIORITY_EXAMPLES = BeamableConstants.MENU_ITEM_PATH_WINDOW_PRIORITY_3;
        private const string MENU_ITEM_PATH_EXAMPLES =  BeamableConstants.MENU_ITEM_PATH_WINDOW_BEAMABLE + "/Examples";
        private const string MENU_ITEM_PATH_SEND_TEST_MAIL = MENU_ITEM_PATH_EXAMPLES + "/MailService/Send Test Mail To Active User";
        
        //  Methods  --------------------------------------
        
        [MenuItem(MENU_ITEM_PATH_SEND_TEST_MAIL, priority = MENU_ITEM_PRIORITY_EXAMPLES)]
        public static void SendTestMailToActiveUser()
        {
            MailServiceExample.SendMailMessage();
        }
    }
}