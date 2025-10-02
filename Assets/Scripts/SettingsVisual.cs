using System.Collections;
using System.Collections.Generic;
using CommonTools;
using UnityEngine;
using TMPro;

namespace Cardwheel
{
    public class SettingsVisual : MonoBehaviour
    {
        enum MENU_BUTTONS { NONE, RETRY, NEW_RUN, MAIN_MENU, CLOSE, SFX, MUSIC, VIBRATE, SPEED, SKIP };
        MENU_BUTTONS m_selectedButton = MENU_BUTTONS.NONE;

        GameObject m_UI;

        TextMeshProUGUI m_bestSpinText;
        TextMeshProUGUI m_mostFrequentColorText;
        TextMeshProUGUI m_seedText;

        TextMeshProUGUI m_sfxText;
        TextMeshProUGUI m_musicText;
        TextMeshProUGUI m_vibrateText;
        TextMeshProUGUI m_speedText;
        TextMeshProUGUI m_skipFirstRound;

        SettingsData settingsData;

        float m_closeTimer = 0.0f;
        float m_closeTime = 0.1f;
        Animation m_animation;

        GUIButtonData m_closeButtonData;
        GUIButtonData m_mainMenuButtonData;
        GUIButtonData m_newRunButtonData;
        GUIButtonData m_retryButtonData;
        GUIButtonData m_sfxButtonData;
        GUIButtonData m_musicButtonData;
        GUIButtonData m_vibrateButtonData;
        GUIButtonData m_speedButtonData;
        GUIButtonData m_skipRound1ButtonData;

        public void Init(Camera camera, SettingsData settingsData)
        {
            this.settingsData = settingsData;

            m_UI = AssetManager.Instance.LoadSettingsUI();
            m_UI.GetComponent<Canvas>().worldCamera = camera;
            CommonVisual.ChangeCanvasScalerMatching(m_UI);
            m_UI.SetActive(false);

            GUIRef guiRef = m_UI.GetComponent<GUIRef>();

            m_bestSpinText = guiRef.GetTextGUI("BestSpin");
            m_mostFrequentColorText = guiRef.GetTextGUI("MostFrequentColor");
            m_seedText = guiRef.GetTextGUI("Seed");

            m_sfxText = guiRef.GetTextGUI("SFX");
            m_musicText = guiRef.GetTextGUI("Music");
            m_vibrateText = guiRef.GetTextGUI("Vibrate");
            m_speedText = guiRef.GetTextGUI("Speed");
            m_skipFirstRound = guiRef.GetTextGUI("SkipRound1");

            m_animation = guiRef.GetAnimation("Animation");

            GUIButtonRef guiButtonRef = m_UI.GetComponent<GUIButtonRef>();
            m_sfxButtonData = guiButtonRef.GetButtonData("SFX");
            m_musicButtonData = guiButtonRef.GetButtonData("Music");
            m_vibrateButtonData = guiButtonRef.GetButtonData("Vibrate");
            m_speedButtonData = guiButtonRef.GetButtonData("Speed");
            m_skipRound1ButtonData = guiButtonRef.GetButtonData("SkipRound1");

            m_mainMenuButtonData = guiButtonRef.GetButtonData("MainMenu");
            m_newRunButtonData = guiButtonRef.GetButtonData("New");
            m_retryButtonData = guiButtonRef.GetButtonData("Retry");
            m_closeButtonData = guiButtonRef.GetButtonData("Close");

            m_sfxButtonData.Button.onClick.AddListener(toggleSFX);
            m_musicButtonData.Button.onClick.AddListener(toggleMusic);
            m_vibrateButtonData.Button.onClick.AddListener(toggleVibrate);
            m_speedButtonData.Button.onClick.AddListener(toggleSpeed);
            m_skipRound1ButtonData.Button.onClick.AddListener(toggleSkipRound1);

            m_mainMenuButtonData.Button.onClick.AddListener(Game.Instance.GoToMainMenu);
            m_newRunButtonData.Button.onClick.AddListener(Game.Instance.StartNewRunSameWheel);
            m_retryButtonData.Button.onClick.AddListener(Game.Instance.RetryRun);

            m_closeButtonData.Button.onClick.AddListener(closeSettings);

            CommonButtonVisual.AddSelectedBorder(m_sfxButtonData);
            CommonButtonVisual.AddSelectedBorder(m_musicButtonData);
            CommonButtonVisual.AddSelectedBorder(m_vibrateButtonData);
            CommonButtonVisual.AddSelectedBorder(m_speedButtonData);
            CommonButtonVisual.AddSelectedBorder(m_skipRound1ButtonData);
            CommonButtonVisual.AddSelectedBorder(m_mainMenuButtonData);
            CommonButtonVisual.AddSelectedBorder(m_newRunButtonData);
            CommonButtonVisual.AddSelectedBorder(m_retryButtonData);
            CommonButtonVisual.AddSelectedBorder(m_closeButtonData);

            selectButton(MENU_BUTTONS.NONE);
        }

