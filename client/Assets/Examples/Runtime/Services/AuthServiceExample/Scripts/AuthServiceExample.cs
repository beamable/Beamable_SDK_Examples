using System;
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

         // Randomize for demo usage
         Email = Random.Range(10000, 99999) +  email; 
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
      
      private IBeamableAPI _beamableAPI;
      private AuthServiceExampleData _authServiceExampleData = new AuthServiceExampleData();
      private MockUser _mockUser01 = null;
      private MockUser _mockUser02 = null;
      private string _lastRegisteredEmail = "";

      //  Unity Methods  --------------------------------
      protected void Start()
      {
         Debug.Log($"Start()");

         // Create 2 new mock users for demo purposes
         _mockUser01 = new MockUser ("Alias01", "blah@blah01.com", "myPasswordFoo");
         _mockUser02 = new MockUser ("Alias02", "blah@blah02.com", "myPasswordBar");
      
         SetupBeamable();
      }

      //  Methods  --------------------------------------
      private async void SetupBeamable()
      { 
         _beamableAPI = await Beamable.API.Instance;
            
         Debug.Log($"beamableAPI.User.id = {_beamableAPI.User.id}");
         
         // Create 2 new users from mock users for demo purposes,
         // Then later from UI: switch and update the active user
         _authServiceExampleData.DetailTexts.Clear();
         await CreateUser(_mockUser01.Alias);
         await UpdateCurrentUser();
         
         await CreateUser(_mockUser02.Alias);
         await UpdateCurrentUser();
         
         _authServiceExampleData.IsBeamableSetup = _beamableAPI != null;
         
         Refresh();

      }
      
      private async Task<EmptyResponse> CreateUser(string alias)
      {
         var tokenResponse = await _beamableAPI.AuthService.CreateUser();
         await _beamableAPI.ApplyToken(tokenResponse); 
         
         await _beamableAPI.StatsService.SetStats("public", new Dictionary<string, string>()
         {
            { "alias", alias },
         });

         string detailText = $"CreateUser() Alias = {alias}";
         _authServiceExampleData.DetailTexts.Add(detailText);

         return new EmptyResponse();
      }

      public void Refresh()
      {
         Debug.Log($"Refresh()");

         if (_authServiceExampleData.IsBeamableSetup)
         {
            _authServiceExampleData.MainTexts.Clear();
            string mainText = $"Active User.Id = {_beamableAPI.User.id}, User.Email = {_beamableAPI.User.email}";
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
            var user = await _beamableAPI.AuthService.RegisterDBCredentials(nextMockUser.Email, 
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
         string detailText = $"UpdateCurrentUser() User.id = {_beamableAPI.User.id}, nextMockUser.Email = {nextMockUser.Email}, isSuccess = {isSuccess}";
         Debug.Log(detailText);
         _authServiceExampleData.DetailTexts.Add(detailText);
         
         if (isSuccess)
         {
            _lastRegisteredEmail = nextMockUser.Email;
         }
         else
         {
            string warningText = $"\nThat update failed because the email is already registered. That is ok.";
            Debug.LogWarning(warningText);
            _authServiceExampleData.DetailTexts.Add(warningText);
         }
         

         Refresh();

         return new EmptyResponse();
      }

      public async Task<EmptyResponse> SwitchCurrentUser()
      {
         // Choose the OTHER mock user
         MockUser nextMockUser = _mockUser02;
         if (_beamableAPI.User.email != _mockUser01.Email)
         {
            nextMockUser = _mockUser01;
         }
         
         Debug.Log($"SwitchCurrentUser() From Email = {_beamableAPI.User.email}");

         bool isSuccess = false;
         string error = "";
         TokenResponse tokenResponse = null;
         try
         {
            bool mergeGamerTagToAccount = false;
            tokenResponse = await _beamableAPI.AuthService.Login(nextMockUser.Email, 
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
         string detailText = $"SwitchCurrentUser() To Email = {nextMockUser.Email}, isSuccess = {isSuccess}";
         if (!isSuccess)
         {
            detailText += $", Error = {error}";
         }
         else
         {
            // This method actually changes the active Beamable user
            await _beamableAPI.ApplyToken(tokenResponse); 
         }
         
         Debug.Log(detailText);
         _authServiceExampleData.DetailTexts.Add(detailText);
         Refresh();

         return new EmptyResponse();
      }
   }
}

