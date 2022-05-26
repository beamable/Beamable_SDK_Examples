using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Common.Api;
using Beamable.Common.Api.Auth;
using UnityEngine;
using UnityEngine.Events;

namespace Beamable.Examples.RestAPI.RestAPIAccountsExample
{
   /// <summary>
   /// Holds data for use in the <see cref="RestAPIAccountsExampleUI"/>.
   /// </summary>
   [System.Serializable]
   public class RestAPIAccountsExampleData
   {
      public List<string> MainTexts = new List<string>();
      public List<string> DetailTexts = new List<string>();
      public bool IsBeamableSetup = false;
   }
   
   /// <summary>
   /// Some Beamable RestAPI calls are sent via post and also require
   /// data to be sent. Here is a working example of a request object
   /// </summary>
   [Serializable]
   public class RegisterDBCredentialsRequest
   {
      public string email, password;
   }
   
   [System.Serializable]
   public class RestAPIAccountsExampleEvent : UnityEvent<RestAPIAccountsExampleData> { }

   /// <summary>
   /// Demonstrates calling the Beamable RestAPI directly.
   /// 
   /// </summary>
   public class RestAPIAccountsExample : MonoBehaviour
   {
      //  Events  ---------------------------------------
      [HideInInspector]
      public RestAPIAccountsExampleEvent OnRefreshed = new RestAPIAccountsExampleEvent();
      
      
      //  Fields  ---------------------------------------
      private BeamContext _beamContext;
      private RestAPIAccountsExampleData _restAPIAccountsExampleData = new RestAPIAccountsExampleData();

      
      //  Unity Methods  --------------------------------
      protected void Start()
      {
         Debug.Log($"Start()");

         SetupBeamable();
      }

      
      //  Methods  --------------------------------------
      private async void SetupBeamable()
      { 
         _beamContext = BeamContext.Default;
         await _beamContext.OnReady;
            
         Debug.Log($"beamContext.PlayerId = {_beamContext.PlayerId}");

         _restAPIAccountsExampleData.IsBeamableSetup = true;
         _restAPIAccountsExampleData.DetailTexts.Clear();
         
         Refresh();
      }

      public async Task Call_BasicAccountsMe()
      {
         // Documentation: https://docs.beamable.com/reference/get_basic-accounts-me
         User userPromise = await _beamContext.Requester.Request<User>(Method.GET, "/basic/accounts/me");
         
         string detail = $"Request User. Response: Id = {userPromise.id}, Email = {userPromise.email}";
         
         _restAPIAccountsExampleData.DetailTexts.Add(detail);
         Refresh();
      }
      
      public async Task Call_BasicAccountsRegister()
      {
         // Documentation: https://docs.beamable.com/reference/post_basic-accounts-register
         var req = new RegisterDBCredentialsRequest 
         {
            email = "test@test123.com",
            password = "testpassword"
         };

         User userPromise = null;
         string detail = "";
         try
         {
            userPromise = await _beamContext.Requester.Request<User>(Method.POST, $"/basic/accounts/register", req);
            detail = $"Register User. Response: Id = {userPromise.id}, Email = {userPromise.email}";
         }
         catch (Exception exception)
         {
            if (exception.Message.Contains("EmailAlreadyRegisteredError"))
            {
               detail = $"Register User. Response: That account already has an email. No problems.";
            }
            else
            {
               detail = $"Register User. Response: error = {exception.Message}";
            }
         }
         
         _restAPIAccountsExampleData.DetailTexts.Add(detail);
         Refresh();
      }
      

      
      public void Refresh()
      {
         if (_restAPIAccountsExampleData.IsBeamableSetup)
         {
            _restAPIAccountsExampleData.MainTexts.Clear();

            string mainText = "Click UI Buttons. See UI Results.";
            _restAPIAccountsExampleData.MainTexts.Add(mainText);
         }

         OnRefreshed?.Invoke(_restAPIAccountsExampleData);
      }

      //  Event Handlers  -------------------------------
   }
}

