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

        public void Init(RunData runData, Balance balance, Camera camera)
        {
            m_UI = AssetManager.Instance.LoadRoundCompleteUI();
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
            m_moneyText = guiRef.GetTextGUI("Money");
            m_spinsText = guiRef.GetTextGUI("Spins");
            guiRef.GetButton("Claim").onClick.AddListener(Game.Instance.ClaimRoundRewardAndGoToShop);

            CommonVisual.InitTopBarGUI(guiRef.GetGameObject("TopBar"), ref m_topBarGUI);
            CommonVisual.InitCardsBallsSpinWheelGUI(runData, balance, guiRef.GetGameObject("CardsAndBalls"), ref m_cardsBallsSpinWheelGUI);

            Hide();
        }

        public void Show(RunData runData, Balance balance)
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

            Canvas.ForceUpdateCanvases();
            m_verticalLayoutGroup.enabled = false;
            m_verticalLayoutGroup.enabled = true;
        }

        public void Tick(RunData runData, Balance balance, float dt)
        {
            CommonSlotsVisual.TickSpinWheelUI(runData, balance.UISpinWheelSpeed, dt, m_cardsBallsSpinWheelGUI);
        }

        public void Hide()
        {
            m_UI.SetActive(false);
            CommonVisual.HideJokers();
        }
    }
}