        public void Show(RunData runData, Balance balance, SettingsData settingsData, int availableInputs)
        {
            m_UI.SetActive(true);

            m_bestSpinText.text = runData.BestSpin.ToString("N0");

            m_mostFrequentColorText.text = Logic.GetMostPlayedSlotType(runData).ToString();
            m_mostFrequentColorText.color = balance.SlotColors[(int)Logic.GetMostPlayedSlotType(runData)];
            m_seedText.text = Logic.EncodeSeed(runData.StartSeed);

            updateToggles(settingsData);

            if (Logic.IsBitSet(availableInputs, (byte)INPUT_TYPES.GAMEPAD) || Logic.IsBitSet(availableInputs, (byte)INPUT_TYPES.KEYBOARD))
                selectButton(MENU_BUTTONS.CLOSE);
            else
                selectButton(MENU_BUTTONS.NONE);
        }

        void selectButton(MENU_BUTTONS newSelectedButton)
        {
            m_selectedButton = newSelectedButton;

            m_sfxButtonData.SelectedGO.SetActive(m_selectedButton == MENU_BUTTONS.SFX);
            m_musicButtonData.SelectedGO.SetActive(m_selectedButton == MENU_BUTTONS.MUSIC);
            m_vibrateButtonData.SelectedGO.SetActive(m_selectedButton == MENU_BUTTONS.VIBRATE);
            m_speedButtonData.SelectedGO.SetActive(m_selectedButton == MENU_BUTTONS.SPEED);
            m_skipRound1ButtonData.SelectedGO.SetActive(m_selectedButton == MENU_BUTTONS.SKIP);

            m_mainMenuButtonData.SelectedGO.SetActive(m_selectedButton == MENU_BUTTONS.MAIN_MENU);
            m_newRunButtonData.SelectedGO.SetActive(m_selectedButton == MENU_BUTTONS.NEW_RUN);
            m_retryButtonData.SelectedGO.SetActive(m_selectedButton == MENU_BUTTONS.RETRY);

            m_closeButtonData.SelectedGO.SetActive(m_selectedButton == MENU_BUTTONS.CLOSE);
        }

        public void Tick(RunData runData, float dt, int availableInputs)
        {
            if (CommonVisual.AnimateCloseTick(ref m_closeTimer, dt))
                Game.Instance.SetMenuState(runData.PrevMenuState);

            handleInput(availableInputs);
        }

