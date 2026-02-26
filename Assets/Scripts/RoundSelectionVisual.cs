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
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Cardwheel
{
    public class RoundGUIInfo
    {
        public TextMeshProUGUI Title;
        public TextMeshProUGUI Goal;
        public TextMeshProUGUI Reward;
        public GameObject Cover;
        public TextMeshProUGUI Description;
        public GUIButtonData PlayButtonData;
        public GameObject Shine;
    }

    public class RegularRoundGUIInfo
    {
        public GUIButtonData SkipButtonData;
    }

    public class BossRoundGUIInfo
    {
        // boss only
        public TextMeshProUGUI RerollButtonText;
        public GUIButtonData RerollButtonData;
        public GameObject Shine;
    }

    public class RoundSelectionVisual : MonoBehaviour
    {
        public enum MENU_BUTTONS
        {
            PLAY,
            SKIP,
            REROLL,
            SETTINGS = 10,
            WHEEL = 11,
            BALLS = 12,
            JOKER_1 = 20,
            JOKER_2 = 21,
            JOKER_3 = 22,
            JOKER_4 = 23,
            JOKER_5 = 24,
        };

        MENU_BUTTONS m_selectedButton = MENU_BUTTONS.PLAY;

        public AnimationCurve SlotScaleCurve;

        GameObject m_UI;

        TopBarGUI m_topBarGUI;

        RoundGUIInfo[] m_roundGUIInfos;
        BossRoundGUIInfo m_bossRoundGUIInfo;
        RegularRoundGUIInfo[] m_regularRoundGUIInfo;
        VerticalLayoutGroup m_verticalLayoutGroup;
        HorizontalLayoutGroup m_horiontalLayoutGroup;

        CardsBallsSpinWheelGUI m_cardsBallsSpinWheelGUI;

        float m_slotAnimTimer = 0.0f;
        float m_slotAnimTime = 1.5f;

        RunData runData;
        Balance balance;

        bool m_skipHappened;
        int m_abandonCount;

        int m_sortSlotsCount;

        public void Init(RunData runData, Balance balance, Camera camera)
        {
            this.runData = runData;
            this.balance = balance;

            m_UI = AssetManager.Instance.LoadRoundSelectionUI();
            CommonVisual.ChangeCanvasScalerMatching(m_UI);

            m_UI.GetComponent<Canvas>().worldCamera = camera;

            m_roundGUIInfos = new RoundGUIInfo[3];
            m_regularRoundGUIInfo = new RegularRoundGUIInfo[2];
            for (int i = 0; i < 2; i++)
                m_regularRoundGUIInfo[i] = new RegularRoundGUIInfo();
            m_bossRoundGUIInfo = new BossRoundGUIInfo();

            GUIRef guiRef = m_UI.GetComponent<GUIRef>();
            GameObject RoundSelection = guiRef.GetGameObject("RoundSelection");

            m_verticalLayoutGroup = RoundSelection.GetComponent<VerticalLayoutGroup>();
            m_horiontalLayoutGroup = RoundSelection.GetComponent<HorizontalLayoutGroup>();

            for (int i = 0; i < 3; i++)
            {
                m_roundGUIInfos[i] = new RoundGUIInfo();
                if (i < 2)
                    FillRoundGUIInfoNoBoss(
                        guiRef.GetGameObject("Round" + (i + 1)),
                        m_roundGUIInfos[i],
                        m_regularRoundGUIInfo[i]
                    );
                else
                    FillRoundGUIInfoBoss(
                        guiRef.GetGameObject("Round" + (i + 1)),
                        m_roundGUIInfos[i],
                        m_bossRoundGUIInfo
                    );
            }

            CommonVisual.InitTopBarGUI(guiRef.GetGameObject("TopBar"), ref m_topBarGUI);
            CommonVisual.InitCardsBallsSpinWheelGUI(
                balance,
                guiRef.GetGameObject("CardsAndBalls"),
                ref m_cardsBallsSpinWheelGUI
            );

            Hide();
        }

        void fillRoundGUIInfoCommon(GameObject go, RoundGUIInfo roundGUIInfo)
        {
            GUIRef guiRef = go.GetComponent<GUIRef>();
            roundGUIInfo.Title = guiRef.GetTextGUI("Title");
            roundGUIInfo.Goal = guiRef.GetTextGUI("Goal");
            roundGUIInfo.Reward = guiRef.GetTextGUI("Reward");
            roundGUIInfo.Cover = guiRef.GetGameObject("Cover");

            GUIButtonRef gUIButtonRef = go.GetComponent<GUIButtonRef>();
            roundGUIInfo.PlayButtonData = gUIButtonRef.GetButtonData("Play");
            roundGUIInfo.PlayButtonData.Button.onClick.AddListener(Game.Instance.StartRound);
            CommonButtonVisual.AddSelectedBorder(roundGUIInfo.PlayButtonData);

            roundGUIInfo.Shine = guiRef.GetGameObject("Shine");

            roundGUIInfo.Description = guiRef.GetTextGUI("Description");
        }

        void FillRoundGUIInfoNoBoss(
            GameObject go,
            RoundGUIInfo roundGUIInfo,
            RegularRoundGUIInfo regularRoundGUIInfo
        )
        {
            fillRoundGUIInfoCommon(go, roundGUIInfo);

            GUIButtonRef guiButtonRef = go.GetComponent<GUIButtonRef>();

            regularRoundGUIInfo.SkipButtonData = guiButtonRef.GetButtonData("Skip");
            regularRoundGUIInfo.SkipButtonData.Button.onClick.AddListener(skip);
            CommonButtonVisual.AddSelectedBorder(regularRoundGUIInfo.SkipButtonData);
        }

        void FillRoundGUIInfoBoss(
            GameObject go,
            RoundGUIInfo roundGUIInfo,
            BossRoundGUIInfo bossRoundGUIInfo
        )
        {
            fillRoundGUIInfoCommon(go, roundGUIInfo);

            GUIRef guiRef = go.GetComponent<GUIRef>();
            bossRoundGUIInfo.RerollButtonText = guiRef.GetTextGUI("Reroll");

            GUIButtonRef guiButtonRef = go.GetComponent<GUIButtonRef>();
            bossRoundGUIInfo.RerollButtonData = guiButtonRef.GetButtonData("Reroll");
            bossRoundGUIInfo.RerollButtonData.Button.onClick.AddListener(tryUseBossReroll);
            CommonButtonVisual.AddSelectedBorder(bossRoundGUIInfo.RerollButtonData);
        }

        public void Show()
        {
            Logic.SetDataForNextRound(
                runData,
                balance,
                CommonSlotsVisual.ChangedSlotsIdxs,
                ref CommonSlotsVisual.ChangedSlotsCount
            );

            int bigRound = runData.Round / 3;
            int smallRound = runData.Round % 3;
            for (int i = 0; i < 3; i++)
            {
                m_roundGUIInfos[i].Title.text = CommonVisual.GetRoundString(bigRound, i);
                // m_roundGUIInfos[i].Description.gameObject.SetActive(smallRound == i && i < 2);
                CommonVisual.ShowRoundInGameInfo(
                    runData,
                    balance,
                    bigRound,
                    i,
                    m_roundGUIInfos[i].Description
                );

                string goalText =
                    i < smallRound
                        ? "Complete"
                        : Logic.GetRoundGoal(runData, balance, bigRound, i).ToString("N0");
                m_roundGUIInfos[i].Goal.text = goalText;
                m_roundGUIInfos[i].Reward.text = "◇" + balance.RoundReward[i].ToString("N0");
                m_roundGUIInfos[i].Cover.SetActive(smallRound != i);
                m_roundGUIInfos[i].Shine.SetActive(smallRound == i);
                CommonButtonVisual.UpdateButtonIcons(
                    m_roundGUIInfos[i].PlayButtonData,
                    Game.Instance.GetGamepadType()
                );

                if (i < 2)
                    CommonButtonVisual.UpdateButtonIcons(
                        m_regularRoundGUIInfo[i].SkipButtonData,
                        Game.Instance.GetGamepadType()
                    );
            }

            m_bossRoundGUIInfo.RerollButtonText.text = "Reroll\n(" + runData.BossRerolls + " left)";
            m_bossRoundGUIInfo.RerollButtonData.Button.image.color =
                runData.BossRerolls > 0 ? balance.RerollColorEnabled : balance.ButtonColorDisabled;
            CommonButtonVisual.UpdateButtonIcons(
                m_bossRoundGUIInfo.RerollButtonData,
                Game.Instance.GetGamepadType()
            );

            CommonButtonVisual.UpdateButtonIcons(
                m_topBarGUI.SettingsButtonData,
                Game.Instance.GetGamepadType()
            );

            CommonButtonVisual.UpdateCommonButtonIcons(
                m_topBarGUI,
                m_cardsBallsSpinWheelGUI,
                Game.Instance.GetGamepadType()
            );

            m_UI.SetActive(true);

            CommonVisual.ShowTopBar(runData, m_topBarGUI, "Round Selection");

            CommonVisual.ShowJokers(runData, balance, m_cardsBallsSpinWheelGUI.JokerParent);
            CommonVisual.ShowBalls(runData, balance, m_cardsBallsSpinWheelGUI);

            CommonSlotsVisual.ShowSpinWheelForRound(
                runData,
                balance,
                m_cardsBallsSpinWheelGUI.ScoringSlots,
                runData.Round
            );

            if (CommonSlotsVisual.ChangedSlotsCount > 0)
                m_slotAnimTimer = m_slotAnimTime;

            selectButton(MENU_BUTTONS.PLAY);

            Canvas.ForceUpdateCanvases();
            if (m_verticalLayoutGroup != null)
            {
                m_verticalLayoutGroup.enabled = false;
                m_verticalLayoutGroup.enabled = true;
            }
            if (m_horiontalLayoutGroup != null)
            {
                m_horiontalLayoutGroup.enabled = false;
                m_horiontalLayoutGroup.enabled = true;
            }
            Canvas.ForceUpdateCanvases();

            m_sortSlotsCount = 0;

            Debug.Log("prevMenuState " + runData.PrevMenuState.ToString());

            // should only show it if chips/mult gained
            if (m_skipHappened)
                CommonVisual.ShowAbandonedCardPackPopups(runData, balance, m_abandonCount);

            m_abandonCount = runData.CardPackAbandonTotal;

            m_skipHappened = false;

            // CommonSlotsVisual.ChangedSlotsCount = 0;
        }

        void selectButton(MENU_BUTTONS selectedButton)
        {
            Game.Instance.LastSelectedMenuButton[(int)runData.MenuState] = (int)selectedButton;

            Debug.Log("selectMenuButton(" + selectedButton.ToString() + ")");
            int smallRound = runData.Round % 3;

            hideAllButtonSelections();

            m_selectedButton = selectedButton;

            m_roundGUIInfos[smallRound]
                .PlayButtonData.SelectedGO.SetActive(
                    CommonButtonVisual.ShowSelected() && m_selectedButton == MENU_BUTTONS.PLAY
                );
            if (smallRound < 2)
                m_regularRoundGUIInfo[smallRound]
                    .SkipButtonData.SelectedGO.SetActive(
                        CommonButtonVisual.ShowSelected() && m_selectedButton == MENU_BUTTONS.SKIP
                    );
            else
                m_bossRoundGUIInfo.RerollButtonData.SelectedGO.SetActive(
                    CommonButtonVisual.ShowSelected() && m_selectedButton == MENU_BUTTONS.REROLL
                );

            CommonButtonVisual.CommonSelectButton(
                m_topBarGUI,
                m_cardsBallsSpinWheelGUI,
                (COMMON_BUTTONS)m_selectedButton
            );
        }

        public void SelectPrevButton(MENU_BUTTONS selectedButton)
        {
            if (
                selectedButton == MENU_BUTTONS.BALLS
                || selectedButton == MENU_BUTTONS.WHEEL
                || selectedButton == MENU_BUTTONS.SETTINGS
            )
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

        void hideAllButtonSelections()
        {
            for (int i = 0; i < 3; i++)
            {
                m_roundGUIInfos[i].PlayButtonData.SelectedGO.SetActive(false);
                if (i < 2)
                    m_regularRoundGUIInfo[i].SkipButtonData.SelectedGO.SetActive(false);
                else
                    m_bossRoundGUIInfo.RerollButtonData.SelectedGO.SetActive(false);
            }

            CommonButtonVisual.HideAllButtonSelections(m_topBarGUI, m_cardsBallsSpinWheelGUI);
        }

        public void Hide()
        {
            m_UI.SetActive(false);
            CommonVisual.HideJokers();
        }

        public void Tick(float dt)
        {
            if (m_slotAnimTimer > 0.0f)
            {
                m_slotAnimTimer -= dt;

                Span<int> jokerIdxs = new int[balance.MaxJokersInHand];
                int jokerCount = 0;

                if (m_slotAnimTimer <= 0.0f)
                    if (
                        m_sortSlotsCount == 0
                        && Logic.CheckForSortSlotsJoker(runData, balance, jokerIdxs, ref jokerCount)
                    )
                    {
                        sortSlots();
                        for (int jIdx = 0; jIdx < jokerCount; jIdx++)
                            CommonVisual.JokerGUIs[jokerIdxs[jIdx]].Animation.Play("ScoreGrow");
                    }

                float value = 1.0f - m_slotAnimTimer;
                if (value < 0.0f)
                    value = 0.0f;
                CommonSlotsVisual.TickHighlightChangedSlots(
                    runData,
                    balance,
                    value,
                    SlotScaleCurve,
                    m_cardsBallsSpinWheelGUI.ScoringSlots,
                    runData.SlotTypeInGame
                );
            }

            CommonSlotsVisual.TickSpinWheelUI(
                runData,
                balance.UISpinWheelSpeed,
                dt,
                m_cardsBallsSpinWheelGUI
            );
            CommonSlotsVisual.TickSortingPopup(dt, m_cardsBallsSpinWheelGUI);

            handleInput(runData);
        }

        void handleInput(RunData runData)
        {
            int smallRound = runData.Round % 3;

            // common input (settings, balls, spinwheel and jokers)
            if (
                CommonButtonVisual.CommonHandleInput(
                    m_topBarGUI,
                    m_cardsBallsSpinWheelGUI,
                    (COMMON_BUTTONS)m_selectedButton
                )
            )
                return;

            int newSelectedButton = CommonButtonVisual.CommonNavigation(
                runData,
                (COMMON_BUTTONS)m_selectedButton
            );
            if (newSelectedButton > -1)
            {
                selectButton((MENU_BUTTONS)newSelectedButton);
                return;
            }

            // handle gamepad buttons
            if (
                CommonButtonVisual.NavigateGamepadButton(m_roundGUIInfos[smallRound].PlayButtonData)
                || m_selectedButton == MENU_BUTTONS.PLAY && CommonButtonVisual.NavigateEnter()
            )
            {
                Game.Instance.StartRound();
                return;
            }

            if (
                (
                    smallRound < 2
                    && CommonButtonVisual.NavigateGamepadButton(
                        m_regularRoundGUIInfo[smallRound].SkipButtonData
                    )
                )
                || m_selectedButton == MENU_BUTTONS.SKIP && CommonButtonVisual.NavigateEnter()
            )
            {
                skip();
                return;
            }

            if (
                (
                    smallRound == 2
                    && CommonButtonVisual.NavigateGamepadButton(m_bossRoundGUIInfo.RerollButtonData)
                )
                || m_selectedButton == MENU_BUTTONS.REROLL && CommonButtonVisual.NavigateEnter()
            )
            {
                tryUseBossReroll();
                return;
            }

            // handle navigation
            if (m_selectedButton == MENU_BUTTONS.PLAY && CommonButtonVisual.NavigateUp())
            {
                if (smallRound < 2)
                    selectButton(MENU_BUTTONS.SKIP);
                else
                    selectButton(MENU_BUTTONS.REROLL);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.PLAY && CommonButtonVisual.NavigateRight())
            {
                selectButton(MENU_BUTTONS.WHEEL);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.PLAY && CommonButtonVisual.NavigateLeft())
            {
                selectButton(MENU_BUTTONS.SETTINGS);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.SKIP && CommonButtonVisual.NavigateDown())
            {
                selectButton(MENU_BUTTONS.PLAY);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.SKIP && CommonButtonVisual.NavigateLeft())
            {
                selectButton(MENU_BUTTONS.SETTINGS);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.REROLL && CommonButtonVisual.NavigateDown())
            {
                selectButton(MENU_BUTTONS.PLAY);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.REROLL && CommonButtonVisual.NavigateLeft())
            {
                selectButton(MENU_BUTTONS.SETTINGS);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.SKIP && CommonButtonVisual.NavigateRight())
            {
                selectButton(MENU_BUTTONS.WHEEL);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.REROLL && CommonButtonVisual.NavigateRight())
            {
                selectButton(MENU_BUTTONS.WHEEL);
                return;
            }

            if (
                m_selectedButton == MENU_BUTTONS.SETTINGS
                && (CommonButtonVisual.NavigateRight() || CommonButtonVisual.NavigateDown())
            )
            {
                selectButton(MENU_BUTTONS.PLAY);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.WHEEL && CommonButtonVisual.NavigateLeft())
            {
                selectButton(MENU_BUTTONS.PLAY);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.BALLS && CommonButtonVisual.NavigateLeft())
            {
                selectButton(MENU_BUTTONS.PLAY);
                return;
            }
        }

        public void skip()
        {
            SoundManager.Instance.PlaySFXButtonOK();

            m_skipHappened = true;

            int skipType = Logic.GetSkipTypeForRound(runData, balance, runData.Round);

            int addedJokerCount;
            Logic.Skip(
                runData,
                balance,
                CommonSlotsVisual.ChangedSlotsIdxs,
                ref CommonSlotsVisual.ChangedSlotsCount,
                out addedJokerCount
            );

            if (
                balance.SkipBalance.DoubleMoney[skipType]
                || balance.SkipBalance.MoneyNow[skipType] > 0
                || balance.SkipBalance.MoneyForSpinsUsed[skipType] > 0
                || balance.SkipBalance.MoneyForSpinsUnused[skipType] > 0
            )
            {
                m_topBarGUI.MoneyAnim.Play();
            }
            if (CommonSlotsVisual.ChangedSlotsCount > 0)
                m_slotAnimTimer = m_slotAnimTime;

            if (balance.SkipBalance.Change2SlotsToPlayedColor[skipType])
                m_slotAnimTimer = m_slotAnimTime;

            if (balance.SkipBalance.SortSlots[skipType])
                sortSlots();

            if (balance.SkipBalance.CardPackIdx[skipType] > -1)
            {
                int cardPackIdx = balance.SkipBalance.CardPackIdx[skipType];
                Logic.OpenCardPack(runData, balance, cardPackIdx);

                if (balance.CardPackType[runData.SelectedShopCardPackIdx] == CARD_PACK_TYPE.BALL)
                    Game.Instance.SetMenuState(MENU_STATE.CARD_PACK_BALL);
                if (balance.CardPackType[runData.SelectedShopCardPackIdx] == CARD_PACK_TYPE.SLOT)
                    Game.Instance.SetMenuState(MENU_STATE.CARD_PACK_SLOT);
                if (balance.CardPackType[runData.SelectedShopCardPackIdx] == CARD_PACK_TYPE.CHIPS)
                    Game.Instance.SetMenuState(MENU_STATE.CARD_PACK_CHIPS);
            }
            else
                Show();

            for (int jkrIdx = 0; jkrIdx < runData.JokerCount; jkrIdx++)
            {
                int jokerType = runData.JokerTypes[jkrIdx];
                {
                    if (balance.JokerBalance.RoundSkippedChipsAdd[jokerType] > 0)
                    {
                        CommonVisual.JokerGUIs[jkrIdx].JokerChipsText.text =
                            "+" + balance.JokerBalance.RoundSkippedChipsAdd[jokerType];
                        CommonVisual.JokerGUIs[jkrIdx].JokerChips.SetActive(true);
                    }
                    if (balance.JokerBalance.RoundSkippedMultiplierAdd[jokerType] > 0)
                    {
                        CommonVisual.JokerGUIs[jkrIdx].JokerMultText.text =
                            "+" + balance.JokerBalance.RoundSkippedMultiplierAdd[jokerType] + "x";
                        CommonVisual.JokerGUIs[jkrIdx].JokerMult.SetActive(true);
                    }
                    if (balance.JokerBalance.RoundSkippedMultiplierMult[jokerType] > 0)
                    {
                        CommonVisual.JokerGUIs[jkrIdx].JokerMultText.text =
                            "+x" + balance.JokerBalance.RoundSkippedMultiplierMult[jokerType];
                        CommonVisual.JokerGUIs[jkrIdx].JokerMult.SetActive(true);
                    }
                    if (balance.SkipBalance.IncreaseJokerSellValue[skipType] > 0)
                    {
                        CommonVisual.JokerGUIs[jkrIdx].JokerMoneyText.text =
                            "+" + balance.SkipBalance.IncreaseJokerSellValue[skipType];
                        CommonVisual.JokerGUIs[jkrIdx].JokerMoney.SetActive(true);
                    }
                }
            }

            if (
                balance.SkipBalance.AddCommonRandomJoker[skipType] > 0
                || balance.SkipBalance.AddUncommonRandomJoker[skipType] > 0
            )
            {
                CommonVisual.ShowJokers(runData, balance, m_cardsBallsSpinWheelGUI.JokerParent);

                for (
                    int jkrIdx = runData.JokerCount - addedJokerCount;
                    jkrIdx < runData.JokerCount;
                    jkrIdx++
                )
                    CommonVisual.JokerGUIs[jkrIdx].Animation.Play("ScoreGrow");
            }
        }

        private void tryUseBossReroll()
        {
            if (Logic.TryUseBossRerolls(runData, balance))
            {
                Show();
                RunDataIO.SaveRun(runData, balance);
            }
        }

        private void sortSlots()
        {
            CommonSlotsVisual.SortSlotsRoundSelection(runData, balance, m_cardsBallsSpinWheelGUI);
            if (CommonSlotsVisual.ChangedSlotsCount > 0)
                m_slotAnimTimer = m_slotAnimTime;

            m_sortSlotsCount++;
        }
    }
}
