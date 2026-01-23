using System;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.WindowsStandalone;
using UnityEngine;
using NUnit.Framework.Constraints;
using System.Linq;



#if UNITY_EDITOR_OSX
using UnityEditor.iOS.Xcode;
#endif

namespace Cardwheel
{

    [InitializeOnLoad]
    public class ChangeBuildTarget
    {
        [MenuItem("Cardwheel/BuildTarget/Mac")]
        public static void BuildTargetMac()
        {
            EditorUserBuildSettings.SwitchActiveBuildTargetAsync(BuildPipeline.GetBuildTargetGroup(BuildTarget.StandaloneOSX), BuildTarget.StandaloneOSX);
        }

        [MenuItem("Cardwheel/BuildTarget/PC")]
        public static void BuildTargetPC()
        {
            EditorUserBuildSettings.SwitchActiveBuildTargetAsync(BuildPipeline.GetBuildTargetGroup(BuildTarget.StandaloneWindows64), BuildTarget.StandaloneWindows64);
        }

        [MenuItem("Cardwheel/BuildTarget/Steamdeck")]
        public static void BuildTargetSteamdeck()
        {
            EditorUserBuildSettings.SwitchActiveBuildTargetAsync(BuildPipeline.GetBuildTargetGroup(BuildTarget.StandaloneLinux64), BuildTarget.StandaloneLinux64);
        }

        [MenuItem("Cardwheel/BuildTarget/iOS")]
        public static void BuildTargetiOS()
        {
            EditorUserBuildSettings.SwitchActiveBuildTargetAsync(BuildPipeline.GetBuildTargetGroup(BuildTarget.iOS), BuildTarget.iOS);
        }

        [MenuItem("Cardwheel/BuildTarget/Android")]
        public static void BuildTargetAndroid()
        {
            EditorUserBuildSettings.SwitchActiveBuildTargetAsync(BuildPipeline.GetBuildTargetGroup(BuildTarget.Android), BuildTarget.Android);
        }
    }
}