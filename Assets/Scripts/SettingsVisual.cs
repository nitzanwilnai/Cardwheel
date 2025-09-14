using System.Collections;
using System.Collections.Generic;
using CommonTools;
using UnityEngine;
using TMPro;

namespace Cardwheel
{
    public class SettingsVisual : MonoBehaviour
    {
        GameObject m_UI;

        TextMeshProUGUI m_bestSpinText;
        TextMeshProUGUI m_mostFrequentColorText;
        TextMeshProUGUI m_seedText;

        TextMeshProUGUI m_sfxText;
        TextMeshProUGUI m_musicText;
        TextMeshProUGUI m_vibrateText;
        TextMeshProUGUI m_speedText;
        TextMeshProUGUI m_skipFirstRound;

        SettingsData m_settingsDataRef;

        float m_closeTimer = 0.0f;
        float m_closeTime = 0.1f;
        Animation m_animation;

        public void Init(Camera camera, SettingsData settingsData)
        {
            m_settingsDataRef = settingsData;

            m_UI = AssetManager.Instance.LoadSettingsUI();
            m_UI.GetComponent<Canvas>().worldCamera = camera;
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

            guiRef.GetButton("SFX").onClick.AddListener(toggleSFX);
            guiRef.GetButton("Music").onClick.AddListener(toggleMusic);
            guiRef.GetButton("Vibrate").onClick.AddListener(toggleVibrate);
            guiRef.GetButton("Speed").onClick.AddListener(toggleSpeed);
            guiRef.GetButton("SkipRound1").onClick.AddListener(toggleSkipRound1);

            guiRef.GetButton("MainMenu").onClick.AddListener(Game.Instance.GoToMainMenu);
            guiRef.GetButton("New").onClick.AddListener(Game.Instance.StartNewRunSameWheel);
            guiRef.GetButton("Retry").onClick.AddListener(Game.Instance.RetryRun);

            guiRef.GetButton("Close").onClick.AddListener(Game.Instance.CloseSettings);
            m_animation = guiRef.GetAnimation("Animation");
        }

        public void Show(RunData runData, Balance balance, SettingsData settingsData)
        {
            m_UI.SetActive(true);

            m_bestSpinText.text = runData.BestSpin.ToString("N0");

            m_mostFrequentColorText.text = Logic.GetMostPlayedSlotType(runData).ToString();
            m_mostFrequentColorText.color = balance.SlotColors[(int)Logic.GetMostPlayedSlotType(runData)];
            m_seedText.text = Logic.EncodeSeed(runData.StartSeed);

            updateToggles(settingsData);
        }

        public void Tick(RunData runData, float dt)
        {
            if (CommonVisual.AnimateCloseTick(ref m_closeTimer, dt))
                Game.Instance.SetMenuState(runData.PrevMenuState);
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

        public void AnimateClose()
        {
            Debug.Log("clip count " + m_animation.GetClipCount());
            Debug.Log("clips " + m_animation.GetClip("Settings Close"));
            CommonVisual.AnimateClose(ref m_closeTimer, m_closeTime, m_animation, "Settings Close");
        }

        void toggleSFX()
        {
            m_settingsDataRef.SFX = !m_settingsDataRef.SFX;
            updateToggles(m_settingsDataRef);
            SettingsDataIO.SaveSettings(m_settingsDataRef);
        }

        void toggleMusic()
        {
            m_settingsDataRef.Music = !m_settingsDataRef.Music;
            updateToggles(m_settingsDataRef);
            SettingsDataIO.SaveSettings(m_settingsDataRef);

            MusicManager.Instance.Mute(m_settingsDataRef);
        }

        void toggleVibrate()
        {
            m_settingsDataRef.Vibrate = !m_settingsDataRef.Vibrate;
            updateToggles(m_settingsDataRef);
            SettingsDataIO.SaveSettings(m_settingsDataRef);
        }

        void toggleSpeed()
        {
            m_settingsDataRef.Speed *= 2.0f;
            if (m_settingsDataRef.Speed > 4.0f)
                m_settingsDataRef.Speed = 0.5f;
            updateToggles(m_settingsDataRef);
            SettingsDataIO.SaveSettings(m_settingsDataRef);
        }

        void toggleSkipRound1()
        {
            m_settingsDataRef.SkipRound1 = !m_settingsDataRef.SkipRound1;
            updateToggles(m_settingsDataRef);
            SettingsDataIO.SaveSettings(m_settingsDataRef);
        }
    }
}