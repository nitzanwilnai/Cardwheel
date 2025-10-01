using System;
using CommonTools;
using TMPro;
using UnityEngine;
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

        public void Init(Balance balance, Camera camera)
        {
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
                    FillRoundGUIInfoNoBoss(guiRef.GetGameObject("Round" + (i + 1)), m_roundGUIInfos[i], m_regularRoundGUIInfo[i]);
                else
                    FillRoundGUIInfoBoss(guiRef.GetGameObject("Round" + (i + 1)), m_roundGUIInfos[i], m_bossRoundGUIInfo);
            }

            CommonVisual.InitTopBarGUI(guiRef.GetGameObject("TopBar"), ref m_topBarGUI);
            CommonVisual.InitCardsBallsSpinWheelGUI(balance, guiRef.GetGameObject("CardsAndBalls"), ref m_cardsBallsSpinWheelGUI);

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

            roundGUIInfo.Description = guiRef.GetTextGUI("Description");
        }

        void FillRoundGUIInfoNoBoss(GameObject go, RoundGUIInfo roundGUIInfo, RegularRoundGUIInfo regularRoundGUIInfo)
        {
            fillRoundGUIInfoCommon(go, roundGUIInfo);

            GUIButtonRef guiButtonRef = go.GetComponent<GUIButtonRef>();

            regularRoundGUIInfo.SkipButtonData = guiButtonRef.GetButtonData("Skip");
            regularRoundGUIInfo.SkipButtonData.Button.onClick.AddListener(Game.Instance.SkipRound);
            CommonButtonVisual.AddSelectedBorder(regularRoundGUIInfo.SkipButtonData);
        }

        void FillRoundGUIInfoBoss(GameObject go, RoundGUIInfo roundGUIInfo, BossRoundGUIInfo bossRoundGUIInfo)
        {
            fillRoundGUIInfoCommon(go, roundGUIInfo);

            GUIRef guiRef = go.GetComponent<GUIRef>();
            bossRoundGUIInfo.RerollButtonText = guiRef.GetTextGUI("Reroll");

            GUIButtonRef guiButtonRef = go.GetComponent<GUIButtonRef>();
            bossRoundGUIInfo.RerollButtonData = guiButtonRef.GetButtonData("Reroll");
            bossRoundGUIInfo.RerollButtonData.Button.onClick.AddListener(Game.Instance.UseBossReroll);
            CommonButtonVisual.AddSelectedBorder(bossRoundGUIInfo.RerollButtonData);
        }

        public void Show(RunData runData, Balance balance, GAMEPAD_TYPE gamepadType, int availableInputs)
        {
            Logic.SetDataForNextRound(runData, balance);

            int bigRound = runData.Round / 3;
            int smallRound = runData.Round % 3;
            for (int i = 0; i < 3; i++)
            {
                m_roundGUIInfos[i].Title.text = CommonVisual.GetRoundString(bigRound, i);
                // m_roundGUIInfos[i].Description.gameObject.SetActive(smallRound == i && i < 2);
                CommonVisual.ShowRoundInGameInfo(runData, balance, bigRound, i, m_roundGUIInfos[i].Description);

                string goalText = i < smallRound ? "Complete" : Logic.GetRoundGoal(runData, balance, bigRound, i).ToString("N0");
                m_roundGUIInfos[i].Goal.text = goalText;
                m_roundGUIInfos[i].Reward.text = "$" + balance.RoundReward[i].ToString("N0");
                m_roundGUIInfos[i].Cover.SetActive(smallRound != i);
                CommonButtonVisual.UpdateButtonIcons(m_roundGUIInfos[i].PlayButtonData, gamepadType);

                if (i < 2)
                    CommonButtonVisual.UpdateButtonIcons(m_regularRoundGUIInfo[i].SkipButtonData, gamepadType);
            }

            m_bossRoundGUIInfo.RerollButtonText.text = "Reroll\n(" + runData.BossRerolls + " left)";
            m_bossRoundGUIInfo.RerollButtonData.Button.image.color = runData.BossRerolls > 0 ? balance.RerollColorEnabled : balance.ButtonColorDisabled;
            CommonButtonVisual.UpdateButtonIcons(m_bossRoundGUIInfo.RerollButtonData, gamepadType);

            CommonButtonVisual.UpdateButtonIcons(m_topBarGUI.SettingsButtonData, gamepadType);

            CommonButtonVisual.UpdateCommonButtonIcons(m_topBarGUI, m_cardsBallsSpinWheelGUI, gamepadType);

            m_UI.SetActive(true);

            CommonVisual.ShowTopBar(runData, m_topBarGUI, "Round Selection");

            CommonVisual.ShowJokers(runData, balance, m_cardsBallsSpinWheelGUI.JokerParent);
            CommonVisual.ShowBalls(runData, balance, m_cardsBallsSpinWheelGUI);

            CommonSlotsVisual.ShowSpinWheelForRound(runData, balance, m_cardsBallsSpinWheelGUI.ScoringSlots, runData.Round);

            selectButton(runData, MENU_BUTTONS.PLAY, availableInputs);

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
        }

        void selectButton(RunData runData, MENU_BUTTONS selectedButton, int availableInputs)
        {
            Debug.Log("selectMenuButton(" + selectedButton.ToString() + ")");
            int smallRound = runData.Round % 3;

            hideAllButtonSelections();

            m_selectedButton = selectedButton;
            if (Logic.IsBitSet(availableInputs, (byte)INPUT_TYPES.GAMEPAD) || Logic.IsBitSet(availableInputs, (byte)INPUT_TYPES.KEYBOARD))
            {
                m_roundGUIInfos[smallRound].PlayButtonData.SelectedGO.SetActive(m_selectedButton == MENU_BUTTONS.PLAY);

                m_regularRoundGUIInfo[smallRound].SkipButtonData.SelectedGO.SetActive(m_selectedButton == MENU_BUTTONS.SKIP);

                m_bossRoundGUIInfo.RerollButtonData.SelectedGO.SetActive(m_selectedButton == MENU_BUTTONS.REROLL);

                CommonButtonVisual.CommonSelectButton(m_topBarGUI, m_cardsBallsSpinWheelGUI, (COMMON_BUTTONS)m_selectedButton);
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

        public void Tick(RunData runData, Balance balance, float dt, int availableInputs)
        {
            if (m_slotAnimTimer > 0.0f)
            {
                m_slotAnimTimer -= dt;

                Span<int> jokerIdxs = new int[balance.MaxJokersInHand];
                int jokerCount = 0;

                if (m_slotAnimTimer <= 0.0f)
                    if (Logic.CheckForSortSlotsJoker(runData, balance, jokerIdxs, ref jokerCount))
                    {
                        SortSlots(runData, balance);
                        for (int jIdx = 0; jIdx < jokerCount; jIdx++)
                            CommonVisual.JokerGUIs[jokerIdxs[jIdx]].Animation.Play("ScoreGrow");
                    }

                float value = 1.0f - m_slotAnimTimer;
                if (value < 0.0f)
                    value = 0.0f;
                CommonSlotsVisual.TickHighlightChangedSlots(value, SlotScaleCurve, m_cardsBallsSpinWheelGUI.ScoringSlots, runData.SlotTypeInGame, runData.SlotColors);
            }

            CommonSlotsVisual.TickSpinWheelUI(runData, balance.UISpinWheelSpeed, dt, m_cardsBallsSpinWheelGUI);
            CommonSlotsVisual.TickSortingPopup(dt, m_cardsBallsSpinWheelGUI);

            handleInput(runData, availableInputs);
        }

        void handleInput(RunData runData, int availableInputs)
        {
            int smallRound = runData.Round % 3;

            // common input (settings, balls, spinwheel and jokers)
            if (CommonButtonVisual.CommonHandleInput(m_topBarGUI, m_cardsBallsSpinWheelGUI, availableInputs, (COMMON_BUTTONS)m_selectedButton))
                return;

            int newSelectedButton = CommonButtonVisual.CommonNavigation(availableInputs, (COMMON_BUTTONS)m_selectedButton);
            if (newSelectedButton > -1)
            {
                selectButton(runData, (MENU_BUTTONS)newSelectedButton, availableInputs);
                return;
            }

            // handle gamepad buttons
            if (CommonButtonVisual.NavigateGamepadButton(m_roundGUIInfos[smallRound].PlayButtonData, availableInputs))
            {
                Game.Instance.StartRound();
                return;
            }

            if (smallRound < 2 && CommonButtonVisual.NavigateGamepadButton(m_regularRoundGUIInfo[smallRound].SkipButtonData, availableInputs))
            {
                Game.Instance.SkipRound();
                return;
            }

            if (smallRound == 2 && CommonButtonVisual.NavigateGamepadButton(m_bossRoundGUIInfo.RerollButtonData, availableInputs))
            {
                Game.Instance.UseBossReroll();
                return;
            }

            // handle button trigger
            if (m_selectedButton == MENU_BUTTONS.PLAY && CommonButtonVisual.NavigateEnter(availableInputs))
            {
                Game.Instance.StartRound();
                return;
            }
            if (m_selectedButton == MENU_BUTTONS.SKIP && CommonButtonVisual.NavigateEnter(availableInputs))
            {
                Game.Instance.SkipRound();
                return;
            }
            if (m_selectedButton == MENU_BUTTONS.REROLL && CommonButtonVisual.NavigateEnter(availableInputs))
            {
                Game.Instance.UseBossReroll();
                return;
            }
            if (m_selectedButton == MENU_BUTTONS.SETTINGS && CommonButtonVisual.NavigateEnter(availableInputs))
            {
                Game.Instance.GoToSettings();
                return;
            }
            if (m_selectedButton == MENU_BUTTONS.BALLS && CommonButtonVisual.NavigateEnter(availableInputs))
            {
                Game.Instance.GoToBallScreen();
                return;
            }
            if (m_selectedButton == MENU_BUTTONS.WHEEL && CommonButtonVisual.NavigateEnter(availableInputs))
            {
                Game.Instance.GoToChipsInfo();
                return;
            }

            // handle navigation
            if (m_selectedButton == MENU_BUTTONS.PLAY && CommonButtonVisual.NavigateUp(availableInputs))
            {
                if (smallRound < 2)
                    selectButton(runData, MENU_BUTTONS.SKIP, availableInputs);
                else
                    selectButton(runData, MENU_BUTTONS.REROLL, availableInputs);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.PLAY && CommonButtonVisual.NavigateRight(availableInputs))
            {
                selectButton(runData, MENU_BUTTONS.WHEEL, availableInputs);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.PLAY && CommonButtonVisual.NavigateLeft(availableInputs))
            {
                selectButton(runData, MENU_BUTTONS.SETTINGS, availableInputs);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.SKIP && CommonButtonVisual.NavigateDown(availableInputs))
            {
                selectButton(runData, MENU_BUTTONS.PLAY, availableInputs);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.SKIP && CommonButtonVisual.NavigateLeft(availableInputs))
            {
                selectButton(runData, MENU_BUTTONS.SETTINGS, availableInputs);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.REROLL && CommonButtonVisual.NavigateDown(availableInputs))
            {
                selectButton(runData, MENU_BUTTONS.PLAY, availableInputs);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.REROLL && CommonButtonVisual.NavigateLeft(availableInputs))
            {
                selectButton(runData, MENU_BUTTONS.SETTINGS, availableInputs);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.SKIP && CommonButtonVisual.NavigateRight(availableInputs))
            {
                selectButton(runData, MENU_BUTTONS.WHEEL, availableInputs);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.REROLL && CommonButtonVisual.NavigateRight(availableInputs))
            {
                selectButton(runData, MENU_BUTTONS.WHEEL, availableInputs);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.SETTINGS && (CommonButtonVisual.NavigateRight(availableInputs) || CommonButtonVisual.NavigateDown(availableInputs)))
            {
                selectButton(runData, MENU_BUTTONS.PLAY, availableInputs);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.WHEEL && CommonButtonVisual.NavigateLeft(availableInputs))
            {
                selectButton(runData, MENU_BUTTONS.PLAY, availableInputs);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.BALLS && CommonButtonVisual.NavigateLeft(availableInputs))
            {
                selectButton(runData, MENU_BUTTONS.PLAY, availableInputs);
                return;
            }

        }

        public void Skip(RunData runData, Balance balance, GAMEPAD_TYPE gamepadType, int avaiableInputs)
        {
            int skipIdx = runData.Round % balance.SkipBalance.NumSkips;
            int skipType = runData.SkipType[skipIdx];

            Logic.Skip(runData, balance, CommonSlotsVisual.AffectedSlotsIdxs, ref CommonSlotsVisual.AffectedSlotsCount);
            Show(runData, balance, gamepadType, avaiableInputs);

            if (balance.SkipBalance.DoubleMoney[skipType] ||
                balance.SkipBalance.MoneyNow[skipType] > 0 ||
                balance.SkipBalance.MoneyForSpinsUsed[skipType] > 0 ||
                balance.SkipBalance.MoneyForSpinsUnused[skipType] > 0)
            {
                m_topBarGUI.MoneyAnim.Play();
            }
            if (balance.SkipBalance.Change2SlotsToPlayedColor[skipType])
            {
                m_slotAnimTimer = m_slotAnimTime;
            }
            if (balance.SkipBalance.SortSlots[skipType])
                SortSlots(runData, balance);

            if (balance.SkipBalance.CardPackIdx[skipType] > -1)
            {
                Game.Instance.OpenCardPack(balance.SkipBalance.CardPackIdx[skipType]);
            }
        }

        public void TryUseBossReroll(RunData runData, Balance balance, GAMEPAD_TYPE gamepadType, int avaiableInputs)
        {
            if (Logic.TryUseBossRerolls(runData, balance))
                Show(runData, balance, gamepadType, avaiableInputs);
        }

        public void SortSlots(RunData runData, Balance balance)
        {
            CommonSlotsVisual.SortSlotsRoundSelection(runData, balance, m_cardsBallsSpinWheelGUI);
        }
    }
}
