using Beamable.Common.Tournaments;
using UnityEngine;

namespace Beamable.Examples.Features.TournamentFlow
{
    /// <summary>
    /// Demonstrates <see cref="TournamentFlow"/>.
    /// </summary>
    public class TournamentFlowSetScoreExample : MonoBehaviour
    {
        [SerializeField] private TournamentRef _tournamentRef = null;
        [SerializeField] private double _score = 100;

        protected void Start()
        {
            Debug.Log("Start()");
            TournamentsSetScore(_tournamentRef.Id, _score);
        }

        private async void TournamentsSetScore(string id, double score)
        {
            await  Beamable.API.Instance.Then(async beamableAPI =>
            {
                long userId = beamableAPI.User.id;
                
                //TODO: Why isn't "Tournaments" called "TournamentService" for parity with other API?
                await beamableAPI.Tournaments.SetScore(id, userId, score);
                
                Debug.Log($"Tournaments.SetScore({id},{userId},{score})");
            });
        }
    }
}