using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Beamable.Api.Stats;
using Beamable.Common.Api;
using UnityEngine;

namespace Beamable.Examples.Shared
{
    /// <summary>
    /// Useful, reusable functionality
    /// for use in this example project
    /// </summary>
    public static class ExampleProjectHelper
    {

        //  Methods  --------------------------------------
        
        /// <summary>
        /// Get a public stat using the <see cref="StatsService"/>
        /// </summary>
        public static async Task<string> GetPublicPlayerStat(IBeamableAPI beamableAPI, string statKey)
        {
            string domain = "game";
            string access = "public";
            string type = "player";
            long id = beamableAPI.User.id;

            Dictionary<string, string> getStats = new Dictionary<string, string>();
            // Get Value
            try
            {
                getStats = await beamableAPI.StatsService.GetStats(domain, access, type, id);
            }
            catch (Exception e)
            {
                Debug.Log("error: " + e.Message);
            }
    

            string value = "";
            if (getStats.ContainsKey(statKey))
            {
                getStats.TryGetValue(statKey, out value);
            }

            return value;
        }
        
        /// <summary>
        /// Set a public stat using the <see cref="StatsService"/>
        /// </summary>
        public static async Task<EmptyResponse> SetPublicPlayerStat(IBeamableAPI beamableAPI, string statKey, string statValue)
        {
            string access = "public";
            long id = beamableAPI.User.id;

            Dictionary<string, string> setStats =
                new Dictionary<string, string>() {{statKey, statValue}};

            await beamableAPI.StatsService.SetStats(access, setStats);

            return new EmptyResponse();
        }

        /// <summary>
        /// Get the reponse from a <see cref="HttpWebRequest"/>
        /// and convert GZip response to text.
        /// </summary>
        public static async Task<string> GetResponseFromHttpWebRequest(string path)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest) WebRequest.Create(path);
            httpWebRequest.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            WebResponse webResponse = await httpWebRequest.GetResponseAsync();
            string resultText = "";
            using (StreamReader streamReader = new StreamReader(webResponse.GetResponseStream()))
            {
                resultText = await streamReader.ReadToEndAsync();
            }

            return resultText;
        }
    }
}