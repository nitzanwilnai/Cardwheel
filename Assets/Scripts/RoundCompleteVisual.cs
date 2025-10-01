using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CommonTools;
using TMPro;

namespace Cardwheel
{
    public class RoundCompleteVisual : MonoBehaviour
    {
        public enum MENU_BUTTONS
        {
            CLAIM_REWARD,
            SETTINGS = 10,
            WHEEL = 11,
            BALLS = 12,
            JOKER_1 = 20,
            JOKER_2 = 21,
            JOKER_3 = 22,
            JOKER_4 = 23,
            JOKER_5 = 24,
        };
        MENU_BUTTONS m_selectedButton = MENU_BUTTONS.CLAIM_REWARD;

        GameObject m_UI;

        VerticalLayoutGroup m_verticalLayoutGroup;

        public GameObject m_interestGO;
        public GameObject m_specialGO;
        public GameObject m_spinsGO;
        public TextMeshProUGUI m_rewardText;
        public TextMeshProUGUI m_interestText;
        public TextMeshProUGUI m_specialText;
        public TextMeshProUGUI m_totalText;
        public TextMeshProUGUI m_moneyText;
        public TextMeshProUGUI m_spinsText;

        TopBarGUI m_topBarGUI;
        CardsBallsSpinWheelGUI m_cardsBallsSpinWheelGUI;

        GUIButtonData m_claimRewardData;

        RunData runData;
        Balance balance;

        public void Init(RunData runData, Balance balance, Camera camera)
        {
            this.runData = runData;
            this.balance = balance;

            m_UI = AssetManager.Instance.LoadRoundCompleteUI();
            CommonVisual.ChangeCanvasScalerMatching(m_UI);

            m_UI.GetComponent<Canvas>().worldCamera = camera;

            m_verticalLayoutGroup = m_UI.GetComponent<VerticalLayoutGroup>();

            GUIRef guiRef = m_UI.GetComponent<GUIRef>();
            m_interestGO = guiRef.GetGameObject("Interest");
            m_specialGO = guiRef.GetGameObject("Special");
            m_spinsGO = guiRef.GetGameObject("Spins");

            m_rewardText = guiRef.GetTextGUI("Reward");
            m_interestText = guiRef.GetTextGUI("Interest");
            m_specialText = guiRef.GetTextGUI("Special");
            m_totalText = guiRef.GetTextGUI("Total");
            m_spinsText = guiRef.GetTextGUI("Spins");

            GUIButtonRef guiButtonRef = m_UI.GetComponent<GUIButtonRef>();
            m_claimRewardData = guiButtonRef.GetButtonData("Claim");
            m_claimRewardData.Button.onClick.AddListener(claimRoundRewardAndGoToShop);
            CommonButtonVisual.AddSelectedBorder(m_claimRewardData);

            CommonVisual.InitTopBarGUI(guiRef.GetGameObject("TopBar"), ref m_topBarGUI);
            CommonVisual.InitCardsBallsSpinWheelGUI(balance, guiRef.GetGameObject("CardsAndBalls"), ref m_cardsBallsSpinWheelGUI);

            m_moneyText = m_topBarGUI.MoneyText;

            Hide();
        }

        public void Show(RunData runData, Balance balance, GAMEPAD_TYPE gamepadType, int avaiableInputs)
        {
            m_moneyText.text = runData.Money.ToString("N0");
            m_rewardText.text = "$" + balance.RoundReward[runData.Round % 3].ToString("N0");

            int spins = Logic.GetRoundCompleteMoneyFromSpins(runData);
            m_spinsText.text = "$" + spins.ToString("N0");
            m_spinsGO.SetActive(spins > 0);

            int interest = Logic.GetInterestForRound(runData, balance);
            m_interestText.text = "$" + interest.ToString("N0");
            m_interestGO.SetActive(interest > 0);

            int specialMoney = Logic.GetRoundCompleteMoneyFromJokers(runData, balance);
            if (Logic.InBossRound(runData))
                specialMoney += runData.MoneyAfterBoss;
            m_specialText.text = "$" + specialMoney.ToString("N0");
            m_specialGO.SetActive(specialMoney > 0);

            int total = balance.RoundReward[runData.Round % 3] + interest + specialMoney + spins;
            m_totalText.text = "Claim Reward $" + total.ToString("N0");

            m_UI.SetActive(true);

            CommonVisual.ShowTopBar(runData, m_topBarGUI, "Round Complete");
            CommonVisual.ShowJokersBallsAndSpinWheel(runData, balance, m_cardsBallsSpinWheelGUI, runData.SlotTypeInGame);

            CommonButtonVisual.UpdateCommonButtonIcons(m_topBarGUI, m_cardsBallsSpinWheelGUI, gamepadType);
            CommonButtonVisual.UpdateButtonIcons(m_claimRewardData, gamepadType);

            selectButton(MENU_BUTTONS.CLAIM_REWARD, avaiableInputs);

            Canvas.ForceUpdateCanvases();

            if (m_verticalLayoutGroup != null)
            {
                m_verticalLayoutGroup.enabled = false;
                m_verticalLayoutGroup.enabled = true;
            }
        }

