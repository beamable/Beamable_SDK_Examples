using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Beamable.Common.Api;
using Beamable.Common.Api.CloudData;
using Beamable.Examples.Services.CloudSavingService;
using Beamable.Examples.Services.ConnectivityService;
using Beamable.Examples.Shared;
using UnityEngine;
using UnityEngine.Events;

namespace Beamable.Examples.Services.TrialDataService
{
    /// <summary>
    /// Represents the data in
    /// the A/B Testing trial.
    /// </summary>
    [System.Serializable]
    public class MyPlayerProgression
    {
        public int MaxHealth = 100;
        public int MaxInventorySpace = 10;
    }

    /// <summary>
    /// Holds data for use in the <see cref="ConnectivityServiceExampleUI"/>.
    /// </summary>
    [System.Serializable]
    public class TrialDataServiceExampleData
    {
        public int PlayerLevel = -1;
        public string DataName = "MyPlayerProgression";
        public MyPlayerProgression MyPlayerProgression = null;
        public List<CloudMetaData> CloudMetaDatas = new List<CloudMetaData>();
        public bool IsInABTest { get { return CloudMetaDatas.Count > 0; } }
    }

    [System.Serializable]
    public class RefreshedUnityEvent : UnityEvent<TrialDataServiceExampleData>  { }

    /// <summary>
    /// Demonstrates <see cref="TrialDataService"/>.
    ///
    /// NOTE: This demo uses other concepts
    /// too. See <see cref="CloudSavingServiceExample"/>
    /// for more info.
    /// 
    /// </summary>
    public class TrialDataServiceExample : MonoBehaviour
    {
        //  Events  ---------------------------------------
        [HideInInspector] public RefreshedUnityEvent OnRefreshed = new RefreshedUnityEvent();

        //  Fields  ---------------------------------------
        private const string StatKey = "PLAYER_LEVEL";
        private TrialDataServiceExampleData _data = new TrialDataServiceExampleData();
        private ICloudDataApi _trialDataService;
        private IBeamableAPI _beamableAPI;

        //  Unity Methods  --------------------------------
        protected void Start()
        {
            Debug.Log($"Start() Instructions...\n" +
                      " * Setup AB Testing in Portal per https://docs.beamable.com/docs/abtesting-code\n" +
                      " * Run The Scene\n" +
                      " * See onscreen UI for results.\n" +
                      " * If IsInABTest is false, then repeat these steps.\n");

            SetupBeamable();
        }


        //  Methods  --------------------------------------
        private async void SetupBeamable()
        {
            _beamableAPI = await Beamable.API.Instance;

            Debug.Log($"beamableAPI.User.id = {_beamableAPI.User.id}");

            _trialDataService = _beamableAPI.TrialDataService;

            await TogglePlayerlevelStat();
        }

        
        public async Task<EmptyResponse> TogglePlayerlevelStat()
        {
            // Get Value
            string playerLevel = await ExampleProjectHelper.GetPublicPlayerStat(
                _beamableAPI, StatKey);

            _data.PlayerLevel = -1;
            if (!string.IsNullOrEmpty(playerLevel))
            {
                _data.PlayerLevel = Int32.Parse(playerLevel);
            }

            // Toggle Value
            if (_data.PlayerLevel != 1)
            {
                _data.PlayerLevel = 1;
            }
            else
            {
                _data.PlayerLevel = 2;
            }

            // Set Value
            await ExampleProjectHelper.SetPublicPlayerStat(
                _beamableAPI, StatKey, _data.PlayerLevel.ToString());

            // Reload the trial
            await LoadTrialData();
            return new EmptyResponse();
        }


        private async Task<EmptyResponse> LoadTrialData()
        {
            // Load any trials
            GetCloudDataManifestResponse playerManifestResponse =
                await _trialDataService.GetPlayerManifest();
            
            // Loop through trials
            _data.MyPlayerProgression = null;
            _data.CloudMetaDatas = playerManifestResponse.meta;
            foreach (CloudMetaData cloudMetaData in _data.CloudMetaDatas)
            {
                string path = $"http://{cloudMetaData.uri}";

                string response = 
                    await ExampleProjectHelper.GetResponseFromHttpWebRequest(path);

                MyPlayerProgression myPlayerProgression = 
                    JsonUtility.FromJson<MyPlayerProgression>(response);

                // If trial is related, store data
                if (myPlayerProgression != null)
                {
                    _data.MyPlayerProgression = myPlayerProgression;
                }
            }

            Refresh();
            return new EmptyResponse();
        }


        public void Refresh()
        {
            string refreshLog = $"Refresh() ..." +
                                $"\n * PlayerLevel = {_data.PlayerLevel}" +
                                $"\n * CloudMetaDatas.Count = {_data.CloudMetaDatas.Count}" +
                                $"\n\n";

            //Debug.Log(refreshLog);

            // Send relevant data to the UI for rendering
            OnRefreshed?.Invoke(_data);
        }
    }
}