using UnityEngine;
using CommonTools;
using TMPro;

namespace Cardwheel
{
    public class WinScreenVisual : MonoBehaviour
    {
        GameObject m_UI;

        TextMeshProUGUI m_bestSpinText;
        TextMeshProUGUI m_wheelPlayedText;
        TextMeshProUGUI m_mostFrequentColorText;
        TextMeshProUGUI m_seedText;
        // TextMeshProUGUI m_title;

        CardsBallsSpinWheelGUI m_cardsBallsSpinWheelGUI;

        float m_time;

        public void Init(RunData runData, Balance balance, Camera camera)
        {
            m_UI = AssetManager.Instance.LoadWinScreenUI();
            m_UI.GetComponent<Canvas>().worldCamera = camera;

            GUIRef guiRef = m_UI.GetComponent<GUIRef>();
            m_bestSpinText = guiRef.GetTextGUI("BestSpin");
            m_wheelPlayedText = guiRef.GetTextGUI("WheelPlayed");
            m_mostFrequentColorText = guiRef.GetTextGUI("MostFrequentColor");
            m_seedText = guiRef.GetTextGUI("Seed");

            guiRef.GetButton("Copy").onClick.AddListener(Game.Instance.CopySeed);
            guiRef.GetButton("MainMenu").onClick.AddListener(Game.Instance.GoToMainMenu);
            guiRef.GetButton("NewGame").onClick.AddListener(Game.Instance.StartNewRunSameWheel);
            guiRef.GetButton("Retry").onClick.AddListener(Game.Instance.RetryRun);

            CommonVisual.InitCardsBallsSpinWheelGUI(runData, balance, guiRef.GetGameObject("CardsAndBalls"), ref m_cardsBallsSpinWheelGUI);

            Hide();
        }

        public void Show(RunData runData, Balance balance)
        {
            m_UI.SetActive(true);

            m_bestSpinText.text = runData.BestSpin.ToString("N0");
            m_wheelPlayedText.text = CommonVisual.AddOrdinal(runData.WheelIdx + 1);

            m_mostFrequentColorText.text = Logic.GetMostPlayedSlotType(runData).ToString();
            m_seedText.text = Logic.EncodeSeed(runData.StartSeed);

            CommonVisual.ShowJokersBallsAndSpinWheel(runData, balance, m_cardsBallsSpinWheelGUI, runData.SlotType);
        }
        public void Hide()
        {
            m_UI.SetActive(false);
        }
    }
}