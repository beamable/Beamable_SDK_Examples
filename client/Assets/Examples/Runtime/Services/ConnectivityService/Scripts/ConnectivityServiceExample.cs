using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Beamable.Examples.Services.ConnectivityService
{
    /// <summary>
    /// Holds data for use in the <see cref="ConnectivityServiceExampleUI"/>.
    /// </summary>
    [System.Serializable]
    public class ConnectivityServiceExampleData
    {
        public List<string> OutputLogs = new List<string>();
        public bool HasConnectivity = false;
    }
   
    [System.Serializable]
    public class RefreshedUnityEvent : UnityEvent<ConnectivityServiceExampleData> { }
    
    /// <summary>
    /// Demonstrates <see cref="ConnectivityService"/>.
    /// </summary>
    public class ConnectivityServiceExample : MonoBehaviour
    {
        //  Events  ---------------------------------------
        [HideInInspector]
        public RefreshedUnityEvent OnRefreshed = new RefreshedUnityEvent();
        
        //  Fields  ---------------------------------------
        private IBeamableAPI _beamableAPI = null;
        private ConnectivityServiceExampleData _data = new ConnectivityServiceExampleData();
    
        //  Unity Methods  --------------------------------
        protected void Start()
        {
            Debug.Log($"Start() Instructions...\n" +
                      " * Ensure Computer's Internet Is Active\n" +
                      " * Run The Scene\n" +
                      " * See Onscreen UI Show HasConnectivity = true\n" +
                      " * Ensure Computer's Internet Is NOT Active\n" +
                      " * See Onscreen UI Show HasConnectivity = false\n");

            SetupBeamable();
        }
        
        //  Methods  --------------------------------------
        private async void SetupBeamable()
        {
            _beamableAPI = await Beamable.API.Instance;

            Debug.Log($"beamableAPI.User.id = {_beamableAPI.User.id}");

            // Observe ConnectivityService Changes
            _beamableAPI.ConnectivityService.OnConnectivityChanged += ConnectivityService_OnConnectivityChanged;
            
            // Update UI Immediately
            bool hasConnectivity = _beamableAPI.ConnectivityService.HasConnectivity;
            ConnectivityService_OnConnectivityChanged(hasConnectivity);
        }
        
        public void ToggleHasInternet()
        {
            _beamableAPI.ConnectivityService.SetHasInternet(!_data.HasConnectivity);
        }

        public void Refresh()
        {
            string refreshLog = $"Refresh() ..." +
                                $"\n * HasConnectivity = {_data.HasConnectivity}" + 
                                $"\n * OutputLogs = {_data.OutputLogs.Count}\n\n";
            
            //Debug.Log(refreshLog);
            
            // Send relevant data to the UI for rendering
            OnRefreshed?.Invoke(_data);
        }
        
        //  Event Handlers  -------------------------------
        private void ConnectivityService_OnConnectivityChanged(bool hasConnectivity)
        {
            _data.HasConnectivity = hasConnectivity;
            
            _data.OutputLogs.Add($"HasConnectivity = {_data.HasConnectivity}");
            
            Refresh();
        }
    }
}