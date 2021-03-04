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
        protected void Start()
        {
            Debug.Log($"Start()");
            
            LeaderboardServiceSetScore(_leaderboardRef.Id, _score);
        }

        //  Methods  --------------------------------------
        private async void LeaderboardServiceSetScore(string id, double score)
        {
            var beamableAPI = await Beamable.API.Instance;
            
            Debug.Log($"beamableAPI.User.id = {beamableAPI.User.id}");
            
            await beamableAPI.LeaderboardService.SetScore(id, score);
                
            Debug.Log($"LeaderboardService.SetScore({id},{score})");
        }
    }
}























