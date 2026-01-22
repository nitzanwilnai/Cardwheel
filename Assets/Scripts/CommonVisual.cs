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

namespace Cardwheel
{
    public struct TopBarGUI
    {
        public TextMeshProUGUI MoneyText;
        public Animation MoneyAnim;
        public TextMeshProUGUI TitleText;
        public GUIButtonData SettingsButtonData;
    }

    public struct CardsBallsSpinWheelGUI
    {
        public Transform JokerParent;
        public Image[] Balls;
        public ScoringSlot[] ScoringSlots;
        public GameObject SortingPopup;
        public SpinCircle SpinCircle;
        public GUIButtonData BallsButtonData;
        public GUIButtonData SpinwheelButtonData;
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
                JokerGUIs[i].SelectedGO = guiRef.GetGameObject("Selected");

                JokerGUIs[i].JokerChips = guiRef.GetGameObject("JokerChips");
                JokerGUIs[i].JokerMult = guiRef.GetGameObject("JokerMult");
                JokerGUIs[i].JokerColor = guiRef.GetGameObject("JokerColor");
                JokerGUIs[i].JokerMoney = guiRef.GetGameObject("JokerMoney");
                JokerGUIs[i].JokerChipsText = guiRef.GetTextGUI("JokerChips");
                JokerGUIs[i].JokerMultText = guiRef.GetTextGUI("JokerMult");
                JokerGUIs[i].JokerColorText = guiRef.GetTextGUI("JokerColor");
                JokerGUIs[i].JokerMoneyText = guiRef.GetTextGUI("JokerMoney");

                JokerGUIs[i].DebuffGO.SetActive(false);
                JokerGUIs[i].RainbowGO.SetActive(false);
                JokerGUIs[i].MetalGO.SetActive(false);
                JokerGUIs[i].ShinyGO.SetActive(false);
                JokerGUIs[i].SelectedGO.SetActive(false);

                JokerGUIs[i].JokerChips.SetActive(false);
                JokerGUIs[i].JokerMult.SetActive(false);
                JokerGUIs[i].JokerColor.SetActive(false);

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
                JokerGUIs[jkrIdx].SelectedGO.SetActive(false);

                JokerGUIs[jkrIdx].JokerChips.SetActive(false);
                JokerGUIs[jkrIdx].JokerMult.SetActive(false);
                JokerGUIs[jkrIdx].JokerColor.SetActive(false);
                JokerGUIs[jkrIdx].JokerMoney.SetActive(false);
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

        public static void UnselectAllJokers()
        {
            for (int jkrIdx = 0; jkrIdx < JokerPool.Length; jkrIdx++)
                JokerGUIs[jkrIdx].SelectedGO.SetActive(false);
        }

        public static void SelectJoker(int jkrIdx)
        {
            JokerGUIs[jkrIdx].SelectedGO.SetActive(CommonButtonVisual.ShowSelected());
        }

        public static void UpdateJokerDebuff(RunData runData)
        {
            for (int jkrIdx = 0; jkrIdx < runData.JokerCount; jkrIdx++)
                JokerGUIs[jkrIdx].DebuffGO.SetActive(runData.UseJoker[jkrIdx] == 0);

        }

