using System;
using CommonTools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Lofelt.NiceVibrations;

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
    }

    public class Board : MonoBehaviour
    {
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

        public GameObject BallPrefab;
        public Transform BallParent;
        int m_numBalls;
        public GameObject[] BallsGO;
        public Transform[] BallStartPos;
        Vector3[] m_ballStartPos;
        Rigidbody2D[] m_ballsRB;
        BallSprites[] m_ballSprites;

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

        // rounds
        public float NextSpinTime;
        float m_nextSpinTimer;

        // spins
        TextMeshProUGUI m_spinsText;
        Button m_spinButton;
        Image m_spinButtonImage;

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

        float m_prevSpinWheelAngle;

        // AI
        float m_waitingForInputTime = 0.0f;
        float m_prevSpinWheelZ;
        public bool AUTO_DROP = false;
        public float AutoDropAngle = 238.6025f;

        public bool SpinTest = false;

        bool m_showSlotEffects = true;
        bool m_slotsDebuffed = false;

        [Header("Debug")]
        public bool ShowDebug;
        public float DebugRotationSpeed;


        // Start is called before the first frame update
        public void Init(Balance balance, GameInfoSO gameInfoSO)
        {
            transform.localPosition = gameInfoSO.Position;
            transform.localScale = gameInfoSO.Scale;

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
            GUIRef guiRef = m_UI.GetComponent<GUIRef>();

            CommonVisual.InitTopBarGUI(guiRef.GetGameObject("TopBar"), ref m_topBarGUI);

            m_spinButton = guiRef.GetButton("Spin");
            m_spinButtonImage = guiRef.GetImage("Spin");
            m_spinButton.onClick.AddListener(Game.Instance.DropBalls);
            m_roundChipsText = guiRef.GetTextGUI("Score");
            m_roundMultiplierText = guiRef.GetTextGUI("Multiplier");
            m_roundMultiplierAnimation = guiRef.GetAnimation("Multiplier");
            m_roundScoreAnimation = guiRef.GetAnimation("Score");
            m_totalScoreText = guiRef.GetTextGUI("TotalScore");
            m_totalScoreAnimation = guiRef.GetAnimation("TotalScore");
            m_totalRoundScoreText = guiRef.GetTextGUI("TotalRoundScore");
            m_totalRoundScoreAnimation = guiRef.GetAnimation("TotalRoundScore");
            m_goalText = guiRef.GetTextGUI("Goal");

            guiRef.GetButton("Info").onClick.AddListener(Game.Instance.ShowGameInfo);

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

            m_UI.SetActive(false);
            BoardSpites.SetActive(false);

            SpinWheelLights.Init();
        }

        public void Show(RunData runData, Balance balance)
        {
            m_goalText.text = Logic.GetRoundGoal(runData, balance).ToString("N0");
            m_totalScoreText.text = runData.TotalChips.ToString("N0");

            m_UI.SetActive(true);
            BoardSpites.SetActive(true);

            for (int i = 0; i < m_numBalls; i++)
                m_ballStartPos[i] = BallStartPos[i].position;

            m_showSlotEffects = true;
            m_slotsDebuffed = false;
            CommonSlotsVisual.CheckSpinWheelDebuffForNewRound(runData, balance, runData.Round, out m_showSlotEffects, out m_slotsDebuffed);

            int useBallSprite = 1;
            bool ballsDebuffed = false;
            if (Logic.InBossRound(runData))
            {
                int bossType = Logic.GetBossTypeForRound(runData);

                if (balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.BALL_EFFECTS_HIDDEN)
                    useBallSprite = 0;

                if (balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.BALLS_DEBUFFED ||
                    balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.BALLS_DEBUFFED_FIRST_SPIN)
                    ballsDebuffed = true;
            }


            CommonSlotsVisual.ShowSpinWheel(runData, balance, m_scoringSlots, runData.SlotTypeInGame, m_showSlotEffects, runData.UseSlotsSpecial == 0);

            CommonVisual.ShowJokersInGame(runData, balance, m_jokerParent);

            showBalls(runData, balance, useBallSprite, ballsDebuffed);

            SpinWheelLights.StartAnimation();

            CommonVisual.UpdateTopBarMoney(runData, m_topBarGUI);
        }

        public void showBalls(RunData runData, Balance balance, int useBallSprite, bool debuffed)
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

        public void AnimateRoundChipsText(RunData runData, SettingsData settingsData)
        {
            m_roundChipsText.text = runData.SpinChips.ToString("N0");
            m_roundScoreAnimation.Play();
            SoundManager.Instance.PlaySFXScoring(settingsData);
        }

        public void AnimateRoundMultipierText(RunData runData, SettingsData settingsData)
        {
            m_roundMultiplierText.text = "x" + CommonVisual.GetMultiplierString(runData.SpinMultiplier);
            m_roundMultiplierAnimation.Play();
            SoundManager.Instance.PlaySFXScoring(settingsData);
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

        public void ShowJokerColorPopup(RunData runData, Balance balance, int jokerIdx)
        {
            m_jokerColorGO.SetActive(false);
            m_jokerColorGO.transform.position = CommonVisual.JokerPool[jokerIdx].transform.position;
            m_jokerColorGO.SetActive(true);
            SLOT_TYPE leastPlayedColor = Logic.GetLeastPlayedSlotType(runData);
            m_jokerColorText.text = leastPlayedColor.ToString();
            m_jokerColorText.color = balance.SlotColors[(int)leastPlayedColor];
        }

        void setGameState(GAME_STATE newGamState, Balance balance)
        {
            Debug.Log("setGameState(" + newGamState + ") m_scoringTimer " + m_scoringTimer);
            GameState = newGamState;

            hideJokerPopups();

            if (GameState == GAME_STATE.WAITING_FOR_INPUT)
            {
                m_spinButton.interactable = true;
                m_spinButtonImage.color = balance.ButtonColorEnabled;
            }
            else
            {
                m_spinButton.interactable = false;
                m_spinButtonImage.color = balance.ButtonColorDisabled;
            }
        }

        // Update is called once per frame
        public void Tick(RunData runData, Balance balance, SettingsData settingsData, float dt)
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
                SoundManager.Instance.PlaySFXWheelSpin(settingsData);
            }

            if (GameState == GAME_STATE.START_ROUND)
            {
                m_scoringTimer += dt * settingsData.Speed;
                Debug.Log(GameState.ToString() + " m_scoringTime " + m_scoringTimer);
                if (m_scoringTimer > ScoringTime)
                {
                    setGameState(GAME_STATE.JOKER_PRE_ROUND, balance);
                    return;
                }
            }
            if (GameState == GAME_STATE.JOKER_PRE_ROUND)
            {
                m_scoringTimer += dt * settingsData.Speed;
                if (m_scoringTimer > ScoringTime)
                {
                    while (m_scoringIdx < runData.JokerCount)
                    {
                        int jokerIdx = m_scoringIdx;
                        int jokerType = runData.JokerTypes[jokerIdx];
                        m_scoringIdx++;

                        if (balance.JokerBalance.MultiplierAddForLeastPlayedColor[jokerType] > 0.0f)
                        {
                            ShowJokerColorPopup(runData, balance, jokerIdx);

                            m_scoringTimer = 0.0f;
                            break;
                        }

                        int slotChangedIdx = Logic.JokerPreRoundTryModifySlot(runData, balance, jokerType);
                        if (slotChangedIdx > -1)
                        {
                            CommonSlotsVisual.AffectedSlotsIdxs[CommonSlotsVisual.AffectedSlotsCount++] = slotChangedIdx;
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
                        setGameState(GAME_STATE.WAITING_FOR_INPUT, balance);
                        m_waitingForInputTime = 0.0f;
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
                        //todo check all balls are in slots
                        if (Logic.AllBallsInSlot(runData) && allBlocksLocked())
                        {
                            startScoring(runData, balance);
                            SpinState = SPIN_STATE.DONE;
                            if (SpinTest)
                                doSpinTest(runData, balance);
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
                        setGameState(GAME_STATE.SCORING_SLOT_MULTIPLIER_ADD, balance);
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

                                AnimateRoundChipsText(runData, settingsData);
                                break;
                            }
                        }
                    }
                }
            }
            if (GameState == GAME_STATE.SCORING_SLOT_MULTIPLIER_ADD)
            {
                m_scoringTimer += dt * settingsData.Speed; ;
                if (m_scoringTimer > ScoringTime)
                {
                    BallsMultiplierGO.SetActive(false);

                    if (m_scoringIdx >= balance.MaxBalls)
                    {
                        setGameState(GAME_STATE.SCORING_SLOT_MONEY, balance);
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

                                AnimateRoundMultipierText(runData, settingsData);
                                break;
                            }
                        }
                    }
                }
            }
            if (GameState == GAME_STATE.SCORING_SLOT_MONEY)
            {
                m_scoringTimer += dt * settingsData.Speed; ;
                if (m_scoringTimer > ScoringTime)
                {
                    BallsMoneyGO.SetActive(false);

                    if (m_scoringIdx >= balance.MaxBalls)
                    {
                        setGameState(GAME_STATE.SCORING_BALL_CHIPS, balance);
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
                m_scoringTimer += dt * settingsData.Speed; ;
                if (m_scoringTimer > ScoringTime)
                {
                    BallsChipsGO.SetActive(false);

                    if (m_scoringIdx >= balance.MaxBalls)
                    {
                        setGameState(GAME_STATE.SCORING_BALL_MULTIPLIER_ADD, balance);
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

                                AnimateRoundChipsText(runData, settingsData);
                                break;
                            }
                        }
                    }
                }
            }
            if (GameState == GAME_STATE.SCORING_BALL_MULTIPLIER_ADD)
            {
                m_scoringTimer += dt * settingsData.Speed; ;
                if (m_scoringTimer > ScoringTime)
                {
                    BallsMultiplierGO.SetActive(false);

                    if (m_scoringIdx >= balance.MaxBalls)
                    {
                        setGameState(GAME_STATE.SCORING_BALL_MONEY, balance);
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

                                AnimateRoundMultipierText(runData, settingsData);
                                break;
                            }
                        }
                    }
                }
            }
            if (GameState == GAME_STATE.SCORING_BALL_MONEY)
            {
                m_scoringTimer += dt * settingsData.Speed; ;
                if (m_scoringTimer > ScoringTime)
                {
                    BallsMoneyGO.SetActive(false);

                    if (m_scoringIdx >= balance.MaxBalls)
                    {
                        int jokerIdx = Logic.CheckJokerRetriggerBalls(runData, balance);
                        if (jokerIdx > -1)
                        {
                            CommonVisual.JokerGUIs[jokerIdx].Animation.Play("ScoreGrow");

                            setGameState(GAME_STATE.SCORING_SLOT_CHIPS, balance);
                        }
                        else
                        {
                            setGameState(GAME_STATE.SCORING_JOKER_CHIPS, balance);
                        }
                        m_scoringIdx = 0;
                    }
                    else
                    {
                        while (m_scoringIdx < balance.MaxBalls)
                        {
                            Span<int> jokerIdxs = new int[runData.JokerCount];
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
                m_scoringTimer += dt * settingsData.Speed; ;
                if (m_scoringTimer > ScoringTime)
                {
                    if (m_scoringIdx >= runData.JokerCount)
                    {
                        setGameState(GAME_STATE.SCORING_JOKER_MULTIPLIER_ADD, balance);
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

                                    AnimateRoundChipsText(runData, settingsData);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            if (GameState == GAME_STATE.SCORING_JOKER_MULTIPLIER_ADD)
            {
                m_scoringTimer += dt * settingsData.Speed; ;
                if (m_scoringTimer > ScoringTime)
                {
                    if (m_scoringIdx >= runData.JokerCount)
                    {
                        setGameState(GAME_STATE.SCORING_SLOT_MULTIPLIER_MULT, balance);
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

                                    AnimateRoundMultipierText(runData, settingsData);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            if (GameState == GAME_STATE.SCORING_SLOT_MULTIPLIER_MULT)
            {
                m_scoringTimer += dt * settingsData.Speed; ;
                if (m_scoringTimer > ScoringTime)
                {
                    BallsMultiplierGO.SetActive(false);

                    if (m_scoringIdx >= balance.MaxBalls)
                    {
                        setGameState(GAME_STATE.SCORING_BALL_MULTIPLIER_MULT, balance);
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

                                AnimateRoundMultipierText(runData, settingsData);
                                break;
                            }
                        }
                    }
                }
            }
            if (GameState == GAME_STATE.SCORING_BALL_MULTIPLIER_MULT)
            {
                m_scoringTimer += dt * settingsData.Speed; ;
                if (m_scoringTimer > ScoringTime)
                {
                    BallsMultiplierGO.SetActive(false);

                    if (m_scoringIdx >= balance.MaxBalls)
                    {
                        setGameState(GAME_STATE.SCORING_JOKER_MULTIPLIER_MULT, balance);
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

                                AnimateRoundMultipierText(runData, settingsData);
                                break;
                            }
                        }
                    }
                }
            }
            if (GameState == GAME_STATE.SCORING_JOKER_MULTIPLIER_MULT)
            {
                m_scoringTimer += dt * settingsData.Speed; ;
                if (m_scoringTimer > ScoringTime)
                {
                    if (m_scoringIdx >= runData.JokerCount)
                    {
                        setGameState(GAME_STATE.SCORING_ROUND_TOTAL, balance);
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

                                    AnimateRoundMultipierText(runData, settingsData);
                                    break;
                                }
                            }
                        }
                    }
                }
            }
            if (GameState == GAME_STATE.SCORING_ROUND_TOTAL)
            {
                m_scoringTimer += dt * settingsData.Speed; ;
                if (m_scoringTimer > ScoringTime)
                {
                    m_scoringTimer = 0.0f;

                    int roundTotalScore = Mathf.FloorToInt((float)runData.SpinChips * runData.SpinMultiplier);
                    m_totalRoundScoreText.text = roundTotalScore.ToString("N0");
                    m_totalRoundScoreAnimation.Play();

                    if (runData.BestSpin < roundTotalScore)
                        runData.BestSpin = roundTotalScore;
                    runData.TotalChips += roundTotalScore;
                    m_totalScoreText.text = runData.TotalChips.ToString("N0");
                    m_totalScoreAnimation.Play();
                    SoundManager.Instance.PlaySFXScoringTotal(settingsData);

                    Logic.JokerPostSpin(runData, balance);

                    setGameState(GAME_STATE.JOKER_POST_SPIN, balance);
                }
            }
            if (GameState == GAME_STATE.JOKER_POST_SPIN)
            {
                m_scoringTimer += dt * settingsData.Speed; ;
                if (m_scoringTimer > ScoringTime)
                {
                    if (m_scoringIdx >= runData.JokerCount)
                    {
                        if (Logic.CheckRoundComplete(runData, balance))
                        {
                            setGameState(GAME_STATE.JOKER_POST_ROUND, balance);
                            m_scoringIdx = 0;
                        }
                        else
                        {
                            setGameState(GAME_STATE.BOSS_POST_SPIN, balance);
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

                                    SoundManager.Instance.PlaySFXScoring(settingsData);
                                    break;
                                }

                                if (balance.JokerBalance.MoneyPerSpin[jokerType] != 0)
                                {
                                    // lose money and update top bar
                                    CommonVisual.UpdateTopBarMoney(runData, m_topBarGUI);
                                    CommonVisual.JokerGUIs[jokerIdx].Animation.Play("ScoreGrow");
                                    SoundManager.Instance.PlaySFXMoney(settingsData);
                                }
                            }
                        }
                    }
                }
            }
            if (GameState == GAME_STATE.JOKER_POST_ROUND)
            {
                m_scoringTimer += dt * settingsData.Speed; ;
                if (m_scoringTimer > ScoringTime)
                {
                    if (m_scoringIdx >= runData.JokerCount)
                    {
                        setGameState(GAME_STATE.SPIN_OVER, balance);
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
                m_scoringTimer += dt * settingsData.Speed; ;
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
                            SoundManager.Instance.PlaySFXMoney(settingsData);
                        }
                    }
                    m_nextSpinTimer = 0.0f;
                    setGameState(GAME_STATE.SPIN_OVER, balance);
                }
            }
            if (GameState == GAME_STATE.SPIN_OVER)
            {
                m_nextSpinTimer += dt * settingsData.Speed; ;
                if (m_nextSpinTimer > NextSpinTime)
                {
                    Logic.SpinComplete(runData, balance);

                    if (Logic.InBossRound(runData))
                    {
                        int bossType = Logic.GetBossTypeForRound(runData);

                        if (runData.CurrentSpin == 1)
                        {
                            if (balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.BALLS_DEBUFFED_FIRST_SPIN)
                                showBalls(runData, balance, 1, false);
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
                        DropBalls(balance);
                    }
                }
            }
            m_prevSpinWheelZ = SpinCircle.transform.eulerAngles.z;

            if (ShowDebug)
            {
                DebugRotationSpeed = runData.RotationSpeed;
            }
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
            setGameState(GAME_STATE.SCORING_SLOT_CHIPS, balance);
            m_scoringIdx = 0;
            m_scoringTimer = ScoringTime;

            Logic.StartScoring(runData, balance);
        }

        public void StartRound(RunData runData, Balance balance)
        {
            m_totalScoreText.text = runData.TotalChips.ToString("N0");

            int bigRound = runData.Round / 3;
            int smallRound = runData.Round % 3;

            string title = "Round " + (bigRound + 1).ToString() + " - " + (smallRound + 1).ToString();
            m_bossDescriptionGO.SetActive(Logic.InBossRound(runData));
            if (Logic.InBossRound(runData))
            {
                int bossType = Logic.GetBossTypeForRound(runData);

                m_bossDescription.text = CommonVisual.GetBossDescription(runData, balance, "Boss: ");
            }
            CommonVisual.ShowTopBar(runData, m_topBarGUI, title);

            runData.SpinWheelAngle = 0.0f;

            m_scoringTimer = ScoringTime;
            m_scoringIdx = 0;
            setGameState(GAME_STATE.START_ROUND, balance);
            resetSpin(runData, balance);

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

            for (int i = 0; i < m_scoringSlots.Length; i++)
                m_scoringSlots[i].LockGO.SetActive(false);
        }

        private void startSpin(RunData runData, Balance balance)
        {
            resetSpin(runData, balance);

            setGameState(GAME_STATE.SPIN_UP, balance);
            SpinState = SPIN_STATE.SPIN_UP;

            Logic.StartSpin(runData, balance);
            if (Logic.InBossRound(runData))
            {
                bool slotsChanged;
                Logic.StartSpinBossEffect(runData, balance, out slotsChanged, CommonSlotsVisual.AffectedSlotsIdxs, ref CommonSlotsVisual.AffectedSlotsCount);
                if (slotsChanged)
                {
                    CommonSlotsVisual.ShowSpinWheel(runData, balance, m_scoringSlots, runData.SlotTypeInGame, m_showSlotEffects, runData.UseSlotsSpecial == 0);
                    m_slotAnimTimer = m_slotAnimTime;
                }

                int bossType = Logic.GetBossTypeForRound(runData);
                if (balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.JUMBLE_BALLS)
                {
                    Logic.JumbleBalls(runData, balance);
                    showBalls(runData, balance, 1, false);
                }
                if (balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.RANDOM_JOKE_DEBUFFED_PER_SPIN)
                {
                    CommonVisual.UpdateJokerDebuff(runData);
                }
            }

            m_spinsText.text = runData.CurrentSpin.ToString("N0") + " / " + runData.MaxSpinsThisRound.ToString("N0");

            RunDataIO.SaveRun(runData, balance);
        }

        public void DropBalls(Balance balance)
        {
            if (GameState == GAME_STATE.WAITING_FOR_INPUT)
            {
                // m_waitingForInputTime 0.9099129 angle 238.6025 - 6 balls
                Debug.Log("m_waitingForInputTime " + m_waitingForInputTime + " angle " + SpinCircle.transform.rotation.eulerAngles.z);
                m_droppedAngle = SpinCircle.transform.rotation.eulerAngles.z;

                GateGO.SetActive(false);
                setGameState(GAME_STATE.BALLS_DROPPED, balance);
                SpinState = SPIN_STATE.SPIN_BALLS;
            }
        }

        public void BallInSlot(RunData runData, Balance balance, SettingsData settingsData, int ballIdx, int slotIdx)
        {
            int slotChangedIdx;
            int slotChangeJokerIdx;
            int jokerMultIncIdx;
            int jokerMultInc;
            if (Logic.BallInSlot(runData, balance, ballIdx, slotIdx, out slotChangedIdx, out slotChangeJokerIdx, out jokerMultIncIdx, out jokerMultInc))
            {
                m_ballsRB[ballIdx].bodyType = RigidbodyType2D.Static;
                if (slotChangedIdx > -1)
                {
                    m_slotAnimTimer = m_slotAnimTime;
                    CommonSlotsVisual.AffectedSlotsIdxs[CommonSlotsVisual.AffectedSlotsCount++] = slotChangedIdx;
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

            }
        }

        private void ShowBallMoney(RunData runData, SettingsData settingsData, int ballIdx, int money)
        {
            Vector3 ballPos = BallsGO[ballIdx].transform.position;
            BallsMoneyGO.SetActive(true);
            BallsMoneyGO.transform.position = new Vector3(ballPos.x, ballPos.y, ballPos.z - 10.0f);
            m_ballsMoneyText.text = "$" + money.ToString("N0");

            CommonVisual.UpdateTopBarMoney(runData, m_topBarGUI);
            SoundManager.Instance.PlaySFXMoney(settingsData);
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

#if UNITY_EDITOR
        float m_droppedAngle;
        float m_increaseSize;
        void doSpinTest(RunData runData, Balance balance)
        {
            Span<int> slotTypeCount = new int[4];
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
            // else
            // {
            //     AutoDropAngle += m_increaseSize;
            //     if (AutoDropAngle > 152.0f)
            //     {
            //         AutoDropAngle = 148.0f;
            //         m_increaseSize /= 2.0f;
            //     }
            // }

            startSpin(runData, balance);
        }
#endif
    }
}