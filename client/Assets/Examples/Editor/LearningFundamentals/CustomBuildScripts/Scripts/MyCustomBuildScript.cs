using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif

namespace Beamable.Examples.LearningFundamentals.CustomBuildScripts
{
    /// <summary>
    /// Demonstrates CustomBuildScripts programming.
    ///
    /// The following is an arbitrary example. Replace with
    /// whatever are the specific project needs.
    ///
    /// Usage
    ///  * CancelMatchmaking The Scene
    ///  * Unity -> File -> Build And Run 
    ///  * See results in the Unity Console Window
    /// 
    /// </summary>
    public static class MyCustomBuildScript 
    {
        //  Unity Methods  --------------------------------
        
        /// <summary>
        /// Runs after compiling Unity and before native platform (such as Xcode for iOS).
        /// </summary>
        [PostProcessBuild]
        public static void OnPostProcessBuild(BuildTarget target, string path)
        {
            Debug.Log($"MyCustomBuildScript.cs, OnPostProcessBuild({target})");
#if UNITY_IOS
            OnPostProcessBuild_iOS (target, path);
#endif
        }
        
        //  Methods  --------------------------------------
#if UNITY_IOS
      private static void OnPostProcessBuild_iOS(BuildTarget target, string path)
      {
         if (target == BuildTarget.iOS && AccountManagementConfiguration.Instance.EnableGoogleSignInOnApple)
         {
            UpdateAppleProjectSettings(path);
         }
      }

      private static void UpdateAppleProjectSettings(string path)
      {
         var pbxPath = PBXProject.GetPBXProjectPath(path);
         var project = new PBXProject();
         project.ReadFromFile(pbxPath);

#if UNITY_2019_1_OR_NEWER
         var target = project.GetUnityMainTargetGuid();
#else
         var target = project.TargetGuidByName(PBXProject.GetUnityTargetName());
#endif

         project.AddBuildProperty(target, "OTHER_LDFLAGS", "-ObjC");

         File.WriteAllText(pbxPath, project.WriteToString());
      }
#endif
    }
}
