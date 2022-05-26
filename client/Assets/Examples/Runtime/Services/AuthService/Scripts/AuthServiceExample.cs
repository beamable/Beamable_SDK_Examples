using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Api;
using Beamable.Common.Api;
using Beamable.Common.Api.Auth;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Beamable.Examples.Services.AuthService
{
   /// <summary>
   /// Holds data for each mock user to toggle between for demo
   /// </summary>
   public class MockUser
   {
      public string Alias = "";
      public string Email = "";
      public string Password = "";

      public MockUser(string alias, string email, string password)
      {
         Alias = alias;
         Password = password;
         Email = email;
      }

      // Randomize for demo usage
      public static string GetMockEmail(string index)
      {
         return "blah@blah" + Random.Range(10000, 99999) + "_" + index + ".com";
      }
   }
   
   
   /// <summary>
   /// Holds data for use in the <see cref="AuthServiceExampleUI"/>.
   /// </summary>
   [System.Serializable]
   public class AuthServiceExampleData
   {
      public List<string> MainTexts = new List<string>();
      public List<string> DetailTexts = new List<string>();
      public bool IsBeamableSetup = false;
   }
   
   
   [System.Serializable]
   public class AuthServiceExampleExampleEvent : UnityEvent<AuthServiceExampleData> { }

   /// <summary>
   /// Demonstrates <see cref="AuthService"/>.
   ///
   /// NOTE: The notion of "Signing in" to an account can be misleading, because
   /// the game always ensures there is a player token loaded, even it is an anonymous
   /// user with no credentials.
   /// 
   /// </summary>
   public class AuthServiceExample : MonoBehaviour
   {
      //  Events  ---------------------------------------
      [HideInInspector]
      public AuthServiceExampleExampleEvent OnRefreshed = new AuthServiceExampleExampleEvent();
      
      
      //  Fields  ---------------------------------------
      private BeamContext _context;
      private AuthServiceExampleData _authServiceExampleData = new AuthServiceExampleData();
      private MockUser _mockUser01 = null;
      private MockUser _mockUser02 = null;
      private string _lastRegisteredEmail = "";

      
      //  Unity Methods  --------------------------------
      protected void Start()
      {
         Debug.Log($"Start()");

         // Create 2 new mock users for demo purposes
         string mockEmail01 = MockUser.GetMockEmail("01");
         string mockEmail02 = MockUser.GetMockEmail("02");
         _mockUser01 = new MockUser ("Alias01", mockEmail01, "myPasswordFoo");
         _mockUser02 = new MockUser ("Alias02", mockEmail02, "myPasswordBar");
      
         SetupBeamable();
      }

      
      protected void OnDestroy()
      {
         _context = BeamContext.Default;
         
         // Unsubscribe to events
         _context.Api.OnUserChanged -= BeamableAPI_OnUserChanged;
         _context.OnUserLoggingOut -= BeamableAPI_OnUserLoggingOut;

         _context.ClearPlayerAndStop();
      }

      
      //  Methods  --------------------------------------
      private async void SetupBeamable()
      { 
         _context = BeamContext.Default;
         await _context.OnReady;

         Debug.Log($"context.PlayerId = {_context.PlayerId}");
         
         // Create 2 new users from mock users for demo purposes,
         // Then later from UI: switch and update the active user
         _authServiceExampleData.DetailTexts.Clear();
 
         await CreateUser(_mockUser01.Alias);
         await UpdateCurrentUser();
         
         await CreateUser(_mockUser02.Alias);
         await UpdateCurrentUser();
         
         _authServiceExampleData.IsBeamableSetup = _context != null;
         
         // Subscribe to events
         _context.Api.OnUserChanged += BeamableAPI_OnUserChanged;
         _context.OnUserLoggingOut += BeamableAPI_OnUserLoggingOut;
         
         Refresh();
      }
      
      
      private async Task<EmptyResponse> CreateUser(string alias)
      {
         var tokenResponse = await _context.Api.AuthService.CreateUser();
         await _context.Api.ApplyToken(tokenResponse); 
         
         await _context.Api.StatsService.SetStats("public", new Dictionary<string, string>()
         {
            { "alias", alias },
         });

         string detailText = $"CreateUser() Alias = {alias}";
         _authServiceExampleData.DetailTexts.Add(detailText);

         return new EmptyResponse();
      }

      
      public void Refresh()
      {
         //Debug.Log($"Refresh()");

         if (_authServiceExampleData.IsBeamableSetup)
         {
            _authServiceExampleData.MainTexts.Clear();
            string mainText = $"Active User.Id = {_context.PlayerId}, User.Email = {_context.Api.User.email}";
            _authServiceExampleData.MainTexts.Add(mainText);
         }

         OnRefreshed?.Invoke(_authServiceExampleData);
      }

      
      /// <summary>
      /// Attach an email/password to active user
      /// </summary>
      public async Task<EmptyResponse> UpdateCurrentUser()
      {
         // Choose the OTHER mock user
         MockUser nextMockUser = _mockUser02;
         if (_lastRegisteredEmail != _mockUser01.Email)
         {
            nextMockUser = _mockUser01;
         }

         bool isSuccess = false;
         string error = "";
         try
         {
            // This method actually changes the active Beamable user
            var user = await _context.Api.AuthService.RegisterDBCredentials(nextMockUser.Email, 
               nextMockUser.Password);
            
            isSuccess = true;
         }
         catch (PlatformRequesterException e) 
         {
            // Errors if: The email has either already been taken by another account, or
            // The current account already has an email.
            error = e.Message;
         }
         
         // Update UI
         _authServiceExampleData.DetailTexts.Clear();
         string detailText = $"UpdateCurrentUser() User.id = {_context.PlayerId}, nextMockUser.Email = {nextMockUser.Email}, isSuccess = {isSuccess}";
         Debug.Log(detailText);
         _authServiceExampleData.DetailTexts.Add(detailText);
         
         if (isSuccess)
         {
            _lastRegisteredEmail = nextMockUser.Email;
         }
         else
         {
            string warningText = $"\nThat email was already registered. That is ok.";
            _authServiceExampleData.DetailTexts.Add(warningText);
            //Debug.Log(warningText);
         }
         
         Refresh();

         return new EmptyResponse();
      }

      
      public async Task<EmptyResponse> SwitchCurrentUser()
      {
         // Choose the OTHER mock user
         MockUser nextMockUser = _mockUser02;
         if (_context.Api.User.email != _mockUser01.Email)
         {
            nextMockUser = _mockUser01;
         }
         
         Debug.Log($"SwitchCurrentUser() From User.id = {_context.PlayerId}");

         bool isSuccess = false;
         string error = "";
         TokenResponse tokenResponse = null;
         try
         {
            bool mergeGamerTagToAccount = false;
            tokenResponse = await _context.Api.AuthService.Login(nextMockUser.Email, 
               nextMockUser.Password, mergeGamerTagToAccount);
            
            isSuccess = true;

         }
         catch (PlatformRequesterException e) 
         {
            // Errors if: No account with that email exists yet
            error = e.Message;
         }
         
         // Update UI
         _authServiceExampleData.DetailTexts.Clear();
         string detailText = $"SwitchCurrentUser() To User.Email = {nextMockUser.Email}, isSuccess = {isSuccess}";
         if (!isSuccess)
         {
            detailText += $", Error = {error}";
         }
         else
         {
            // This method actually changes the active Beamable user
            await _context.Api.ApplyToken(tokenResponse); 
         }
         
         Debug.Log(detailText);
         _authServiceExampleData.DetailTexts.Add(detailText);
         Refresh();

         return new EmptyResponse();
      }
      
      
      //  Event Handlers  -------------------------------
      private void BeamableAPI_OnUserLoggingOut(User user)
      {
         Debug.Log($"OnUserLoggingOut() User.id = {user.id}");
      }

      
      private void BeamableAPI_OnUserChanged(User user)
      {
         Debug.Log($"OnUserChanged() User.id = {user.id}");
      }
   }
}

