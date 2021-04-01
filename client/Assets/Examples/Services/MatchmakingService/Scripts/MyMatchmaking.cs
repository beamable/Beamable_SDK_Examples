using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Beamable.Common.Content;
using UnityEngine;

namespace Beamable.Examples.Services.MatchmakingService
{
   [Serializable]
   public class SimGameTypeRef : ContentRef<SimGameType> { }

   /// <summary>
   /// Contains the in-progress matchmaking data.
   /// When the process is complete, this contains
   /// the players list and the RoomId
   /// </summary>
   [Serializable]
   public class MyMatchmakingResult
   {
      //  Properties  -------------------------------------
      public bool IsComplete { get { return !string.IsNullOrEmpty(RoomId); } }
      public long LocalPlayerDbid { get { return _localPlayerDbid; } }
      public int TargetPlayerCount { get { return _targetPlayerCount; } }

      //  Fields  -----------------------------------------
      public string RoomId;
      public int SecondsRemaining;
      public List<long> Players = new List<long>();
      public string ErrorMessage = "";
      private long _localPlayerDbid;
      private int _targetPlayerCount;

      //  Constructor  ------------------------------------
      public MyMatchmakingResult(long localPlayerDbid, int targetPlayerCount)
      {
         _localPlayerDbid = localPlayerDbid;
         _targetPlayerCount = targetPlayerCount;
      }


      //  Other Methods  ----------------------------------
      public override string ToString()
      {
         return $"[MyMatchmakingResult (" +
            $"RoomId = {RoomId}, " +
            $"TargetPlayerCount = {TargetPlayerCount}, " +
            $"players.Count = {Players.Count})]";
      }
   }

   /// <summary>
   /// This example is for reference only. Use as
   /// inspiration for usage in production.
   /// </summary>
   public class MyMatchmaking
   {
      //  Events  -----------------------------------------
      public event Action<MyMatchmakingResult> OnProgress;
      public event Action<MyMatchmakingResult> OnComplete;

      //  Properties  -------------------------------------
      public MyMatchmakingResult MyMatchmakingResult { get { return _myMatchmakingResult; } }

      //  Fields  -----------------------------------------
      public static bool IsDebugLogging = false;
      public const string DefaultRoomId = "DefaultRoom";
      public const int Delay = 1000;

      private MyMatchmakingResult _myMatchmakingResult;
      private Experimental.Api.Matchmaking.MatchmakingService _matchmakingService;
      private SimGameType _simGameType;
      private CancellationTokenSource _matchmakingOngoing;

      //  Constructor  ------------------------------------
      public MyMatchmaking(Experimental.Api.Matchmaking.MatchmakingService matchmakingService,
         SimGameType simGameType, long localPlayerDbid)
      {
         _matchmakingService = matchmakingService;
         _simGameType = simGameType;

         _myMatchmakingResult = new MyMatchmakingResult(localPlayerDbid, _simGameType.maxPlayers);
      }

      //  Other Methods  ----------------------------------
      public async Task Start()
      {
         _myMatchmakingResult.RoomId = "";
         _myMatchmakingResult.SecondsRemaining = 0;

         DebugLog($"MyMatchmaking.Start() TargetPlayerCount = {_simGameType.maxPlayers}");
         var handle = await _matchmakingService.StartMatchmaking(_simGameType.Id);

         try
         {
            _matchmakingOngoing = new CancellationTokenSource();
            var token = _matchmakingOngoing.Token;
            while (!handle.Status.GameStarted)
            {
               if (token.IsCancellationRequested) return;

               _myMatchmakingResult.Players = handle.Status.Players;
               _myMatchmakingResult.SecondsRemaining = handle.Status.SecondsRemaining;
               _myMatchmakingResult.RoomId = handle.Status.GameId;
               OnProgress?.Invoke(_myMatchmakingResult);
               await Task.Delay(1000, token);
            }
         }
         finally
         {
            _matchmakingOngoing.Dispose();
            _matchmakingOngoing = null;
         }

         // Invoke Progress #2
         OnProgress?.Invoke(_myMatchmakingResult);

         // Invoke Complete
         OnComplete?.Invoke(_myMatchmakingResult);
      }

      public async void Stop()
      {
         await _matchmakingService.CancelMatchmaking(_simGameType.Id);
         _matchmakingOngoing?.Cancel();
      }

      private static void DebugLog(string message)
      {
         if (IsDebugLogging)
         {
            Debug.Log(message);
         }
      }
   }
}