        void hideAllButtonSelections()
        {
            m_claimRewardData.SelectedGO.SetActive(false);

            CommonButtonVisual.HideAllButtonSelections(m_topBarGUI, m_cardsBallsSpinWheelGUI);
        }

        void selectButton(MENU_BUTTONS selectedButton, int availableInputs)
        {
            m_selectedButton = selectedButton;

            hideAllButtonSelections();

            if (Logic.IsBitSet(availableInputs, (byte)INPUT_TYPES.GAMEPAD) || Logic.IsBitSet(availableInputs, (byte)INPUT_TYPES.KEYBOARD))
            {
                m_claimRewardData.SelectedGO.SetActive(m_selectedButton == MENU_BUTTONS.CLAIM_REWARD);

                CommonButtonVisual.CommonSelectButton(m_topBarGUI, m_cardsBallsSpinWheelGUI, (COMMON_BUTTONS)m_selectedButton);
            }
        }

        public void Tick(RunData runData, Balance balance, float dt, int avaiableInputs)
        {
            CommonSlotsVisual.TickSpinWheelUI(runData, balance.UISpinWheelSpeed, dt, m_cardsBallsSpinWheelGUI);

            handleInput(avaiableInputs);
        }

        void handleInput(int availableInputs)
        {
            // common input (settings, balls, spinwheel and jokers)
            if (CommonButtonVisual.CommonHandleInput(m_topBarGUI, m_cardsBallsSpinWheelGUI, availableInputs, (COMMON_BUTTONS)m_selectedButton))
                return;

            int newSelectedButton = CommonButtonVisual.CommonNavigation(availableInputs, (COMMON_BUTTONS)m_selectedButton);
            if (newSelectedButton > -1)
            {
                selectButton((MENU_BUTTONS)newSelectedButton, availableInputs);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.CLAIM_REWARD && CommonButtonVisual.NavigateEnter(availableInputs))
            {
                claimRoundRewardAndGoToShop();
                return;
            }


            if (m_selectedButton == MENU_BUTTONS.CLAIM_REWARD && CommonButtonVisual.NavigateRight(availableInputs))
            {
                selectButton(MENU_BUTTONS.WHEEL, availableInputs);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.CLAIM_REWARD && CommonButtonVisual.NavigateLeft(availableInputs))
            {
                selectButton(MENU_BUTTONS.SETTINGS, availableInputs);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.SETTINGS && (CommonButtonVisual.NavigateRight(availableInputs) || CommonButtonVisual.NavigateDown(availableInputs)))
            {
                selectButton(MENU_BUTTONS.CLAIM_REWARD, availableInputs);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.WHEEL && (CommonButtonVisual.NavigateLeft(availableInputs) || CommonButtonVisual.NavigateDown(availableInputs)))
            {
                selectButton(MENU_BUTTONS.CLAIM_REWARD, availableInputs);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.BALLS && CommonButtonVisual.NavigateLeft(availableInputs))
            {
                selectButton(MENU_BUTTONS.CLAIM_REWARD, availableInputs);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.WHEEL && CommonButtonVisual.NavigateUp(availableInputs))
            {
                selectButton(MENU_BUTTONS.BALLS, availableInputs);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.BALLS && CommonButtonVisual.NavigateDown(availableInputs))
            {
                selectButton(MENU_BUTTONS.WHEEL, availableInputs);
                return;
            }
        }

        public void Hide()
        {
            m_UI.SetActive(false);
            CommonVisual.HideJokers();
        }

        void claimRoundRewardAndGoToShop()
        {
            SoundManager.Instance.PlaySFXMoney();

            Logic.ClaimRoundReward(runData, balance);
            Logic.PopulateShop(runData, balance);
            Game.Instance.SetMenuState(MENU_STATE.SHOP);
        }
    }
}