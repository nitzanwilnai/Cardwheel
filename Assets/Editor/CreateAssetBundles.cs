using UnityEditor;
using System.IO;
using UnityEngine;

namespace Cardwheel
{

    [InitializeOnLoad]
    public class CreateAssetBundles
    {
        private const string UseAssetBundlesMenuName = "Cardwheel/Asset Bundles/Use Asset Bundles";

        static CreateAssetBundles()
        {
            AssetManager.UseAssetBundles = EditorPrefs.GetBool(UseAssetBundlesMenuName, false);
        }

        [MenuItem(UseAssetBundlesMenuName)]
        private static void ToggleUseAssetBundles()
        {
            AssetManager.UseAssetBundles = !AssetManager.UseAssetBundles;
            EditorPrefs.SetBool(UseAssetBundlesMenuName, AssetManager.UseAssetBundles);
        }

        [MenuItem(UseAssetBundlesMenuName, true)]
        private static bool TogleUseAssetBundlesValidate()
        {
            Menu.SetChecked(UseAssetBundlesMenuName, AssetManager.UseAssetBundles);
            return true;
        }

        [MenuItem("Cardwheel/Asset Bundles/Build AssetBundles iOS")]
        public static void BuildAllAssetBundlesiOS()
        {
            BuildAllAssetBundles(BuildTarget.iOS);
        }

        [MenuItem("Cardwheel/Asset Bundles/Build AssetBundles Android")]
        public static void BuildAllAssetBundlesAndroid()
        {
            BuildAllAssetBundles(BuildTarget.Android);
        }

        [MenuItem("Cardwheel/Asset Bundles/Build AssetBundles OSX")]
        public static void BuildAllAssetBundlesOSX()
        {
            BuildAllAssetBundles(BuildTarget.StandaloneOSX);
        }

        [MenuItem("Cardwheel/Asset Bundles/Build AssetBundles Windows")]
        public static void BuildAllAssetBundlesWindows()
        {
            BuildAllAssetBundles(BuildTarget.StandaloneWindows);
        }

        public static void BuildAllAssetBundles(BuildTarget buildTarget)
        {
            string assetBundleDirectory = "Assets/StreamingAssets/AssetBundles";
            if (!Directory.Exists(assetBundleDirectory))
                Directory.CreateDirectory(assetBundleDirectory);

            BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, buildTarget);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}