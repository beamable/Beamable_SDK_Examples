using System.IO;
using Beamable;
using Beamable.Api.CloudSaving;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DisruptorBeam.Examples.Features.CloudSave
{
  [System.Serializable]
  public class AudioSettings
  {
    public float Volume = 100;
    public bool IsMuted = false;
  }
  
  /// <summary>
  /// Demonstrates <see cref="CloudSave"/>.
  /// </summary>
  public class CloudSaveExample : MonoBehaviour
  {
    //TODO: After demo works, move this to private if possible
    public AudioSettings AudioSettings;
    
    
    private IBeamableAPI _beamableAPI;
    private CloudSavingService _cloudSavingService;

    protected async void Start()
    {
      Debug.Log("Start()");
                
      await Beamable.API.Instance.Then(beamableAPI =>
      {
        _beamableAPI = beamableAPI;
        _cloudSavingService = _beamableAPI.CloudSavingService;

        /* Subscribe to the UpdatedReceived event to call your own 
        custom code when data on disk is changed by the server*/
        _cloudSavingService.UpdateReceived += ReloadSceneAfterServerChanged;

        /* Init the service, which will first download content that the server may have,
        that the client does not. The client will then upload any content that it has,
        that the server is missing.*/
        _cloudSavingService.Init();

        AudioSettings = ReloadOrCreateAudioSettings();
      });
  
    }

    private AudioSettings ReloadOrCreateAudioSettings()
    {
      AudioSettings settings;
      Directory.CreateDirectory(_cloudSavingService.LocalCloudDataFullPath);

      var audioFileName = "audioFile.json";
      var audioPath = $"{_cloudSavingService.LocalCloudDataFullPath}{Path.DirectorySeparatorChar}{audioFileName}";

      // Load AudioSettings 
      if (File.Exists(audioPath))
      {
        var json = File.ReadAllText(audioPath);
        settings = JsonUtility.FromJson<AudioSettings>(json);
      }
      else
      {
        // Create AudioSettings
        settings = new AudioSettings
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
    
    private void ReloadSceneAfterServerChanged(ManifestResponse manifest)
    {
      // If the settings are changed by the server, reload the scene.
      SceneManager.LoadScene(0);
    }
  }
}
