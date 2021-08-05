﻿using System.Collections.Generic;
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
        public bool IsUIInteractable = false;
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
        private TrialDataServiceExampleData _data = new TrialDataServiceExampleData();
        private ICloudDataApi _trialDataService;
        private IBeamableAPI _beamableAPI;

        
        //  Unity Methods  --------------------------------
        protected void Start()
        {
            Debug.Log($"Start() Instructions...\n" +
                      "\n * Setup AB Testing in Portal per https://docs.beamable.com/docs/abtesting-code" +
                      "\n * Run The Scene" +
                      "\n * See onscreen UI for results" +
                      "\n * If IsInABTest is false, something is incorrect. Repeat these steps" + 
                      "\n * If IsInABTest is true, everything is correct. Visit the portal to change " +
                      "the `PLAYER_LEVEL` stat value, then repeat these steps see load other data.\n");

            SetupBeamable();
        }


        //  Methods  --------------------------------------
        private async void SetupBeamable()
        {
            _beamableAPI = await Beamable.API.Instance;

            Debug.Log($"beamableAPI.User.id = {_beamableAPI.User.id}");

            _trialDataService = _beamableAPI.TrialDataService;

            await LoadTrialData();
        }

        
        public async Task<EmptyResponse> LoadTrialData()
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

                // Load the data, respecting GZip format
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

            _data.IsUIInteractable = true;
            Refresh();
            return new EmptyResponse();
        }


        public void Refresh()
        {
            string refreshLog = $"Refresh() ..." +
                                $"\n * IsInABTest = {_data.IsInABTest}" +
                                $"\n * CloudMetaDatas.Count = {_data.CloudMetaDatas.Count}" +
                                $"\n\n";

            //Debug.Log(refreshLog);

            // Send relevant data to the UI for rendering
            OnRefreshed?.Invoke(_data);
        }
    }
}