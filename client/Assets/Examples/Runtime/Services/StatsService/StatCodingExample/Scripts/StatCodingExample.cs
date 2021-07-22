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
            var beamableAPI = await Beamable.API.Instance;

            Debug.Log($"beamableAPI.User.id = {beamableAPI.User.id}");

            string statKey = "MyExampleStat";
            string access = "public";
            string domain = "client";
            string type = "player";
            long id = beamableAPI.User.id;

            // Set Value
            Dictionary<string, string> setStats =
                new Dictionary<string, string>() { { statKey, "99" } };

            await beamableAPI.StatsService.SetStats(access, setStats);

            // Get Value
            Dictionary<string, string> getStats =
                await beamableAPI.StatsService.GetStats(domain, access, type, id);

            string myExampleStatValue = "";
            getStats.TryGetValue(statKey, out myExampleStatValue);

            Debug.Log($"myExampleStatValue = {myExampleStatValue}");
        }
    }
}