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
    public class BuildGame
    {
        [MenuItem("Cardwheel/Build/Android")]
        public static void BuildAndroid()
        {
            setAndroidBuildNumber();

            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "");

            DateTime theTime = DateTime.Now;
            string dateTime = theTime.ToString("yyyy-MM-dd HH.mm.ss");

            EditorUserBuildSettings.buildAppBundle = false;

            Build(BuildTarget.Android, Application.dataPath + "/../../Build/Cardwheel " + dateTime + ".apk", BuildOptions.None, "Assets/Scenes/MainGameScene V.unity");

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [MenuItem("Cardwheel/Build/Android Release TEST")]
        public static void BuildAndroidReleaseTEST()
        {
            setAndroidBuildNumber();

            PlayerSettings.Android.useCustomKeystore = false;

            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "RELEASE");
            DateTime theTime = DateTime.Now;
            string dateTime = theTime.ToString("yyyy-MM-dd HH.mm.ss");
            EditorUserBuildSettings.buildAppBundle = false;
            Build(BuildTarget.Android, Application.dataPath + "/../../Build/ReleaseTest " + dateTime + ".apk", BuildOptions.None, "Assets/Scenes/MainGameScene V.unity");
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "");

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        [MenuItem("Cardwheel/Build/Android Release")]
        public static void BuildAndroidRelease()
        {
            setAndroidBuildNumber();

            PlayerSettings.Android.useCustomKeystore = true;
            // string path = Application.dataPath + "/../Keys/cardwheel.keystore";
            // PlayerSettings.Android.keystoreName = path;
            // PlayerSettings.Android.keystorePass = "";
            // PlayerSettings.Android.keyaliasName = "";
            // PlayerSettings.Android.keyaliasPass = "";

            //PlayerSettings.Android.minifyRelease = true;

            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "RELEASE");
            DateTime theTime = DateTime.Now;
            string dateTime = theTime.ToString("yyyy-MM-dd HH.mm.ss");
            EditorUserBuildSettings.buildAppBundle = true;
            EditorUserBuildSettings.androidCreateSymbols = AndroidCreateSymbols.Public;

            PlayerSettings.Android.targetArchitectures = AndroidArchitecture.ARMv7 | AndroidArchitecture.ARM64;

            Build(BuildTarget.Android, Application.dataPath + "/../../Build/Release " + dateTime + ".aab", BuildOptions.None, "Assets/Scenes/MainGameScene V.unity");

            EditorUserBuildSettings.androidCreateSymbols = AndroidCreateSymbols.Disabled;
            EditorUserBuildSettings.buildAppBundle = false;
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, "");

            PlayerSettings.Android.useCustomKeystore = false;

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        static void setAndroidBuildNumber()
        {
            string buildNumText = File.ReadAllText("Assets/Resources/AndroidBuildNum.txt");
            Debug.LogFormat("Build num " + buildNumText);
            int buildNum;
            if (int.TryParse(buildNumText, out buildNum))
            {
                buildNum++;
                PlayerSettings.Android.bundleVersionCode = buildNum;
                File.WriteAllText("Assets/Resources/AndroidBuildNum.txt", buildNum.ToString());
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        [MenuItem("Cardwheel/Build/iOS")]
        public static void BuildiOS()
        {
            setiOSBuildNumber();

            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, "");

            Build(BuildTarget.iOS, Application.dataPath + "/../../Build/iOSBuild", BuildOptions.AutoRunPlayer, "Assets/Scenes/MainGameScene V.unity");

            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, "");
        }

        [MenuItem("Cardwheel/Build/iOS Release")]
        public static void BuildiOSRelease()
        {
            setiOSBuildNumber();

            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, "RELEASE");

            Build(BuildTarget.iOS, Application.dataPath + "/../../Build/iOSBuild", BuildOptions.AutoRunPlayer, "Assets/Scenes/MainGameScene V.unity");

            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, "");
        }

        static void setiOSBuildNumber()
        {
            string buildNumText = File.ReadAllText("Assets/Resources/iOSBuildNum.txt");
            Debug.LogFormat("Build num " + buildNumText);
            int buildNum;
            if (int.TryParse(buildNumText, out buildNum))
            {
                buildNum++;
                PlayerSettings.iOS.buildNumber = buildNum.ToString();
                File.WriteAllText("Assets/Resources/iOSBuildNum.txt", buildNum.ToString());
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        static void Build(BuildTarget buildTarget, string path, BuildOptions options, string scene)
        {
            BalanceParser.ParseLocal();

            float time = Time.realtimeSinceStartup;

            DateTime theTime = DateTime.Now;
            string dateTime = theTime.ToString("yyyy-MM-dd HH.mm.ss");
            File.WriteAllText("Assets/Resources/Version.txt", dateTime);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            string text = File.ReadAllText("Assets/Resources/Version.txt");
            Debug.LogFormat("version saved as " + text);

            Debug.LogFormat("Build {0} to {1}", buildTarget, path);

            CreateAssetBundles.BuildAllAssetBundles(buildTarget);

            Debug.LogFormat("BuildAllAssetBundles elapsed time {0}", Time.realtimeSinceStartup - time);

            // Get filename.
            string[] levels = new string[] { scene };

            // Build player.
            BuildPipeline.BuildPlayer(levels, path, buildTarget, options);

            Debug.LogFormat("Build elapsed time {0}", Time.realtimeSinceStartup - time);
        }

        [PostProcessBuild(999)]
        public static void OnPostProcessBuild(BuildTarget buildTarget, string path)
        {
            if (buildTarget == BuildTarget.iOS)
            {
                copyPodfile(path);
                modifyXcodeFrameworks(path);
                modifyXCodePList(path);
            }
        }

        private static void modifyXcodeFrameworks(string path)
        {
#if UNITY_EDITOR_OSX
            string projPath = PBXProject.GetPBXProjectPath(path);

            var project = new PBXProject();
            project.ReadFromFile(projPath);

            string mainTargetGuid = project.GetUnityMainTargetGuid();

            foreach (var targetGuid in new[] { mainTargetGuid, project.GetUnityFrameworkTargetGuid() })
            {
                project.SetBuildProperty(targetGuid, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "NO");
                project.SetBuildProperty(targetGuid, "ENABLE_BITCODE", "NO");
                project.SetTeamId(targetGuid, "LGRYQ2SFF9");

                Debug.Log("project.GetUnityMainTargetGuid()" + project.GetUnityMainTargetGuid());
                Debug.Log("project.GetUnityFrameworkTargetGuid() " + project.GetUnityFrameworkTargetGuid());
            }

            project.SetBuildProperty(mainTargetGuid, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "YES");
            project.SetBuildProperty(mainTargetGuid, "ENABLE_BITCODE", "NO");

            project.WriteToFile(projPath);
#endif
        }

        private static void modifyXCodePList(string path)
        {
#if UNITY_EDITOR_OSX
            string plistPath = path + "/Info.plist";
            PlistDocument plist = new PlistDocument();
            plist.ReadFromFile(plistPath);

            PlistElementDict rootDict = plist.root;

            rootDict.SetBoolean("ITSAppUsesNonExemptEncryption", false);

            File.WriteAllText(plistPath, plist.WriteToString());
#endif
        }

        private static void copyPodfile(string pathToBuiltProject)
        {
#if UNITY_EDITOR_OSX
            var destPodfilePath = pathToBuiltProject + "/Podfile";

            Debug.Log(String.Format("Append text to {0}", destPodfilePath));
            using (StreamWriter sw = File.AppendText(destPodfilePath))
            {
                sw.WriteLine("");
                sw.WriteLine("post_install do |installer|");
                sw.WriteLine("  installer.generated_projects.each do |project|");
                sw.WriteLine("    project.targets.each do |target|");
                sw.WriteLine("        target.build_configurations.each do |config|");
                sw.WriteLine("            config.build_settings[\"DEVELOPMENT_TEAM\"] = \"65294JA6HM\"");
                sw.WriteLine("         end");
                sw.WriteLine("    end");
                sw.WriteLine("  end");
                sw.WriteLine("end");
            }
#endif
        }
    }
}