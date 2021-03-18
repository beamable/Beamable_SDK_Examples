﻿using UnityEngine;

namespace Beamable.Examples.Features.InventoryFlow
{
    /// <summary>
    /// Demonstrates <see cref="InventoryFlow"/>.
    /// </summary>
    public class InventoryFlowExample : MonoBehaviour
    {
        //  Unity Methods  --------------------------------
        protected void Start()
        {
            Debug.Log($"Start() Instructions...\n" +
                      " * Run The Scene\n" +
                      " * See UI representing inventory on-screen\n" +
                      " * No inventory items shown? Open portal. Gift items to our active DBID.");
        }
    }
}