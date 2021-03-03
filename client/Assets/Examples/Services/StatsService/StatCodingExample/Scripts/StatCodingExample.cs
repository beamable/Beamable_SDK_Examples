using System.Collections.Generic;
using UnityEngine;

namespace Beamable.Examples.Services.Stats
{
    public class StatCodingExample : MonoBehaviour
    {
        //  Unity Methods   ------------------------------
        protected void Start()
        {
            Debug.Log($"Start()");
            
            Beamable.API.Instance.Then(beamableAPI =>
            {
                Debug.Log($"beamableAPI.User.id = {beamableAPI.User.id}");
                
                UseStatCoding(beamableAPI);
            });
        }

        //  Other Methods   ------------------------------
        private async void UseStatCoding(IBeamableAPI beamableAPI)
        {
            string statKey = "MyExampleStat";
            string access = "public";
            string domain = "client";
            string type = "player";
            long id = beamableAPI.User.id;

            // Set Value
            Dictionary<string, string> setStats =
                new Dictionary<string, string>() { { statKey, "99" } };

            await beamableAPI.Stats.SetStats(access, setStats);

            // Get Value
            Dictionary<string, string> getStats = 
                await beamableAPI.Stats.GetStats(domain, access, type, id);

            string myExampleStatValue = "";
            getStats.TryGetValue(statKey, out myExampleStatValue);
         
            Debug.Log($"myExampleStatValue = {myExampleStatValue}");
        }
    }
}