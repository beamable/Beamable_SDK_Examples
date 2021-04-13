/* Uncomment Code After Completing Steps: https://docs.beamable.com/docs/adding-facebook-sign-in
 
using System.Collections.Generic;
using Beamable.AccountManagement;
using Beamable.Common.Api.Auth;
using UnityEngine;
using Facebook.Unity;

namespace Beamable.Examples.Integrations.Facebook
{
    public class FacebookWrapper : MonoBehaviour
    {
        //  Fields  --------------------------------
        private AccountManagementSignals _signals = null;
        private bool _hasAttachedListener = false;
    
        //  Unity Methods  --------------------------------
        protected void Start()
        {
            Debug.Log($"Start()");
    
            SetupBeamable();
        }
      
        protected void Update()
        {
            if (!_hasAttachedListener && _signals.ThirdPartyLoginAttempted != null)
            {
                _signals.ThirdPartyLoginAttempted.AddListener(StartFacebookLogin);
                _hasAttachedListener = true;
            }
        }
        
        //  Methods  --------------------------------------
        private void SetupBeamable ()
        {
            _signals = gameObject.AddComponent<AccountManagementSignals>();
    
            if (!FB.IsInitialized) {
                // Initialize the Facebook SDK
                FB.Init(FB_InitCallback, FB_OnHideUnity);
            } else {
                // Already initialized, signal an app activation App Event
                FB.ActivateApp();
            }
        }
      
        public void StartFacebookLogin(ThirdPartyLoginPromise promise)
        {
            if (promise.ThirdParty != AuthThirdParty.Facebook) return;
            var perms = new List<string>(){"public_profile", "email"};
            FB.LogInWithReadPermissions(perms, result => FB_AuthCallback(promise, result));
        }

        
        //  Event Handlers  -------------------------------
       private void FB_InitCallback ()
       {
           if (FB.IsInitialized) {
               // Signal an app activation App Event
               FB.ActivateApp();
               Debug.Log("Facebook SDK Initialized. Success.");
           } else {
               Debug.Log("Facebook SDK Not Initialized. Failure.");
           }
       }
       
       private void FB_OnHideUnity (bool isGameShown)
       {
           if (!isGameShown) {
               // Pause the game - we will need to hide
               Time.timeScale = 0;
           } else {
               // Resume the game - we're getting focus again
               Time.timeScale = 1;
           }
       }
       
       private void FB_AuthCallback (ThirdPartyLoginPromise promise, ILoginResult result) 
       {
           if (!string.IsNullOrEmpty(result.Error))
           {
               promise.CompleteError(new ErrorCode(0, GameSystem.GAME_CLIENT, "Facebook Error", "Add details..."));
               return;
           }
           if (FB.IsLoggedIn) {
               // AccessToken class will have session details
               var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
               // Print current access token's User ID
               Debug.Log(aToken.UserId);
               // Print current access token's granted permissions
               foreach (string perm in aToken.Permissions) {
                   Debug.Log(perm);
               }
               promise.CompleteSuccess(new ThirdPartyLoginResponse()
               {
                   AuthToken = aToken.TokenString
               });
           } else {
               Debug.Log("User cancelled login");
               promise.CompleteSuccess(ThirdPartyLoginResponse.CANCELLED);
           }
       }
    }
}
*/
