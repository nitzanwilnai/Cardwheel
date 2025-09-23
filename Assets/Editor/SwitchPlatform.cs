using UnityEditor;
using System.IO;
using UnityEngine;
using System;
using UnityEditor.Callbacks;
using UnityEditor.Build.Reporting;
using System.Collections.Generic;
#if UNITY_EDITOR_OSX
using UnityEditor.iOS.Xcode;
#endif

namespace Cardwheel
{

    [InitializeOnLoad]
    public class SwitchPlatform
    {
        [MenuItem("Cardwheel/Platform/Steam OSX")]
        public static void SetPlatformSteamOSX()
        {
            if (EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneOSX))
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, "STEAM");

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                Debug.Log("Build Target Switched to Steam OSX");
            }
            else
                Debug.LogError("Set Platform Steam OSX ERROR!");
        }

        [MenuItem("Cardwheel/Platform/Android")]
        public static void SetPlatformAndroid()
        {
            if (EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Android, BuildTarget.Android))
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "");

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                Debug.Log("Build Target Switched to Android");
            }
            else
                Debug.LogError("Set Platform Android ERROR!");
        }

        [MenuItem("Cardwheel/Platform/iOS")]
        public static void SetPlatformIOS()
        {
            if (EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.iOS, BuildTarget.iOS))
            {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, "");

                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();

                Debug.Log("Build Target Switched to iOS");
            }
            else
                Debug.LogError("Set Platform iOS ERROR!");
        }

    }
}