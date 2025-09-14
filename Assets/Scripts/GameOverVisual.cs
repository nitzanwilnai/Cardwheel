using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonTools;
using TMPro;

namespace Cardwheel
{
    public class GameOverVisual : MonoBehaviour
    {
        GameObject m_UI;

        TextMeshProUGUI m_bestSpinText;
        TextMeshProUGUI m_roundReachedText;
        TextMeshProUGUI m_mostFrequentColorText;
        TextMeshProUGUI m_wheelPlayedText;
        TextMeshProUGUI m_seedText;

        public void Init(Camera camera)
        {
            m_UI = AssetManager.Instance.LoadGameOverUI();
            m_UI.GetComponent<Canvas>().worldCamera = camera;

            GUIRef guiRef = m_UI.GetComponent<GUIRef>();
            m_bestSpinText = guiRef.GetTextGUI("BestSpin");
            m_roundReachedText = guiRef.GetTextGUI("RoundReached");
            m_mostFrequentColorText = guiRef.GetTextGUI("MostFrequentColor");
            m_wheelPlayedText = guiRef.GetTextGUI("WheelPlayed");
            m_seedText = guiRef.GetTextGUI("Seed");

            guiRef.GetButton("Copy").onClick.AddListener(Game.Instance.CopySeed);
            guiRef.GetButton("MainMenu").onClick.AddListener(Game.Instance.GoToMainMenu);
            guiRef.GetButton("NewGame").onClick.AddListener(Game.Instance.StartNewRunSameWheel);
            guiRef.GetButton("Retry").onClick.AddListener(Game.Instance.RetryRun);

            Hide();
        }

        public void Show(RunData runData)
        {
            m_UI.SetActive(true);

            m_bestSpinText.text = runData.BestSpin.ToString("N0");
            m_roundReachedText.text = CommonVisual.GetRoundString(runData.Round / 3, runData.Round % 3);
            m_wheelPlayedText.text = CommonVisual.AddOrdinal(runData.WheelIdx + 1);

            m_mostFrequentColorText.text = Logic.GetMostPlayedSlotType(runData).ToString();
            m_seedText.text = Logic.EncodeSeed(runData.StartSeed);
        }
        public void Hide()
        {
            m_UI.SetActive(false);
        }
    }
}