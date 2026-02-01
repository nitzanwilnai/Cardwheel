/*
  Cardwheel — Non-Commercial, No-Modification License
  Copyright © 2025 Nitzan Wilnai
  Source Code: https://github.com/nitzanwilnai/Cardwheel

  Permission is granted to view and run this code for non-commercial purposes only.
  Modification, redistribution of altered versions, and commercial use are strictly prohibited.

  See the LICENSE file for full legal terms.
*/

using CommonTools;
using UnityEngine;
using TMPro;

namespace Cardwheel
{
    public class MainMenuSettingsVisual
    {
        enum MENU_BUTTONS { NONE, SFX, MUSIC, VIBRATE, SPEED, SKIP, CLOSE, RESTORE };
        MENU_BUTTONS m_selectedButton = MENU_BUTTONS.NONE;

        GameObject m_UI;

        TextMeshProUGUI m_sfxText;
        TextMeshProUGUI m_musicText;
        TextMeshProUGUI m_vibrateText;
        TextMeshProUGUI m_speedText;
        TextMeshProUGUI m_skipFirstRound;

        float m_closeTimer = 0.0f;
        float m_closeTime = 0.1f;
        Animation m_animation;

        GUIButtonData m_closeButtonData;
        GUIButtonData m_sfxButtonData;
        GUIButtonData m_musicButtonData;
        GUIButtonData m_vibrateButtonData;
        GUIButtonData m_speedButtonData;
        GUIButtonData m_skipRound1ButtonData;
        GUIButtonData m_restorePurchasesData;

        SettingsData settingsData;

        public void Init(Camera camera, SettingsData settingsData)
        {
            this.settingsData = settingsData;

            m_UI = AssetManager.Instance.LoadMainMenuSettingsUI();
            m_UI.GetComponent<Canvas>().worldCamera = camera;
            CommonVisual.ChangeCanvasScalerMatchingSimple(m_UI);
            m_UI.SetActive(false);

            GUIRef guiRef = m_UI.GetComponent<GUIRef>();

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
            m_restorePurchasesData = guiButtonRef.GetButtonData("RestorePurchase");

            m_closeButtonData = guiButtonRef.GetButtonData("Close");

            m_sfxButtonData.Button.onClick.AddListener(toggleSFX);
            m_musicButtonData.Button.onClick.AddListener(toggleMusic);
            m_vibrateButtonData.Button.onClick.AddListener(toggleVibrate);
            m_speedButtonData.Button.onClick.AddListener(toggleSpeed);
            m_skipRound1ButtonData.Button.onClick.AddListener(toggleSkipRound1);

            m_restorePurchasesData.Button.gameObject.SetActive(false);
#if UNITY_IOS || UNITY_ANDROID
            m_restorePurchasesData.Button.onClick.AddListener(Game.Instance.RestorePurchases);
            m_restorePurchasesData.Button.gameObject.SetActive(true);
#endif
            m_closeButtonData.Button.onClick.AddListener(closeSettings);

            CommonButtonVisual.AddSelectedBorder(m_sfxButtonData);
            CommonButtonVisual.AddSelectedBorder(m_musicButtonData);
            CommonButtonVisual.AddSelectedBorder(m_vibrateButtonData);
            CommonButtonVisual.AddSelectedBorder(m_speedButtonData);
            CommonButtonVisual.AddSelectedBorder(m_skipRound1ButtonData);
            CommonButtonVisual.AddSelectedBorder(m_closeButtonData);

            selectButton(MENU_BUTTONS.NONE);
        }

        public void Show()
        {
            m_UI.SetActive(true);

            CommonButtonVisual.UpdateButtonIcons(m_closeButtonData, Game.Instance.GetGamepadType());

            updateToggles(settingsData);

            selectButton(MENU_BUTTONS.CLOSE);
        }

        void selectButton(MENU_BUTTONS newSelectedButton)
        {
            m_selectedButton = newSelectedButton;

            m_sfxButtonData.SelectedGO.SetActive(CommonButtonVisual.ShowSelected() && m_selectedButton == MENU_BUTTONS.SFX);
            m_musicButtonData.SelectedGO.SetActive(CommonButtonVisual.ShowSelected() && m_selectedButton == MENU_BUTTONS.MUSIC);
            m_vibrateButtonData.SelectedGO.SetActive(CommonButtonVisual.ShowSelected() && m_selectedButton == MENU_BUTTONS.VIBRATE);
            m_speedButtonData.SelectedGO.SetActive(CommonButtonVisual.ShowSelected() && m_selectedButton == MENU_BUTTONS.SPEED);
            m_skipRound1ButtonData.SelectedGO.SetActive(CommonButtonVisual.ShowSelected() && m_selectedButton == MENU_BUTTONS.SKIP);
            m_closeButtonData.SelectedGO.SetActive(CommonButtonVisual.ShowSelected() && m_selectedButton == MENU_BUTTONS.CLOSE);
        }

        public void Tick(float dt)
        {
            if (CommonVisual.AnimateCloseTick(ref m_closeTimer, dt))
                Game.Instance.SetMenuState(MENU_STATE.MAIN_MENU);

            handleInput();
        }

        void handleInput()
        {
            if ((m_selectedButton == MENU_BUTTONS.CLOSE && CommonButtonVisual.NavigateEnter()) || CommonButtonVisual.NavigateGamepadButton(m_closeButtonData))
            {
                Game.Instance.GoToMainMenu();
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.RESTORE && CommonButtonVisual.NavigateEnter())
            {
                Game.Instance.RestorePurchases();
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.SFX && CommonButtonVisual.NavigateEnter())
            {
                toggleSFX();
                return;
            }
            if (m_selectedButton == MENU_BUTTONS.MUSIC && CommonButtonVisual.NavigateEnter())
            {
                toggleMusic();
                return;
            }
            if (m_selectedButton == MENU_BUTTONS.VIBRATE && CommonButtonVisual.NavigateEnter())
            {
                toggleVibrate();
                return;
            }
            if (m_selectedButton == MENU_BUTTONS.SPEED && CommonButtonVisual.NavigateEnter())
            {
                toggleSpeed();
                return;
            }
            if (m_selectedButton == MENU_BUTTONS.SKIP && CommonButtonVisual.NavigateEnter())
            {
                toggleSkipRound1();
                return;
            }

            if (m_selectedButton >= MENU_BUTTONS.SFX && m_selectedButton < MENU_BUTTONS.CLOSE)
                if (CommonButtonVisual.NavigateDown())
                {
                    selectButton(m_selectedButton + 1);
                    return;
                }

            if (m_selectedButton > MENU_BUTTONS.SFX && m_selectedButton <= MENU_BUTTONS.CLOSE)
                if (CommonButtonVisual.NavigateUp())
                {
                    selectButton(m_selectedButton - 1);
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