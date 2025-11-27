using System;
using System.IO;
using CommonTools;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;
using UnityEngine.InputSystem.DualShock;

// #if STEAM
// using Steamworks;
// #endif

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Cardwheel
{
    public enum MENU_STATE
    {
        NONE,
        LOADING,
        MAIN_MENU,
        SETTINGS,
        WHEEL_SELECTION,
        IN_GAME,
        ROUND_SELECTION,
        ROUND_COMPLETE,
        SHOP,
        CARD_PACK_BALL,
        BALL_SCREEN,
        CARD_PACK_SLOT,
        CARD_PACK_CHIPS,
        CHIPS_INFO,
        IN_GAME_INFO,
        SHOP_INFO,
        JOKER_INFO_POPUP,
        GAME_OVER,
        WIN_SCREEN,
        LAST, // should always be last!
    };

    public enum INPUT_TYPES { KEYBOARD, GAMEPAD };

    public class Game : Singleton<Game>
    {
        public Board Board;

        public Camera Camera;

        public GameObject UIDebug;

        public string CommonBundle;
        public GameInfoSO GameInfoSO;

        MainMenuVisual m_mainMenuVisual;
        RoundSelectionVisual m_roundSelectionVisual;
        RoundCompleteVisual m_roundCompleteVisual;
        GameOverVisual m_gameOverVisual;
        ShopVisual m_shopVisual;
        CardPackBallVisual m_cardPackBallVisual;
        CardPackSlotVisual m_cardPackSlotVisual;
        CardPackChipsVisual m_cardPackChipsVisual;
        JokerInfoPopupVisual m_jokerInfoPopupVisual;
        BallScreenVisual m_ballScreenVisual;
        SettingsVisual m_settingsVisual;
        WinScreenVisual m_winScreenVisual;
        ChipsInfoVisual m_chipsInfoVisual;
        GameInfoVisual m_gameInfoVisual;
        ShopInfoVisual m_shopInfoVisual;
        WheelSelectionVisual m_wheelSelectionVisual;
        TutorialVisual m_tutorialVisual;

        RunData m_runData;
        SettingsData m_settingsData;
        GameData m_gameData;
        Balance m_balance;

        [Header("Set to 0 for Random")]
        public uint StartSeed;

        public GAMEPAD_TYPE m_gamepadType = GAMEPAD_TYPE.NONE;

        public int[] LastSelectedMenuButton;

        void gamepadInit()
        {
            // #if STEAM
            //             bool steamInit = SteamAPI.Init();
            //             if (!steamInit)
            //             {
            //                 Debug.LogError("[Steamworks.NET] SteamAPI_Init() failed. Refer to Valve's documentation or the comment above this line for more information.", this);
            //             }
            //             else
            //                 Debug.Log("Steam Init Success");

            // #endif
        }

        void gamepadCheck()
        {
            m_gamepadType = GAMEPAD_TYPE.NONE;

            if (GameInfoSO.KeyboardGamepadSupport)
            {
                if (Gamepad.current != null)
                {
                    string gamepadString = Gamepad.current.description.ToJson();

                    if (Gamepad.current is XInputController)
                        m_gamepadType = GAMEPAD_TYPE.XBOX;
                    else if (Gamepad.current is DualShockGamepad)
                        m_gamepadType = GAMEPAD_TYPE.PS5;
                    else if (gamepadString.Contains("Steam"))
                        m_gamepadType = GAMEPAD_TYPE.STEAM;

                    Cursor.visible = false;
                }
            }
        }

        public GAMEPAD_TYPE GetGamepadType()
        {
            return m_gamepadType;
        }

        override protected void Awake()
        {
            base.Awake();

            Application.targetFrameRate = 60;

            gamepadInit();

            UIDebug.SetActive(false);

            LastSelectedMenuButton = new int[(int)MENU_STATE.LAST];

            AssetManager.Instance.LoadCommonAssetBundle();
            AssetManager.Instance.LoadCommonUIAssetBundle(GameInfoSO.CommonBundle, GameInfoSO.CommonBundleUIPath);

            m_settingsData = new SettingsData();
            m_settingsData.SFX = m_settingsData.Music = m_settingsData.Vibrate = true;
            m_settingsData.SkipRound1 = false;
            m_settingsData.Speed = 1.0f;
            SettingsDataIO.LoadSettings(m_settingsData);

            m_runData = new RunData();

            string encodedSeed = Logic.EncodeSeed(StartSeed);
            uint decodedSeed = Logic.DecodeSeed(encodedSeed);
            Debug.Log("Start seed " + StartSeed + " Encdoed " + encodedSeed + " decoded " + decodedSeed);

            m_balance = new Balance();
            m_balance.LoadBalance();
            Logic.AllocateRunData(m_runData, m_balance);
            Board.Init(m_runData, m_balance, GameInfoSO, m_settingsData, Camera);

            m_gameData = new GameData();

            tryLoadGameData();

#if UNITY_EDITOR
            // m_gameData.MenuTutorialFlags = 0;
#endif

            CommonVisual.InitJokers(m_balance);
            CommonVisual.InitBalls(m_balance);
            CommonSlotsVisual.Init(m_balance);

            m_mainMenuVisual = AssetManager.Instance.LoadMainMenuVisual();
            m_roundSelectionVisual = AssetManager.Instance.LoadRoundSelectionVisual();
            m_roundCompleteVisual = AssetManager.Instance.LoadRoundCompleteVisual();
            m_gameOverVisual = AssetManager.Instance.LoadGameOverVisual();
            m_shopVisual = AssetManager.Instance.LoadShopVisual();
            m_cardPackBallVisual = AssetManager.Instance.LoadCardPackBallVisual();
            m_cardPackSlotVisual = AssetManager.Instance.LoadCardPackSlotlVisual();
            m_cardPackChipsVisual = AssetManager.Instance.LoadCardPackChipsVisual();
            m_jokerInfoPopupVisual = AssetManager.Instance.LoadJokerInfoPopupVisual();
            m_ballScreenVisual = AssetManager.Instance.LoadBallScreenVisual();
            m_settingsVisual = AssetManager.Instance.LoadSettingsVisual();
            m_winScreenVisual = AssetManager.Instance.LoadWinScreenVisual();
            m_chipsInfoVisual = AssetManager.Instance.LoadChipsInfoVisual();
            m_gameInfoVisual = AssetManager.Instance.LoadGameInfoVisual();
            m_shopInfoVisual = AssetManager.Instance.LoadShopInfoVisual();
            m_wheelSelectionVisual = AssetManager.Instance.LoadWheelSelectionVisual();
            m_tutorialVisual = new TutorialVisual();

            m_mainMenuVisual.Init(Camera, m_balance);
            m_roundSelectionVisual.Init(m_runData, m_balance, Camera);
            m_roundCompleteVisual.Init(m_gameData, m_runData, m_balance, Camera);
            m_gameOverVisual.Init(m_runData, Camera);
            m_shopVisual.Init(m_runData, m_balance, Camera);
            m_cardPackBallVisual.Init(m_runData, m_balance, Camera);
            m_cardPackSlotVisual.Init(m_runData, m_balance, Camera);
            m_cardPackChipsVisual.Init(m_runData, m_balance, Camera);
            m_jokerInfoPopupVisual.Init(m_runData, m_balance, Camera);
            m_ballScreenVisual.Init(m_runData, m_balance, Camera);
            m_settingsVisual.Init(m_runData, m_balance, Camera, m_settingsData);
            m_winScreenVisual.Init(m_runData, m_balance, Camera);
            m_chipsInfoVisual.Init(Camera);
            m_gameInfoVisual.Init(Camera, m_balance);
            m_shopInfoVisual.Init(Camera);
            m_wheelSelectionVisual.Init(Camera, m_gameData, m_balance);
            m_tutorialVisual.Init(m_gameData, m_runData, m_balance, Camera);

            MusicManager.Instance.Init(m_settingsData);
            MusicManager.Instance.PlayMusic();

            SetMenuState(MENU_STATE.MAIN_MENU);
        }

        private void tryLoadGameData()
        {
            if (GameDataIO.LoadGameData(m_gameData, m_balance))
            {
                Debug.Log("loaded gamedata v4");
                // loaded v4
            }
            else if (GameDataIOV3.LoadGameData(m_gameData, m_balance))
            {
                Debug.Log("loaded gamedata v3");
                // loaded v3
            }
            else
                Logic.AllocateGameData(m_gameData, m_balance);
        }

        public void Start()
        {
            SoundManager.Instance.Init(m_settingsData);
        }

        public void SetMenuState(MENU_STATE newMenuState)
        {
            bool goingBackToPrevMenu = newMenuState == m_runData.PrevMenuState;

            Debug.Log("SetMenuState(" + newMenuState.ToString() + ")");
            if (newMenuState != m_runData.MenuState)
                Logic.SetMenuState(m_runData, newMenuState);

            if (newMenuState > MENU_STATE.IN_GAME)
            {
                RunDataIO.SaveRun(m_runData, m_balance);
            }

            hideMenuState(m_runData.PrevMenuState);
            showMenuState(m_runData.MenuState, goingBackToPrevMenu);
        }

        void hideMenuState(MENU_STATE menuState)
        {
            if (menuState == MENU_STATE.MAIN_MENU)
                m_mainMenuVisual.Hide();
            else if (menuState == MENU_STATE.ROUND_SELECTION)
                m_roundSelectionVisual.Hide();
            else if (menuState == MENU_STATE.ROUND_COMPLETE)
                m_roundCompleteVisual.Hide();
            else if (menuState == MENU_STATE.GAME_OVER)
                m_gameOverVisual.Hide();
            else if (menuState == MENU_STATE.SHOP)
                m_shopVisual.Hide();
            else if (menuState == MENU_STATE.CARD_PACK_BALL)
                m_cardPackBallVisual.Hide();
            else if (menuState == MENU_STATE.CARD_PACK_SLOT)
                m_cardPackSlotVisual.Hide();
            else if (menuState == MENU_STATE.CARD_PACK_CHIPS)
                m_cardPackChipsVisual.Hide();
            else if (menuState == MENU_STATE.IN_GAME)
                Board.Hide();
            else if (menuState == MENU_STATE.BALL_SCREEN)
                m_ballScreenVisual.Hide();
            else if (menuState == MENU_STATE.SETTINGS)
                m_settingsVisual.Hide();
            else if (menuState == MENU_STATE.WIN_SCREEN)
                m_winScreenVisual.Hide();
            else if (menuState == MENU_STATE.CHIPS_INFO)
                m_chipsInfoVisual.Hide();
            else if (menuState == MENU_STATE.IN_GAME_INFO)
                m_gameInfoVisual.Hide();
            else if (menuState == MENU_STATE.SHOP_INFO)
                m_shopInfoVisual.Hide();
            else if (menuState == MENU_STATE.WHEEL_SELECTION)
                m_wheelSelectionVisual.Hide();
            else if (menuState == MENU_STATE.JOKER_INFO_POPUP)
                m_jokerInfoPopupVisual.Hide();
        }

        void showMenuState(MENU_STATE menuState, bool goingBackToPrevMenu)
        {
            gamepadCheck();

            int prevSelectedMenuButton = LastSelectedMenuButton[(int)menuState];

            if (menuState == MENU_STATE.MAIN_MENU)
                m_mainMenuVisual.Show(m_gameData);
            else if (menuState == MENU_STATE.ROUND_SELECTION)
            {
                m_roundSelectionVisual.Show();
                if (goingBackToPrevMenu)
                    m_roundSelectionVisual.SelectPrevButton((RoundSelectionVisual.MENU_BUTTONS)prevSelectedMenuButton);
            }
            else if (menuState == MENU_STATE.ROUND_COMPLETE)
            {
                m_roundCompleteVisual.Show();
                if (goingBackToPrevMenu)
                    m_roundCompleteVisual.SelectPrevButton((RoundCompleteVisual.MENU_BUTTONS)prevSelectedMenuButton);
            }
            else if (menuState == MENU_STATE.GAME_OVER)
                m_gameOverVisual.Show();
            else if (menuState == MENU_STATE.SHOP)
            {
                m_shopVisual.Show();
                if (goingBackToPrevMenu)
                    m_shopVisual.SelectPrevButton((ShopVisual.SHOP_MENU_BUTTONS)prevSelectedMenuButton);
            }
            else if (menuState == MENU_STATE.CARD_PACK_BALL)
                m_cardPackBallVisual.Show();
            else if (menuState == MENU_STATE.CARD_PACK_SLOT)
                m_cardPackSlotVisual.Show();
            else if (menuState == MENU_STATE.CARD_PACK_CHIPS)
                m_cardPackChipsVisual.Show();
            else if (menuState == MENU_STATE.IN_GAME)
            {
                Board.Show();
                if (goingBackToPrevMenu)
                    Board.SelectPrevButton((Board.MENU_BUTTONS)prevSelectedMenuButton);
            }
            else if (menuState == MENU_STATE.BALL_SCREEN)
                m_ballScreenVisual.Show();
            else if (menuState == MENU_STATE.SETTINGS)
                m_settingsVisual.Show();
            else if (menuState == MENU_STATE.WIN_SCREEN)
            {
                m_winScreenVisual.Show();
                if (goingBackToPrevMenu)
                    m_winScreenVisual.SelectPrevButton((WinScreenVisual.MENU_BUTTONS)prevSelectedMenuButton);
            }
            else if (menuState == MENU_STATE.CHIPS_INFO)
                m_chipsInfoVisual.Show(m_runData, m_balance);
            else if (menuState == MENU_STATE.IN_GAME_INFO)
                m_gameInfoVisual.Show(m_runData, m_balance);
            else if (menuState == MENU_STATE.SHOP_INFO)
                m_shopInfoVisual.Show(m_runData, m_balance);
            else if (menuState == MENU_STATE.WHEEL_SELECTION)
                m_wheelSelectionVisual.Show();
            else if (menuState == MENU_STATE.JOKER_INFO_POPUP)
            {
                // has to be shown after setMenuState;
            }

            if (!Logic.IsFlagSet(m_gameData.MenuTutorialFlags, (int)menuState) && m_balance.MenuTutorialText[(int)menuState].Length > 0)
                m_tutorialVisual.Show();
            else
                m_tutorialVisual.Hide();
        }

        void OnEnable()
        {
#if !RELEASE
            Application.logMessageReceived += HandleLog;
#endif
        }

        void OnDisable()
        {
#if !RELEASE
            Application.logMessageReceived -= HandleLog;
#endif
        }

        void HandleLog(string logString, string stackTrace, LogType type)
        {
            if (type == LogType.Error || type == LogType.Exception)
            {
                if (logString.Contains("<RI.Hid> Failed to create device file"))
                    return;

                UIDebug.SetActive(true);
                GUIRef guiRef = UIDebug.GetComponent<GUIRef>();
                guiRef.GetTextGUI("DebugText").text = type.ToString() + "\n\n" + logString + "\n\n" + stackTrace;
                Canvas.ForceUpdateCanvases();

#if UNITY_EDITOR
                EditorApplication.isPaused = true;
                Time.timeScale = 1.0f;
#endif
            }
        }

        // Update is called once per frame
        void Update()
        {
            float dt = Time.deltaTime;

            // #if STEAM
            //             SteamInput.RunFrame();
            // #endif

            if (!Logic.IsFlagSet(m_gameData.MenuTutorialFlags, (int)m_runData.MenuState))
                if (m_tutorialVisual.TutorialClosed())
                    return;

            if (m_runData.MenuState == MENU_STATE.IN_GAME)
            {
                Board.Tick(dt);
            }
            else if (m_runData.MenuState == MENU_STATE.BALL_SCREEN)
            {
                m_ballScreenVisual.Tick(dt);
            }
            else if (m_runData.MenuState == MENU_STATE.CARD_PACK_BALL)
            {
                m_cardPackBallVisual.Tick(dt);
            }
            else if (m_runData.MenuState == MENU_STATE.CARD_PACK_SLOT)
            {
                m_cardPackSlotVisual.Tick(dt);
            }
            else if (m_runData.MenuState == MENU_STATE.CARD_PACK_CHIPS)
            {
                m_cardPackChipsVisual.Tick(dt);
            }
            else if (m_runData.MenuState == MENU_STATE.SHOP)
            {
                m_shopVisual.Tick(dt);
            }
            else if (m_runData.MenuState == MENU_STATE.ROUND_SELECTION)
            {
                m_roundSelectionVisual.Tick(dt);
            }
            else if (m_runData.MenuState == MENU_STATE.ROUND_COMPLETE)
            {
                m_roundCompleteVisual.Tick(dt);
            }
            else if (m_runData.MenuState == MENU_STATE.IN_GAME_INFO)
            {
                m_gameInfoVisual.Tick(m_runData, dt);
            }
            else if (m_runData.MenuState == MENU_STATE.SHOP_INFO)
            {
                m_shopInfoVisual.Tick(m_runData, dt);
            }
            else if (m_runData.MenuState == MENU_STATE.MAIN_MENU)
            {
                m_mainMenuVisual.Tick(m_balance, dt);
            }
            else if (m_runData.MenuState == MENU_STATE.SETTINGS)
            {
                m_settingsVisual.Tick(dt);
            }
            else if (m_runData.MenuState == MENU_STATE.CHIPS_INFO)
            {
                m_chipsInfoVisual.Tick(m_runData, dt);
            }
            else if (m_runData.MenuState == MENU_STATE.WHEEL_SELECTION)
            {
                m_wheelSelectionVisual.Tick(dt);
            }
            else if (m_runData.MenuState == MENU_STATE.JOKER_INFO_POPUP)
            {
                m_jokerInfoPopupVisual.Tick();
            }
            else if (m_runData.MenuState == MENU_STATE.GAME_OVER)
            {
                m_gameOverVisual.Tick();
            }
            else if (m_runData.MenuState == MENU_STATE.WIN_SCREEN)
            {
                m_winScreenVisual.Tick(dt);
            }


#if UNITY_EDITOR
            if (Keyboard.current.sKey.wasPressedThisFrame)
            {
                if (!Directory.Exists("Screenshots"))
                    Directory.CreateDirectory("Screenshots");

                DateTimeOffset now = DateTime.UtcNow;
                string name = "Screenshots/" + Screen.width + "x" + Screen.height + "_" + now.ToString("yyyy-MM-dd HH.mm.ss") + ".png";
                ScreenCapture.CaptureScreenshot(name);
            }

            if (Keyboard.current.xKey.wasPressedThisFrame)
                m_runData.TotalChips += 100000;

            if (Keyboard.current.cKey.wasPressedThisFrame)
                RoundComplete();

            if (Keyboard.current.oKey.wasPressedThisFrame)
                ContinueRun();



            if (Keyboard.current.mKey.wasPressedThisFrame)
                m_runData.Money += 100;

            if (Keyboard.current.rKey.wasPressedThisFrame)
                m_runData.BossRerolls++;

            if (Keyboard.current.wKey.wasPressedThisFrame)
                WinScreen();

            if (Keyboard.current.gKey.wasPressedThisFrame)
                SetMenuState(MENU_STATE.GAME_OVER);

            if (Keyboard.current.pKey.wasPressedThisFrame)
                EditorApplication.isPaused = true;

            if (Keyboard.current.dKey.wasPressedThisFrame)
            {
                gamepadCheck();
            }
#endif
        }

        void LateUpdate()
        {
            float dt = Time.deltaTime;

            if (m_runData.MenuState == MENU_STATE.BALL_SCREEN)
            {
                m_ballScreenVisual.HandleTouchInput();
            }
            else if (m_runData.MenuState == MENU_STATE.CARD_PACK_BALL)
            {
                m_cardPackBallVisual.HandleTouchInput();
            }
        }

        public void GoToMainMenu()
        {
            SoundManager.Instance.PlaySFXButtonOK();

            SetMenuState(MENU_STATE.MAIN_MENU);
        }

        public void RetryRun()
        {
            SoundManager.Instance.PlaySFXButtonOK();

            StartGame(m_runData.StartSeed, m_runData.WheelIdx);
        }

        public void StartNewRunSameWheel()
        {
            StartNewRun(m_runData.WheelIdx);
        }

        public void StartNewRun(int wheelIdx)
        {
            SoundManager.Instance.PlaySFXButtonOK();

            uint seed = (uint)Mathf.FloorToInt(UnityEngine.Random.value * int.MaxValue);
#if UNITY_EDITOR
            if (StartSeed > 0)
                seed = StartSeed;
#endif
            StartGame(seed, wheelIdx);
        }

        public void ContinueRun()
        {
            hideMenuState(m_runData.MenuState);

            if (RunDataIO.LoadRun(m_runData))
            {
                Debug.Log("Loaded rundata v5");
                // loaded v5
            }
            else if (RunDataIOV4.LoadRun(m_runData))
            {
                Debug.Log("Loaded rundata v4");
                // loaded v4
            }

            if (m_runData.MenuState == MENU_STATE.JOKER_INFO_POPUP)
                m_runData.MenuState = m_runData.PrevMenuState;
            showMenuState(m_runData.MenuState, false);
        }

        public void StartGame(uint seed, int wheelIdx)
        {
            Logic.StartNewGame(m_runData, m_balance, wheelIdx, seed);
            if (m_settingsData.SkipRound1)
                SkipRound1();
            else
                SetMenuState(MENU_STATE.ROUND_SELECTION);
        }

        public void StartRound()
        {
#if UNITY_IOS || UNITY_ANDROID
            // GoogleAdsManager.Instance.LoadInterstitialAd();
#endif
            SoundManager.Instance.PlaySFXButtonOK();

            Board.StartRound(m_runData, m_balance);
            SetMenuState(MENU_STATE.IN_GAME);
        }

        public void RoundComplete()
        {
            SoundManager.Instance.PlaySFXWinRound();

            Logic.RoundComplete(m_runData, m_balance);
            SetMenuState(MENU_STATE.ROUND_COMPLETE);
        }

        public void WinScreen()
        {
            SoundManager.Instance.PlaySFXWinGame();

            Logic.WinGame(m_gameData, m_runData);

            Logic.RoundComplete(m_runData, m_balance);
            SetMenuState(MENU_STATE.WIN_SCREEN);

            GameDataIO.SaveGameData(m_gameData);
        }

        public void GameOver()
        {
            SoundManager.Instance.PlaySFXGameOver();

            Logic.GameOver(m_gameData);

            SetMenuState(MENU_STATE.GAME_OVER);

            GameDataIO.SaveGameData(m_gameData);
        }

        public void BallInSlot(int ballIdx, int slotIdx)
        {
            Board.BallInSlot(m_runData, m_balance, ballIdx, slotIdx);

            SoundManager.Instance.PlaySFXMarbleInSlot();
        }

        public void SkipRound1()
        {
            m_runData.CurrentSpin = 3;
            Logic.ClaimRoundReward(m_runData, m_balance);
            Logic.PopulateShop(m_runData, m_balance);
            SetMenuState(MENU_STATE.SHOP);
        }

        public void ShowJokerInfoPopup(int jokerIdx)
        {
            SoundManager.Instance.PlaySFXButtonOK();

            SetMenuState(MENU_STATE.JOKER_INFO_POPUP);
            m_jokerInfoPopupVisual.Show(jokerIdx);
        }

        public void ShowJokerInfoPopupInGame(int jokerIdx)
        {
            SoundManager.Instance.PlaySFXButtonOK();

            SetMenuState(MENU_STATE.JOKER_INFO_POPUP);
            m_jokerInfoPopupVisual.ShowInGame(jokerIdx);
        }

        public void ShowJokerInfoPopupFromWinScreen(int jokerIdx)
        {
            SoundManager.Instance.PlaySFXButtonOK();

            SetMenuState(MENU_STATE.JOKER_INFO_POPUP);
            m_jokerInfoPopupVisual.ShowFromWinGame(jokerIdx);
        }

        public void AbandonCardPack()
        {
            SoundManager.Instance.PlaySFXButtonOK();

            Logic.AbandonCardPack(m_runData);

            SetMenuState(m_runData.PrevMenuState);
        }

        public void RerollCardPack()
        {
            SoundManager.Instance.PlaySFXMoney();

            if (m_balance.CardPackType[m_runData.SelectedShopCardPackIdx] == CARD_PACK_TYPE.BALL)
                m_cardPackBallVisual.Reroll();
            if (m_balance.CardPackType[m_runData.SelectedShopCardPackIdx] == CARD_PACK_TYPE.SLOT)
                m_cardPackSlotVisual.Reroll();
            if (m_balance.CardPackType[m_runData.SelectedShopCardPackIdx] == CARD_PACK_TYPE.CHIPS)
                m_cardPackChipsVisual.Reroll();

            RunDataIO.SaveRun(m_runData, m_balance);
        }

        public void GoToBallScreen()
        {
            SoundManager.Instance.PlaySFXButtonOK();

            SetMenuState(MENU_STATE.BALL_SCREEN);
        }

        public void CopySeed()
        {
            TextEditor te = new TextEditor();
            te.text = Logic.EncodeSeed(m_runData.StartSeed);
            te.SelectAll();
            te.Copy();
        }

        public void GoToSettings()
        {
            SoundManager.Instance.PlaySFXButtonOK();

            SetMenuState(MENU_STATE.SETTINGS);
        }

        public void BallBallCollision()
        {
            // Debug.Log("ball ball collision");

            SoundManager.Instance.PlaySFXMarbleMarble();
        }

        public void BallSpinWheelCollision()
        {
            // Debug.Log("ball spinwheel collision");

            SoundManager.Instance.PlaySFXMarbleSlot();
        }

        public void GoToChipsInfo()
        {
            SoundManager.Instance.PlaySFXButtonOK();

            SetMenuState(MENU_STATE.CHIPS_INFO);
        }

        public void GoToPrivacyPolicy()
        {
            Application.OpenURL("https://nitzanwilnai.github.io/PrivacyPolicy/Cardwheel");
        }

        public void ExitGame()
        {
            Application.Quit();
        }
    }
}