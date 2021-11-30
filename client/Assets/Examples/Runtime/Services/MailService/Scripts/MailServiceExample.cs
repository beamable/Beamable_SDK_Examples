using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Common.Api;
using Beamable.Common.Api.Mail;
using Beamable.Examples.Shared;
using UnityEngine;
using UnityEngine.Events;

namespace Beamable.Examples.Services.MailService
{
   /// <summary>
   /// Holds data for use in the <see cref="MailServiceExampleUI"/>.
   /// </summary>
   [System.Serializable]
   public class MailServiceExampleData
   {
      public long Dbid = 0;
      public int UnreadMailCount = 0;
      
      // UI messages that indicate mail exists or not
      public List<string> UnreadMailLogs = new List<string>();
      public List<string> UpdateMailLogs = new List<string>();
      public List<string> SendMailMessageLogs = new List<string>();
      public List<string> MailMessageLogs = new List<string>();
      
   }
   
   [System.Serializable]
   public class RefreshedUnityEvent : UnityEvent<MailServiceExampleData> { }
   
   /// <summary>
   /// Demonstrates <see cref="MailService"/>.
   /// </summary>
   public class MailServiceExample : MonoBehaviour
   {
      //  Events  ---------------------------------------
      [HideInInspector]
      public RefreshedUnityEvent OnRefreshed = new RefreshedUnityEvent();
      
      
      //  Fields  ---------------------------------------
      private IBeamableAPI _beamableAPI;
      private const string MailCategory = "";
      private MailServiceExampleData _data = new MailServiceExampleData();

      
      //  Unity Methods  --------------------------------
      protected void Start()
      {
         string startLog = $"Start() Instructions..\n" +
                           $"\n * Play Scene" +
                           $"\n * Check for mail using UI. Probably none" +
                           $"\n * Stop Scene" +
                           $"\n * Unity → Window → Beamable → Examples → MailService → Send Test Mail To Active User" +
                           $"\n * Play Scene" +
                           $"\n * Check for mail using UI. Probably some\n\n";
         
         Debug.Log(startLog);

         Refresh();
         SetupBeamable();
      }

      
      //  Methods  --------------------------------------
      private async void SetupBeamable()
      { 
         _beamableAPI = await Beamable.API.Instance;

         _data.Dbid = _beamableAPI.User.id;
         Debug.Log($"beamableAPI.User.id = {_data.Dbid}");
         
         // Fetch All Mail
         _beamableAPI.MailService.Subscribe(async mailQueryResponse =>
         {
            _data.UnreadMailLogs.Clear();
            _data.UnreadMailCount = mailQueryResponse.unreadCount;
            string unreadMailLog = $"unreadCount = {_data.UnreadMailCount}";
            _data.UnreadMailLogs.Add(unreadMailLog);

            await GetMail();
            Refresh();
         });

         Refresh();
      }

      
      private async Task<EmptyResponse> GetMail()
      {
         _data.MailMessageLogs.Clear();
         var listMailResponse = await _beamableAPI.MailService.GetMail(MailCategory);
         foreach (var mailMessage in listMailResponse.result)
         {
            string mailMessageLog = $"MailMessage" +
                                    $"\n\tname = {mailMessage.senderGamerTag}" +
                                    $"\n\tname = {mailMessage.receiverGamerTag}" +
                                    $"\n\tname = {mailMessage.subject}" +
                                    $"\n\tname = {mailMessage.body}" +
                                    $"\n";
            _data.MailMessageLogs.Add(mailMessageLog);
         }
         
         Refresh();

         return new EmptyResponse();
      }

      
      public async void UpdateMail()
      {
         _data.UpdateMailLogs.Clear();
         var mailUpdateRequest = new MailUpdateRequest();
         
         // Arbitrary Example - Toggle "read" to "unread"
         var listMailResponse = await _beamableAPI.MailService.GetMail(MailCategory);
         foreach (var mailMessage in listMailResponse.result)
         {
            MailState newMailState = MailState.Read;
            if (mailMessage.MailState == MailState.Read)
            {
               newMailState = MailState.Unread;
            }
            mailUpdateRequest.Add(mailMessage.id, newMailState, true, mailMessage.expires);
         }
    
         await _beamableAPI.MailService.Update(mailUpdateRequest);
         
         string updateMailLog = $"updateMailRequests = {mailUpdateRequest.updateMailRequests.Count}";
         _data.UpdateMailLogs.Add(updateMailLog);
         
         Refresh();
      }
      
      
      public void Refresh()
      {
         string refreshLog = $"Refresh() ...\n" +
                             $"\n * UnreadMailCount.Count = {_data.UnreadMailCount}" +
                             $"\n * UnreadMailLogs.Count = {_data.UnreadMailLogs.Count}" +
                             $"\n * MainLogs.Count = {_data.SendMailMessageLogs.Count}" +
                             $"\n * MatchmakingLogs.Count = {_data.MailMessageLogs.Count}\n\n";
         
         //Debug.Log(refreshLog);
         
         // Send relevant data to the UI for rendering
         OnRefreshed?.Invoke(_data);
      }
      
      
      /// <summary>
      /// NOTE: This must be 1. called at runtime from a user with
      /// admin privileges or 2. called at edit-time from any user
      ///
      /// For this demo, #2 is used.
      /// 
      /// </summary>
      public static async void SendMailMessage()
      {
         if (Application.isPlaying)
         {
            Debug.Log ($"SendMailMessage() Failed. Must call at edit-time.");
            return;
         }
         
         IBeamableAPI beamableAPIFromStatic = await Beamable.API.Instance;
         long userId = beamableAPIFromStatic.User.id;
      
         // Arbitrary Example - Send mail from ME to ME 
         var mailSendRequest = new MailSendRequest();
         var mailSendEntry = new MailSendEntry();
         mailSendEntry.category = MailCategory;
         mailSendEntry.senderGamerTag = userId;
         mailSendEntry.receiverGamerTag = userId;
         mailSendEntry.body = $"Test Mail Body From {userId}.";
         mailSendEntry.subject = $"Test Mail Subject From {userId}.";
         mailSendRequest.Add(mailSendEntry);

         // Call may fail if sender lacks permissions
         bool isSuccess = true;
         try
         {
            Debug.Log($"Edit-Time, beamableAPI.User.id = {userId}");
            
            var emptyResponse = await beamableAPIFromStatic.MailService.SendMail(mailSendRequest);
         }
         catch (Exception e)
         {
            Debug.LogError(e.Message + "\n\n");
            Debug.LogWarning($"Solution To Error: Add the beamableAPI.User.id of {userId} with the role of 'Admin' via Portal → Teams and retry this operation.\n\n");
            isSuccess = false;
         }

         if (isSuccess)
         {
            Debug.Log ($"SendMailMessage() Success!");
         }
      }
   }
}

