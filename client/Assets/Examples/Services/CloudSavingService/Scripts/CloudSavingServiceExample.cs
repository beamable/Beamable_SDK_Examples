using System.IO;
using Beamable.Api.CloudSaving;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Beamable.Examples.Services.CloudSavingService
{
  [System.Serializable]
  public class MyCustomSettings
  {
    public float Volume = 100;
    public bool IsMuted = false;
  }
  
  /// <summary>
  /// Demonstrates <see cref="CloudSavingService"/>.
  /// </summary>
  public class CloudSavingServiceExample : MonoBehaviour
  {
    //  Fields  ---------------------------------------
    [SerializeField] private MyCustomSettings myCustomSettings = null;
    private IBeamableAPI _beamableAPI;
    private Api.CloudSaving.CloudSavingService _cloudSavingService;

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
      
      _cloudSavingService = _beamableAPI.CloudSavingService;
      
      /* Subscribe to the UpdatedReceived event to call your own 
      custom code when data on disk does not yet exist and
      is pulled from the server*/
      _cloudSavingService.UpdateReceived += CloudSavingService_OnUpdateReceived;

      /* Init the service, which will first download content that the server may have,
      that the client does not. The client will then upload any content that it has,
      that the server is missing.*/
      await _cloudSavingService.Init();

      myCustomSettings = ReloadOrCreateAudioSettings();
    }

    private MyCustomSettings ReloadOrCreateAudioSettings()
    {
      Debug.Log($"ReloadOrCreateAudioSettings()");
      
      MyCustomSettings settings;
      Directory.CreateDirectory(_cloudSavingService.LocalCloudDataFullPath);

      var audioFileName = "audioFile.json";
      var audioPath = $"{_cloudSavingService.LocalCloudDataFullPath}{Path.DirectorySeparatorChar}{audioFileName}";

      
      if (File.Exists(audioPath))
      {
        // Reload AudioSettings 
        Debug.Log($"Reload AudioSettings");
        
        var json = File.ReadAllText(audioPath);
        settings = JsonUtility.FromJson<MyCustomSettings>(json);
      }
      else
      {
        // Create AudioSettings
        Debug.Log($"Create AudioSettings");
        
        settings = new MyCustomSettings
        {
          IsMuted = false,
          Volume = 1
        };

        var json = JsonUtility.ToJson(settings);
        Directory.CreateDirectory(Path.GetDirectoryName(audioPath));
        
        /* Once the data is written to disk, the service will automatically upload
        the contents to the cloud.*/
        File.WriteAllText(audioPath, json);

      }
      return settings;
    }
    
    //  Event Handlers  -------------------------------
    private void CloudSavingService_OnUpdateReceived(ManifestResponse manifest)
    {
      Debug.Log($"CloudSavingService_OnUpdateReceived()");
      
      // If the settings are changed by the server, reload the scene.
      SceneManager.LoadScene(0);
    }
  }
}
