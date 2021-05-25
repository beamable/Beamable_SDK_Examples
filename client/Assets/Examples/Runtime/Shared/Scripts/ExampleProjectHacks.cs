using Beamable.Api;
using Beamable.Service;

namespace Beamable.Examples.Shared
{ 
    public static class ExampleProjectHacks
    {

        //  Methods  --------------------------------------
        
        /// <summary>
        /// Clears all data related to the active runtime user(s)
        /// 
        /// HACK: This is not recommended for production usage
        /// 
        /// </summary>
        public static void ClearDeviceUsersAndReloadScene()
        {
            PlatformService platformService = ServiceManager.Resolve<PlatformService>();
            platformService.ClearDeviceUsers();
            ServiceManager.OnTeardown();
        }
    }
}