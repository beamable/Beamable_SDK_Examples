using System;
using System.Collections.Generic;
using System.IO;
using Beamable.Api.CloudSaving;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Beamable.Examples.Services.CloudSavingService
{
    [Serializable]
    public class RefreshedUnityEvent : UnityEvent<CloudSavingServiceExampleData> {}

    /// <summary>
    /// Holds data for use in the <see cref="CloudSavingServiceExample" />.
    /// </summary>
    [Serializable]
    public class CloudSavingServiceExampleData
    {
        public bool IsFirstFrame = true;
        public bool IsDataFoundFirstFrame = false;
        public MyCustomData MyCustomDataCloud = null;
        public MyCustomData MyCustomDataLocal = null;
        public List<string> InstructionLogs = new List<string>();
        public DataState DataState = DataState.Initializing;
    }

    public enum DataState
    {
        Initializing,
        Pending,
        Synced,
        Unsynced
    }

    /// <summary>
    /// Represents the Cloud Saving data stored on server.
    /// </summary>
    [Serializable]
    public class MyCustomData
    {
        public float Volume = 100;
        public bool IsMuted;

        public override string ToString()
        {
            return $"[MyCustomData (" +
                   $"Volume = {Volume}, " +
                   $"IsMuted = {IsMuted})]";
        }
    }

    /// <summary>
    /// Demonstrates <see cref="CloudSavingService" />.
    /// </summary>
    public class CloudSavingServiceExample : MonoBehaviour
    {
        //  Events  ---------------------------------------
        [HideInInspector] public RefreshedUnityEvent OnRefreshed = new RefreshedUnityEvent();

        // Properties   ----------------------------------
        
        /// <summary>
        /// Dynamically build the local storage for the Cloud Saving Data object
        /// </summary>
        private string FilePath
        {
            get
            {
                // Suggested format
                string fileName = "myCustomData.json";
                
                // Required format
                return $"{_cloudSavingService.LocalCloudDataFullPath}{Path.DirectorySeparatorChar}{fileName}";
            }
        } 
        
        //  Fields  ---------------------------------------
        private BeamContext _beamContext;
        private Api.CloudSaving.CloudSavingService _cloudSavingService;
        private readonly CloudSavingServiceExampleData _cloudSavingServiceExampleData =
            new CloudSavingServiceExampleData();

        
        //  Unity Methods  --------------------------------
        protected void Start()
        {
            Debug.Log($"Start() Instructions...\n\n" + 
                      " * Run Unity Scene\n" + 
                      " * Use in-game menu\n" +
                      " * See in-game text results\n" + 
                      " * CancelMatchmaking Scene and repeat steps to test persistence\n");
            
            _cloudSavingServiceExampleData.InstructionLogs.Clear();
            _cloudSavingServiceExampleData.InstructionLogs.Add("Run Unity Scene");
            _cloudSavingServiceExampleData.InstructionLogs.Add("Use in-game menu");
            _cloudSavingServiceExampleData.InstructionLogs.Add("See in-game text results");
            _cloudSavingServiceExampleData.InstructionLogs.Add("CancelMatchmaking Scene and repeat steps to test persistence");

            SetupBeamable();
        }


        //  Methods  --------------------------------------
        private async void SetupBeamable()
        {
            _beamContext = BeamContext.Default;
            await _beamContext.OnReady;

            Debug.Log($"_beamContext.PlayerId = {_beamContext.PlayerId}");

            _cloudSavingService = _beamContext.Api.CloudSavingService;

            // Subscribe to the UpdatedReceived event to handle
            // when data on disk does not yet exist and is pulled
            // from the server
            _cloudSavingService.UpdateReceived +=
                CloudSavingService_OnUpdateReceived;

            // Subscribe to the OnError event to handle
            // when the service fails
            _cloudSavingService.OnError += CloudSavingService_OnError;

            // Check isInitializing, as best practice
            if (!_cloudSavingService.isInitializing)
                // Init the service, which will first download content
                // that the server may have, that the client does not.
                // The client will then upload any content that it has,
                // that the server is missing
                await _cloudSavingService.Init();
            else
                throw new Exception("Cannot call Init() when " +
                                    $"isInitializing = {_cloudSavingService.isInitializing}");

            // Check isInitializing, as best practice
            if (!_cloudSavingService.isInitializing)
            {
                // Resets the local cloud data to match the server cloud data
                // IF DESIRED, UNCOMMENT THE FOLLOWING LINE
                //await _cloudSavingService.ReinitializeUserData();
            }
            else
            {
                throw new Exception("Cannot call Init() when " +
                                    $"isInitializing = {_cloudSavingService.isInitializing}");
            }

            LoadAndSave();
            Refresh();
        }


        private void LoadAndSave()
        {
            _cloudSavingServiceExampleData.DataState = DataState.Pending;
            _cloudSavingServiceExampleData.MyCustomDataLocal = LoadData();

            // Determines if the data was found on
            // the very first checking of the scene
            // Useful for demonstration purposes only
            if (_cloudSavingServiceExampleData.IsFirstFrame == true)
            {
                _cloudSavingServiceExampleData.IsFirstFrame = false;
                _cloudSavingServiceExampleData.IsDataFoundFirstFrame = 
                    _cloudSavingServiceExampleData.MyCustomDataLocal != null;
            }
            
            if (_cloudSavingServiceExampleData.MyCustomDataLocal == null)
            {
                // Create Data - Default
                _cloudSavingServiceExampleData.MyCustomDataLocal = new MyCustomData
                {
                    IsMuted = false,
                    Volume = 1
                };

                SaveDataInternal(_cloudSavingServiceExampleData.MyCustomDataLocal);
            }

        }

        public MyCustomData LoadData()
        {
            _cloudSavingServiceExampleData.DataState = DataState.Pending;
            var loaded = LoadDataInternal();

            _cloudSavingServiceExampleData.MyCustomDataCloud = loaded;
            _cloudSavingServiceExampleData.MyCustomDataLocal = loaded;
            
            Refresh();
            
            return _cloudSavingServiceExampleData.MyCustomDataCloud;
        }
        
        private MyCustomData LoadDataInternal()
        {
            if (!Directory.Exists(_cloudSavingService.LocalCloudDataFullPath))
            {
                Directory.CreateDirectory(_cloudSavingService.LocalCloudDataFullPath);
            }
            
            MyCustomData myCustomData = null;
            
            if (File.Exists(FilePath))
            {
                var json = File.ReadAllText(FilePath);
                myCustomData = JsonUtility.FromJson<MyCustomData>(json);
            }
            
            return myCustomData;
        }
      
        public void RandomizeData()
        {
            _cloudSavingServiceExampleData.DataState = DataState.Pending;

            // Create Data - Random
            _cloudSavingServiceExampleData.MyCustomDataLocal = new MyCustomData
            {
                IsMuted = UnityEngine.Random.value > 0.5f,
                Volume = UnityEngine.Random.Range(0, 10) * 0.1f
            };

            Refresh();
        }

        public void SaveData()
        {
            _cloudSavingServiceExampleData.DataState = DataState.Pending;

            SaveDataInternal(_cloudSavingServiceExampleData.MyCustomDataLocal);
            
            Refresh();
        }

        private void SaveDataInternal(MyCustomData myCustomData)
        {
            var json = JsonUtility.ToJson(myCustomData);
            
            if (!Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
            }

            // Once the data is written to disk, the service will
            // automatically upload the contents to the cloud
            File.WriteAllText(FilePath, json);

            _cloudSavingServiceExampleData.MyCustomDataCloud = myCustomData;

        }

        public void Refresh()
        {
            // Use DataState to display info to the user via UI
            if (_cloudSavingServiceExampleData.MyCustomDataCloud == null &&
                _cloudSavingServiceExampleData.MyCustomDataLocal == null)
            {
                // Connecting
                _cloudSavingServiceExampleData.DataState = DataState.Initializing;
            }
            else if (_cloudSavingServiceExampleData.MyCustomDataCloud == null ||
                _cloudSavingServiceExampleData.MyCustomDataLocal == null)
            {
                // Data transfer pending
                _cloudSavingServiceExampleData.DataState = DataState.Pending;
            }
            else if (_cloudSavingServiceExampleData.MyCustomDataCloud ==
                _cloudSavingServiceExampleData.MyCustomDataLocal)
            {
                // Local and Cloud are Synced
                _cloudSavingServiceExampleData.DataState = DataState.Synced;
            }
            else
            {
                // Local and Cloud are Not Synced
                _cloudSavingServiceExampleData.DataState = DataState.Unsynced;
            }
            
            //Debug.Log("Refresh()");

            OnRefreshed.Invoke(_cloudSavingServiceExampleData);
        }


        //  Event Handlers  -------------------------------
        private void CloudSavingService_OnUpdateReceived(ManifestResponse manifest)
        {
            Debug.Log("CloudSavingService_OnUpdateReceived()");

            // If the settings are changed by the server...
            // Reload the scene or something project-specific to reload your game
            SceneManager.LoadScene(0);
        }


        private void CloudSavingService_OnError(CloudSavingError cloudSavingError)
        {
            Debug.Log($"CloudSavingService_OnError() Message = {cloudSavingError.Message}");
        }
    }
}