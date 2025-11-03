using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonTools;
using TMPro;

namespace Cardwheel
{
    public class GameOverVisual : MonoBehaviour
    {

        public enum MENU_BUTTONS
        {
            COPY,
            MAIN_MENU,
            NEW_GAME,
            RETRY
        };
        MENU_BUTTONS m_selectedButton;

        GameObject m_UI;

        GUIButtonData m_copyButtonData;
        GUIButtonData m_mainMenuButtonData;
        GUIButtonData m_newGameButtonData;
        GUIButtonData m_retryButtonData;

        TextMeshProUGUI m_bestSpinText;
        TextMeshProUGUI m_roundReachedText;
        TextMeshProUGUI m_mostFrequentColorText;
        TextMeshProUGUI m_wheelPlayedText;
        TextMeshProUGUI m_seedText;

        public void Init(Camera camera)
        {
            m_UI = AssetManager.Instance.LoadGameOverUI();
            CommonVisual.ChangeCanvasScalerMatching(m_UI);

            m_UI.GetComponent<Canvas>().worldCamera = camera;

            GUIRef guiRef = m_UI.GetComponent<GUIRef>();
            m_bestSpinText = guiRef.GetTextGUI("BestSpin");
            m_roundReachedText = guiRef.GetTextGUI("RoundReached");
            m_mostFrequentColorText = guiRef.GetTextGUI("MostFrequentColor");
            m_wheelPlayedText = guiRef.GetTextGUI("WheelPlayed");
            m_seedText = guiRef.GetTextGUI("Seed");

            GUIButtonRef guiButtonRef = m_UI.GetComponent<GUIButtonRef>();
            m_copyButtonData = guiButtonRef.GetButtonData("Copy");
            m_mainMenuButtonData = guiButtonRef.GetButtonData("MainMenu");
            m_newGameButtonData = guiButtonRef.GetButtonData("NewGame");
            m_retryButtonData = guiButtonRef.GetButtonData("Retry");

            m_copyButtonData.Button.onClick.AddListener(Game.Instance.CopySeed);
            m_mainMenuButtonData.Button.onClick.AddListener(Game.Instance.GoToMainMenu);
            m_newGameButtonData.Button.onClick.AddListener(Game.Instance.StartNewRunSameWheel);
            m_retryButtonData.Button.onClick.AddListener(Game.Instance.RetryRun);

            CommonButtonVisual.AddSelectedBorder(m_copyButtonData);
            CommonButtonVisual.AddSelectedBorder(m_mainMenuButtonData);
            CommonButtonVisual.AddSelectedBorder(m_newGameButtonData);
            CommonButtonVisual.AddSelectedBorder(m_retryButtonData);

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

            selectButton(MENU_BUTTONS.MAIN_MENU);
        }

        void selectButton(MENU_BUTTONS newSelectedButton)
        {
            m_selectedButton = newSelectedButton;

            m_copyButtonData.SelectedGO.SetActive(CommonButtonVisual.ShowSelected() && m_selectedButton == MENU_BUTTONS.COPY);
            m_mainMenuButtonData.SelectedGO.SetActive(CommonButtonVisual.ShowSelected() && m_selectedButton == MENU_BUTTONS.MAIN_MENU);
            m_newGameButtonData.SelectedGO.SetActive(CommonButtonVisual.ShowSelected() && m_selectedButton == MENU_BUTTONS.NEW_GAME);
            m_retryButtonData.SelectedGO.SetActive(CommonButtonVisual.ShowSelected() && m_selectedButton == MENU_BUTTONS.RETRY);
        }

        public void Hide()
        {
            m_UI.SetActive(false);
        }

        public void Tick()
        {
            if (m_selectedButton == MENU_BUTTONS.COPY && CommonButtonVisual.NavigateEnter(Game.Instance.GetTickAvailableInputs()))
            {
                Game.Instance.CopySeed();
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.MAIN_MENU && CommonButtonVisual.NavigateEnter(Game.Instance.GetTickAvailableInputs()))
            {
                Game.Instance.GoToMainMenu();
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.NEW_GAME && CommonButtonVisual.NavigateEnter(Game.Instance.GetTickAvailableInputs()))
            {
                Game.Instance.StartNewRunSameWheel();
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.RETRY && CommonButtonVisual.NavigateEnter(Game.Instance.GetTickAvailableInputs()))
            {
                Game.Instance.RetryRun();
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.MAIN_MENU && CommonButtonVisual.NavigateUp(Game.Instance.GetTickAvailableInputs()))
            {
                selectButton(MENU_BUTTONS.NEW_GAME);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.NEW_GAME && CommonButtonVisual.NavigateUp(Game.Instance.GetTickAvailableInputs()))
            {
                selectButton(MENU_BUTTONS.COPY);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.RETRY && CommonButtonVisual.NavigateUp(Game.Instance.GetTickAvailableInputs()))
            {
                selectButton(MENU_BUTTONS.COPY);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.COPY && CommonButtonVisual.NavigateDown(Game.Instance.GetTickAvailableInputs()))
            {
                selectButton(MENU_BUTTONS.NEW_GAME);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.NEW_GAME && CommonButtonVisual.NavigateDown(Game.Instance.GetTickAvailableInputs()))
            {
                selectButton(MENU_BUTTONS.MAIN_MENU);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.RETRY && CommonButtonVisual.NavigateDown(Game.Instance.GetTickAvailableInputs()))
            {
                selectButton(MENU_BUTTONS.MAIN_MENU);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.NEW_GAME && CommonButtonVisual.NavigateRight(Game.Instance.GetTickAvailableInputs()))
            {
                selectButton(MENU_BUTTONS.RETRY);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.RETRY && CommonButtonVisual.NavigateLeft(Game.Instance.GetTickAvailableInputs()))
            {
                selectButton(MENU_BUTTONS.NEW_GAME);
                return;
            }
        }
    }
}