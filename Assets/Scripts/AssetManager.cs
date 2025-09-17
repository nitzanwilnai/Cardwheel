using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using CommonTools;
using UnityEditor;

namespace Cardwheel
{
    public class AssetManager : Singleton<AssetManager>
    {
        public static bool UseAssetBundles = false;

        AssetBundle m_commonBundle;
        AssetBundle m_commonBundleUI;
        public string m_commonUIBundlePath;

        public void LoadCommonAssetBundle()
        {
#if UNITY_EDITOR
            if (UseAssetBundles)
                m_commonBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "AssetBundles/common"));
#else
            m_commonBundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "AssetBundles/common"));
#endif
        }

        public void LoadCommonUIAssetBundle(string commonUIBundle, string commonUIBundlePath)
        {
            m_commonUIBundlePath = commonUIBundlePath;
#if UNITY_EDITOR
            if (UseAssetBundles)
                m_commonBundleUI = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "AssetBundles/" + commonUIBundle));
#else
            m_commonBundleUI = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, "AssetBundles/" + commonUIBundle));
#endif
        }

        public void UnloadCommonAssetBundle()
        {
            if (m_commonBundle != null)
                m_commonBundle.Unload(true);

            if (m_commonBundleUI != null)
                m_commonBundleUI.Unload(true);
        }

        GameObject loadGameObject(AssetBundle assetBundle, string objName, string localPath)
        {
            // Debug.Log("loadGameObject objName " + objName + " localPath " + localPath);

            GameObject go = null;
#if UNITY_EDITOR
            if (UseAssetBundles)
                go = assetBundle.LoadAsset<GameObject>(objName);
            else
                go = (GameObject)AssetDatabase.LoadAssetAtPath(localPath, typeof(GameObject));
#else
            go = assetBundle.LoadAsset<GameObject>(objName);
#endif
            return go;
        }

        Sprite loadSprite(AssetBundle assetBundle, string objName, string localPath)
        {
            Sprite sprite = null;
#if UNITY_EDITOR
            if (UseAssetBundles)
                sprite = assetBundle.LoadAsset<Sprite>(objName);
            else
                sprite = (Sprite)AssetDatabase.LoadAssetAtPath(localPath, typeof(Sprite));
#else
            sprite = assetBundle.LoadAsset<Sprite>(objName);
#endif
            return sprite;
        }

        public GameObject LoadInGameUI()
        {
            return Instantiate(loadGameObject(m_commonBundleUI, "UI - In Game", "Assets/Prefabs/" + m_commonUIBundlePath + "/UI - In Game.prefab"));
        }

        public MainMenuVisual LoadMainMenuVisual()
        {
            return Instantiate(loadGameObject(m_commonBundle, "MainMenuVisual", "Assets/Prefabs/Common/MainMenuVisual.prefab")).GetComponent<MainMenuVisual>();
        }

        public GameObject LoadMainMenuUI()
        {
            return Instantiate(loadGameObject(m_commonBundleUI, "UI - Main Menu", "Assets/Prefabs/" + m_commonUIBundlePath + "/UI - Main Menu.prefab"));
        }

        public RoundSelectionVisual LoadRoundSelectionVisual()
        {
            return Instantiate(loadGameObject(m_commonBundle, "RoundSelectionVisual", "Assets/Prefabs/Common/RoundSelectionVisual.prefab")).GetComponent<RoundSelectionVisual>();
        }

        public GameObject LoadRoundSelectionUI()
        {
            return Instantiate(loadGameObject(m_commonBundleUI, "UI - Round Selection", "Assets/Prefabs/" + m_commonUIBundlePath + "/UI - Round Selection.prefab"));
        }

        public RoundCompleteVisual LoadRoundCompleteVisual()
        {
            return Instantiate(loadGameObject(m_commonBundle, "RoundCompleteVisual", "Assets/Prefabs/Common/RoundCompleteVisual.prefab")).GetComponent<RoundCompleteVisual>();
        }

        public GameObject LoadRoundCompleteUI()
        {
            return Instantiate(loadGameObject(m_commonBundleUI, "UI - Round Complete", "Assets/Prefabs/" + m_commonUIBundlePath + "/UI - Round Complete.prefab"));
        }

        public GameOverVisual LoadGameOverVisual()
        {
            return Instantiate(loadGameObject(m_commonBundle, "GameOverVisual", "Assets/Prefabs/Common/GameOverVisual.prefab")).GetComponent<GameOverVisual>();
        }

        public GameObject LoadGameOverUI()
        {
            return Instantiate(loadGameObject(m_commonBundleUI, "UI - Game Over", "Assets/Prefabs/" + m_commonUIBundlePath + "/UI - Game Over.prefab"));
        }

        public ShopVisual LoadShopVisual()
        {
            return Instantiate(loadGameObject(m_commonBundle, "ShopVisual", "Assets/Prefabs/Common/ShopVisual.prefab")).GetComponent<ShopVisual>();
        }

        public GameObject LoadShopUI()
        {
            return Instantiate(loadGameObject(m_commonBundleUI, "UI - Shop", "Assets/Prefabs/" + m_commonUIBundlePath + "/UI - Shop.prefab"));
        }

        public CardPackBallVisual LoadCardPackBallVisual()
        {
            return Instantiate(loadGameObject(m_commonBundle, "CardPackBallVisual", "Assets/Prefabs/Common/CardPackBallVisual.prefab")).GetComponent<CardPackBallVisual>();
        }

        public GameObject LoadCardPackBallUI()
        {
            return Instantiate(loadGameObject(m_commonBundleUI, "UI - Card Pack Balls", "Assets/Prefabs/" + m_commonUIBundlePath + "/UI - Card Pack Balls.prefab"));
        }

        public CardPackSlotVisual LoadCardPackSlotlVisual()
        {
            return Instantiate(loadGameObject(m_commonBundle, "CardPackSlotVisual", "Assets/Prefabs/Common/CardPackSlotVisual.prefab")).GetComponent<CardPackSlotVisual>();
        }

        public GameObject LoadCardPackSlotUI()
        {
            return Instantiate(loadGameObject(m_commonBundleUI, "UI - Card Pack Slot", "Assets/Prefabs/" + m_commonUIBundlePath + "/UI - Card Pack Slot.prefab"));
        }

        public CardPackChipsVisual LoadCardPackChipsVisual()
        {
            return Instantiate(loadGameObject(m_commonBundle, "CardPackChipsVisual", "Assets/Prefabs/Common/CardPackChipsVisual.prefab")).GetComponent<CardPackChipsVisual>();
        }

        public GameObject LoadCardPackChipsUI()
        {
            return Instantiate(loadGameObject(m_commonBundleUI, "UI - Card Pack Chips", "Assets/Prefabs/" + m_commonUIBundlePath + "/UI - Card Pack Chips.prefab"));
        }

        public JokerInfoPopupVisual LoadJokerInfoPopupVisual()
        {
            return Instantiate(loadGameObject(m_commonBundle, "JokerInfoPopupVisual", "Assets/Prefabs/Common/JokerInfoPopupVisual.prefab")).GetComponent<JokerInfoPopupVisual>();
        }

        public GameObject LoadJokerInfoPopupUI()
        {
            return Instantiate(loadGameObject(m_commonBundleUI, "UI - Joker Info Popup", "Assets/Prefabs/" + m_commonUIBundlePath + "/UI - Joker Info Popup.prefab"));
        }

        public BallScreenVisual LoadBallScreenVisual()
        {
            return Instantiate(loadGameObject(m_commonBundle, "BallScreenVisual", "Assets/Prefabs/Common/BallScreenVisual.prefab")).GetComponent<BallScreenVisual>();
        }

        public GameObject LoadBallScreenUI()
        {
            return Instantiate(loadGameObject(m_commonBundleUI, "UI - Ball Screen", "Assets/Prefabs/" + m_commonUIBundlePath + "/UI - Ball Screen.prefab"));
        }

        public SettingsVisual LoadSettingsVisual()
        {
            return Instantiate(loadGameObject(m_commonBundle, "SettingsVisual", "Assets/Prefabs/Common/SettingsVisual.prefab")).GetComponent<SettingsVisual>();
        }

        public GameObject LoadSettingsUI()
        {
            return Instantiate(loadGameObject(m_commonBundleUI, "UI - Settings", "Assets/Prefabs/" + m_commonUIBundlePath + "/UI - Settings.prefab"));
        }

        public WinScreenVisual LoadWinScreenVisual()
        {
            return Instantiate(loadGameObject(m_commonBundle, "WinScreenVisual", "Assets/Prefabs/Common/WinScreenVisual.prefab")).GetComponent<WinScreenVisual>();
        }

        public GameObject LoadWinScreenUI()
        {
            return Instantiate(loadGameObject(m_commonBundleUI, "UI - Win Screen", "Assets/Prefabs/" + m_commonUIBundlePath + "/UI - Win Screen.prefab"));
        }

        public ChipsInfoVisual LoadChipsInfoVisual()
        {
            return Instantiate(loadGameObject(m_commonBundle, "ChipsInfoVisual", "Assets/Prefabs/Common/ChipsInfoVisual.prefab")).GetComponent<ChipsInfoVisual>();
        }

        public GameObject LoadChipsInfoUI()
        {
            return Instantiate(loadGameObject(m_commonBundleUI, "UI - Chips Info", "Assets/Prefabs/" + m_commonUIBundlePath + "/UI - Chips Info.prefab"));
        }

        public GameInfoVisual LoadGameInfoVisual()
        {
            return Instantiate(loadGameObject(m_commonBundle, "GameInfoVisual", "Assets/Prefabs/Common/GameInfoVisual.prefab")).GetComponent<GameInfoVisual>();
        }

        public GameObject LoadGameInfoUI()
        {
            return Instantiate(loadGameObject(m_commonBundleUI, "UI - In Game Info", "Assets/Prefabs/" + m_commonUIBundlePath + "/UI - In Game Info.prefab"));
        }

        public ShopInfoVisual LoadShopInfoVisual()
        {
            return Instantiate(loadGameObject(m_commonBundle, "ShopInfoVisual", "Assets/Prefabs/Common/ShopInfoVisual.prefab")).GetComponent<ShopInfoVisual>();
        }

        public GameObject LoadShopInfoUI()
        {
            return Instantiate(loadGameObject(m_commonBundleUI, "UI - Shop Info", "Assets/Prefabs/" + m_commonUIBundlePath + "/UI - Shop Info.prefab"));
        }

        public WheelSelectionVisual LoadWheelSelectionVisual()
        {
            return Instantiate(loadGameObject(m_commonBundle, "WheelSelectionVisual", "Assets/Prefabs/Common/WheelSelectionVisual.prefab")).GetComponent<WheelSelectionVisual>();
        }

        public GameObject LoadWheelSelectionUI()
        {
            return Instantiate(loadGameObject(m_commonBundleUI, "UI - Wheel Selection", "Assets/Prefabs/" + m_commonUIBundlePath + "/UI - Wheel Selection.prefab"));
        }


        public GameObject LoadJokerPrefab()
        {
            return Instantiate(loadGameObject(m_commonBundle, "Joker", "Assets/Prefabs/Common/Joker.prefab"));
        }

        public Sprite LoadJokerSprite(string name)
        {
            return loadSprite(m_commonBundle, name, "Assets/Textures/Jokers/" + name + ".png");
        }

        public Sprite LoadBallCardSprite()
        {
            return loadSprite(m_commonBundle, "Ball Card", "Assets/Textures/Cards/Ball Card.png");
        }

        public Sprite LoadSlotCardSprite()
        {
            return loadSprite(m_commonBundle, "Slot Card", "Assets/Textures/Cards/Slot Card.png");
        }

        public Sprite LoadChipsCardSprite()
        {
            return loadSprite(m_commonBundle, "Chips Card", "Assets/Textures/Cards/Chips Card.png");
        }

        public Sprite LoadBallCardPackSprite()
        {
            return loadSprite(m_commonBundle, "CardPack_Balls", "Assets/Textures/CardPack/CardPack_Balls.png");
        }

        public Sprite LoadSlotCardPackSprite()
        {
            return loadSprite(m_commonBundle, "CardPack_Slots", "Assets/Textures/CardPack/CardPack_Slots.png");
        }

        public Sprite LoadChipsCardPackSprite()
        {
            return loadSprite(m_commonBundle, "CardPack_SlotChips", "Assets/Textures/CardPack/CardPack_SlotChips.png");
        }

        public Sprite LoadBallSprite(string name)
        {
            return loadSprite(m_commonBundle, name, "Assets/Textures/Balls/" + name + ".png");
        }

        public GameObject GetDescriptionGO(string name, Transform parent)
        {
            return Instantiate(loadGameObject(m_commonBundle, name, "Assets/Prefabs/Descriptions/" + name + ".prefab"), parent);
        }

        public Sprite LoadVoucherSprite(string name)
        {
            return loadSprite(m_commonBundle, name, "Assets/Textures/Vouchers/" + name + ".png");
        }
    }
}