        // NOTE - all code here should be in ShowJokerDescriptionInShop too!
        static void ShowJokerDescriptionWithIndex(RunData runData, Balance balance, GameObject go, int jokerType, int jokerIdx)
        {
            if (balance.JokerBalance.ChipsIncreasePerSpin[jokerType] > 0)
            {
                double chipIncrease = jokerIdx > -1 ? runData.JokerChips[jokerIdx] : 0;
                go.GetComponent<GUIRef>().GetTextGUI("Current").text = "(Current " + chipIncrease.ToString("N0") + ")";
            }

            if (balance.JokerBalance.MultIncreasePerUnusedSpin[jokerType] != 0 ||
                balance.JokerBalance.MultIncreasePerUsedSpin[jokerType] != 0)
            {
                double multiplierAdd = jokerIdx > -1 ? runData.JokerMultiplierAdd[jokerIdx] : 0.0f;
                go.GetComponent<GUIRef>().GetTextGUI("Current").text = "(Current " + multiplierAdd.ToString("N0") + "x)";
            }

            if (balance.JokerBalance.SubtractChipsPerSpin[jokerType].x > 0.0f)
            {
                double chipIncrease = jokerIdx > -1 ? runData.JokerChips[jokerIdx] : 0;
                go.GetComponent<GUIRef>().GetTextGUI("Current").text = "(Current " + chipIncrease.ToString("N0") + ")";
            }
            if (balance.JokerBalance.SubtractMultiplierAddPerRound[jokerType].x > 0.0f)
            {
                double multiplierAdd = jokerIdx > -1 ? runData.JokerMultiplierAdd[jokerIdx] : balance.JokerBalance.SubtractMultiplierAddPerRound[jokerType].x;
                go.GetComponent<GUIRef>().GetTextGUI("Current").text = "(Current " + multiplierAdd.ToString("N0") + "x)";
            }

            if (balance.JokerBalance.ChipsIncreasePerBall[jokerType] > 0)
            {
                double chipIncrease = jokerIdx > -1 ? runData.JokerChips[jokerIdx] : 0;
                go.GetComponent<GUIRef>().GetTextGUI("Current").text = "(Current " + chipIncrease.ToString("N0") + ")";
            }

            if (balance.JokerBalance.MultAddIncreasePerBall[jokerType] > 0)
            {
                double multiplierAdd = jokerIdx > -1 ? runData.JokerMultiplierAdd[jokerIdx] : 0.0f;
                go.GetComponent<GUIRef>().GetTextGUI("Current").text = "(Current " + multiplierAdd.ToString("N1") + "x)";
            }

            if (balance.JokerBalance.MultMultIncreasePerBall[jokerType] > 0)
            {
                double multiplierMult = jokerIdx > -1 ? runData.JokerMultiplierMult[jokerIdx] : 0.0f;
                go.GetComponent<GUIRef>().GetTextGUI("Current").text = "(Current x" + multiplierMult.ToString("N2") + ")";
            }

            if (balance.JokerBalance.MultIncreaseForSize[jokerType] > 0.0f)
            {
                double multiplierAdd = jokerIdx > -1 ? runData.JokerMultiplierAdd[jokerIdx] : 0.0f;
                go.GetComponent<GUIRef>().GetTextGUI("Current").text = "(Current " + multiplierAdd.ToString("N0") + "x)";
            }
            if (balance.JokerBalance.BallIncMultRemoveSlotMod[jokerType] > 0)
            {
                double multIncrease = jokerIdx > -1 ? runData.JokerMultiplierAdd[jokerIdx] : 0;
                go.GetComponent<GUIRef>().GetTextGUI("Current").text = "(Current " + multIncrease + ")";
            }
            if (balance.JokerBalance.IncreaseSellValueEveryRound[jokerType] > 0)
            {
                int sellValue = jokerIdx > -1 ? runData.JokerSellValues[jokerIdx] : balance.JokerBalance.Cost[jokerType] / 2;
                go.GetComponent<GUIRef>().GetTextGUI("Current").text = "(Current ◇" + sellValue + ")";
            }
            if (balance.JokerBalance.PerNoJokerMultiplierAdd[jokerType] > 0)
            {
                int numNoJokers = balance.MaxJokersInHand - runData.JokerCount;
                if (jokerIdx == -1)
                    numNoJokers--; // subtract us if we are added
                if (numNoJokers < 0)
                    numNoJokers = 0;
                int chips = (int)balance.JokerBalance.PerNoJokerMultiplierAdd[jokerType];
                go.GetComponent<GUIRef>().GetTextGUI("Current").text = "(Current " + (numNoJokers * chips) + "x)";
            }

            if (balance.JokerBalance.RoundSkippedChipsAdd[jokerType] > 0)
            {
                int value = jokerIdx > -1 ? runData.JokerSkipCount[jokerIdx] * balance.JokerBalance.RoundSkippedChipsAdd[jokerType] : 0;
                go.GetComponent<GUIRef>().GetTextGUI("Current").text = "(Current " + value + ")";
            }
            if (balance.JokerBalance.RoundSkippedMultiplierAdd[jokerType] > 0)
            {
                float value = jokerIdx > -1 ? runData.JokerSkipCount[jokerIdx] * balance.JokerBalance.RoundSkippedMultiplierAdd[jokerType] : 0;
                go.GetComponent<GUIRef>().GetTextGUI("Current").text = "(Current " + value + "x)";
            }
            if (balance.JokerBalance.RoundSkippedMultiplierMult[jokerType] > 0)
            {
                float value = jokerIdx > -1 ? runData.JokerSkipCount[jokerIdx] * balance.JokerBalance.RoundSkippedMultiplierMult[jokerType] : 0;
                go.GetComponent<GUIRef>().GetTextGUI("Current").text = "(Current x" + value + ")";
            }

            if (balance.JokerBalance.ChipsIncreasePerXSpins[jokerType].x > 0)
            {
                int chipIncrease = jokerIdx > -1 ? (int)Logic.GetValueForTriggerSpins(runData, balance.JokerBalance.ChipsIncreasePerXSpins[jokerType], jokerIdx) : 0;
                go.GetComponent<GUIRef>().GetTextGUI("Current").text = "(Current " + chipIncrease + ")";
            }

            if (balance.JokerBalance.MultAddIncreasePerXSpins[jokerType].x > 0)
            {
                float multiplierAdd = jokerIdx > -1 ? Logic.GetValueForTriggerSpins(runData, balance.JokerBalance.MultAddIncreasePerXSpins[jokerType], jokerIdx) : 0.0f;
                go.GetComponent<GUIRef>().GetTextGUI("Current").text = "(Current " + multiplierAdd.ToString("N1") + "x)";
            }

            if (balance.JokerBalance.MultMultIncreasePerXSpins[jokerType].x > 0)
            {
                float multiplierMult = jokerIdx > -1 ? Logic.GetValueForTriggerSpins(runData, balance.JokerBalance.MultMultIncreasePerXSpins[jokerType], jokerIdx) : 0.0f;
                go.GetComponent<GUIRef>().GetTextGUI("Current").text = "(Current x" + multiplierMult.ToString("N2") + ")";
            }
        }

