/*
  Cardwheel — Non-Commercial, No-Modification License
  Copyright © 2025 Nitzan Wilnai
  Source Code: https://github.com/nitzanwilnai/Cardwheel

  Permission is granted to view and run this code for non-commercial purposes only.
  Modification, redistribution of altered versions, and commercial use are strictly prohibited.

  See the LICENSE file for full legal terms.
*/

using UnityEngine;
using CommonTools;
using TMPro;

namespace Cardwheel
{
    public class WinScreenVisual : MonoBehaviour
    {
        public enum MENU_BUTTONS
        {
            COPY,
            MAIN_MENU,
            NEW_GAME,
            RETRY,
            WHEEL = 11,
            BALLS = 12,
            JOKER_1 = 20,
            JOKER_2 = 21,
            JOKER_3 = 22,
            JOKER_4 = 23,
            JOKER_5 = 24,
        };
        MENU_BUTTONS m_selectedButton;

        GameObject m_UI;

        TextMeshProUGUI m_bestSpinText;
        TextMeshProUGUI m_wheelPlayedText;
        TextMeshProUGUI m_mostFrequentColorText;
        TextMeshProUGUI m_seedText;

        GUIButtonData m_copyButtonData;
        GUIButtonData m_mainMenuButtonData;
        GUIButtonData m_newGameButtonData;
        GUIButtonData m_retryButtonData;
        GUIButtonData m_continueButtonData;

        CardsBallsSpinWheelGUI m_cardsBallsSpinWheelGUI;

        RunData runData;
        Balance balance;

        public void Init(RunData runData, Balance balance, Camera camera)
        {
            this.runData = runData;
            this.balance = balance;

            m_UI = AssetManager.Instance.LoadWinScreenUI();
            m_UI.GetComponent<Canvas>().worldCamera = camera;
            CommonVisual.ChangeCanvasScalerMatching(m_UI);

            GUIRef guiRef = m_UI.GetComponent<GUIRef>();
            m_bestSpinText = guiRef.GetTextGUI("BestSpin");
            m_wheelPlayedText = guiRef.GetTextGUI("WheelPlayed");
            m_mostFrequentColorText = guiRef.GetTextGUI("MostFrequentColor");
            m_seedText = guiRef.GetTextGUI("Seed");

            GUIButtonRef guiButtonRef = m_UI.GetComponent<GUIButtonRef>();
            m_copyButtonData = guiButtonRef.GetButtonData("Copy");
            m_mainMenuButtonData = guiButtonRef.GetButtonData("MainMenu");
            m_newGameButtonData = guiButtonRef.GetButtonData("NewGame");
            m_retryButtonData = guiButtonRef.GetButtonData("Retry");
            m_continueButtonData = guiButtonRef.GetButtonData("Continue");

            m_copyButtonData.Button.onClick.AddListener(Game.Instance.CopySeed);
            m_mainMenuButtonData.Button.onClick.AddListener(Game.Instance.GoToMainMenu);
            m_newGameButtonData.Button.onClick.AddListener(Game.Instance.StartNewRunSameWheel);
            m_retryButtonData.Button.onClick.AddListener(Game.Instance.RetryRun);
            m_continueButtonData.Button.onClick.AddListener(Game.Instance.EndlessMode);

            CommonButtonVisual.AddSelectedBorder(m_copyButtonData);
            CommonButtonVisual.AddSelectedBorder(m_mainMenuButtonData);
            CommonButtonVisual.AddSelectedBorder(m_newGameButtonData);
            CommonButtonVisual.AddSelectedBorder(m_retryButtonData);
            CommonButtonVisual.AddSelectedBorder(m_continueButtonData);

            CommonVisual.InitCardsBallsSpinWheelGUI(balance, guiRef.GetGameObject("CardsAndBalls"), ref m_cardsBallsSpinWheelGUI);

            Hide();
        }

        public void Show()
        {
            m_UI.SetActive(true);

            m_bestSpinText.text = runData.BestSpin.ToString("N0");
            m_wheelPlayedText.text = CommonVisual.AddOrdinal(runData.WheelIdx + 1);

            m_mostFrequentColorText.text = Logic.GetMostPlayedSlotType(runData).ToString();
            m_seedText.text = Logic.EncodeSeed(runData.StartSeed);

            CommonVisual.ShowJokersBallsAndSpinWheel(runData, balance, m_cardsBallsSpinWheelGUI, runData.SlotType);
            selectButton(MENU_BUTTONS.MAIN_MENU);
        }

