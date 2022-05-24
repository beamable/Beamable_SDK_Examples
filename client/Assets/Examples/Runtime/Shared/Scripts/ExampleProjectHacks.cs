using Beamable.Api;
using Beamable.Service;

namespace Beamable.Examples.Shared
{ 
    /// <summary>
    /// These are hacks and Call_BasicAccountsRegister shortcuts used to
    /// assist in the examples.
    ///
    /// HACK: These hacks are not recommended for production usage
    /// 
    /// </summary>
    public static class ExampleProjectHacks
    {

        //  Methods  --------------------------------------
        
        /// <summary>
        /// Clears all data related to the active runtime user(s)
        /// </summary>
        public static async void ClearDeviceUsersAndReloadScene()
        {
            BeamContext context = BeamContext.Default;
            await context.OnReady;
            await Beam.ClearAndStopAllContexts();
            await Beam.ResetToScene();
        }
    }
}