using System.Collections.Generic;
using Beamable.Common;
using Beamable.Common.Api.Events;
using UnityEngine;
using UnityEngine.Events;

namespace Beamable.Examples.Services.EventsService
{
   /// <summary>
   /// Holds data for use in the <see cref="EventsServiceExampleUI"/>.
   /// </summary>
   [System.Serializable]
   public class EventsServiceExampleData
   {
      public long Dbid = 0;
      public int Score = 0;
      public List<string> RunningEventsLogs = new List<string>();
      public List<string> SetScoreLogs = new List<string>();
      public List<string> ClaimLogs = new List<string>();
   }
   
   /// <summary>
   /// Dispatch event, observed by <see cref="EventsServiceExampleUI"/>.
   /// </summary>
   [System.Serializable]
   public class RefreshedUnityEvent : UnityEvent<EventsServiceExampleData> { }
   

   /// <summary>
   /// Demonstrates <see cref="EventsService"/>.
   /// </summary>
   public class EventsServiceExample : MonoBehaviour
   {
      //  Events  ---------------------------------------
      public RefreshedUnityEvent OnRefreshed = new RefreshedUnityEvent();
      
      //  Fields  ---------------------------------------
      private IBeamableAPI _beamableAPI;

      private EventsServiceExampleData _data = new EventsServiceExampleData();

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

         // Fetch All Events
         _beamableAPI.EventsService.Subscribe(eventsGetResponse =>
         {
            _data.RunningEventsLogs.Clear();
            int index = 0;
            foreach (EventView eventView in eventsGetResponse.running)
            {
               index++;
               string endTime = $"{eventView.endTime.ToShortDateString()} at " +
                                $"{eventView.endTime.ToShortTimeString()}";
               string currentPhase = eventView.currentPhase.name;
               string rulesCount = eventView.currentPhase.rules.Count.ToString();

               string log = $"Event #{index}" +
                            $"\n\tname = {eventView.name}" + 
                            $"\n\tendTime = {endTime}" +
                            $"\n\tcurrentPhase = {currentPhase}" +
                            $"\n\trulesCount = {rulesCount}";
               _data.RunningEventsLogs.Add(log);
            }
            Refresh();
         });

         Refresh();
      }

      public async void SetScoreInEvents()
      {
         _data.Score += 1;
         _data.SetScoreLogs.Clear();
         
         // SetScore() in **ALL** events.
         // Typical usage is to SetScore() in just one event.
         EventsGetResponse eventsGetResponse = await _beamableAPI.EventsService.GetCurrent();
         foreach (EventView eventView in eventsGetResponse.running)
         {
            Unit unit = await _beamableAPI.EventsService.SetScore(
               eventView.id, _data.Score, false, new Dictionary<string, object>());

            string log = $"SetScore()" +
                         $"\n\tname = {eventView.name}" +
                         $"\n\tscore = {_data.Score}";
            _data.SetScoreLogs.Add(log);
         }
      }
      
      public async void ClaimRewardsInEvents()
      {
         _data.ClaimLogs.Clear();
         
         // Claim() in **ALL** events.
         // Typical usage is to Claim() in just one event.
         EventsGetResponse eventsGetResponse = await _beamableAPI.EventsService.GetCurrent();
         foreach (EventView eventView in eventsGetResponse.running)
         {
            EventClaimResponse eventClaimResponse = await _beamableAPI.EventsService.Claim(eventView.id);

            string log = $"Claim()" +
                         $"\n\tname = {eventView.name}" +
                         $"\n\tscoreRewards = {eventClaimResponse.view.scoreRewards.Count}" +
                         $"\n\trankRewards = {eventClaimResponse.view.rankRewards.Count}";
            _data.ClaimLogs.Add(log);
         }
      }
      
      public void Refresh()
      {
         Debug.Log($"Refresh()");
         Debug.Log($"\trunningEventsLogs.Count = {_data.RunningEventsLogs.Count}");
         Debug.Log($"\tsetScoreLogs.Count = {_data.SetScoreLogs.Count}");
         Debug.Log($"\tclaimLog.Count = {_data.ClaimLogs.Count}");
         
         // Send relevant data to the UI for rendering
         OnRefreshed?.Invoke(_data);
         
      }
   }
}