        void handleInput(int availableInputs)
        {
            if ((m_selectedButton == MENU_BUTTONS.CLOSE && CommonButtonVisual.NavigateEnter(availableInputs)) || CommonButtonVisual.NavigateGamepadButton(m_closeButtonData, availableInputs))
            {
                closeSettings();
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.MAIN_MENU && CommonButtonVisual.NavigateEnter(availableInputs))
            {
                Game.Instance.GoToMainMenu();
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.NEW_RUN && CommonButtonVisual.NavigateEnter(availableInputs))
            {
                Game.Instance.StartNewRunSameWheel();
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.RETRY && CommonButtonVisual.NavigateEnter(availableInputs))
            {
                Game.Instance.RetryRun();
                return;
            }


            if (m_selectedButton == MENU_BUTTONS.SFX && CommonButtonVisual.NavigateEnter(availableInputs))
            {
                toggleSFX();
                return;
            }
            if (m_selectedButton == MENU_BUTTONS.MUSIC && CommonButtonVisual.NavigateEnter(availableInputs))
            {
                toggleMusic();
                return;
            }
            if (m_selectedButton == MENU_BUTTONS.VIBRATE && CommonButtonVisual.NavigateEnter(availableInputs))
            {
                toggleVibrate();
                return;
            }
            if (m_selectedButton == MENU_BUTTONS.SPEED && CommonButtonVisual.NavigateEnter(availableInputs))
            {
                toggleSpeed();
                return;
            }
            if (m_selectedButton == MENU_BUTTONS.SKIP && CommonButtonVisual.NavigateEnter(availableInputs))
            {
                toggleSkipRound1();
                return;
            }

            if (m_selectedButton >= MENU_BUTTONS.SFX && m_selectedButton < MENU_BUTTONS.SKIP)
                if (CommonButtonVisual.NavigateDown(availableInputs))
                {
                    selectButton(m_selectedButton + 1);
                    return;
                }

            if (m_selectedButton > MENU_BUTTONS.SFX && m_selectedButton <= MENU_BUTTONS.SKIP)
                if (CommonButtonVisual.NavigateUp(availableInputs))
                {
                    selectButton(m_selectedButton - 1);
                    return;
                }
            if (m_selectedButton >= MENU_BUTTONS.RETRY && m_selectedButton < MENU_BUTTONS.CLOSE)
                if (CommonButtonVisual.NavigateDown(availableInputs))
                {
                    selectButton(m_selectedButton + 1);
                    return;
                }
            if (m_selectedButton > MENU_BUTTONS.RETRY && m_selectedButton <= MENU_BUTTONS.CLOSE)
                if (CommonButtonVisual.NavigateUp(availableInputs))
                {
                    selectButton(m_selectedButton - 1);
                    return;
                }
            if (m_selectedButton >= MENU_BUTTONS.RETRY && m_selectedButton <= MENU_BUTTONS.CLOSE)
                if (CommonButtonVisual.NavigateRight(availableInputs))
                {
                    selectButton(MENU_BUTTONS.SFX);
                    return;
                }
            if (m_selectedButton >= MENU_BUTTONS.SFX && m_selectedButton <= MENU_BUTTONS.SKIP)
                if (CommonButtonVisual.NavigateLeft(availableInputs))
                {
                    selectButton(MENU_BUTTONS.CLOSE);
                    return;
                }
        }

        void updateToggles(SettingsData settingsData)
        {
            m_sfxText.text = settingsData.SFX ? "On" : "Off";
            m_musicText.text = settingsData.Music ? "On" : "Off";
            m_vibrateText.text = settingsData.Vibrate ? "On" : "Off";
            m_speedText.text = settingsData.Speed.ToString("N1");
            m_skipFirstRound.text = settingsData.SkipRound1 ? "On" : "Off";
        }

        public void Hide()
        {
            m_UI.SetActive(false);
        }

        void closeSettings()
        {
            SoundManager.Instance.PlaySFXButtonOK();

            Debug.Log("clip count " + m_animation.GetClipCount());
            Debug.Log("clips " + m_animation.GetClip("Settings Close"));
            CommonVisual.AnimateClose(ref m_closeTimer, m_closeTime, m_animation, "Settings Close");
        }

        void toggleSFX()
        {
            settingsData.SFX = !settingsData.SFX;
            updateToggles(settingsData);
            SettingsDataIO.SaveSettings(settingsData);
        }

        void toggleMusic()
        {
            settingsData.Music = !settingsData.Music;
            updateToggles(settingsData);
            SettingsDataIO.SaveSettings(settingsData);

            MusicManager.Instance.Mute();
        }

        void toggleVibrate()
        {
            settingsData.Vibrate = !settingsData.Vibrate;
            updateToggles(settingsData);
            SettingsDataIO.SaveSettings(settingsData);
        }

        void toggleSpeed()
        {
            settingsData.Speed *= 2.0f;
            if (settingsData.Speed > 4.0f)
                settingsData.Speed = 0.5f;
            updateToggles(settingsData);
            SettingsDataIO.SaveSettings(settingsData);
        }

        void toggleSkipRound1()
        {
            settingsData.SkipRound1 = !settingsData.SkipRound1;
            updateToggles(settingsData);
            SettingsDataIO.SaveSettings(settingsData);
        }
    }
}