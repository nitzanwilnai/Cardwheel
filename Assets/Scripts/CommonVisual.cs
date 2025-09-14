using CommonTools;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace Cardwheel
{
    public struct TopBarGUI
    {
        public TextMeshProUGUI MoneyText;
        public Animation MoneyAnim;
        public TextMeshProUGUI TitleText;
        public Button SettingsButton;
    }

    public struct CardsBallsSpinWheelGUI
    {
        public Transform JokerParent;
        public Image[] Balls;
        public ScoringSlot[] ScoringSlots;
        public GameObject SortingPopup;
        public SpinCircle SpinCircle;
    }

    public static class CommonVisual
    {
        public static GameObject[] JokerPool;
        public static JokerGUI[] JokerGUIs;
        public static Image[] TopBarBalls;

        public static void InitJokers(Balance balance)
        {
            JokerPool = new GameObject[balance.MaxJokersInHand];
            JokerGUIs = new JokerGUI[balance.MaxJokersInHand];
            for (int i = 0; i < balance.MaxJokersInHand; i++)
            {
                GameObject go = AssetManager.Instance.LoadJokerPrefab();

                go.SetActive(false);

                GUIRef guiRef = go.GetComponent<GUIRef>();
                JokerGUIs[i].CardImage = guiRef.GetImage("Joker");
                JokerGUIs[i].Button = guiRef.GetButton("Joker");
                JokerGUIs[i].Animation = guiRef.GetAnimation("Joker");
                JokerGUIs[i].DebuffGO = guiRef.GetGameObject("Debuffed");
                JokerGUIs[i].RainbowGO = guiRef.GetGameObject("Rainbow");
                JokerGUIs[i].MetalGO = guiRef.GetGameObject("Metal");
                JokerGUIs[i].ShinyGO = guiRef.GetGameObject("Shiny");

                JokerGUIs[i].DebuffGO.SetActive(false);
                JokerGUIs[i].RainbowGO.SetActive(false);
                JokerGUIs[i].MetalGO.SetActive(false);
                JokerGUIs[i].ShinyGO.SetActive(false);

                JokerPool[i] = go;
            }
        }

        public static void ShowJokersInGame(RunData runData, Balance balance, Transform jokerParent)
        {
            ShowJokersCommon(runData, balance, jokerParent);

            for (int jkrIdx = 0; jkrIdx < runData.JokerCount; jkrIdx++)
            {
                int localJokerIdx = jkrIdx;
                JokerGUIs[jkrIdx].Button.onClick.AddListener(() => Game.Instance.ShowJokerInfoPopupInGame(localJokerIdx));
            }
        }

        public static void ShowJokers(RunData runData, Balance balance, Transform jokerParent)
        {
            ShowJokersCommon(runData, balance, jokerParent);

            for (int jkrIdx = 0; jkrIdx < runData.JokerCount; jkrIdx++)
            {
                int localJokerIdx = jkrIdx;
                JokerGUIs[jkrIdx].Button.onClick.AddListener(() => Game.Instance.ShowJokerInfoPopup(localJokerIdx));
            }
        }

        public static void ShowJokersCommon(RunData runData, Balance balance, Transform jokerParent)
        {
            for (int jkrIdx = 0; jkrIdx < JokerPool.Length; jkrIdx++)
            {
                JokerPool[jkrIdx].transform.SetParent(jokerParent);
                JokerPool[jkrIdx].transform.localPosition = Vector3.zero;
                JokerPool[jkrIdx].transform.localScale = Vector3.one;
                JokerPool[jkrIdx].SetActive(false);

                JokerGUIs[jkrIdx].DebuffGO.SetActive(false);
                JokerGUIs[jkrIdx].RainbowGO.SetActive(false);
                JokerGUIs[jkrIdx].MetalGO.SetActive(false);
                JokerGUIs[jkrIdx].ShinyGO.SetActive(false);
            }

            for (int jkrIdx = 0; jkrIdx < runData.JokerCount; jkrIdx++)
            {
                int jokerType = runData.JokerTypes[jkrIdx];
                JokerPool[jkrIdx].SetActive(true);
                JokerGUIs[jkrIdx].DebuffGO.SetActive(runData.UseJoker[jkrIdx] == 0);

                JokerGUIs[jkrIdx].CardImage.sprite = AssetManager.Instance.LoadJokerSprite(balance.JokerBalance.JokerSpritesNames[jokerType]);

                JokerGUIs[jkrIdx].Button.onClick.RemoveAllListeners();
            }
        }

        public static void UpdateJokerDebuff(RunData runData)
        {
            for (int jkrIdx = 0; jkrIdx < runData.JokerCount; jkrIdx++)
                JokerGUIs[jkrIdx].DebuffGO.SetActive(runData.UseJoker[jkrIdx] == 0);

        }

        public static void ShowJokerDescriptionInHand(RunData runData, Balance balance, GameObject go, int jokerIdx)
        {
            int jokerType = runData.JokerTypes[jokerIdx];
            showJokerDescriptionCommon(runData, balance, go, jokerType);
            if (balance.JokerBalance.ChipsIncreasePerSpin[jokerType] > 0)
            {
                int chipIncrease = jokerIdx > -1 ? runData.JokerChips[jokerIdx] : 0;
                go.GetComponent<GUIRef>().GetTextGUI("Current").text = "(Current +" + chipIncrease + ")";
            }

            if (balance.JokerBalance.MultIncreasePerUnusedSpin[jokerType] != 0 ||
                balance.JokerBalance.MultIncreasePerUsedSpin[jokerType] != 0)
            {
                float multiplierAdd = jokerIdx > -1 ? runData.JokerMultiplierAdd[jokerIdx] : 0.0f;
                go.GetComponent<GUIRef>().GetTextGUI("Current").text = "(Current +" + multiplierAdd.ToString("N0") + "x)";
            }

            if (balance.JokerBalance.SubtractChipsPerSpin[jokerType].x > 0.0f)
            {
                int chipIncrease = jokerIdx > -1 ? runData.JokerChips[jokerIdx] : 0;
                go.GetComponent<GUIRef>().GetTextGUI("Current").text = "(Current +" + chipIncrease + ")";
            }
            if (balance.JokerBalance.SubtractMultiplierAddPerRound[jokerType].x > 0.0f)
            {
                float multiplierAdd = jokerIdx > -1 ? runData.JokerMultiplierAdd[jokerIdx] : 0.0f;
                go.GetComponent<GUIRef>().GetTextGUI("Current").text = "(Current +" + multiplierAdd.ToString("N0") + "x)";
            }

            if (balance.JokerBalance.ChipsIncreasePerBall[jokerType] > 0)
            {
                int chipIncrease = jokerIdx > -1 ? runData.JokerChips[jokerIdx] : 0;
                go.GetComponent<GUIRef>().GetTextGUI("Current").text = "(Current +" + chipIncrease + ")";
            }

            if (balance.JokerBalance.MultIncreaseForSize[jokerType] > 0.0f)
            {
                float multiplierAdd = jokerIdx > -1 ? runData.JokerMultiplierAdd[jokerIdx] : 0.0f;
                go.GetComponent<GUIRef>().GetTextGUI("Current").text = "(Current +" + multiplierAdd.ToString("N0") + "x)";
            }
            if (balance.JokerBalance.PerNoJokerMultiplierAdd[jokerType] > 0)
            {
                int numNoJokers = balance.MaxJokersInHand - runData.JokerCount;
                int chips = (int)balance.JokerBalance.PerNoJokerMultiplierAdd[jokerType];
                go.GetComponent<GUIRef>().GetTextGUI("Current").text = "(Current +" + (numNoJokers * chips) + "x)";
            }
        }

        public static void ShowJokerDescriptionShop(RunData runData, Balance balance, GameObject go, int jokerType)
        {
            showJokerDescriptionCommon(runData, balance, go, jokerType);
            if (balance.JokerBalance.ChipsIncreasePerSpin[jokerType] > 0)
                go.GetComponent<GUIRef>().GetTextGUI("Current").text = "(Current 0)";
            if (balance.JokerBalance.MultIncreasePerUnusedSpin[jokerType] > 0.0f || balance.JokerBalance.MultIncreasePerUsedSpin[jokerType] < 0.0f)
                go.GetComponent<GUIRef>().GetTextGUI("Current").text = "(Current 0)";
            if (balance.JokerBalance.SubtractChipsPerSpin[jokerType].x > 0.0f)
                go.GetComponent<GUIRef>().GetTextGUI("Current").text = "(Current +" + balance.JokerBalance.SubtractChipsPerSpin[jokerType].x.ToString("N0") + ")";
            if (balance.JokerBalance.SubtractMultiplierAddPerRound[jokerType].x > 0.0f)
                go.GetComponent<GUIRef>().GetTextGUI("Current").text = "(Current +" + balance.JokerBalance.SubtractMultiplierAddPerRound[jokerType].x.ToString("N0") + "x)";
            if (balance.JokerBalance.ChipsIncreasePerBall[jokerType] > 0)
                go.GetComponent<GUIRef>().GetTextGUI("Current").text = "(Current 0)";
            if (balance.JokerBalance.MultIncreaseForSize[jokerType] > 0.0f)
                go.GetComponent<GUIRef>().GetTextGUI("Current").text = "(Current 0)";
            if (balance.JokerBalance.PerNoJokerMultiplierAdd[jokerType] > 0)
            {
                int numNoJokers = balance.MaxJokersInHand - (runData.JokerCount + 1);
                if (numNoJokers < 0)
                    numNoJokers = 0;
                int chips = (int)balance.JokerBalance.PerNoJokerMultiplierAdd[jokerType];
                go.GetComponent<GUIRef>().GetTextGUI("Current").text = "(Current +" + (numNoJokers * chips) + "x)";
            }

        }

        static void showJokerDescriptionCommon(RunData runData, Balance balance, GameObject go, int jokerType)
        {
            if (balance.JokerBalance.PerJokerMultiplierAdd[jokerType] > 0)
            {
                int chips = (int)balance.JokerBalance.PerJokerMultiplierAdd[jokerType];
                go.GetComponent<GUIRef>().GetTextGUI("Current").text = "(Current +" + ((runData.JokerCount + 1) * chips) + "x)";
            }
            if (balance.JokerBalance.MultiplierMultForSpecialBall[jokerType] > 0.0f)
            {
                int numSpecialBalls = 0;
                for (int ballIdx = 0; ballIdx < balance.MaxBalls; ballIdx++)
                    if (runData.BallTypes[ballIdx] > 0)
                        numSpecialBalls++;
                go.GetComponent<GUIRef>().GetTextGUI("Current").text = "(Current x" + (numSpecialBalls * balance.JokerBalance.MultiplierMultForSpecialBall[jokerType]) + ")";

            }
            if (balance.JokerBalance.MultiplierMultForNonSpecialBall[jokerType] > 0.0f)
            {
                int numRegularBalls = 0;
                for (int ballIdx = 0; ballIdx < balance.MaxBalls; ballIdx++)
                    if (runData.BallTypes[ballIdx] == 0)
                        numRegularBalls++;
                go.GetComponent<GUIRef>().GetTextGUI("Current").text = "(Current x" + (numRegularBalls * balance.JokerBalance.MultiplierMultForNonSpecialBall[jokerType]) + ")";
            }

            if (balance.JokerBalance.MultiplierMultEveryShopReroll[jokerType] > 0.0f)
                go.GetComponent<GUIRef>().GetTextGUI("Current").text = "(Current x" + (runData.ShopRerollTotal * balance.JokerBalance.MultiplierMultEveryShopReroll[jokerType]) + ")";

            if (balance.JokerBalance.MultiplierMultEveryCardPackReroll[jokerType] > 0.0f)
                go.GetComponent<GUIRef>().GetTextGUI("Current").text = "(Current x" + (runData.CardPackRerollTotal * balance.JokerBalance.MultiplierMultEveryCardPackReroll[jokerType]) + ")";

            if (balance.JokerBalance.ChipsPerDollar[jokerType] > 0)
                go.GetComponent<GUIRef>().GetTextGUI("Current").text = "(Current +" + (balance.JokerBalance.ChipsPerDollar[jokerType] * runData.Money).ToString("N0") + ")";

            if (balance.JokerBalance.ChipsAddForEveryNonSlotMod[jokerType] > 0.0f)
            {
                int chipsAdd = Logic.GetNumNonModedSlots(runData, balance) * balance.JokerBalance.ChipsAddForEveryNonSlotMod[jokerType];
                go.GetComponent<GUIRef>().GetTextGUI("Current").text = "(Current +" + chipsAdd.ToString("N0") + ")";
            }

            if (balance.JokerBalance.MultiplierAddForEverySlotMod[jokerType] > 0.0f)
            {
                float multiplierAdd = Logic.GetNumModedSlots(runData, balance) * balance.JokerBalance.MultiplierAddForEverySlotMod[jokerType];
                go.GetComponent<GUIRef>().GetTextGUI("Current").text = "(Current +" + multiplierAdd.ToString("N0") + "x)";
            }

            if (balance.JokerBalance.MultiplierAddForLeastPlayedColor[jokerType] > 0.0f)
            {
                SLOT_TYPE slotType = runData.LeastPlayedColorAtRoundStart;
                go.GetComponent<GUIRef>().GetTextGUI("Current").text = "<color=#" + balance.SlotColors[(int)slotType].ToHexString() + ">" + slotType.ToString() + "</color>";
            }
        }

        public static void HideJokers()
        {
            for (int jkrIdx = 0; jkrIdx < JokerPool.Length; jkrIdx++)
                JokerPool[jkrIdx].SetActive(false);
        }

        public static void InitBalls(Balance balance)
        {
            TopBarBalls = new Image[balance.MaxBalls];
        }

        public static void ShowJokersBallsAndSpinWheel(RunData runData, Balance balance, CardsBallsSpinWheelGUI cardsBallsSpinWheelGUI, SLOT_TYPE[] slotType)
        {
            cardsBallsSpinWheelGUI.SortingPopup.SetActive(false);
            ShowJokers(runData, balance, cardsBallsSpinWheelGUI.JokerParent);
            ShowBalls(runData, balance, cardsBallsSpinWheelGUI);
            CommonSlotsVisual.ShowSpinWheelUI(runData, balance, cardsBallsSpinWheelGUI.ScoringSlots, slotType);
        }

        public static void ShowBalls(RunData runData, Balance balance, CardsBallsSpinWheelGUI cardsBallsSpinWheelGUI)
        {
            for (int ballIdx = 0; ballIdx < balance.MaxBalls; ballIdx++)
            {
                int ballType = runData.BallTypes[ballIdx];
                cardsBallsSpinWheelGUI.Balls[ballIdx].sprite = AssetManager.Instance.LoadBallSprite(balance.BallBalance.BallSprite[ballType]);
            }
        }

        public static void InitTopBarGUI(GameObject go, ref TopBarGUI topBarGUI)
        {
            GUIRef guiRef = go.GetComponent<GUIRef>();
            topBarGUI.MoneyText = guiRef.GetTextGUI("Money");
            topBarGUI.MoneyAnim = guiRef.GetAnimation("Money");
            topBarGUI.TitleText = guiRef.GetTextGUI("Title");
            topBarGUI.SettingsButton = guiRef.GetButton("Settings");
            topBarGUI.SettingsButton.onClick.AddListener(Game.Instance.GoToSettings);
        }

        public static void ShowTopBarNoSettings(RunData runData, TopBarGUI topBarGUI, string title)
        {
            ShowTopBar(runData, topBarGUI, title);
            topBarGUI.SettingsButton.gameObject.SetActive(false);
        }

        public static void ShowTopBar(RunData runData, TopBarGUI topBarGUI, string title)
        {
            topBarGUI.MoneyText.text = "$" + runData.Money.ToString("N0");
            topBarGUI.TitleText.text = title;
        }

        public static void UpdateTopBarMoney(RunData runData, TopBarGUI topBarGUI)
        {
            topBarGUI.MoneyText.text = "$" + runData.Money.ToString("N0");
            topBarGUI.MoneyAnim.Play();
        }

        public static void InitCardsBallsSpinWheelGUI(RunData runData, Balance balance, GameObject go, ref CardsBallsSpinWheelGUI cardsBallsSpinWheelGUI)
        {
            GUIRef guiRef = go.GetComponentInParent<GUIRef>();
            cardsBallsSpinWheelGUI.JokerParent = guiRef.GetGameObject("Cards").transform;
            cardsBallsSpinWheelGUI.Balls = new Image[balance.MaxBalls];
            for (int i = 0; i < balance.MaxBalls; i++)
                cardsBallsSpinWheelGUI.Balls[i] = guiRef.GetImage("Ball" + (i + 1).ToString());
            guiRef.GetButton("BallScreen").onClick.AddListener(Game.Instance.GoToBallScreen);
            guiRef.GetButton("SpinWheel").onClick.AddListener(Game.Instance.GoToChipsInfo);

            SpinWheelRef spinWheelRef = guiRef.GetGameObject("SpinWheel").GetComponent<SpinWheelRef>();
            cardsBallsSpinWheelGUI.SortingPopup = spinWheelRef.SortingPopup;
            cardsBallsSpinWheelGUI.SpinCircle = spinWheelRef.SpinCircle;
            cardsBallsSpinWheelGUI.SortingPopup.SetActive(false);
            cardsBallsSpinWheelGUI.ScoringSlots = new ScoringSlot[spinWheelRef.SlotGO.Length];
            for (int i = 0; i < spinWheelRef.SlotGO.Length; i++)
                cardsBallsSpinWheelGUI.ScoringSlots[i] = spinWheelRef.SlotGO[i].GetComponentInChildren<ScoringSlot>();

        }

        public static string GetMultiplierString(float value)
        {
            if (value - Mathf.Round(value) > 0.0f)
                return value.ToString("N2");
            else
                return value.ToString("N0");
        }

        public static string ColorText(Balance balance, string title)
        {
            string colorTitle = "";
            for (int i = 0; i < title.Length; i++)
            {
                colorTitle += "<color=#" + balance.SlotColors[i % balance.SlotColors.Length].ToHexString() + ">";
                colorTitle += title[i];
                colorTitle += "</color>";
            }

            return colorTitle;
        }

        public static string GetRoundString(int bigRound, int smallRound)
        {
            return (smallRound < 2 ? "Round " : "Boss ") + (bigRound + 1).ToString() + " - " + (smallRound + 1).ToString();
        }

        public static string GetBossDescription(RunData runData, Balance balance, string title)
        {
            int bossType = Logic.GetBossTypeForRound(runData);
            string bossText = title + balance.BossBalance.Description[bossType];

            if (balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.ONLY_PLAY_MOST_USED_COLOR)
                bossText += " (" + Logic.GetMostPlayedSlotType(runData).ToString() + ")";

            if (balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.DEBUFF_MOST_USED_COLOR)
                bossText += " (" + Logic.GetMostPlayedSlotType(runData).ToString() + ")";

            if (balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.MOST_PLAYED_BASE_CHIPS_TO_FIVE)
                bossText += " (" + Logic.GetMostPlayedSlotType(runData).ToString() + ")";

            return bossText;
        }

        public static void ShowRoundInGameInfo(RunData runData, Balance balance, int bigRound, int i, TextMeshProUGUI description)
        {
            if (i == 2)
            {
                description.text = GetBossDescription(runData, balance, "WARNING\n");
            }
            else
            {
                int skipIdx = ((bigRound * 3) + i) % balance.SkipBalance.NumSkips;
                int skipType = runData.SkipType[skipIdx];

                string extra = getSkipDescriptionCurrent(runData, balance, skipType);

                description.text = balance.SkipBalance.SkipDescription[skipType] + extra;
            }
        }

        public static void ShowRoundShopInfo(RunData runData, Balance balance, int bigRound, int i, TextMeshProUGUI description)
        {
            if (i == 2)
            {
                description.text = GetBossDescription(runData, balance, "Boss: ");
            }
            else
            {
                int skipIdx = ((bigRound * 3) + i) % balance.SkipBalance.NumSkips;
                int skipType = runData.SkipType[skipIdx];

                string extra = getSkipDescriptionCurrent(runData, balance, skipType);

                description.text = "Skip: " + balance.SkipBalance.SkipDescription[skipType] + extra;
            }
        }

        private static string getSkipDescriptionCurrent(RunData runData, Balance balance, int skipType)
        {
            string extra = "";
            if (balance.SkipBalance.DoubleMoney[skipType])
                extra = " (Current $" + Logic.GetDoubleMoneyLimit20(runData).ToString("N0") + ")";
            if (balance.SkipBalance.Change2SlotsToPlayedColor[skipType])
                extra = " (" + Logic.GetMostPlayedSlotType(runData).ToString() + ")";
            if (balance.SkipBalance.MoneyForSpinsUsed[skipType] > 0)
                extra = "\n(Current $" + (runData.SpinsUsed * balance.SkipBalance.MoneyForSpinsUsed[skipType]).ToString("N0") + ")";
            if (balance.SkipBalance.MoneyForSpinsUnused[skipType] > 0)
                extra = "\n(Current $" + (runData.SpinsUnused * balance.SkipBalance.MoneyForSpinsUnused[skipType]).ToString("N0") + ")";
            return extra;
        }

        public static void AnimateClose(ref float closeTimer, float closeTime, Animation animation, string animationString)
        {
            closeTimer = closeTime;
            animation.Play(animationString);
        }

        public static bool AnimateCloseTick(ref float m_closeTimer, float dt)
        {
            if (m_closeTimer > 0.0f)
            {
                m_closeTimer -= dt;
                if (m_closeTimer < 0.0f)
                    return true;
            }
            return false;
        }

        public static string AddOrdinal(int num)
        {
            if (num <= 0) return num.ToString();

            switch (num % 100)
            {
                case 11:
                case 12:
                case 13:
                    return num + "th";
            }

            switch (num % 10)
            {
                case 1:
                    return num + "st";
                case 2:
                    return num + "nd";
                case 3:
                    return num + "rd";
                default:
                    return num + "th";
            }
        }
    }
}