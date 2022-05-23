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
            var beamContext = BeamContext.Default;
            await beamContext.OnReady;
            var userId = beamContext.PlayerId;

            Debug.Log($"beamContext.PlayerId = {userId}");

            // Need to fetch the status for the current tournament cycle in order to set the score.
            var current = await beamContext.Api.TournamentsService.GetTournamentInfo(id);

            // This allows the currently logged in user to join the tournament by its content id.
            await beamContext.Api.TournamentsService.JoinTournament(current.tournamentId, 0);

            // Let's set the score for this player!
            await beamContext.Api.TournamentsService.SetScore(current.tournamentId, userId, score);

            Debug.Log($"Tournaments.SetScore({id},{userId},{score})");
        }
    }
}