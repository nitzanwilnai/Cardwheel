/*
  Cardwheel — Non-Commercial, No-Modification License
  Copyright © 2025 Nitzan Wilnai
  Source Code: https://github.com/nitzanwilnai/Cardwheel

  Permission is granted to view and run this code for non-commercial purposes only.
  Modification, redistribution of altered versions, and commercial use are strictly prohibited.

  See the LICENSE file for full legal terms.
*/

using System;
using CommonTools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using ParticleSystemDOD;

namespace Cardwheel
{
    public enum SLOT_TYPE { RED, ORANGE, GREEN, BLUE, LAST, NONE };
    public enum CARD_PACK_TYPE { NONE, BALL, SLOT, CHIPS };

    public struct BallSprites
    {
        public SpriteRenderer BallSprite;
        public GameObject BallDebuffedGO;
    }

    public struct JokerGUI
    {
        public Image CardImage;
        public Image Border;
        public Button Button;
        public Animation Animation;
        public GameObject DebuffGO;
        public GameObject RainbowGO;
        public GameObject ShinyGO;
        public GameObject MetalGO;
        public GameObject SelectedGO;

        public GameObject JokerChips;
        public GameObject JokerMult;
        public GameObject JokerColor;
        public TextMeshProUGUI JokerChipsText;
        public TextMeshProUGUI JokerMultText;
        public TextMeshProUGUI JokerColorText;
    }

    public class Board : MonoBehaviour
    {
        public enum MENU_BUTTONS
        {
            DROP,
            INFO,
            SETTINGS = 10,
            JOKER_1 = 20,
            JOKER_2 = 21,
            JOKER_3 = 22,
            JOKER_4 = 23,
            JOKER_5 = 24,
        };
        MENU_BUTTONS m_selectedButton = MENU_BUTTONS.DROP;

        public GameObject GateGO;

        public enum GAME_STATE
        {
            START_ROUND,
            JOKER_PRE_ROUND,
            SPIN_UP,
            WAITING_FOR_INPUT,
            BALLS_DROPPED,
            SCORING_SLOT_CHIPS,
            SCORING_SLOT_MULTIPLIER_ADD,
            SCORING_SLOT_MONEY,
            SCORING_BALL_CHIPS,
            SCORING_BALL_MULTIPLIER_ADD,
            SCORING_BALL_MONEY,
            SCORING_JOKER_CHIPS,
            SCORING_JOKER_MULTIPLIER_ADD,
            SCORING_SLOT_MULTIPLIER_MULT,
            SCORING_BALL_MULTIPLIER_MULT,
            SCORING_JOKER_MULTIPLIER_MULT,
            SCORING_ROUND_TOTAL,
            JOKER_POST_SPIN,
            JOKER_POST_ROUND,
            BOSS_POST_SPIN,
            SPIN_OVER
        };
        public GAME_STATE GameState = GAME_STATE.SPIN_UP;

        public enum SPIN_STATE { SPIN_UP, SPIN_WAIT, SPIN_BALLS, RE_SPIN, SPIN_DOWN, DONE };
        public SPIN_STATE SpinState = SPIN_STATE.SPIN_UP;

        public SpinWheelLights SpinWheelLights;

        public GameObject BoardSpites;
        GameObject m_UI;

        TopBarGUI m_topBarGUI;

        public Transform BallParticleParent;
        int m_numBalls;
        public Ball[] BallsGO;
        public Transform[] BallStartPos;
        Vector3[] m_ballStartPos;
        Rigidbody2D[] m_ballsRB;
        BallSprites[] m_ballSprites;
        public ParticleSystemSmokeBoard ParticleSystemSmokeBoard;

        public GameObject BallsChipsGO;
        public GameObject BallsMultiplierGO;
        public GameObject BallsMoneyGO;
        TextMeshPro m_ballsChipsText;
        TextMeshPro m_ballsMultiplierText;
        TextMeshPro m_ballsMoneyText;

        public GameObject[] SlotsGO;
        ScoringSlot[] m_scoringSlots;

        public float BallInSlotTime;

        public SpinCircle SpinCircle;
        public float MaxSpin;
        public AnimationCurve SpinUpAnimationCurve;
        public AnimationCurve SpinDownAnimationCurve;
        public AnimationCurve ReSpinAnimationCurve;
        float m_spinAnimTime;
        public float SpinUpTime;
        public float SpinDownTime;
        public float SpinDownMultiplier;
        public float RespinMultiplier;

        public AnimationCurve BallSnapAnimCurve;

        // scoring
        Animation m_roundScoreAnimation;
        Animation m_roundMultiplierAnimation;
        TextMeshProUGUI m_roundChipsText;
        TextMeshProUGUI m_roundMultiplierText;
        TextMeshProUGUI m_totalScoreText;
        Animation m_totalScoreAnimation;
        TextMeshProUGUI m_totalRoundScoreText;
        Animation m_totalRoundScoreAnimation;
        int m_scoringIdx;
        public float ScoringTime;
        float m_scoringTimer;
        TextMeshProUGUI m_goalText;
        bool[] m_ballLockedInSlot;

        TextMeshProUGUI m_bossDescription;
        GameObject m_bossDescriptionGO;
        Animation m_bossGrowAnim;

        // rounds
        public float NextSpinTime;
        float m_nextSpinTimer;

        // spins
        TextMeshProUGUI m_spinsText;
        Image m_spinButtonImage;
        GUIButtonData m_spinButtonData;
        GUIButtonData m_spinButtonData2;
        GUIButtonData m_infoButtonData;

        [Header("Jokers")]
        Transform m_jokerParent;
        GameObject m_jokerChipsGO;
        GameObject m_jokerMultGO;
        GameObject m_jokerColorGO;
        TextMeshProUGUI m_jokerChipsText;
        TextMeshProUGUI m_jokerMultText;
        TextMeshProUGUI m_jokerColorText;

        float m_slotAnimTimer;
        float m_slotAnimTime = 1.0f;
        public AnimationCurve SlotScaleAnimCurve;
        float[] m_slotJuiceTimer;

        float m_prevSpinWheelAngle;

        // AI
        float m_waitingForInputTime = 0.0f;
        float m_prevSpinWheelZ;
        public bool AUTO_DROP = false;
        public float AutoDropAngle = 238.6025f;

        public bool SpinTest = false;

        bool m_showSlotEffects = true;
        bool m_slotsDebuffed = false;

        Balance balance;
        SettingsData settingsData;
        RunData runData;

        [Header("Debug")]
        public bool ShowDebug;
        public float DebugRotationSpeed;