        void selectButton(MENU_BUTTONS selectedButton)
        {
            Game.Instance.LastSelectedMenuButton[(int)runData.MenuState] = (int)selectedButton;

            m_selectedButton = selectedButton;

            m_cardsBallsSpinWheelGUI.BallsButtonData.SelectedGO.SetActive(false);
            m_cardsBallsSpinWheelGUI.SpinwheelButtonData.SelectedGO.SetActive(false);
            CommonVisual.UnselectAllJokers();

            if (CommonButtonVisual.ShowSelected())
            {
                m_copyButtonData.SelectedGO.SetActive(m_selectedButton == MENU_BUTTONS.COPY);
                m_mainMenuButtonData.SelectedGO.SetActive(m_selectedButton == MENU_BUTTONS.MAIN_MENU);
                m_newGameButtonData.SelectedGO.SetActive(m_selectedButton == MENU_BUTTONS.NEW_GAME);
                m_retryButtonData.SelectedGO.SetActive(m_selectedButton == MENU_BUTTONS.RETRY);

                CommonButtonVisual.CommonSelectButtonNoTopBar(m_cardsBallsSpinWheelGUI, (COMMON_BUTTONS)m_selectedButton);
            }
        }

        public void SelectPrevButton(MENU_BUTTONS selectedButton)
        {
            if (selectedButton == MENU_BUTTONS.BALLS || selectedButton == MENU_BUTTONS.WHEEL)
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

        public void Hide()
        {
            m_UI.SetActive(false);
        }

        public void Tick(float dt)
        {
            CommonSlotsVisual.TickSpinWheelUI(runData, balance.UISpinWheelSpeed, dt, m_cardsBallsSpinWheelGUI);

            if ((m_selectedButton == MENU_BUTTONS.WHEEL && CommonButtonVisual.NavigateEnter()) || CommonButtonVisual.NavigateGamepadButton(m_cardsBallsSpinWheelGUI.SpinwheelButtonData))
            {
                selectButton(MENU_BUTTONS.WHEEL);
                Game.Instance.GoToChipsInfo();
                return;
            }

            if ((m_selectedButton >= MENU_BUTTONS.JOKER_1 && m_selectedButton <= MENU_BUTTONS.JOKER_5) && CommonButtonVisual.NavigateEnter())
            {
                Game.Instance.ShowJokerInfoPopupFromWinScreen((int)m_selectedButton - (int)COMMON_BUTTONS.JOKER_1);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.COPY && CommonButtonVisual.NavigateEnter())
            {
                Game.Instance.CopySeed();
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.MAIN_MENU && CommonButtonVisual.NavigateEnter())
            {
                Game.Instance.GoToMainMenu();
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.NEW_GAME && CommonButtonVisual.NavigateEnter())
            {
                Game.Instance.StartNewRunSameWheel();
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.RETRY && CommonButtonVisual.NavigateEnter())
            {
                Game.Instance.RetryRun();
                return;
            }

            // navigation
            int newSelectedButton = CommonButtonVisual.CommonNavigation(runData, (COMMON_BUTTONS)m_selectedButton);
            if (newSelectedButton > -1)
            {
                selectButton((MENU_BUTTONS)newSelectedButton);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.MAIN_MENU && CommonButtonVisual.NavigateUp())
            {
                selectButton(MENU_BUTTONS.NEW_GAME);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.NEW_GAME && CommonButtonVisual.NavigateUp())
            {
                selectButton(MENU_BUTTONS.COPY);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.RETRY && CommonButtonVisual.NavigateUp())
            {
                selectButton(MENU_BUTTONS.COPY);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.COPY && CommonButtonVisual.NavigateDown())
            {
                selectButton(MENU_BUTTONS.NEW_GAME);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.NEW_GAME && CommonButtonVisual.NavigateDown())
            {
                selectButton(MENU_BUTTONS.MAIN_MENU);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.RETRY && CommonButtonVisual.NavigateDown())
            {
                selectButton(MENU_BUTTONS.MAIN_MENU);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.NEW_GAME && CommonButtonVisual.NavigateRight())
            {
                selectButton(MENU_BUTTONS.RETRY);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.RETRY && CommonButtonVisual.NavigateLeft())
            {
                selectButton(MENU_BUTTONS.NEW_GAME);
                return;
            }

            if (m_selectedButton < MENU_BUTTONS.RETRY && CommonButtonVisual.NavigateLeft())
            {
                selectButton(MENU_BUTTONS.WHEEL);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.WHEEL && CommonButtonVisual.NavigateRight())
            {
                selectButton(MENU_BUTTONS.COPY);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.BALLS && CommonButtonVisual.NavigateRight())
            {
                selectButton(MENU_BUTTONS.COPY);
                return;
            }

            if (runData.JokerCount == (m_selectedButton - MENU_BUTTONS.JOKER_1 + 1) && CommonButtonVisual.NavigateRight())
            {
                selectButton(MENU_BUTTONS.COPY);
                return;
            }
        }
    }
}