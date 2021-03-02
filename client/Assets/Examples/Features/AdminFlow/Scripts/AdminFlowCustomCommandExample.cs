using Beamable.ConsoleCommands;
using UnityEngine;

namespace Beamable.Examples.Features.AdminFlow2
{
    [BeamableConsoleCommandProvider]
    public class CustomConsoleCommandProvider 
    {
        [BeamableConsoleCommand ("Add", "A sample addition command", "Add <int> <int>")]
        public string Add(string[] args)
        {
            var a = int.Parse(args[0]);
            var b = int.Parse(args[1]);
            return "Result: " + (a + b);
        }
    }
    
    /// <summary>
    /// Demonstrates <see cref="AdminFlow"/>.
    /// </summary>
    public class AdminFlowCustomCommandExample : MonoBehaviour
    {
        protected void Start()
        {
            Debug.Log("Start() Instructions...\n" + 
                      " * Run The Scene\n" + 
                      " * Type '~' in Unity Game Window to open Admin Console\n" + 
                      " * Type 'Add 5 10'\n" + 
                      " * See 'Result: 15' in Unity Console Window\n");
        }
    }
}