        // Start is called before the first frame update
        public void Init(RunData runData, Balance balance, GameInfoSO gameInfoSO, SettingsData settingsData, Camera camera)
        {
            this.runData = runData;
            this.balance = balance;
            this.settingsData = settingsData;

            transform.localPosition = gameInfoSO.Position;
            transform.localScale = gameInfoSO.Scale;
            Physics2D.gravity = new Vector3(0.0f, gameInfoSO.Gravity, 0.0f);

            m_numBalls = BallsGO.Length;
            if (m_numBalls != balance.MaxBalls)
                Debug.LogError("Balance MaxBalls " + balance.MaxBalls + " BallsGO.Length " + m_numBalls);

            m_ballsRB = new Rigidbody2D[m_numBalls];
            m_ballStartPos = new Vector3[m_numBalls];
            m_ballSprites = new BallSprites[m_numBalls];
            m_ballLockedInSlot = new bool[m_numBalls];
            for (int i = 0; i < m_numBalls; i++)
            {
                BallsGO[i].name = i.ToString();
                m_ballsRB[i] = BallsGO[i].GetComponentInChildren<Rigidbody2D>();
                m_ballsRB[i].name = i.ToString();
                GUIRef ballGuiRef = BallsGO[i].GetComponent<GUIRef>();
                m_ballSprites[i].BallSprite = ballGuiRef.GetGameObject("Ball").GetComponent<SpriteRenderer>();
                m_ballSprites[i].BallDebuffedGO = ballGuiRef.GetGameObject("Debuffed");
            }

            m_ballsChipsText = BallsChipsGO.GetComponentInChildren<TextMeshPro>();
            m_ballsMultiplierText = BallsMultiplierGO.GetComponentInChildren<TextMeshPro>();
            m_ballsMoneyText = BallsMoneyGO.GetComponentInChildren<TextMeshPro>();

            m_scoringSlots = new ScoringSlot[SlotsGO.Length];
            for (int i = 0; i < SlotsGO.Length; i++)
            {
                m_scoringSlots[i] = SlotsGO[i].GetComponentInChildren<ScoringSlot>();
                m_scoringSlots[i].Index = i;
            }

            m_UI = AssetManager.Instance.LoadInGameUI();
            m_UI.GetComponent<Canvas>().worldCamera = camera;
            CommonVisual.ChangeCanvasScalerMatching(m_UI);

            CommonVisual.ChangeCanvasScalerMatching(m_UI);

            GUIButtonRef guiButtonRef = m_UI.GetComponent<GUIButtonRef>();
            m_spinButtonData = guiButtonRef.GetButtonData("Spin");
            m_spinButtonData.Button.onClick.AddListener(dropBalls);
            CommonButtonVisual.AddSelectedBorder(m_spinButtonData);

            m_spinButtonData2 = guiButtonRef.GetButtonData("Spin2");
            m_spinButtonData2.Button.onClick.AddListener(dropBalls);

            m_infoButtonData = guiButtonRef.GetButtonData("Info");
            m_infoButtonData.Button.onClick.AddListener(showGameInfo);
            CommonButtonVisual.AddSelectedBorder(m_infoButtonData);

            GUIRef guiRef = m_UI.GetComponent<GUIRef>();
            CommonVisual.InitTopBarGUI(guiRef.GetGameObject("TopBar"), ref m_topBarGUI);

            m_spinButtonImage = guiRef.GetImage("Spin");
            m_roundChipsText = guiRef.GetTextGUI("Score");
            m_roundMultiplierText = guiRef.GetTextGUI("Multiplier");
            m_roundMultiplierAnimation = guiRef.GetAnimation("Multiplier");
            m_roundScoreAnimation = guiRef.GetAnimation("Score");
            m_totalScoreText = guiRef.GetTextGUI("TotalScore");
            m_totalScoreAnimation = guiRef.GetAnimation("TotalScore");
            m_totalRoundScoreText = guiRef.GetTextGUI("TotalRoundScore");
            m_totalRoundScoreAnimation = guiRef.GetAnimation("TotalRoundScore");
            m_goalText = guiRef.GetTextGUI("Goal");

            m_spinsText = guiRef.GetTextGUI("Spins");

            m_jokerChipsGO = guiRef.GetGameObject("JokerChips");
            m_jokerMultGO = guiRef.GetGameObject("JokerMult");
            m_jokerColorGO = guiRef.GetGameObject("JokerColor");
            m_jokerChipsText = guiRef.GetTextGUI("JokerChips");
            m_jokerMultText = guiRef.GetTextGUI("JokerMult");
            m_jokerColorText = guiRef.GetTextGUI("JokerColor");
            hideJokerPopups();

            m_jokerParent = guiRef.GetGameObject("JokerParent").transform;

            m_bossDescription = guiRef.GetTextGUI("BossDescription");
            m_bossDescriptionGO = guiRef.GetGameObject("BossDescription");
            m_bossGrowAnim = guiRef.GetAnimation("BossDescription");

            m_UI.SetActive(false);
            BoardSpites.SetActive(false);

            SpinWheelLights.Init();

            m_slotJuiceTimer = new float[balance.NumSlots];

            ParticleSystemSmokeBoard.Init(BallParticleParent);
        }

        public void Show()
        {
            Debug.Log("Show() GameState " + GameState);

            m_goalText.text = Logic.GetRoundGoal(runData, balance).ToString("N0");
            m_totalScoreText.text = runData.TotalChips.ToString("N0");

            m_UI.SetActive(true);
            BoardSpites.SetActive(true);

            if (GameState == GAME_STATE.START_ROUND)
            {
                resetSpin(runData, balance);
            }

            // if (GameState < GAME_STATE.WAITING_FOR_INPUT)
            // {
            //     for (int ballIdx = 0; ballIdx < m_numBalls; ballIdx++)
            //     {
            //         m_ballStartPos[ballIdx] = BallStartPos[ballIdx].position;
            //         m_ballsRB[ballIdx].bodyType = RigidbodyType2D.Static;
            //     }
            // }
            // else if (GameState >= GAME_STATE.WAITING_FOR_INPUT)
            // {
            //     for (int ballIdx = 0; ballIdx < m_numBalls; ballIdx++)
            //     {
            //         m_ballStartPos[ballIdx] = BallStartPos[ballIdx].position;
            //         m_ballsRB[ballIdx].bodyType = RigidbodyType2D.Dynamic;
            //         m_ballsRB[ballIdx].angularVelocity = 0.0f;
            //         m_ballsRB[ballIdx].linearVelocity = Vector3.zero;
            //     }
            //     Debug.Log("Set balls to STATIC");
            // }

            m_showSlotEffects = true;
            m_slotsDebuffed = false;
            CommonSlotsVisual.CheckSpinWheelDebuffForNewRound(runData, balance, runData.Round, out m_showSlotEffects, out m_slotsDebuffed);

            int useBallSprite = 1;
            if (Logic.InBossRound(runData))
            {
                int bossType = Logic.GetBossTypeForRound(runData);

                if (balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.BALL_EFFECTS_HIDDEN)
                    useBallSprite = 0;
            }
            m_bossDescriptionGO.SetActive(Logic.InBossRound(runData));

            int bigRound = runData.Round / 3;
            int smallRound = runData.Round % 3;
            string title = Logic.InBossRound(runData) ? "Boss " : "Round ";
            title += (bigRound + 1).ToString() + " - " + (smallRound + 1).ToString();
            m_bossDescriptionGO.SetActive(Logic.InBossRound(runData));
            if (Logic.InBossRound(runData))
                m_bossDescription.text = CommonVisual.GetBossDescription(runData, balance, "Boss: ");
            CommonVisual.ShowTopBar(runData, m_topBarGUI, title);

            CommonSlotsVisual.ShowSpinWheel(runData, balance, m_scoringSlots, runData.SlotTypeInGame, m_showSlotEffects, runData.UseSlotsSpecial == 0);

            CommonVisual.ShowJokersInGame(runData, balance, m_jokerParent);

            showBallsInGame(useBallSprite, runData.UseBallsSpecial == 0);

            SpinWheelLights.StartAnimation();

            CommonVisual.ShowMoney(runData, m_topBarGUI);

            CommonButtonVisual.UpdateButtonIcons(m_spinButtonData, Game.Instance.GetGamepadType());
            CommonButtonVisual.UpdateButtonIcons(m_infoButtonData, Game.Instance.GetGamepadType());
            CommonButtonVisual.UpdateButtonIcons(m_topBarGUI.SettingsButtonData, Game.Instance.GetGamepadType());

            BallsChipsGO.SetActive(false);
            BallsMultiplierGO.SetActive(false);
            BallsMoneyGO.SetActive(false);

            selectButton(MENU_BUTTONS.DROP);
        }

        void selectButton(MENU_BUTTONS selectedButton)
        {
            Game.Instance.LastSelectedMenuButton[(int)runData.MenuState] = (int)selectedButton;

            m_selectedButton = selectedButton;

            m_spinButtonData.SelectedGO.SetActive(CommonButtonVisual.ShowSelected() && m_selectedButton == MENU_BUTTONS.DROP);
            m_infoButtonData.SelectedGO.SetActive(CommonButtonVisual.ShowSelected() && m_selectedButton == MENU_BUTTONS.INFO);

            m_topBarGUI.SettingsButtonData.SelectedGO.SetActive(CommonButtonVisual.ShowSelected() && m_selectedButton == MENU_BUTTONS.SETTINGS);

            CommonVisual.UnselectAllJokers();
            if (m_selectedButton >= MENU_BUTTONS.JOKER_1 && m_selectedButton <= MENU_BUTTONS.JOKER_5)
                CommonVisual.SelectJoker((int)m_selectedButton - (int)MENU_BUTTONS.JOKER_1);
        }

