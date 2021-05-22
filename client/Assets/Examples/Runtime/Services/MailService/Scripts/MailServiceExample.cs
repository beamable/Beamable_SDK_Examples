using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Common.Api;
using Beamable.Common.Api.Mail;
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
      
      public string NextMailSubject { get { return $"Mail Subject #{UnreadMailLogs.Count}";}}
      public string NextMailBody { get { return $"Mail Body #{UnreadMailLogs.Count}";}}
      
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
      private MailServiceExampleData _data = new MailServiceExampleData();
      private const string MailCategory = "";

      //  Unity Methods  --------------------------------
      protected void Start()
      {
         Debug.Log($"Start()");

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

      
      public async void SendMailMessage()
      {
         // Log, Sending!
         _data.SendMailMessageLogs.Clear();
         string sendMailMessageLog = $"MailService.SendMail() Sending...";
         _data.SendMailMessageLogs.Add(sendMailMessageLog);
         Refresh();
         
         // Arbitrary Example - Send mail from ME to ME 
         var mailSendRequest = new MailSendRequest();
         var mailSendEntry = new MailSendEntry();
         mailSendEntry.body = _data.NextMailBody;
         mailSendEntry.subject = _data.NextMailSubject;
         mailSendEntry.category = MailCategory;
         mailSendEntry.senderGamerTag = _data.Dbid;
         mailSendEntry.receiverGamerTag = _data.Dbid;
         mailSendRequest.Add(mailSendEntry);

         // Call may fail if sender lacks permissions
         var isSuccess = true;
         try
         {
            var emptyResponse = await _beamableAPI.MailService.SendMail(mailSendRequest);
         }
         catch (Exception e)
         {
            Debug.LogError(e.Message);
            isSuccess = false;
         }
        
         // Log, Sent!
         _data.SendMailMessageLogs.Clear();
         sendMailMessageLog = $"MailService.SendMail() Sent. isSuccess={isSuccess}!";
         _data.SendMailMessageLogs.Add(sendMailMessageLog);
         Refresh();
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
         Debug.Log($"Refresh()");
         Debug.Log($"\tUnreadMailCount = {_data.UnreadMailCount}");
         Debug.Log($"\tUnreadMailLogs.Count = {_data.UnreadMailLogs.Count}");
         Debug.Log($"\tSendMailMessageLogs.Count = {_data.SendMailMessageLogs.Count}");
         Debug.Log($"\tMailMessageLogs.Count = {_data.MailMessageLogs.Count}");
         
         // Send relevant data to the UI for rendering
         OnRefreshed?.Invoke(_data);
      }
   }
}

