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
    }

    public class BossRoundGUIInfo
    {
        // boss only
        public TextMeshProUGUI RerollButtonText;
        public Button RerollButton;
    }

    public class RoundSelectionVisual : MonoBehaviour
    {
        public AnimationCurve SlotScaleCurve;

        GameObject m_UI;

        TopBarGUI m_topBarGUI;

        RoundGUIInfo[] m_roundGUIInfos;
        BossRoundGUIInfo m_bossRoundGUIInfo;
        VerticalLayoutGroup m_verticalLayoutGroup;
        HorizontalLayoutGroup m_horiontalLayoutGroup;

        CardsBallsSpinWheelGUI m_cardsBallsSpinWheelGUI;

        float m_slotAnimTimer = 0.0f;
        float m_slotAnimTime = 1.5f;

        public void Init(RunData runData, Balance balance, Camera camera)
        {
            m_UI = AssetManager.Instance.LoadRoundSelectionUI();
            m_UI.GetComponent<Canvas>().worldCamera = camera;

            m_verticalLayoutGroup = m_UI.GetComponent<VerticalLayoutGroup>();
            m_horiontalLayoutGroup = m_UI.GetComponent<HorizontalLayoutGroup>();

            m_roundGUIInfos = new RoundGUIInfo[3];
            m_bossRoundGUIInfo = new BossRoundGUIInfo();
            GUIRef guiRef = m_UI.GetComponent<GUIRef>();

            for (int i = 0; i < 3; i++)
            {
                m_roundGUIInfos[i] = new RoundGUIInfo();
                if (i < 2)
                    FillRoundGUIInfoNoBoss(guiRef.GetGameObject("Round" + (i + 1)), m_roundGUIInfos[i]);
                else
                    FillRoundGUIInfoBoss(guiRef.GetGameObject("Round" + (i + 1)), m_roundGUIInfos[i], m_bossRoundGUIInfo);
            }

            CommonVisual.InitTopBarGUI(guiRef.GetGameObject("TopBar"), ref m_topBarGUI);
            CommonVisual.InitCardsBallsSpinWheelGUI(runData, balance, guiRef.GetGameObject("CardsAndBalls"), ref m_cardsBallsSpinWheelGUI);

            Hide();
        }

        void fillRoundGUIInfoCommon(GameObject go, RoundGUIInfo roundGUIInfo)
        {
            GUIRef guiRef = go.GetComponent<GUIRef>();
            roundGUIInfo.Title = guiRef.GetTextGUI("Title");
            roundGUIInfo.Goal = guiRef.GetTextGUI("Goal");
            roundGUIInfo.Reward = guiRef.GetTextGUI("Reward");
            roundGUIInfo.Cover = guiRef.GetGameObject("Cover");

            Button playButton = guiRef.GetButton("Play");
            playButton.onClick.AddListener(Game.Instance.StartRound);

            roundGUIInfo.Description = guiRef.GetTextGUI("Description");
        }

        void FillRoundGUIInfoNoBoss(GameObject go, RoundGUIInfo roundGUIInfo)
        {
            fillRoundGUIInfoCommon(go, roundGUIInfo);

            GUIRef guiRef = go.GetComponent<GUIRef>();

            Button skipButton = guiRef.GetButton("Skip");
            skipButton.onClick.AddListener(Game.Instance.SkipRound);
        }

        void FillRoundGUIInfoBoss(GameObject go, RoundGUIInfo roundGUIInfo, BossRoundGUIInfo bossRoundGUIInfo)
        {
            fillRoundGUIInfoCommon(go, roundGUIInfo);

            GUIRef guiRef = go.GetComponent<GUIRef>();

            bossRoundGUIInfo.RerollButton = guiRef.GetButton("Reroll");
            bossRoundGUIInfo.RerollButton.onClick.AddListener(Game.Instance.UseBossReroll);

            bossRoundGUIInfo.RerollButtonText = guiRef.GetTextGUI("Reroll");
        }

        public void Show(RunData runData, Balance balance)
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
            }

            m_bossRoundGUIInfo.RerollButtonText.text = "Reroll\n(" + runData.BossRerolls + " left)";
            m_bossRoundGUIInfo.RerollButton.image.color = runData.BossRerolls > 0 ? balance.RerollColorEnabled : balance.ButtonColorDisabled;

            m_UI.SetActive(true);

            CommonVisual.ShowTopBar(runData, m_topBarGUI, "Round Selection");


            CommonVisual.ShowJokers(runData, balance, m_cardsBallsSpinWheelGUI.JokerParent);
            CommonVisual.ShowBalls(runData, balance, m_cardsBallsSpinWheelGUI);

            CommonSlotsVisual.ShowSpinWheelForRound(runData, balance, m_cardsBallsSpinWheelGUI.ScoringSlots, runData.Round);
            //CommonSlotsVisual.ShowSpinWheelUI(runData, balance, m_cardsBallsSpinWheelGUI.ScoringSlots);

            // CommonVisual.ShowJokersBallsAndSpinWheel(runData, balance, m_cardsBallsSpinWheelGUI);

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
        }

        public void Hide()
        {
            m_UI.SetActive(false);
            CommonVisual.HideJokers();
        }

        public void Tick(RunData runData, Balance balance, float dt)
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
        }

        public void Skip(RunData runData, Balance balance)
        {
            int skipIdx = runData.Round % balance.SkipBalance.NumSkips;
            int skipType = runData.SkipType[skipIdx];

            Logic.Skip(runData, balance, CommonSlotsVisual.AffectedSlotsIdxs, ref CommonSlotsVisual.AffectedSlotsCount);
            Show(runData, balance);

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

        public void TryUseBossReroll(RunData runData, Balance balance)
        {
            if (Logic.TryUseBossRerolls(runData, balance))
                Show(runData, balance);
        }

        public void SortSlots(RunData runData, Balance balance)
        {
            CommonSlotsVisual.SortSlotsRoundSelection(runData, balance, m_cardsBallsSpinWheelGUI);
        }
    }
}