        public void SelectPrevButton(MENU_BUTTONS selectedButton)
        {
            if (selectedButton == MENU_BUTTONS.SETTINGS || selectedButton == MENU_BUTTONS.INFO)
                selectButton(selectedButton);

            if (selectedButton >= MENU_BUTTONS.JOKER_1 && selectedButton <= MENU_BUTTONS.JOKER_5)
            {
                int jokerIdx = selectedButton - MENU_BUTTONS.JOKER_1;
                if (jokerIdx < runData.JokerCount)
                    selectButton(selectedButton);
                else
                    selectButton(MENU_BUTTONS.JOKER_1);
            }
        }

        public void showBallsInGame(int useBallSprite, bool debuffed)
        {
            for (int ballIdx = 0; ballIdx < BallsGO.Length; ballIdx++)
            {
                int ballTypeForSprite = runData.BallTypesInGame[ballIdx] * useBallSprite;
                m_ballSprites[ballIdx].BallSprite.sprite = AssetManager.Instance.LoadBallSprite(balance.BallBalance.BallSprite[ballTypeForSprite]);
                m_ballSprites[ballIdx].BallDebuffedGO.SetActive(ballTypeForSprite > 0 && debuffed);
            }
        }

        public void Hide()
        {
            m_UI.SetActive(false);
            BoardSpites.SetActive(false);

            hideJokerPopups();
            m_roundScoreAnimation.Stop();
            m_roundMultiplierAnimation.Stop();

            CommonVisual.HideJokers();
        }

        private void hideJokerPopups()
        {
            m_jokerMultGO.SetActive(false);
            m_jokerChipsGO.SetActive(false);
            m_jokerColorGO.SetActive(false);
        }

        void animateRoundChipsText()
        {
            m_roundChipsText.text = runData.SpinChips.ToString("N0");
            m_roundScoreAnimation.Play();
            SoundManager.Instance.PlaySFXScoring();
        }

        void animateRoundMultipierText()
        {
            m_roundMultiplierText.text = CommonVisual.GetMultiplierString(runData.SpinMultiplier);
            m_roundMultiplierAnimation.Play();
            SoundManager.Instance.PlaySFXScoring();
        }

        public void ShowBallChipsPopup(int ballIdx, int chips)
        {
            Vector3 ballPos = BallsGO[ballIdx].transform.position;
            BallsChipsGO.transform.position = new Vector3(ballPos.x, ballPos.y, ballPos.z - 10.0f);
            BallsChipsGO.SetActive(true);

            m_ballsChipsText.text = "+" + chips.ToString("N0");
        }

        public void ShowBallMultiplierPopup(int ballIdx, string text)
        {
            Vector3 ballPos = BallsGO[ballIdx].transform.position;
            BallsMultiplierGO.transform.position = new Vector3(ballPos.x, ballPos.y, ballPos.z - 10.0f);
            BallsMultiplierGO.SetActive(true);
            m_ballsMultiplierText.text = text;
        }

        public void ShowJokerChipsPopup(int jokerIdx, string text)
        {
            m_jokerChipsGO.SetActive(false);
            m_jokerChipsGO.transform.position = CommonVisual.JokerPool[jokerIdx].transform.position;
            m_jokerChipsGO.SetActive(true);
            m_jokerChipsText.text = text;
        }

        public void ShowJokerMultPopup(int jokerIdx, string text)
        {
            m_jokerMultGO.SetActive(false);
            m_jokerMultGO.transform.position = CommonVisual.JokerPool[jokerIdx].transform.position;
            m_jokerMultGO.SetActive(true);
            m_jokerMultText.text = text;
        }

        public void ShowJokerColorPopup(int jokerIdx)
        {
            m_jokerColorGO.SetActive(false);
            m_jokerColorGO.transform.position = CommonVisual.JokerPool[jokerIdx].transform.position;
            m_jokerColorGO.SetActive(true);
            SLOT_TYPE leastPlayedColor = Logic.GetLeastPlayedSlotType(runData);
            m_jokerColorText.text = leastPlayedColor.ToString();
            m_jokerColorText.color = balance.SlotColors[(int)leastPlayedColor];
        }

        void setGameState(GAME_STATE newGamState)
        {
            Debug.Log("setGameState(" + newGamState + ") m_scoringTimer " + m_scoringTimer);
            GameState = newGamState;

            hideJokerPopups();

            if (GameState == GAME_STATE.WAITING_FOR_INPUT)
            {
                m_spinButtonData.Button.interactable = true;
                m_spinButtonData2.Button.interactable = true;
                m_spinButtonImage.color = balance.ButtonColorEnabled;
            }
            else
            {
                m_spinButtonData.Button.interactable = false;
                m_spinButtonData2.Button.interactable = false;
                m_spinButtonImage.color = balance.ButtonColorDisabled;
            }
        }

