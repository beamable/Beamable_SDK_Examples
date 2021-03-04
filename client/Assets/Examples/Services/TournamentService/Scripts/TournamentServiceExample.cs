using Beamable.Common.Tournaments;
using UnityEngine;

namespace Beamable.Examples.Services.TournamentService
{
    /// <summary>
    /// Demonstrates <see cref="TournamentService"/>.
    /// </summary>
    public class TournamentServiceExample : MonoBehaviour
    {
        [SerializeField] private TournamentRef _tournamentRef = null;
        [SerializeField] private double _score = 100;

        protected void Start()
        {
            Debug.Log("Start()");
            
            TournamentsSetScore(_tournamentRef.GetId(), _score);
        }

        private async void TournamentsSetScore(string id, double score)
        {
            var beamableAPI = await Beamable.API.Instance;
            
            long userId = beamableAPI.User.id;

            Debug.Log($"beamableAPI.User.id = {userId}");
            
            await beamableAPI.Tournaments.JoinTournament(id, 0);
            
            await beamableAPI.Tournaments.SetScore(id, userId, score);
            
            Debug.Log($"Tournaments.SetScore({id},{userId},{score})");
        }
    }
}