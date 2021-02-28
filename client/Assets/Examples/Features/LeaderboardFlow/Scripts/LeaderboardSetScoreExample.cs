using Beamable.Common.Leaderboards;
using UnityEngine;

namespace Beamable.Examples.Features.LeaderboardFlow
{
    /// <summary>
    /// Demonstrates <see cref="LeaderboardSetScoreExample"/>.
    /// </summary>
    public class LeaderboardSetScoreExample : MonoBehaviour
    {
        [SerializeField] private LeaderboardRef _leaderboardRef = null;
        [SerializeField] private double _score = 100;

        protected void Start()
        {
            Debug.Log("Start()");
            LeaderboardSetScore(_leaderboardRef.Id, _score);
        }

        private async void LeaderboardSetScore(string id, double score)
        {
            await  Beamable.API.Instance.Then(async beamableAPI => 
            {
                await beamableAPI.LeaderboardService.SetScore(id, score);
                Debug.Log($"LeaderboardService.SetScore({id},{score})");
            });
        }
    }
}
