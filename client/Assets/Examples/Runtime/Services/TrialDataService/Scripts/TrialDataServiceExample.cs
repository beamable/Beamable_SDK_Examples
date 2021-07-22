using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Beamable.Api.CloudSaving;
using Beamable.Common.Api;
using Beamable.Common.Api.CloudData;
using Beamable.Examples.Services.CloudSavingService;
using Beamable.Examples.Services.ConnectivityService;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Beamable.Examples.Services.TrialDataService
{
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
    public bool IsDataExistingOnFirstFrame = false;
    public string DataName = "MyPlayerProgression";
    public MyPlayerProgression MyPlayerProgression = null;
    public List<string> OutputLogs = new List<string>();
    public List<CloudMetaData> CloudMetaDatas = new List<CloudMetaData>();
    public bool IsInABTest { get { return CloudMetaDatas.Count > 1; } }
    
  }
   
  [System.Serializable]
  public class RefreshedUnityEvent : UnityEvent<TrialDataServiceExampleData> { }
  
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
    [HideInInspector]
    public RefreshedUnityEvent OnRefreshed = new RefreshedUnityEvent();
    
    //  Fields  ---------------------------------------
    private const string UnsetValue = "-1";
    private TrialDataServiceExampleData _data = new TrialDataServiceExampleData();
    private Api.CloudSaving.CloudSavingService _cloudSavingService;
    private ICloudDataApi _trialDataService;
    private IBeamableAPI _beamableAPI;
    private bool _isFirstFrame = false;

    //  Unity Methods  --------------------------------
    protected void Start()
    {
      Debug.Log($"Start()");

      SetupBeamable();
    }

    
    //  Methods  --------------------------------------
    private async void SetupBeamable()
    {
      _beamableAPI = await Beamable.API.Instance;

      Debug.Log($"beamableAPI.User.id = {_beamableAPI.User.id}");

      _trialDataService = _beamableAPI.TrialDataService;
      
      _cloudSavingService = _beamableAPI.CloudSavingService;
      _cloudSavingService.UpdateReceived +=  CloudSavingService_OnUpdateReceived;
      await _cloudSavingService.Init();
      await _cloudSavingService.EnsureRemoteManifest();

      await TogglePlayerlevel();
      ReloadOrCreateData();
      
    }

    public async Task<EmptyResponse> TogglePlayerlevel()
    {
      string statKey = "PlayerLevel";
      string access = "public";
      string domain = "client";
      string type = "player";
      long id = _beamableAPI.User.id;

      // Get Value
      Dictionary<string, string> getStats =
        await _beamableAPI.StatsService.GetStats(domain, access, type, id);

      string playerLevel = UnsetValue;
      if (getStats.ContainsKey(statKey))
      {
        getStats.TryGetValue(statKey, out playerLevel);
      }
      _data.PlayerLevel = Int32.Parse(playerLevel);
      
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
      Dictionary<string, string> setStats =
        new Dictionary<string, string>() { { statKey, _data.PlayerLevel.ToString()  } };

      await _beamableAPI.StatsService.SetStats(access, setStats);

      Refresh();

      return new EmptyResponse();
    }


    private void ReloadOrCreateData()
    {
      Debug.Log($"ReloadOrCreateData()");
      
      Directory.CreateDirectory(_cloudSavingService.LocalCloudDataFullPath);
      _data.DataName = "MyPlayerProgression";
      var fileName = $"{_data.DataName}.json";
      var filePath = $"{_cloudSavingService.LocalCloudDataFullPath}{Path.DirectorySeparatorChar}{fileName}";

      if (!_isFirstFrame)
      {
        _isFirstFrame = true;
        _data.IsDataExistingOnFirstFrame = File.Exists(filePath);
      }

      string json = "";
      if (File.Exists(filePath))
      {
        // Reload MyPlayerProgression 
        Debug.Log($"Existing MyPlayerProgression found");
        Debug.Log($"Reload MyPlayerProgression");
        
        json = File.ReadAllText(filePath);
        _data.MyPlayerProgression = JsonUtility.FromJson<MyPlayerProgression>(json);
      }
      else
      {
        // Create MyPlayerProgression
        Debug.Log($"Existing MyPlayerProgression not found");
        Debug.Log($"Create MyPlayerProgression");

        _data.MyPlayerProgression = new MyPlayerProgression();
        json = JsonUtility.ToJson(_data.MyPlayerProgression);
        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        File.WriteAllText(filePath, json);
      }
      
      _data.OutputLogs.Clear();
      _data.OutputLogs.Add($"{_data.DataName}\n\t{json}");
      
      Refresh();
    }
    
    public async void Refresh()
    {
      string refreshLog = $"Refresh() ..." +
                          $"\n * IsDataExistingOnFirstFrame = {_data.IsDataExistingOnFirstFrame}" + 
                          $"\n * PlayerLevel = {_data.PlayerLevel}" + 
                          $"\n * CloudMetaDatas.Count = {_data.CloudMetaDatas.Count}" + 
                          $"\n\n";
            
      //Debug.Log(refreshLog);

      if (_trialDataService != null)
      {
        GetCloudDataManifestResponse getCloudDataManifestResponse = 
          await _trialDataService.GetPlayerManifest();

        _data.CloudMetaDatas = getCloudDataManifestResponse.meta;

      }

      // Send relevant data to the UI for rendering
      OnRefreshed?.Invoke(_data);
    }
    
    //  Event Handlers  -------------------------------
    private void CloudSavingService_OnUpdateReceived(ManifestResponse manifest)
    {
      Debug.Log($"CloudSavingService_OnUpdateReceived()");
      
      // If the settings are changed by the server, reload the scene
      SceneManager.LoadScene(0);
    }
  }
}