        public static void ShowJokerDescriptionCommon(RunData runData, Balance balance, GameObject go, int jokerType, int jokerIdx)
        {
            ShowJokerDescriptionWithIndex(runData, balance, go, jokerType, jokerIdx);

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
                go.GetComponent<GUIRef>().GetTextGUI("Current").text = "(Current " + (balance.JokerBalance.ChipsPerDollar[jokerType] * runData.Money).ToString("N0") + ")";

            if (balance.JokerBalance.ChipsAddForEveryNonSlotMod[jokerType] > 0.0f)
            {
                int chipsAdd = Logic.GetNumNonModedSlots(runData, balance) * balance.JokerBalance.ChipsAddForEveryNonSlotMod[jokerType];
                go.GetComponent<GUIRef>().GetTextGUI("Current").text = "(Current " + chipsAdd.ToString("N0") + ")";
            }

            if (balance.JokerBalance.MultiplierAddForEverySlotMod[jokerType] > 0.0f)
            {
                float multiplierAdd = Logic.GetNumModedSlots(runData, balance) * balance.JokerBalance.MultiplierAddForEverySlotMod[jokerType];
                go.GetComponent<GUIRef>().GetTextGUI("Current").text = "(Current " + multiplierAdd.ToString("N0") + "x)";
            }

            if (balance.JokerBalance.MultiplierAddForLeastPlayedColor[jokerType] > 0.0f)
            {
                SLOT_TYPE slotType = runData.LeastPlayedColorAtRoundStart;
                go.GetComponent<GUIRef>().GetTextGUI("Current").text = "<color=#" + ColorUtility.ToHtmlStringRGBA(balance.SlotColors[(int)slotType]) + ">" + slotType.ToString() + "</color>";
            }

            if (balance.JokerBalance.ChipsAddForCardPackAbandon[jokerType] > 0)
            {
                int value = runData.CardPackAbandonTotal * balance.JokerBalance.ChipsAddForCardPackAbandon[jokerType];
                go.GetComponent<GUIRef>().GetTextGUI("Current").text = "(Current " + value + ")";
            }
            if (balance.JokerBalance.MultiplierMultForCardPackAbandon[jokerType] > 0)
            {
                float value = runData.CardPackAbandonTotal * balance.JokerBalance.MultiplierMultForCardPackAbandon[jokerType];
                go.GetComponent<GUIRef>().GetTextGUI("Current").text = "(Current x" + value + ")";
            }

            if (balance.JokerBalance.AddAllSellValueToMult[jokerType])
            {
                int totalSellValue = 0;
                for (int jkrIdx = 0; jkrIdx < runData.JokerCount; jkrIdx++)
                {
                    totalSellValue += runData.JokerSellValues[jkrIdx];
                    go.GetComponent<GUIRef>().GetTextGUI("Current").text = "(Current +x" + totalSellValue.ToString("N0") + ")";
                }
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
                // Debug.Log("CommonVisual showBalls ballType " + ballType + " sprite " + cardsBallsSpinWheelGUI.Balls[ballIdx].sprite.name);
            }
        }

