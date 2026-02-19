using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using NUnit.Framework.Constraints;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.SceneManagement;
using UnityEditor.WindowsStandalone;
using UnityEngine;
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
            switchToHorizontalScene();
            EditorUserBuildSettings.SwitchActiveBuildTargetAsync(
                BuildPipeline.GetBuildTargetGroup(BuildTarget.StandaloneOSX),
                BuildTarget.StandaloneOSX
            );
        }

        [MenuItem("Cardwheel/BuildTarget/PC")]
        public static void BuildTargetPC()
        {
            switchToHorizontalScene();
            EditorUserBuildSettings.SwitchActiveBuildTargetAsync(
                BuildPipeline.GetBuildTargetGroup(BuildTarget.StandaloneWindows64),
                BuildTarget.StandaloneWindows64
            );
        }

        [MenuItem("Cardwheel/BuildTarget/Steamdeck")]
        public static void BuildTargetSteamdeck()
        {
            switchToHorizontalScene();
            EditorUserBuildSettings.SwitchActiveBuildTargetAsync(
                BuildPipeline.GetBuildTargetGroup(BuildTarget.StandaloneLinux64),
                BuildTarget.StandaloneLinux64
            );
        }

        [MenuItem("Cardwheel/BuildTarget/iOS")]
        public static void BuildTargetiOS()
        {
            switchToVerticalScene();
            EditorUserBuildSettings.SwitchActiveBuildTargetAsync(
                BuildPipeline.GetBuildTargetGroup(BuildTarget.iOS),
                BuildTarget.iOS
            );
        }

        [MenuItem("Cardwheel/BuildTarget/Android")]
        public static void BuildTargetAndroid()
        {
            switchToVerticalScene();
            EditorUserBuildSettings.SwitchActiveBuildTargetAsync(
                BuildPipeline.GetBuildTargetGroup(BuildTarget.Android),
                BuildTarget.Android
            );
        }

        private static void switchToVerticalScene()
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene("Assets/Scenes/MainGameScene V.unity");
        }

        private static void switchToHorizontalScene()
        {
            EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
            EditorSceneManager.OpenScene("Assets/Scenes/MainGameScene H.unity");
        }
    }
}
