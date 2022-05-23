using System.Collections.Generic;
using UnityEngine;

namespace Beamable.Examples.Services.StatsService
{
    /// <summary>
    /// Demonstrates <see cref="StatsService"/>.
    /// </summary>
    public class StatCodingExample : MonoBehaviour
    {
        //  Unity Methods  --------------------------------
        protected void Start()
        {
            Debug.Log($"Start()");

            SetupBeamable();
        }

        //  Other Methods   ------------------------------
        private async void SetupBeamable()
        {
            var context = BeamContext.Default;
            await context.OnReady;

            Debug.Log($"context.PlayerId = {context.PlayerId}");

            string statKey = "MyExampleStat";
            string access = "public";
            string domain = "client";
            string type = "player";
            long id = context.PlayerId;

            // Set Value
            Dictionary<string, string> setStats =
                new Dictionary<string, string>() { { statKey, "99" } };

            await context.Api.StatsService.SetStats(access, setStats);

            // Get Value
            Dictionary<string, string> getStats =
                await context.Api.StatsService.GetStats(domain, access, type, id);

            string myExampleStatValue = "";
            getStats.TryGetValue(statKey, out myExampleStatValue);

            Debug.Log($"myExampleStatValue = {myExampleStatValue}");
        }
    }
}