        public static void InitTopBarGUI(GameObject go, ref TopBarGUI topBarGUI)
        {
            GUIRef guiRef = go.GetComponent<GUIRef>();
            topBarGUI.MoneyText = guiRef.GetTextGUI("Money");
            topBarGUI.MoneyAnim = guiRef.GetAnimation("Money");
            topBarGUI.TitleText = guiRef.GetTextGUI("Title");

            GUIButtonRef guiButtonRef = go.GetComponent<GUIButtonRef>();
            topBarGUI.SettingsButtonData = guiButtonRef.GetButtonData("Settings");
            topBarGUI.SettingsButtonData.Button.onClick.AddListener(Game.Instance.GoToSettings);

            CommonButtonVisual.AddSelectedBorder(topBarGUI.SettingsButtonData);
        }

        public static void ShowTopBarNoSettings(RunData runData, TopBarGUI topBarGUI, string title)
        {
            ShowTopBar(runData, topBarGUI, title);
            topBarGUI.SettingsButtonData.Button.gameObject.SetActive(false);
        }

        public static void ShowTopBar(RunData runData, TopBarGUI topBarGUI, string title)
        {
            ShowMoney(runData, topBarGUI);
            topBarGUI.TitleText.text = title;
        }

        public static void UpdateTopBarMoney(RunData runData, TopBarGUI topBarGUI)
        {
            ShowMoney(runData, topBarGUI);
            topBarGUI.MoneyAnim.Play();
        }

        public static void ShowMoney(RunData runData, TopBarGUI topBarGUI)
        {
            topBarGUI.MoneyText.text = "◇" + runData.Money.ToString("N0");
        }

        public static void InitCardsBallsSpinWheelGUI(Balance balance, GameObject go, ref CardsBallsSpinWheelGUI cardsBallsSpinWheelGUI)
        {
            GUIRef guiRef = go.GetComponentInParent<GUIRef>();
            cardsBallsSpinWheelGUI.JokerParent = guiRef.GetGameObject("Cards").transform;
            cardsBallsSpinWheelGUI.Balls = new Image[balance.MaxBalls];
            for (int i = 0; i < balance.MaxBalls; i++)
                cardsBallsSpinWheelGUI.Balls[i] = guiRef.GetImage("Ball" + (i + 1).ToString());

            GUIButtonRef guiButtonRef = go.GetComponentInParent<GUIButtonRef>();
            cardsBallsSpinWheelGUI.BallsButtonData = guiButtonRef.GetButtonData("BallScreen");
            cardsBallsSpinWheelGUI.SpinwheelButtonData = guiButtonRef.GetButtonData("SpinWheel");
            cardsBallsSpinWheelGUI.BallsButtonData.Button.onClick.AddListener(Game.Instance.GoToBallScreen);
            cardsBallsSpinWheelGUI.SpinwheelButtonData.Button.onClick.AddListener(Game.Instance.GoToChipsInfo);
            CommonButtonVisual.AddSelectedBorder(cardsBallsSpinWheelGUI.BallsButtonData);

            SpinWheelRef spinWheelRef = guiRef.GetGameObject("SpinWheel").GetComponent<SpinWheelRef>();
            cardsBallsSpinWheelGUI.SortingPopup = spinWheelRef.SortingPopup;
            cardsBallsSpinWheelGUI.SpinCircle = spinWheelRef.SpinCircle;
            cardsBallsSpinWheelGUI.SortingPopup.SetActive(false);
            cardsBallsSpinWheelGUI.ScoringSlots = new ScoringSlot[spinWheelRef.SlotGO.Length];
            for (int i = 0; i < spinWheelRef.SlotGO.Length; i++)
                cardsBallsSpinWheelGUI.ScoringSlots[i] = spinWheelRef.SlotGO[i].GetComponentInChildren<ScoringSlot>();

        }

