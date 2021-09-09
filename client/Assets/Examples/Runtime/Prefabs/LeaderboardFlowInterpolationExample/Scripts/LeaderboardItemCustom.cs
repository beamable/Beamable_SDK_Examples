using System;
using Beamable.Common.Api.Leaderboards;
using Beamable.Leaderboards;

namespace Beamable.Examples.Prefabs.LeaderboardFlow.LeaderboardFlowInterpolationExample
{
    /// <summary>
    /// Demonstrates <see cref="LeaderboardFlow"/>.
    /// </summary>
    public class LeaderboardItemCustom : LeaderboardItem
    {
        //  Unity Methods  --------------------------------
        public bool _isInitialized = false;
        private RankEntry _rankEntry;

        //  Unity Methods  --------------------------------
        public void Update()
        {
            if (!_isInitialized)
            {
                return;
            }
            
            // Custom Leaderboard
            long scoreTimestamp = long.Parse(_rankEntry.GetStat("leaderboard_score_timestamp"));
            long scoreVelocity = long.Parse(_rankEntry.GetStat("leaderboard_score_velocity"));
            long currentTimestamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
            long secondsSinceSubmission = currentTimestamp - scoreTimestamp;
            long scoreSinceSubmission = (secondsSinceSubmission * scoreVelocity) / 1000;
            //
            double score = _rankEntry.score + scoreSinceSubmission;
            TxtScore.text = $"{score:00000}";

        }

        //  Other Methods  --------------------------------
        public new async void Apply (RankEntry entry)
        {
            // Traditional Leaderboard
            var de = await API.Instance;
            var stats = await de.StatsService.GetStats("client", "public", "player", entry.gt);
            string alias;
            stats.TryGetValue("alias", out alias);
            TxtAlias.text = alias;
            TxtRank.text = entry.rank.ToString();
            
            // Custom Leaderboard
            _rankEntry = entry;
            _isInitialized = true;
        }
    }
}