        // Update is called once per frame
        public void Tick(float dt)
        {
            SpinWheelLights.Tick(dt);

            if (runData.SpinWheelAngle < -360.0f)
                runData.SpinWheelAngle += 360.0f;

            m_prevSpinWheelAngle = runData.SpinWheelAngle;
            runData.SpinWheelAngle += runData.RotationSpeed * dt;
            SpinCircle.Angle = runData.SpinWheelAngle;

            float prevTickAngle = m_prevSpinWheelAngle - (Mathf.FloorToInt(m_prevSpinWheelAngle / 15) * 15.0f);
            float currentTickAngle = runData.SpinWheelAngle - (Mathf.FloorToInt(runData.SpinWheelAngle / 15) * 15.0f);
            if (prevTickAngle < currentTickAngle & runData.RotationSpeed < 0.0f)
            {
                SoundManager.Instance.PlaySFXWheelSpin();
            }

            if (GameState == GAME_STATE.START_ROUND)
            {
                m_scoringTimer += dt;// * settingsData.Speed;
                if (m_scoringTimer > ScoringTime)
                {
                    setGameState(GAME_STATE.JOKER_PRE_ROUND);
                    return;
                }
            }
            if (GameState == GAME_STATE.JOKER_PRE_ROUND)
            {
                m_scoringTimer += dt;// * settingsData.Speed;
                if (m_scoringTimer > ScoringTime)
                {
                    while (m_scoringIdx < runData.JokerCount)
                    {
                        int jokerIdx = m_scoringIdx;
                        int jokerType = runData.JokerTypes[jokerIdx];
                        m_scoringIdx++;

                        if (balance.JokerBalance.MultiplierAddForLeastPlayedColor[jokerType] > 0.0f)
                        {
                            ShowJokerColorPopup(jokerIdx);

                            m_scoringTimer = 0.0f;
                            break;
                        }

                        int slotChangedIdx = Logic.JokerPreRoundTryModifySlot(runData, balance, jokerType);
                        if (slotChangedIdx > -1)
                        {
                            CommonSlotsVisual.ChangedSlotsIdxs[CommonSlotsVisual.ChangedSlotsCount++] = slotChangedIdx;
                            CommonSlotsVisual.ShowSpinWheel(runData, balance, m_scoringSlots, runData.SlotTypeInGame, m_showSlotEffects, runData.UseSlotsSpecial == 0);
                            m_slotAnimTimer = m_slotAnimTime;

                            CommonVisual.JokerGUIs[jokerIdx].Animation.Play("ScoreGrow");
                            m_scoringTimer = 0.0f;
                            break;
                        }
                    }

                    if (m_scoringIdx >= runData.JokerCount && m_scoringTimer > ScoringTime)
                    {
                        startSpin(runData, balance);
                    }
                }
            }
            if (GameState == GAME_STATE.SPIN_UP)
            {
                if (m_spinAnimTime < SpinUpTime)
                {
                    float spinMultiplier = SpinUpAnimationCurve.Evaluate(m_spinAnimTime / SpinUpTime);
                    runData.RotationSpeed = MaxSpin * spinMultiplier;

                    m_spinAnimTime += dt;

                    if (m_spinAnimTime >= SpinUpTime)
                    {
                        runData.RotationSpeed = MaxSpin;
                        m_spinAnimTime = 0.0f;
                        SpinState = SPIN_STATE.SPIN_WAIT;
                        setGameState(GAME_STATE.WAITING_FOR_INPUT);
                        m_waitingForInputTime = 0.0f;

                        // // hack to fix issue where balls stay static
                        // for (int i = 0; i < m_ballsRB.Length; i++)
                        //     m_ballsRB[i].bodyType = RigidbodyType2D.Dynamic;
                        // Debug.Log("Set balls to DYNAMIC");
                    }
                }
            }

            if (GameState == GAME_STATE.BALLS_DROPPED)
            {
                for (int ballIdx = 0; ballIdx < runData.BallSlotIdx.Length; ballIdx++)
                {
                    if (runData.BallSlotIdx[ballIdx] > -1)
                    {
                        Vector3 slotPos = m_scoringSlots[runData.BallSlotIdx[ballIdx]].transform.position;
                        Vector3 ballPos = BallsGO[ballIdx].transform.position;
                        ballPos.z = slotPos.z;

                        // move out of runData
                        runData.BallSnapTime[ballIdx] += dt;
                        if (runData.BallSnapTime[ballIdx] > 1.0f)
                            runData.BallSnapTime[ballIdx] = 1.0f;
                        runData.BallSnapVelocity[ballIdx] = BallSnapAnimCurve.Evaluate(runData.BallSnapTime[ballIdx]) * 25.0f;
                        float ballTravelDist = dt * runData.BallSnapVelocity[ballIdx];
                        if (!m_ballLockedInSlot[ballIdx] && Vector3.Distance(ballPos, slotPos) > ballTravelDist * 1.01f)
                        {
                            ballPos += (slotPos - ballPos).normalized * ballTravelDist;
                        }
                        else
                        {
                            m_ballLockedInSlot[ballIdx] = true;
                            ballPos = slotPos;
                        }

                        ballPos.z = -5.0f;
                        BallsGO[ballIdx].transform.position = ballPos;
                    }
                }

                if (SpinState == SPIN_STATE.SPIN_BALLS)
                {
                    // do nothing
                    if (m_spinAnimTime >= SpinDownTime)
                    {
                        SpinState = SPIN_STATE.SPIN_DOWN;
                        m_spinAnimTime = 0.0f;
                    }
                }
                else if (SpinState == SPIN_STATE.SPIN_DOWN)
                {
                    float spinDownMultiplier = SpinDownMultiplier;
                    if (!Logic.AllBallsInSlot(runData))
                        spinDownMultiplier /= 2.0f;
                    float spinTime = m_spinAnimTime * spinDownMultiplier;
                    // if (!Logic.AllBallsInSlot(runData) && spinTime >= 0.3f)
                    //     spinTime = 0.3f;

                    if (spinTime > 1.0f)
                    {
                        if (Logic.AllBallsInSlot(runData) && allBlocksLocked())
                        {
                            startScoring(runData, balance);
                            SpinState = SPIN_STATE.DONE;
#if UNITY_EDITOR
                            if (SpinTest)
                                doSpinTest(runData, balance);
#endif
                        }
                        else if (spinTime > 2.0f)
                        {
                            SpinState = SPIN_STATE.RE_SPIN;
                            m_spinAnimTime = 0.0f;
                        }
                    }

                    if (spinTime > 1.0f)
                        spinTime = 1.0f;

                    float spinMultiplier = SpinDownAnimationCurve.Evaluate(spinTime);
                    float newSpeed = MaxSpin * spinMultiplier;
                    // if (!Logic.AllBallsInSlot(runData) && newSpeed < MaxSpin / 2.0f)
                    //     newSpeed = MaxSpin / 2.0f;
                    runData.RotationSpeed = newSpeed;
                }
                else if (SpinState == SPIN_STATE.RE_SPIN)
                {
                    float spinTime = m_spinAnimTime * RespinMultiplier;
                    if (spinTime > 1.0f)
                    {
                        spinTime = 1.0f;
                        SpinState = SPIN_STATE.SPIN_DOWN;
                        m_spinAnimTime = 0.0f;
                    }
                    float spinMultiplier = ReSpinAnimationCurve.Evaluate(spinTime);
                    runData.RotationSpeed = MaxSpin * spinMultiplier;
                }
                m_spinAnimTime += dt;
            }

            if (GameState == GAME_STATE.SCORING_SLOT_CHIPS)
            {
                m_scoringTimer += dt * settingsData.Speed;
                if (m_scoringTimer > ScoringTime)
                {
                    BallsChipsGO.SetActive(false);

                    if (m_scoringIdx >= balance.MaxBalls)
                    {
                        setGameState(GAME_STATE.SCORING_SLOT_MULTIPLIER_ADD);
                        m_scoringIdx = 0;
                    }
                    else
                    {
                        while (m_scoringIdx < balance.MaxBalls)
                        {
                            int ballIdx = runData.BallScoreIdxs[m_scoringIdx];
                            m_scoringIdx++;

                            int chips = Logic.CalculateSlotBallChips(runData, balance, ballIdx);
                            if (chips > 0)
                            {
                                Debug.Log(Time.realtimeSinceStartupAsDouble + " " + GameState.ToString() + "  " + chips.ToString() + " Chips for Ball " + runData.BallTypesInGame[ballIdx].ToString() + " in Slot ");

                                m_scoringTimer = 0.0f;

                                ShowBallChipsPopup(ballIdx, chips);

                                animateRoundChipsText();
                                break;
                            }
                        }
                    }
                }
            }
            if (GameState == GAME_STATE.SCORING_SLOT_MULTIPLIER_ADD)
            {
                m_scoringTimer += dt * settingsData.Speed;
                if (m_scoringTimer > ScoringTime)
                {
                    BallsMultiplierGO.SetActive(false);

                    if (m_scoringIdx >= balance.MaxBalls)
                    {
                        setGameState(GAME_STATE.SCORING_SLOT_MONEY);
                        m_scoringIdx = 0;
                    }
                    else
                    {
                        while (m_scoringIdx < balance.MaxBalls)
                        {
                            int ballIdx = runData.BallScoreIdxs[m_scoringIdx];
                            m_scoringIdx++;
                            float multiplier = Logic.CalculateSlotBallMultiplierAdd(runData, balance, ballIdx);
                            if (multiplier > 0.0f)
                            {
                                Debug.Log(Time.realtimeSinceStartupAsDouble + " " + GameState.ToString() + "  " + multiplier.ToString() + "x Multiplier Add for Ball " + runData.BallTypesInGame[ballIdx].ToString() + " in Slot");

                                m_scoringTimer = 0.0f;

                                ShowBallMultiplierPopup(ballIdx, "+" + CommonVisual.GetMultiplierString(multiplier) + "x");

                                animateRoundMultipierText();
                                break;
                            }
                        }
                    }
                }
            }
            if (GameState == GAME_STATE.SCORING_SLOT_MONEY)
            {
                m_scoringTimer += dt * settingsData.Speed;
                if (m_scoringTimer > ScoringTime)
                {
                    BallsMoneyGO.SetActive(false);

                    if (m_scoringIdx >= balance.MaxBalls)
                    {
                        setGameState(GAME_STATE.SCORING_BALL_CHIPS);
                        m_scoringIdx = 0;
                    }
                    else
                    {
                        while (m_scoringIdx < balance.MaxBalls)
                        {
                            int ballIdx = runData.BallScoreIdxs[m_scoringIdx];
                            m_scoringIdx++;
                            int money = Logic.CalculateSlotMoney(runData, balance, ballIdx);
                            if (money > 0)
                            {
                                Debug.Log(Time.realtimeSinceStartupAsDouble + " " + GameState.ToString() + "  " + money.ToString() + " Money for ball " + runData.BallTypesInGame[ballIdx].ToString());

                                m_scoringTimer = 0.0f;

                                ShowBallMoney(runData, settingsData, ballIdx, money);
                                break;
                            }
                        }
                    }
                }
            }
            if (GameState == GAME_STATE.SCORING_BALL_CHIPS)
            {
                m_scoringTimer += dt * settingsData.Speed;
                if (m_scoringTimer > ScoringTime)
                {
                    BallsChipsGO.SetActive(false);

                    if (m_scoringIdx >= balance.MaxBalls)
                    {
                        setGameState(GAME_STATE.SCORING_BALL_MULTIPLIER_ADD);
                        m_scoringIdx = 0;
                    }
                    else
                    {
                        while (m_scoringIdx < balance.MaxBalls)
                        {
                            int ballIdx = runData.BallScoreIdxs[m_scoringIdx];
                            m_scoringIdx++;

                            int chips = Logic.CalculateBallChips(runData, balance, ballIdx);
                            if (chips > 0)
                            {
                                Debug.Log(Time.realtimeSinceStartupAsDouble + " " + GameState.ToString() + "  " + chips.ToString() + "Chips for ball " + runData.BallTypesInGame[ballIdx].ToString());

                                m_scoringTimer = 0.0f;

                                ShowBallChipsPopup(ballIdx, chips);

                                animateRoundChipsText();
                                break;
                            }
                        }
                    }
                }
            }
            if (GameState == GAME_STATE.SCORING_BALL_MULTIPLIER_ADD)
            {
                m_scoringTimer += dt * settingsData.Speed;
                if (m_scoringTimer > ScoringTime)
                {
                    BallsMultiplierGO.SetActive(false);

                    if (m_scoringIdx >= balance.MaxBalls)
                    {
                        setGameState(GAME_STATE.SCORING_BALL_MONEY);
                        m_scoringIdx = 0;
                    }
                    else
                    {
                        while (m_scoringIdx < balance.MaxBalls)
                        {
                            int ballIdx = runData.BallScoreIdxs[m_scoringIdx];
                            m_scoringIdx++;
                            float multiplier = Logic.CalculateBallMultiplierAdd(runData, balance, ballIdx);
                            if (multiplier > 0.0f)
                            {
                                Debug.Log(Time.realtimeSinceStartupAsDouble + " " + GameState.ToString() + "  " + multiplier.ToString() + "x Multiplier Add for ball " + runData.BallTypesInGame[ballIdx].ToString());

                                m_scoringTimer = 0.0f;

                                ShowBallMultiplierPopup(ballIdx, "+" + CommonVisual.GetMultiplierString(multiplier) + "x");

                                animateRoundMultipierText();
                                break;
                            }
                        }
                    }
                }
            }
            if (GameState == GAME_STATE.SCORING_BALL_MONEY)
            {
                m_scoringTimer += dt * settingsData.Speed;
                if (m_scoringTimer > ScoringTime)
                {
                    BallsMoneyGO.SetActive(false);

                    if (m_scoringIdx >= balance.MaxBalls)
                    {
                        int jokerIdx = Logic.CheckJokerRetriggerBalls(runData, balance);
                        if (jokerIdx > -1)
                        {
                            CommonVisual.JokerGUIs[jokerIdx].Animation.Play("ScoreGrow");

                            setGameState(GAME_STATE.SCORING_SLOT_CHIPS);
                        }
                        else
                        {
                            setGameState(GAME_STATE.SCORING_JOKER_CHIPS);
                        }
                        m_scoringIdx = 0;
                    }
                    else
                    {
                        Span<int> jokerIdxs = stackalloc int[runData.JokerCount];
                        while (m_scoringIdx < balance.MaxBalls)
                        {
                            int jokerCount = 0;
                            int ballIdx = runData.BallScoreIdxs[m_scoringIdx];
                            m_scoringIdx++;
                            int money = Logic.CalculateBallMoney(runData, balance, ballIdx, jokerIdxs, ref jokerCount);
                            if (money > 0)
                            {
                                Debug.Log(Time.realtimeSinceStartupAsDouble + " " + GameState.ToString() + "  " + money.ToString() + " Money for ball " + runData.BallTypesInGame[ballIdx].ToString());

                                m_scoringTimer = 0.0f;

                                ShowBallMoney(runData, settingsData, ballIdx, money);

                                for (int jIdx = 0; jIdx < jokerCount; jIdx++)
                                    CommonVisual.JokerGUIs[jokerIdxs[jIdx]].Animation.Play("ScoreGrow");
                                break;
                            }
                        }
                    }
                }
            }
            if (GameState == GAME_STATE.SCORING_JOKER_CHIPS)
            {
                m_scoringTimer += dt * settingsData.Speed;
                if (m_scoringTimer > ScoringTime)
                {
                    if (m_scoringIdx >= runData.JokerCount)
                    {
                        setGameState(GAME_STATE.SCORING_JOKER_MULTIPLIER_ADD);
                        m_scoringIdx = 0;
                    }
                    else
                    {
                        // find next joker
                        while (m_scoringIdx < runData.JokerCount)
                        {
                            int jokerIdx = m_scoringIdx;
                            m_scoringIdx++;

                            int jokerType = runData.JokerTypes[jokerIdx];
                            if (jokerType > -1)
                            {
                                int chips = Logic.CalculateJokerChipsAdd(runData, balance, jokerIdx, jokerType);
                                if (chips > 0)
                                {
                                    Debug.Log(Time.realtimeSinceStartupAsDouble + " " + GameState.ToString() + "  " + chips.ToString() + " Chips for Joker " + jokerType.ToString());

                                    m_scoringTimer = 0.0f;

                                    ShowJokerChipsPopup(jokerIdx, "+" + chips.ToString("N0"));

                                    animateRoundChipsText();
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            if (GameState == GAME_STATE.SCORING_JOKER_MULTIPLIER_ADD)
            {
                m_scoringTimer += dt * settingsData.Speed;
                if (m_scoringTimer > ScoringTime)
                {
                    if (m_scoringIdx >= runData.JokerCount)
                    {
                        setGameState(GAME_STATE.SCORING_SLOT_MULTIPLIER_MULT);
                        m_scoringIdx = 0;
                    }
                    else
                    {
                        // find next joker
                        while (m_scoringIdx < runData.JokerCount)
                        {
                            int jokerIdx = m_scoringIdx;
                            m_scoringIdx++;

                            int jokerType = runData.JokerTypes[jokerIdx];
                            if (jokerType > -1)
                            {
                                float mult = Logic.CalculateJokerMultiplierAdd(runData, balance, jokerIdx, jokerType);
                                if (mult > 0.0f)
                                {
                                    Debug.Log(Time.realtimeSinceStartupAsDouble + " " + GameState.ToString() + "  " + mult.ToString() + "x Multiplier Add for Joker " + jokerType.ToString());

                                    m_scoringTimer = 0.0f;

                                    ShowJokerMultPopup(jokerIdx, "+" + CommonVisual.GetMultiplierString(mult) + "x");

                                    animateRoundMultipierText();
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            if (GameState == GAME_STATE.SCORING_SLOT_MULTIPLIER_MULT)
            {
                m_scoringTimer += dt * settingsData.Speed;
                if (m_scoringTimer > ScoringTime)
                {
                    BallsMultiplierGO.SetActive(false);

                    if (m_scoringIdx >= balance.MaxBalls)
                    {
                        setGameState(GAME_STATE.SCORING_BALL_MULTIPLIER_MULT);
                        m_scoringIdx = 0;
                    }
                    else
                    {
                        while (m_scoringIdx < balance.MaxBalls)
                        {
                            int ballIdx = runData.BallScoreIdxs[m_scoringIdx];
                            m_scoringIdx++;
                            float multiplier = Logic.CalculateSlotBallMultiplierMult(runData, balance, ballIdx);
                            if (multiplier > 0.0f)
                            {
                                Debug.Log(Time.realtimeSinceStartupAsDouble + " " + GameState.ToString() + "  " + multiplier.ToString() + "x Multiplier Add for Ball " + runData.BallTypesInGame[ballIdx].ToString() + " in Slot");

                                m_scoringTimer = 0.0f;

                                ShowBallMultiplierPopup(ballIdx, "x" + CommonVisual.GetMultiplierString(multiplier));

                                animateRoundMultipierText();
                                break;
                            }
                        }
                    }
                }
            }
            if (GameState == GAME_STATE.SCORING_BALL_MULTIPLIER_MULT)
            {
                m_scoringTimer += dt * settingsData.Speed;
                if (m_scoringTimer > ScoringTime)
                {
                    BallsMultiplierGO.SetActive(false);

                    if (m_scoringIdx >= balance.MaxBalls)
                    {
                        setGameState(GAME_STATE.SCORING_JOKER_MULTIPLIER_MULT);
                        m_scoringIdx = 0;
                    }
                    else
                    {
                        while (m_scoringIdx < balance.MaxBalls)
                        {
                            int ballIdx = runData.BallScoreIdxs[m_scoringIdx];
                            m_scoringIdx++;
                            float multiplier = Logic.CalculateBallMultiplierMult(runData, balance, ballIdx);
                            if (multiplier > 1.0f)
                            {
                                Debug.Log(Time.realtimeSinceStartupAsDouble + " " + GameState.ToString() + "  " + multiplier.ToString() + "x Multiplier Mult for ball " + runData.BallTypesInGame[ballIdx].ToString());

                                m_scoringTimer = 0.0f;

                                ShowBallMultiplierPopup(ballIdx, "x" + CommonVisual.GetMultiplierString(multiplier));

                                animateRoundMultipierText();
                                break;
                            }
                        }
                    }
                }
            }
            if (GameState == GAME_STATE.SCORING_JOKER_MULTIPLIER_MULT)
            {
                m_scoringTimer += dt * settingsData.Speed;
                if (m_scoringTimer > ScoringTime)
                {
                    if (m_scoringIdx >= runData.JokerCount)
                    {
                        setGameState(GAME_STATE.SCORING_ROUND_TOTAL);
                        m_scoringIdx = 0;
                    }
                    else
                    {
                        // find next joker
                        while (m_scoringIdx < runData.JokerCount)
                        {
                            int jokerIdx = m_scoringIdx;
                            m_scoringIdx++;
                            int jokerType = runData.JokerTypes[jokerIdx];
                            if (jokerType > -1)
                            {
                                float mult = Logic.CalculateJokerMultiplierMult(runData, balance, jokerIdx, jokerType);
                                if (mult > 1.0f)
                                {
                                    m_scoringTimer = 0.0f;
                                    Debug.Log(Time.realtimeSinceStartupAsDouble + " " + GameState.ToString() + "  " + mult.ToString() + "x Multiplier Mult for Joker " + jokerType.ToString());

                                    ShowJokerMultPopup(jokerIdx, "x" + CommonVisual.GetMultiplierString(mult - 1));

                                    animateRoundMultipierText();
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            if (GameState == GAME_STATE.SCORING_ROUND_TOTAL)
            {
                m_scoringTimer += dt * settingsData.Speed;
                if (m_scoringTimer > ScoringTime)
                {
                    m_scoringTimer = 0.0f;

                    int roundTotalScore = Logic.CalculateTotalScore(runData);
                    m_totalRoundScoreText.text = roundTotalScore.ToString("N0");
                    m_totalRoundScoreAnimation.Play();

                    m_totalScoreText.text = runData.TotalChips.ToString("N0");
                    m_totalScoreAnimation.Play();
                    SoundManager.Instance.PlaySFXScoringTotal();

                    if (Logic.InBossRound(runData) && runData.TotalChips == 0)
                        m_bossGrowAnim.Play("ScoreGrow");

                    Logic.JokerPostSpin(runData, balance);

                    setGameState(GAME_STATE.JOKER_POST_SPIN);
                }
            }
            if (GameState == GAME_STATE.JOKER_POST_SPIN)
            {
                m_scoringTimer += dt;// * settingsData.Speed;
                if (m_scoringTimer > ScoringTime)
                {
                    if (m_scoringIdx >= runData.JokerCount)
                    {
                        if (Logic.CheckRoundComplete(runData, balance))
                        {
                            setGameState(GAME_STATE.JOKER_POST_ROUND);
                            m_scoringIdx = 0;
                        }
                        else
                        {
                            setGameState(GAME_STATE.BOSS_POST_SPIN);
                            m_scoringIdx = 0;
                        }
                    }
                    else
                    {
                        // find next joker
                        while (m_scoringIdx < runData.JokerCount)
                        {
                            int jokerIdx = m_scoringIdx;
                            m_scoringIdx++;
                            int jokerType = runData.JokerTypes[jokerIdx];
                            if (jokerType > -1)
                            {
                                if (balance.JokerBalance.SubtractChipsPerSpin[jokerType].y > 0 && runData.JokerChips[jokerIdx] > 0)
                                {
                                    int amount = Mathf.FloorToInt(balance.JokerBalance.SubtractChipsPerSpin[jokerType].y);
                                    m_scoringTimer = 0.0f;
                                    Debug.Log(Time.realtimeSinceStartupAsDouble + " " + GameState.ToString() + " -" + amount.ToString() + " Chips for Joker " + jokerType.ToString());

                                    ShowJokerChipsPopup(jokerIdx, "-" + amount.ToString("N0"));

                                    SoundManager.Instance.PlaySFXScoring();
                                    break;
                                }

                                if (balance.JokerBalance.MoneyPerSpin[jokerType] != 0)
                                {
                                    // lose money and update top bar
                                    CommonVisual.UpdateTopBarMoney(runData, m_topBarGUI);
                                    CommonVisual.JokerGUIs[jokerIdx].Animation.Play("ScoreGrow");
                                    SoundManager.Instance.PlaySFXMoney();
                                }
                            }
                        }
                    }
                }
            }
            if (GameState == GAME_STATE.JOKER_POST_ROUND)
            {
                m_scoringTimer += dt;// * settingsData.Speed;
                if (m_scoringTimer > ScoringTime)
                {
                    if (m_scoringIdx >= runData.JokerCount)
                    {
                        setGameState(GAME_STATE.SPIN_OVER);
                        m_nextSpinTimer = 0.0f;
                        m_scoringIdx = 0;
                    }
                    else
                    {
                        // find next joker
                        while (m_scoringIdx < runData.JokerCount)
                        {
                            int jokerIdx = m_scoringIdx;
                            m_scoringIdx++;
                            int jokerType = runData.JokerTypes[jokerIdx];
                            if (jokerType > -1)
                            {
                                if (balance.JokerBalance.SubtractMultiplierAddPerRound[jokerType].y > 0 && runData.JokerMultiplierAdd[jokerIdx] > 0)
                                {
                                    int amount = Mathf.FloorToInt(balance.JokerBalance.SubtractMultiplierAddPerRound[jokerType].y);
                                    m_scoringTimer = 0.0f;
                                    Debug.Log(Time.realtimeSinceStartupAsDouble + " " + GameState.ToString() + " -" + amount.ToString() + "x Mult for Joker " + jokerType.ToString());

                                    ShowJokerMultPopup(jokerIdx, "-" + amount.ToString("N0"));
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            if (GameState == GAME_STATE.BOSS_POST_SPIN)
            {
                m_scoringTimer += dt;// * settingsData.Speed;
                if (m_scoringTimer > ScoringTime)
                {
                    if (Logic.InBossRound(runData))
                    {
                        int bossType = Logic.GetBossTypeForRound(runData);

                        if (balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.LOSE_MONEY_EVERY_SPIN)
                        {
                            m_scoringTimer = 0.0f;

                            Logic.PostRoundBossEffect(runData, balance);

                            CommonVisual.UpdateTopBarMoney(runData, m_topBarGUI);
                            SoundManager.Instance.PlaySFXMoney();
                        }
                    }
                    m_nextSpinTimer = 0.0f;
                    setGameState(GAME_STATE.SPIN_OVER);
                }
            }
            if (GameState == GAME_STATE.SPIN_OVER)
            {
                m_nextSpinTimer += dt;// * settingsData.Speed;
                if (m_nextSpinTimer > NextSpinTime)
                {
                    Logic.SpinComplete(runData, balance);

                    if (Logic.InBossRound(runData))
                    {
                        int bossType = Logic.GetBossTypeForRound(runData);

                        if (runData.CurrentSpin == 1)
                        {
                            if (balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.BALLS_DEBUFFED_FIRST_SPIN)
                                showBallsInGame(1, runData.UseBallsSpecial == 0);
                            if (balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.SLOTS_DEBUFFED_FIRST_SPIN)
                                CommonSlotsVisual.ShowSpinWheel(runData, balance, m_scoringSlots, runData.SlotTypeInGame, m_showSlotEffects, runData.UseSlotsSpecial == 0);
                        }
                        if (balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.DIFFERENT_COLOR_EVERY_SPIN)
                            CommonSlotsVisual.ShowSpinWheel(runData, balance, m_scoringSlots, runData.SlotTypeInGame, m_showSlotEffects, runData.UseSlotsSpecial == 0);

                        if (runData.CurrentSpin < 2 && balance.BossBalance.BossEffect[bossType] >= BOSS_EFFECT.ONLY_RED_FIRST_SPIN &&
                            balance.BossBalance.BossEffect[bossType] <= BOSS_EFFECT.ONLY_BLUE_FIRST_SPIN)
                            CommonSlotsVisual.ShowSpinWheel(runData, balance, m_scoringSlots, runData.SlotTypeInGame, m_showSlotEffects, runData.UseSlotsSpecial == 0);

                    }

                    if (Logic.CheckWin(runData, balance))
                        Game.Instance.WinScreen();
                    else if (Logic.CheckRoundComplete(runData, balance))
                        Game.Instance.RoundComplete();
                    else if (Logic.CheckGameOver(runData))
                        Game.Instance.GameOver();
                    else
                        startSpin(runData, balance);
                }
            }

            handleInput();

            if (GameState >= GAME_STATE.BALLS_DROPPED)
            {
                for (int slotIdx = 0; slotIdx < balance.NumSlots; slotIdx++)
                {
                    if (m_slotJuiceTimer[slotIdx] > 0.0f)
                    {
                        float value = 1.0f - m_slotJuiceTimer[slotIdx];
                        m_slotJuiceTimer[slotIdx] -= dt;
                        float colorMult = SlotScaleAnimCurve.Evaluate(value) * 0.5f;
                        int slotType = (int)runData.SlotTypeInGame[slotIdx];
                        m_scoringSlots[slotIdx].SpriteRenderer.color = runData.SlotColors[slotType] + Color.white * colorMult;
                    }
                }
            }

            // slot animation
            if (m_slotAnimTimer > 0.0f)
            {
                m_slotAnimTimer -= dt * settingsData.Speed;
                float value = 1.0f - m_slotAnimTimer;
                if (value > 1.0f)
                    value = 1.0f;

                CommonSlotsVisual.TickHighlightChangedSlots(value, SlotScaleAnimCurve, m_scoringSlots, runData.SlotTypeInGame, runData.SlotColors);
            }

            // AI
            if (GameState == GAME_STATE.WAITING_FOR_INPUT)
            {
                m_waitingForInputTime += dt;

                if (AUTO_DROP)
                {
                    // m_waitingForInputTime 0.9099129 angle 238.6025
                    if (m_prevSpinWheelZ > AutoDropAngle && SpinCircle.transform.eulerAngles.z < AutoDropAngle)
                    {
                        dropBalls();
                    }
                }
            }
            m_prevSpinWheelZ = SpinCircle.transform.eulerAngles.z;

            if (ShowDebug)
            {
                DebugRotationSpeed = runData.RotationSpeed;
            }
        }

        void handleInput()
        {
            if (GameState == GAME_STATE.WAITING_FOR_INPUT)
            {
                if (Keyboard.current != null)
                    if (Keyboard.current.spaceKey.wasPressedThisFrame)
                        dropBalls();

                // button trigger
                if (m_selectedButton == MENU_BUTTONS.DROP && CommonButtonVisual.NavigateEnter())
                {
                    dropBalls();
                    return;
                }

                if (CommonButtonVisual.NavigateGamepadButton(m_spinButtonData))
                {
                    dropBalls();
                    return;
                }
            }
            if (m_selectedButton == MENU_BUTTONS.INFO && CommonButtonVisual.NavigateEnter() || CommonButtonVisual.NavigateGamepadButton(m_infoButtonData))
            {
                showGameInfo();
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.SETTINGS && CommonButtonVisual.NavigateEnter() || CommonButtonVisual.NavigateGamepadButton(m_topBarGUI.SettingsButtonData))
            {
                Game.Instance.GoToSettings();
                return;
            }

            if ((m_selectedButton >= MENU_BUTTONS.JOKER_1 && m_selectedButton <= MENU_BUTTONS.JOKER_5) && CommonButtonVisual.NavigateEnter())
            {
                Game.Instance.ShowJokerInfoPopupInGame((int)m_selectedButton - (int)MENU_BUTTONS.JOKER_1);
                return;
            }

            // navigation
            if (m_selectedButton == MENU_BUTTONS.DROP && CommonButtonVisual.NavigateLeft())
            {
                selectButton(MENU_BUTTONS.INFO);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.DROP && CommonButtonVisual.NavigateUp())
            {
                selectButton(MENU_BUTTONS.JOKER_1);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.INFO && CommonButtonVisual.NavigateRight())
            {
                selectButton(MENU_BUTTONS.DROP);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.INFO && CommonButtonVisual.NavigateUp())
            {
                selectButton(MENU_BUTTONS.SETTINGS);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.SETTINGS && CommonButtonVisual.NavigateDown())
            {
                selectButton(MENU_BUTTONS.INFO);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.SETTINGS && CommonButtonVisual.NavigateRight())
            {
                if (runData.JokerCount > 0)
                    selectButton(MENU_BUTTONS.JOKER_1);
                else
                    selectButton(MENU_BUTTONS.DROP);
                return;
            }

            if ((m_selectedButton >= MENU_BUTTONS.JOKER_1 && m_selectedButton <= MENU_BUTTONS.JOKER_5) && CommonButtonVisual.NavigateDown())
            {
                selectButton(MENU_BUTTONS.DROP);
                return;
            }

            if ((m_selectedButton >= MENU_BUTTONS.JOKER_1 && m_selectedButton <= MENU_BUTTONS.JOKER_5) && CommonButtonVisual.NavigateLeft())
            {
                selectButton(MENU_BUTTONS.SETTINGS);
                return;
            }

            int selectedJokerButton = CommonButtonVisual.CommonNavigation(runData, (COMMON_BUTTONS)m_selectedButton);
            if (selectedJokerButton > -1)
                selectButton((MENU_BUTTONS)selectedJokerButton);

#if UNITY_EDITOR
            if (Keyboard.current.spaceKey.wasReleasedThisFrame)
                dropBalls();

            if (Keyboard.current.vKey.wasReleasedThisFrame)
                for (int i = 0; i < m_numBalls; i++)
                    ParticleSystemSmokeBoard.Emit(Color.white, BallsGO[i].transform.position, 1.0f);
#endif
        }

        public void EmitSmoke(Vector2 position, float magnitude)
        {
            ParticleSystemSmokeBoard.Emit(Color.white, position, magnitude);
        }

        bool allBlocksLocked()
        {
            for (int i = 0; i < m_numBalls; i++)
                if (!m_ballLockedInSlot[i])
                    return false;
            return true;

        }

        private void startScoring(RunData runData, Balance balance)
        {
            Debug.Log("startScoring " + Time.realtimeSinceStartupAsDouble);
            setGameState(GAME_STATE.SCORING_SLOT_CHIPS);
            m_scoringIdx = 0;
            m_scoringTimer = ScoringTime;

            Logic.StartScoring(runData, balance);
        }

        public void StartRound(RunData runData, Balance balance)
        {
            m_totalScoreText.text = runData.TotalChips.ToString("N0");

            runData.SpinWheelAngle = 0.0f;

            m_scoringTimer = ScoringTime;
            m_scoringIdx = 0;
            setGameState(GAME_STATE.START_ROUND);

            m_roundChipsText.text = "0";
            m_roundMultiplierText.text = CommonVisual.GetMultiplierString(balance.BaseMultiplier) + "x";
            m_totalRoundScoreText.text = "0";

            BallsChipsGO.SetActive(false);
            BallsMultiplierGO.SetActive(false);
            BallsMoneyGO.SetActive(false);
        }

        private void resetSpin(RunData runData, Balance balance)
        {
            runData.RotationSpeed = 0.0f;
            m_spinAnimTime = 0.0f;

            m_roundChipsText.text = "0";
            m_roundMultiplierText.text = CommonVisual.GetMultiplierString(balance.BaseMultiplier);
            m_totalRoundScoreText.text = "0";

            BallsChipsGO.SetActive(false);
            BallsMultiplierGO.SetActive(false);
            BallsMoneyGO.SetActive(false);

            GateGO.SetActive(true);
            for (int i = 0; i < m_numBalls; i++)
            {
                BallsGO[i].transform.position = BallStartPos[i].position;
                m_ballLockedInSlot[i] = false;
            }

            for (int i = 0; i < m_ballsRB.Length; i++)
                m_ballsRB[i].bodyType = RigidbodyType2D.Dynamic;
            Debug.Log("Set balls to DYNAMIC");

            for (int i = 0; i < m_scoringSlots.Length; i++)
                m_scoringSlots[i].LockGO.SetActive(false);
        }

        private void startSpin(RunData runData, Balance balance)
        {
            resetSpin(runData, balance);

            setGameState(GAME_STATE.SPIN_UP);
            SpinState = SPIN_STATE.SPIN_UP;

            Logic.StartSpin(runData, balance);
            if (Logic.InBossRound(runData))
            {
                bool slotsChanged;
                Logic.StartSpinBossEffect(runData, balance, out slotsChanged, CommonSlotsVisual.ChangedSlotsIdxs, ref CommonSlotsVisual.ChangedSlotsCount);
                if (slotsChanged)
                {
                    CommonSlotsVisual.ShowSpinWheel(runData, balance, m_scoringSlots, runData.SlotTypeInGame, m_showSlotEffects, runData.UseSlotsSpecial == 0);
                    m_slotAnimTimer = m_slotAnimTime;
                }

                int bossType = Logic.GetBossTypeForRound(runData);
                if (balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.JUMBLE_BALLS)
                {
                    Logic.JumbleBalls(runData, balance);
                    showBallsInGame(1, runData.UseBallsSpecial == 0);
                }
                if (balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.RANDOM_JOKE_DEBUFFED_PER_SPIN)
                {
                    CommonVisual.UpdateJokerDebuff(runData);
                }
            }

            m_spinsText.text = (runData.CurrentSpin + 1).ToString("N0") + " / " + runData.MaxSpinsThisRound.ToString("N0");

            RunDataIO.SaveRun(runData, balance);
        }

        public void dropBalls()
        {
            SoundManager.Instance.PlaySFXGateOpen();

            if (GameState == GAME_STATE.WAITING_FOR_INPUT)
            {
#if UNITY_EDITOR
                // m_waitingForInputTime 0.9099129 angle 238.6025 - 6 balls
                Debug.Log("m_waitingForInputTime " + m_waitingForInputTime + " angle " + SpinCircle.transform.rotation.eulerAngles.z);
                m_droppedAngle = SpinCircle.transform.rotation.eulerAngles.z;
#endif

                Logic.DropBalls(runData);

                GateGO.SetActive(false);
                setGameState(GAME_STATE.BALLS_DROPPED);
                SpinState = SPIN_STATE.SPIN_BALLS;

                RunDataIO.SaveRun(runData, balance);
            }
        }

        public void BallInSlot(RunData runData, Balance balance, int ballIdx, int slotIdx)
        {
            int slotChangedIdx;
            int slotChangeJokerIdx;
            int jokerMultIncIdx;
            int jokerMultInc;
            if (Logic.BallInSlot(runData, balance, ballIdx, slotIdx, out slotChangedIdx, out slotChangeJokerIdx, out jokerMultIncIdx, out jokerMultInc))
            {
                m_ballsRB[ballIdx].bodyType = RigidbodyType2D.Static;
                Debug.Log("Set ball " + ballIdx + " to STATIC");
                if (slotChangedIdx > -1)
                {
                    m_slotAnimTimer = m_slotAnimTime;
                    CommonSlotsVisual.ChangedSlotsIdxs[CommonSlotsVisual.ChangedSlotsCount++] = slotChangedIdx;
                    CommonVisual.JokerGUIs[slotChangeJokerIdx].Animation.Play("ScoreGrow");
                    CommonSlotsVisual.ShowSpinWheel(runData, balance, m_scoringSlots, runData.SlotTypeInGame, m_showSlotEffects, runData.UseSlotsSpecial == 0);
                }

                if (jokerMultIncIdx > -1)
                {
                    CommonVisual.JokerGUIs[slotChangeJokerIdx].Animation.Play("ScoreGrow");

                    ShowJokerMultPopup(jokerMultIncIdx, "+" + CommonVisual.GetMultiplierString(jokerMultInc) + "x");
                }

                // for (int i = 0; i < m_scoringSlots.Length; i++)
                m_scoringSlots[slotIdx].LockGO.SetActive(true);

                m_slotJuiceTimer[slotIdx] = 1.0f;
            }
        }

        private void ShowBallMoney(RunData runData, SettingsData settingsData, int ballIdx, int money)
        {
            Vector3 ballPos = BallsGO[ballIdx].transform.position;
            BallsMoneyGO.SetActive(true);
            BallsMoneyGO.transform.position = new Vector3(ballPos.x, ballPos.y, ballPos.z - 10.0f);
            m_ballsMoneyText.text = "◇" + money.ToString("N0");

            CommonVisual.UpdateTopBarMoney(runData, m_topBarGUI);
            SoundManager.Instance.PlaySFXMoney();
        }

        public void UpdateTopUI(RunData runData, Balance balance)
        {
            CommonVisual.UpdateTopBarMoney(runData, m_topBarGUI);
            CommonVisual.ShowJokersInGame(runData, balance, m_jokerParent);
        }

        public static void SortSlots(RunData runData)
        {
            Logic.SortSlots(runData);
        }

        void showGameInfo()
        {
            SoundManager.Instance.PlaySFXButtonOK();

            Game.Instance.SetMenuState(MENU_STATE.IN_GAME_INFO);
        }


#if UNITY_EDITOR
        float m_droppedAngle;
        float m_increaseSize;
        void doSpinTest(RunData runData, Balance balance)
        {
            Span<int> slotTypeCount = stackalloc int[4];
            Logic.CountNumBallsOnSlotType(runData, balance.MaxBalls, slotTypeCount);

            bool dropped6 = false;
            for (int i = 0; i < 4; i++)
                if (slotTypeCount[i] == 6)
                    dropped6 = true;

            m_increaseSize = 1.0f;
            if (dropped6)
            {
                Debug.Log("m_droppedAngle " + m_droppedAngle + " AutoDropAngle " + AutoDropAngle);
            }
            else
            {
                AutoDropAngle += m_increaseSize;
                if (AutoDropAngle > 152.0f)
                {
                    AutoDropAngle = 148.0f;
                    m_increaseSize /= 2.0f;
                }
            }

            startSpin(runData, balance);
        }
#endif
    }
}