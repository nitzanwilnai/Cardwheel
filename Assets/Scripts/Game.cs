using System;
using System.IO;
using CommonTools;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using Lofelt.NiceVibrations;

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
    };

    public class Game : Singleton<Game>
    {
        public Board Board;

        public Camera Camera;

        public GameObject UIDebug;

        public AudioClip MusicClip;

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

        RunData m_runData;
        SettingsData m_settingsData;
        GameData m_gameData;
        Balance m_balance;

        [Header("Set to 0 for Random")]
        public uint StartSeed;


        override protected void Awake()
        {
            base.Awake();

            UIDebug.SetActive(false);

            Debug.Log("vector2 " + System.Runtime.InteropServices.Marshal.SizeOf(typeof(Vector2)));

            AssetManager.Instance.LoadCommonAssetBundle();
            AssetManager.Instance.LoadCommonUIAssetBundle(GameInfoSO.CommonBundle, GameInfoSO.CommonBundleUIPath);

            m_settingsData = new SettingsData();
            m_settingsData.SFX = m_settingsData.Music = m_settingsData.Vibrate = true;
            m_settingsData.SkipRound1 = false;
            m_settingsData.Speed = 1.0f;
            SettingsDataIO.LoadSettings(m_settingsData);

            m_runData = new RunData();

            string s = Logic.EncodeSeed(StartSeed);
            uint ui = Logic.DecodeSeed(s);
            Debug.Log(StartSeed);
            Debug.Log(s);
            Debug.Log(ui);

            m_balance = new Balance();
            m_balance.LoadBalance();
            Logic.AllocateRunData(m_runData, m_balance);
            Board.Init(m_balance, GameInfoSO);

            m_gameData = new GameData();
            if (!GameDataIO.LoadGameData(m_gameData, m_balance))
                Logic.AllocateGameData(m_gameData, m_balance);

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

            m_mainMenuVisual.Init(Camera, m_balance);
            m_roundSelectionVisual.Init(m_runData, m_balance, Camera);
            m_roundCompleteVisual.Init(m_runData, m_balance, Camera);
            m_gameOverVisual.Init(Camera);
            m_shopVisual.Init(m_runData, m_balance, Camera);
            m_cardPackBallVisual.Init(m_runData, m_balance, Camera);
            m_cardPackSlotVisual.Init(m_runData, m_balance, Camera);
            m_cardPackChipsVisual.Init(m_balance, Camera);
            m_jokerInfoPopupVisual.Init(Camera);
            m_ballScreenVisual.Init(m_balance, Camera);
            m_settingsVisual.Init(Camera, m_settingsData);
            m_winScreenVisual.Init(m_runData, m_balance, Camera);
            m_chipsInfoVisual.Init(Camera);
            m_gameInfoVisual.Init(Camera, m_balance);
            m_shopInfoVisual.Init(Camera, m_balance);
            m_wheelSelectionVisual.Init(Camera, m_balance);

            MusicManager.Instance.Init(m_settingsData);
            MusicManager.Instance.PlayMusic();

            SetMenuState(MENU_STATE.MAIN_MENU);
        }

        public void SetMenuState(MENU_STATE newMenuState)
        {
            m_runData.PrevMenuState = m_runData.MenuState;
            m_runData.MenuState = newMenuState;

            if (newMenuState > MENU_STATE.IN_GAME)
            {
                RunDataIO.SaveRun(m_runData, m_balance);
            }

            hideMenuState(m_runData.PrevMenuState);
            showMenuState(m_runData.MenuState);
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
                m_cardPackBallVisual.Hide(m_balance);
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

        void showMenuState(MENU_STATE menuState)
        {
            if (menuState == MENU_STATE.MAIN_MENU)
                m_mainMenuVisual.Show(m_balance);
            else if (menuState == MENU_STATE.ROUND_SELECTION)
                m_roundSelectionVisual.Show(m_runData, m_balance);
            else if (menuState == MENU_STATE.ROUND_COMPLETE)
                m_roundCompleteVisual.Show(m_runData, m_balance);
            else if (menuState == MENU_STATE.GAME_OVER)
                m_gameOverVisual.Show(m_runData);
            else if (menuState == MENU_STATE.SHOP)
                m_shopVisual.Show(m_runData, m_balance);
            else if (menuState == MENU_STATE.CARD_PACK_BALL)
                m_cardPackBallVisual.Show(m_runData, m_balance);
            else if (menuState == MENU_STATE.CARD_PACK_SLOT)
                m_cardPackSlotVisual.Show(m_runData, m_balance);
            else if (menuState == MENU_STATE.CARD_PACK_CHIPS)
                m_cardPackChipsVisual.Show(m_runData, m_balance);
            else if (menuState == MENU_STATE.IN_GAME)
                Board.Show(m_runData, m_balance);
            else if (menuState == MENU_STATE.BALL_SCREEN)
                m_ballScreenVisual.Show(m_runData, m_balance);
            else if (menuState == MENU_STATE.SETTINGS)
                m_settingsVisual.Show(m_runData, m_balance, m_settingsData);
            else if (menuState == MENU_STATE.WIN_SCREEN)
                m_winScreenVisual.Show(m_runData, m_balance);
            else if (menuState == MENU_STATE.CHIPS_INFO)
                m_chipsInfoVisual.Show(m_runData);
            else if (menuState == MENU_STATE.IN_GAME_INFO)
                m_gameInfoVisual.Show(m_runData, m_balance);
            else if (menuState == MENU_STATE.SHOP_INFO)
                m_shopInfoVisual.Show(m_runData, m_balance);
            else if (menuState == MENU_STATE.WHEEL_SELECTION)
                m_wheelSelectionVisual.Show(m_gameData, m_balance);
            else if (menuState == MENU_STATE.JOKER_INFO_POPUP)
            {
                // has to be shown after setMenuState;
            }
        }

        void OnEnable()
        {
            Application.logMessageReceived += HandleLog;
        }

        void OnDisable()
        {
            Application.logMessageReceived -= HandleLog;
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
            if (m_runData.MenuState == MENU_STATE.IN_GAME)
            {
                Board.Tick(m_runData, m_balance, m_settingsData, dt);
            }
            if (m_runData.MenuState == MENU_STATE.BALL_SCREEN)
            {
                m_ballScreenVisual.Tick(m_runData, Camera, dt);
            }
            if (m_runData.MenuState == MENU_STATE.CARD_PACK_BALL)
            {
                m_cardPackBallVisual.Tick(m_runData, m_balance, Camera, dt);
            }
            if (m_runData.MenuState == MENU_STATE.CARD_PACK_SLOT)
            {
                m_cardPackSlotVisual.Tick(m_runData, m_balance, dt);
            }
            if (m_runData.MenuState == MENU_STATE.CARD_PACK_CHIPS)
            {
                m_cardPackChipsVisual.Tick(m_runData, m_balance, dt);
            }
            if (m_runData.MenuState == MENU_STATE.SHOP)
            {
                m_shopVisual.Tick(m_runData, m_balance, dt);
            }
            if (m_runData.MenuState == MENU_STATE.ROUND_SELECTION)
            {
                m_roundSelectionVisual.Tick(m_runData, m_balance, dt);
            }
            if (m_runData.MenuState == MENU_STATE.ROUND_COMPLETE)
            {
                m_roundCompleteVisual.Tick(m_runData, m_balance, dt);
            }
            if (m_runData.MenuState == MENU_STATE.IN_GAME_INFO)
            {
                m_gameInfoVisual.Tick(m_runData, dt);
            }
            if (m_runData.MenuState == MENU_STATE.SHOP_INFO)
            {
                m_shopInfoVisual.Tick(m_runData, dt);
            }
            if (m_runData.MenuState == MENU_STATE.MAIN_MENU)
            {
                m_mainMenuVisual.Tick(m_balance, dt);
            }
            if (m_runData.MenuState == MENU_STATE.SETTINGS)
            {
                m_settingsVisual.Tick(m_runData, dt);
            }
            if (m_runData.MenuState == MENU_STATE.CHIPS_INFO)
            {
                m_chipsInfoVisual.Tick(m_runData, dt);
            }
            if (m_runData.MenuState == MENU_STATE.WHEEL_SELECTION)
            {
                m_wheelSelectionVisual.Tick(m_gameData, m_balance, dt);
            }

#if UNITY_EDITOR
            if (Input.GetKeyUp("s"))
            {
                if (!Directory.Exists("Screenshots"))
                    Directory.CreateDirectory("Screenshots");

                DateTimeOffset now = DateTime.UtcNow;
                string name = "Screenshots/" + Screen.width + "x" + Screen.height + "_" + now.ToString("yyyy-MM-dd HH.mm.ss") + ".png";
                ScreenCapture.CaptureScreenshot(name);
            }

            if (Input.GetKeyUp(KeyCode.X))
                m_runData.TotalChips += 100000;

            if (Input.GetKeyUp(KeyCode.C))
                RoundComplete();

            if (Input.GetKeyUp(KeyCode.O))
                ContinueRun();

            if (Input.GetKeyUp(KeyCode.A))
                SetMenuState(MENU_STATE.WHEEL_SELECTION);

            // if (Input.GetKeyUp(KeyCode.N))
            //     m_runData.Money = 3;

            if (Input.GetKeyUp(KeyCode.M))
                m_runData.Money += 100;

            if (Input.GetKeyUp(KeyCode.R))
                m_runData.BossRerolls++;

            if (Input.GetKeyUp(KeyCode.W))
                WinScreen();

            if (Input.GetKeyUp(KeyCode.G))
                SetMenuState(MENU_STATE.GAME_OVER);

            if (Input.GetKeyUp(KeyCode.P))
                EditorApplication.isPaused = true;

            if (Input.GetKeyUp(KeyCode.Space))
                DropBalls();

            if (Input.GetKeyUp(KeyCode.S))
            {
                if (m_runData.MenuState == MENU_STATE.SHOP)
                    m_shopVisual.SortSlots(m_runData, m_balance);
            }

#endif
        }

        public void GoToMainMenu()
        {
            SoundManager.Instance.PlaySFXButtonOK(m_settingsData);

            SetMenuState(MENU_STATE.MAIN_MENU);
        }

        public void AnimateGoToWheelSelection()
        {
            SoundManager.Instance.PlaySFXButtonOK(m_settingsData);

            m_mainMenuVisual.AnimateGoToWheelSelection();
        }

        public void AnimateContinueRun()
        {
            SoundManager.Instance.PlaySFXButtonOK(m_settingsData);

            m_mainMenuVisual.AnimateContinueGame();
        }

        public void RetryRun()
        {
            SoundManager.Instance.PlaySFXButtonOK(m_settingsData);

            StartGame(m_runData.StartSeed, m_runData.WheelIdx);
        }

        public void StartNewRunSameWheel()
        {
            StartNewRun(m_runData.WheelIdx);
        }

        public void StartNewRun(int wheelIdx)
        {
            SoundManager.Instance.PlaySFXButtonOK(m_settingsData);

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
            RunDataIO.LoadRun(m_runData, m_balance);
            showMenuState(m_runData.MenuState);
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
            SoundManager.Instance.PlaySFXButtonOK(m_settingsData);

            Board.StartRound(m_runData, m_balance);
            SetMenuState(MENU_STATE.IN_GAME);
        }

        public void SkipRound()
        {
            SoundManager.Instance.PlaySFXButtonOK(m_settingsData);

            m_roundSelectionVisual.Skip(m_runData, m_balance);
        }

        public void UseBossReroll()
        {
            m_roundSelectionVisual.TryUseBossReroll(m_runData, m_balance);

            RunDataIO.SaveRun(m_runData, m_balance);
        }

        public void RoundComplete()
        {
            SoundManager.Instance.PlaySFXWinRound(m_settingsData);

            Logic.RoundComplete(m_runData, m_balance);
            SetMenuState(MENU_STATE.ROUND_COMPLETE);
        }

        public void WinScreen()
        {
            Logic.WinGame(m_gameData, m_runData);

            SoundManager.Instance.PlaySFXWinGame(m_settingsData);

            Logic.RoundComplete(m_runData, m_balance);
            SetMenuState(MENU_STATE.WIN_SCREEN);

            GameDataIO.SaveGameData(m_gameData);
        }

        public void GameOver()
        {
            SoundManager.Instance.PlaySFXGameOver(m_settingsData);

            SetMenuState(MENU_STATE.GAME_OVER);
        }

        public void BallInSlot(int ballIdx, int slotIdx)
        {
            Board.BallInSlot(m_runData, m_balance, m_settingsData, ballIdx, slotIdx);

            SoundManager.Instance.PlaySFXMarbleInSlot(m_settingsData);
        }

        public void DropBalls()
        {
            Board.DropBalls(m_balance);
            SoundManager.Instance.PlaySFXGateOpen(m_settingsData);
        }

        public void SkipRound1()
        {
            m_runData.CurrentSpin = 3;
            Logic.ClaimRoundReward(m_runData, m_balance);
            Logic.PopulateShop(m_runData, m_balance);
            SetMenuState(MENU_STATE.SHOP);
        }

        public void ClaimRoundRewardAndGoToShop()
        {
            SoundManager.Instance.PlaySFXMoney(m_settingsData);

            Logic.ClaimRoundReward(m_runData, m_balance);
            Logic.PopulateShop(m_runData, m_balance);
            SetMenuState(MENU_STATE.SHOP);
        }

        public void RerollShop()
        {
            SoundManager.Instance.PlaySFXMoney(m_settingsData);

            m_shopVisual.RerollShop(m_runData, m_balance);

            RunDataIO.SaveRun(m_runData, m_balance);
        }

        public void ShowJokerBuyPopup(int shopIdx)
        {
            SoundManager.Instance.PlaySFXButtonOK(m_settingsData);

            Debug.Log("Game.ShowBuyPopup(shopIdx " + shopIdx + ")");
            m_shopVisual.ShowJokerBuyPopup(m_runData, m_balance, shopIdx);
        }

        public void ShowCardBuyPopup(int shopIdx)
        {
            SoundManager.Instance.PlaySFXButtonOK(m_settingsData);

            Debug.Log("Game.ShowBuyPopup(shopIdx " + shopIdx + ")");
            m_shopVisual.ShowCardBuyPopup(m_runData, m_balance, shopIdx);
        }

        public void ShowVoucherBuyPopup()
        {
            SoundManager.Instance.PlaySFXButtonOK(m_settingsData);

            m_shopVisual.ShowVoucherBuyPopup(m_runData, m_balance);
        }

        public void BuyShopJoker(int shopJokerIdx)
        {
            SoundManager.Instance.PlaySFXMoney(m_settingsData);

            Debug.Log("Game.BuyShopJoker(shopJokerIdx " + shopJokerIdx + ")");
            m_shopVisual.BuyShopJoker(m_runData, m_balance, shopJokerIdx);

            RunDataIO.SaveRun(m_runData, m_balance);
        }

        public void BuyShopCardPack(int shopCardIdx)
        {
            SoundManager.Instance.PlaySFXMoney(m_settingsData);

            Debug.Log("Game.BuyShopJoker(shopJokerIdx " + shopCardIdx + ")");
            m_shopVisual.BuyShopCardPack(m_runData, m_balance, shopCardIdx);

            if (m_balance.CardPackType[m_runData.SelectedShopCardPackIdx] == CARD_PACK_TYPE.BALL)
                SetMenuState(MENU_STATE.CARD_PACK_BALL);
            if (m_balance.CardPackType[m_runData.SelectedShopCardPackIdx] == CARD_PACK_TYPE.SLOT)
                SetMenuState(MENU_STATE.CARD_PACK_SLOT);
            if (m_balance.CardPackType[m_runData.SelectedShopCardPackIdx] == CARD_PACK_TYPE.CHIPS)
                SetMenuState(MENU_STATE.CARD_PACK_CHIPS);
        }

        public void OpenCardPack(int cardPackIdx)
        {
            Logic.OpenCardPack(m_runData, m_balance, cardPackIdx);

            if (m_balance.CardPackType[m_runData.SelectedShopCardPackIdx] == CARD_PACK_TYPE.BALL)
                SetMenuState(MENU_STATE.CARD_PACK_BALL);
            if (m_balance.CardPackType[m_runData.SelectedShopCardPackIdx] == CARD_PACK_TYPE.SLOT)
                SetMenuState(MENU_STATE.CARD_PACK_SLOT);
            if (m_balance.CardPackType[m_runData.SelectedShopCardPackIdx] == CARD_PACK_TYPE.CHIPS)
                SetMenuState(MENU_STATE.CARD_PACK_CHIPS);
        }

        public void BuyVoucher()
        {
            SoundManager.Instance.PlaySFXMoney(m_settingsData);

            if (!m_runData.VoucherPurchased)
                m_shopVisual.BuyVoucher(m_runData, m_balance);

            RunDataIO.SaveRun(m_runData, m_balance);
        }

        public void ShowJokerInfoPopup(int jokerIdx)
        {
            SoundManager.Instance.PlaySFXButtonOK(m_settingsData);

            SetMenuState(MENU_STATE.JOKER_INFO_POPUP);
            m_jokerInfoPopupVisual.Show(m_runData, m_balance, jokerIdx);
        }

        public void ShowJokerInfoPopupInGame(int jokerIdx)
        {
            SoundManager.Instance.PlaySFXButtonOK(m_settingsData);

            SetMenuState(MENU_STATE.JOKER_INFO_POPUP);
            m_jokerInfoPopupVisual.ShowInGame(m_runData, m_balance, jokerIdx);
        }

        public void HideJokerInfoPopup()
        {
            SoundManager.Instance.PlaySFXButtonCancel(m_settingsData);

            SetMenuState(m_runData.PrevMenuState);
        }

        public void SellJoker(int jokerIdx)
        {
            SoundManager.Instance.PlaySFXMoney(m_settingsData);

            Logic.SellJoker(m_runData, jokerIdx);
            HideJokerInfoPopup();
        }

        public void CloseCardPack()
        {
            SoundManager.Instance.PlaySFXButtonOK(m_settingsData);

            SetMenuState(m_runData.PrevMenuState);
        }

        public void UseCardPackOnBalls(int cardIdx)
        {
            SoundManager.Instance.PlaySFXButtonOK(m_settingsData);

            m_cardPackBallVisual.UseCardPackOnBalls(m_runData, m_balance, cardIdx);
        }

        public void UseCardPackOnSlots(int cardIdx)
        {
            SoundManager.Instance.PlaySFXButtonOK(m_settingsData);

            m_cardPackSlotVisual.UseCardPackOnSlots(m_runData, m_balance, cardIdx);
        }

        public void UseCardPackOnChips(int cardIdx)
        {
            SoundManager.Instance.PlaySFXButtonOK(m_settingsData);

            m_cardPackChipsVisual.UseCardPackChips(m_runData, m_balance, cardIdx);
        }

        public void RerollCardPack()
        {
            SoundManager.Instance.PlaySFXMoney(m_settingsData);

            if (m_balance.CardPackType[m_runData.SelectedShopCardPackIdx] == CARD_PACK_TYPE.BALL)
                m_cardPackBallVisual.Reroll(m_runData, m_balance);
            if (m_balance.CardPackType[m_runData.SelectedShopCardPackIdx] == CARD_PACK_TYPE.SLOT)
                m_cardPackSlotVisual.Reroll(m_runData, m_balance);
            if (m_balance.CardPackType[m_runData.SelectedShopCardPackIdx] == CARD_PACK_TYPE.CHIPS)
                m_cardPackChipsVisual.Reroll(m_runData, m_balance);

            RunDataIO.SaveRun(m_runData, m_balance);
        }

        public void GoToRoundSelection()
        {
            SoundManager.Instance.PlaySFXButtonOK(m_settingsData);

            SetMenuState(MENU_STATE.ROUND_SELECTION);
        }

        public void GoToBallScreen()
        {
            SoundManager.Instance.PlaySFXButtonOK(m_settingsData);

            SetMenuState(MENU_STATE.BALL_SCREEN);
        }

        public void CloseBallScreen()
        {
            SoundManager.Instance.PlaySFXButtonOK(m_settingsData);

            m_ballScreenVisual.AnimateClose();
        }

        public void CopySeed()
        {

        }

        public void GoToSettings()
        {
            SoundManager.Instance.PlaySFXButtonOK(m_settingsData);

            SetMenuState(MENU_STATE.SETTINGS);
        }
        public void CloseSettings()
        {
            SoundManager.Instance.PlaySFXButtonOK(m_settingsData);

            m_settingsVisual.AnimateClose();
        }

        public void BallBallCollision()
        {
            // Debug.Log("ball ball collision");

            SoundManager.Instance.PlaySFXMarbleMarble(m_settingsData);
        }

        public void BallSpinWheelCollision()
        {
            // Debug.Log("ball spinwheel collision");

            SoundManager.Instance.PlaySFXMarbleSlot(m_settingsData);
        }

        public void GoToChipsInfo()
        {
            SoundManager.Instance.PlaySFXButtonOK(m_settingsData);

            SetMenuState(MENU_STATE.CHIPS_INFO);
        }

        public void CloseChipsInfo()
        {
            SoundManager.Instance.PlaySFXButtonOK(m_settingsData);

            m_chipsInfoVisual.AnimateClose();
        }

        public void ShowGameInfo()
        {
            SoundManager.Instance.PlaySFXButtonOK(m_settingsData);

            SetMenuState(MENU_STATE.IN_GAME_INFO);
        }

        public void CloseGameInfo()
        {
            SoundManager.Instance.PlaySFXButtonOK(m_settingsData);

            m_gameInfoVisual.AnimateClose();
        }

        public void ShowShopInfo()
        {
            SoundManager.Instance.PlaySFXButtonOK(m_settingsData);

            SetMenuState(MENU_STATE.SHOP_INFO);
        }

        public void CloseShopInfo()
        {
            SoundManager.Instance.PlaySFXButtonOK(m_settingsData);

            m_shopInfoVisual.AnimateClose();
        }

        public void GoToWheelSelection()
        {
            SoundManager.Instance.PlaySFXButtonOK(m_settingsData);

            SetMenuState(MENU_STATE.WHEEL_SELECTION);
        }

        public void CloseWheelSelection()
        {
            SoundManager.Instance.PlaySFXButtonOK(m_settingsData);

            m_wheelSelectionVisual.AnimateClose();
        }
    }
}