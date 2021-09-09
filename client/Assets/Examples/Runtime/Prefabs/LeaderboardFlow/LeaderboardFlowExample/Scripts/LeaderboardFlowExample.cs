using System.Collections.Generic;
using UnityEngine;

namespace Beamable.Examples.Prefabs.LeaderboardFlow.LeaderboardFlowExample
{
    /// <summary>
    /// Demonstrates <see cref="LeaderboardFlow"/>.
    /// </summary>
    public class LeaderboardFlowExample : MonoBehaviour
    {
        [SerializeField] 
        private List<GameObject> _gameObjectsToHide = null;
        
        //  Unity Methods  --------------------------------
        protected void Start()
        {
            foreach (GameObject go in _gameObjectsToHide)
            {
                //Hide back button - not needed for this demo
                go.SetActive(false);
            }
            
            Debug.Log($"Start() Instructions...\n\n" +
                      " * Run The Scene\n" +
                      " * See UI representing leaderboard on-screen\n" +
                      " * No leaderboard items shown? That's ok. Run the LeaderboardServiceExample.unity first");
        }
    }
}























