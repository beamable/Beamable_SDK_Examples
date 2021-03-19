using System.Threading.Tasks;
using Beamable.Common.Leaderboards;
using UnityEngine;

namespace Beamable.Examples.Services.LeaderboardService
{
    /// <summary>
    /// Demonstrates <see cref="LeaderboardService"/>.
    /// </summary>
    public class LeaderboardServiceExample : MonoBehaviour
    {
        //  Fields  ---------------------------------------
        [SerializeField] private LeaderboardRef _leaderboardRef = null;
        [SerializeField] private double _score = 100;

        //  Unity Methods  --------------------------------
        protected async void Start()
        {
            Debug.Log($"Start()");

            await LeaderboardServiceSetScore(_leaderboardRef.Id, _score);
            await LeaderboardServiceGetScore(_leaderboardRef.Id);
        }

        //  Methods  --------------------------------------
        private async Task LeaderboardServiceSetScore(string id, double score)
        {
            var beamableAPI = await Beamable.API.Instance;

            Debug.Log($"beamableAPI.User.id = {beamableAPI.User.id}");

            await beamableAPI.LeaderboardService.SetScore(id, score);

            Debug.Log($"LeaderboardService.SetScore({id},{score})");
        }

        private async Task LeaderboardServiceGetScore(string id)
        {
            var beamableAPI = await Beamable.API.Instance;
            var score = await beamableAPI.LeaderboardService
                .GetUser(id, beamableAPI.User.id)
                .Map(entry => entry.score);
            Debug.Log($"LeaderboardService.GetScore({id},{score})");
        }
    }
}























