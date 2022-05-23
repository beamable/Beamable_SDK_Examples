using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Common;
using Beamable.Common.Api.Events;
using Beamable.Examples.Shared;
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
      public double Score = 0;
      public List<string> RunningEventsLogs = new List<string>();
      public List<string> SetScoreLogs = new List<string>();
      public List<string> ClaimLogs = new List<string>();

      public bool SetScoreButtonIsInteractable = true;
   }
   
   [System.Serializable]
   public class RefreshedUnityEvent : UnityEvent<EventsServiceExampleData> { }
   
   /// <summary>
   /// Demonstrates <see cref="EventsService"/>.
   /// </summary>
   public class EventsServiceExample : MonoBehaviour
   {
      //  Events  ---------------------------------------
      [HideInInspector]
      public RefreshedUnityEvent OnRefreshed = new RefreshedUnityEvent();
      
      
      //  Fields  ---------------------------------------
      private BeamContext _beamContext;
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
         _beamContext = BeamContext.Default;
         await _beamContext.OnReady;

         _data.Dbid = _beamContext.PlayerId;
         Debug.Log($"beamContext.PlayerId = {_data.Dbid}");

         // Fetch All Events
         _beamContext.Api.EventsService.Subscribe(eventsGetResponse =>
         {
            _data.RunningEventsLogs.Clear();
            int index = 0;
            foreach (EventView eventView in eventsGetResponse.running)
            {
               index++;
               string endTime = $"{eventView.endTime.ToShortDateString()} at " +
                                $"{eventView.endTime.ToShortTimeString()}";
               
               string totalPhaseCount = eventView.allPhases.Count.ToString();
               string totalRulesCount = eventView.currentPhase.rules.Count.ToString();
               string currentPhase = eventView.currentPhase.name;
               _data.Score = eventView.score;
               double groupScore = eventView.groupRewards.groupScore;
               
               string eventLog = $"Event #{index}\n" +
                     $"\n\tname = {eventView.name}" + 
                     $"\n\tendTime = {endTime}" +
                     $"\n\ttotalPhaseCount = {totalPhaseCount}" +
                     $"\n\ttotalRulesCount = {totalRulesCount}" + 
                                 
                     $"\n\n  (Standard Events)" +
                     $"\n\tcurrentPhase = {currentPhase}" +
                     $"\n\tscore = {_data.Score}" +

                     $"\n\n  (Group Events)" +
                     $"\n\tgroupScore = {groupScore}";
               
               _data.RunningEventsLogs.Add(eventLog);
               _data.SetScoreButtonIsInteractable = true;
            }
            Refresh();
         });

         Refresh();
      }

      
      public async void SetScoreInEvents()
      {
         _data.SetScoreButtonIsInteractable = false;
         _data.Score += 1;
         
         // SetScore() in **ALL** events.
         // Typical usage is to SetScore() in just one event.
         EventsGetResponse eventsGetResponse = await _beamContext.Api.EventsService.GetCurrent();
         foreach (EventView eventView in eventsGetResponse.running)
         {
            Unit unit = await _beamContext.Api.EventsService.SetScore(
               eventView.id, _data.Score, false, new Dictionary<string, object>());

            string score = $"SetScore()" +
                         $"\n\tname = {eventView.name}" +
                         $"\n\tscore = {_data.Score}";
            _data.SetScoreLogs.Clear();
            _data.SetScoreLogs.Add(score);
         }
         
         Refresh();
         
         // HACK: Force refresh here (0.10.1)
         // wait (arbitrary milliseconds) for refresh to complete 
         _beamContext.Api.EventsService.Subscribable.ForceRefresh();
         await Task.Delay(300); 
         
         Refresh();
      }
      
      
      public async void ClaimRewardsInEvents()
      {
         _data.ClaimLogs.Clear();
         
         // Claim() in **ALL** events.
         // Typical usage is to Claim() in just one event.
         EventsGetResponse eventsGetResponse = await _beamContext.Api.EventsService.GetCurrent();
         foreach (EventView eventView in eventsGetResponse.running)
         {
            // STANDARD EVENTS
            // The systems supports scoreRewards (redeemable at any time)
            // and rankRewards (redeemable only at end of phase)
            // For this example, we'll honor only scoreRewards
            bool hasClaimableScoreReward = false;
            foreach (var eventReward in eventView.scoreRewards)
            {
               if (eventReward.earned && !eventReward.claimed)
               {
                  Debug.Log($"ClaimableScore. min = {eventReward.min}, " +
                            $"max = {eventReward.max}");
                  
                  hasClaimableScoreReward = true;
               }
            }
            
            // GROUP EVENTS
            // The systems supports scoreRewards (redeemable at any time)
            // and rankRewards (redeemable only at end of phase)
            // For this example, we'll honor only scoreRewards
            bool hasClaimableGroupScoreReward = false;
            if (eventView.groupRewards != null && eventView.groupRewards.scoreRewards != null)
            {
               foreach (var eventReward in eventView.groupRewards.scoreRewards)
               {
                  if (eventReward.earned && !eventReward.claimed)
                  {
                     Debug.Log($"ClaimableGroupScore. min = {eventReward.min}, " +
                               $"max = {eventReward.max}");
                  
                     hasClaimableGroupScoreReward = true;
                  }
               }
            }
            
            // Get value, or default
            double? groupScore = eventView?.groupRewards?.groupScore;
            groupScore = groupScore.HasValue ? groupScore.Value: 0;
            
            bool canClaim = hasClaimableScoreReward || hasClaimableGroupScoreReward;
            string claim = "";
            if (canClaim)
            {
               // Claim() fails if there is nothing to be claimed
               try
               {
                  EventClaimResponse eventClaimResponse = await _beamContext.Api.EventsService.Claim(eventView.id);
                  
                  // Get value, or default
                  int? groupScoreRewardsCount = eventClaimResponse.view.groupRewards?.scoreRewards?.Count;
                  groupScoreRewardsCount = groupScoreRewardsCount.HasValue ? groupScoreRewardsCount.Value: 0;

                  claim += $"Claim() Success\n" +
                           $"\n\tname = {eventView.name}" +
                           $"\n\tcanClaim= {canClaim}" +
                           $"\n\thasClaimableScoreReward = {hasClaimableScoreReward}" +
                           
                           $"\n\n  (Standard Events)" +
                           $"\n\trankRewards = {eventClaimResponse.view.rankRewards.Count}" +
                           $"\n\tscoreRewards = {eventClaimResponse.view.scoreRewards.Count}" +
                           
                           $"\n\n  (Group Events)" +
                           $"\n\tgroupScoreRewardsCount = {groupScoreRewardsCount}";
               }
               catch (Exception e)
               {
                  claim += $"Claim() Failed" +
                           $"\n\tname = {eventView.name}" +
                           $"\n\tcanClaim= {canClaim}" +
                           $"\n\thasClaimableScoreReward = {hasClaimableScoreReward}" +
                           $"\n\terror = {e.Message}";
               }
            }
            else
            {
               claim += $"Claim() not called." +
                        $"\n\tname = {eventView.name}" +
                        $"\n\tcanClaim= {canClaim}" +
                        $"\n\thasClaimableScoreReward = {hasClaimableScoreReward}";
            }
            

            _data.ClaimLogs.Add(claim);
     
         }
         Refresh();
      }
      
      
      public void Refresh()
      {
         string refreshLog = $"Refresh() ...\n" +
                             $"\n * RunningEventsLogs.Count = {_data.RunningEventsLogs.Count}" +
                             $"\n * SetScoreLogs.Count = {_data.SetScoreLogs.Count}" +
                             $"\n * ClaimLog.Count = {_data.ClaimLogs.Count}\n\n";
         //Debug.Log(refreshLog);
         
         // Send relevant data to the UI for rendering
         OnRefreshed?.Invoke(_data);
      }
   }
}

