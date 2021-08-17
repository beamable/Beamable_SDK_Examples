using UnityEngine;

namespace Beamable.Examples.Prefabs.InventoryFlow
{
    /// <summary>
    /// Demonstrates <see cref="InventoryFlow"/>.
    /// </summary>
    public class InventoryFlowExample : MonoBehaviour
    {
        //  Unity Methods  --------------------------------
        protected void Start()
        {
            Debug.Log($"Start() Instructions...\n\n" +
                      " * Run The Scene\n" +
                      " * See UI representing inventory on-screen\n" +
                      " * No inventory items shown? Open portal. Gift items to our active DBID.");
        }
    }
}