        public static string GetMultiplierString(double value)
        {
            if (value - System.Math.Floor(value) > 0.0f)
                return value.ToString("N2");
            else
                return value.ToString("N1");
        }

        public static string ColorText(Balance balance, string title)
        {
            string colorTitle = "";
            for (int i = 0; i < title.Length; i++)
            {
                colorTitle += "<color=#" + ColorUtility.ToHtmlStringRGBA(balance.SlotColors[i % balance.SlotColors.Length]) + ">";
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
            int bossType = Logic.GetBossTypeForRound(runData, balance);
            string bossText = title + balance.BossBalance.Description[bossType];

            if (balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.ONLY_PLAY_MOST_USED_COLOR)
                bossText += " (" + Logic.GetMostPlayedSlotType(runData).ToString() + ")";

            if (balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.DEBUFF_MOST_USED_COLOR)
                bossText += " (" + Logic.GetMostPlayedSlotType(runData).ToString() + ")";

            if (balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.LOSE_MONEY_EVERY_BALL_MOST_COMMON_COLOR)
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
                int skipType = Logic.GetSkipTypeForRound(runData, balance, bigRound * 3 + i);

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
                int skipType = Logic.GetSkipTypeForRound(runData, balance, bigRound * 3 + i);

                string extra = getSkipDescriptionCurrent(runData, balance, skipType);

                description.text = "Skip: " + balance.SkipBalance.SkipDescription[skipType] + extra;
            }
        }

        private static string getSkipDescriptionCurrent(RunData runData, Balance balance, int skipType)
        {
            string extra = "";
            if (balance.SkipBalance.DoubleMoney[skipType])
                extra = " (Current ◇" + Logic.GetDoubleMoneyLimit20(runData).ToString("N0") + ")";
            if (balance.SkipBalance.Change2SlotsToPlayedColor[skipType])
                extra = " (" + Logic.GetMostPlayedSlotType(runData).ToString() + ")";
            if (balance.SkipBalance.MoneyForSpinsUsed[skipType] > 0)
                extra = "\n(Current ◇" + (runData.SpinsUsed * balance.SkipBalance.MoneyForSpinsUsed[skipType]).ToString("N0") + ")";
            if (balance.SkipBalance.MoneyForSpinsUnused[skipType] > 0)
                extra = "\n(Current ◇" + (runData.SpinsUnused * balance.SkipBalance.MoneyForSpinsUnused[skipType]).ToString("N0") + ")";
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

        public static void ChangeCanvasScalerMatching(GameObject UI)
        {
            float ratio = (float)Screen.width / (float)Screen.height;

            CanvasScaler canvasScaler = UI.GetComponent<CanvasScaler>();

            if (Screen.width < Screen.height)
            {
                if (ratio > 9.0f / 16.0f)
                    canvasScaler.matchWidthOrHeight = 1.0f;
                else
                    canvasScaler.matchWidthOrHeight = 0.0f;
            }
            else
            {
                if (ratio < 16.0f / 9.0f && canvasScaler.referenceResolution.x > canvasScaler.referenceResolution.y)
                    canvasScaler.matchWidthOrHeight = 0.0f;
                else
                    canvasScaler.matchWidthOrHeight = 1.0f;
            }
        }

        public static void ShowUpdatedCards(
            RunData runData,
            Balance balance,
            string[] descriptionNames,
            ref float packAnimationTimer,
            CardPackCardGUI[][] cardPackCardGUIs,
            GameObject[] descriptionGOs,
            GUIButtonData rerollButtonData,
            TextMeshProUGUI rerollCostText)
        {
            packAnimationTimer = 0.0f;

            CardPackCommonVisual.ShowCards(runData, balance, cardPackCardGUIs, descriptionGOs, descriptionNames);

            CardPackCommonVisual.ShowRerollButton(runData, balance, rerollButtonData.Button, rerollCostText);
        }

        public static string FormatScientific(double value)
        {
            if (value < 1000000000)
                return value.ToString("N0");
            else
            {
                int exponent = (int)Math.Floor(Math.Log10(value));
                double mantissa = value / Math.Pow(10, exponent);

                // Keep mantissa between 1.00–9.99
                return mantissa.ToString("0.00") + "e" + exponent;
            }
        }

    }

}