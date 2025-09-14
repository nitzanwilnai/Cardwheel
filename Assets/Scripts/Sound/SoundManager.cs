using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonTools;
using Lofelt.NiceVibrations;

namespace Cardwheel
{
    public class SoundManager : Singleton<SoundManager>
    {
        AudioSource m_audioSource;
        public AudioClip SFXMarbleMarble;
        public AudioClip SFXMarbleSlot;
        public AudioClip SFXMarbleInSlot;
        public AudioClip SFXScoring;
        public AudioClip SFXScoringTotal;
        public AudioClip SFXMoney;
        public AudioClip SFXWheelSpin;
        public AudioClip SFXButtonOK;
        public AudioClip SFXButtonCancel;
        public AudioClip SFXGameOver;
        public AudioClip SFXWinRound;
        public AudioClip SFXWinGame;
        public AudioClip SFXGateOpen;

        //Awake is always called before any Start functions
        protected override void Awake()
        {
            base.Awake();

            m_audioSource = GetComponent<AudioSource>();
        }

        protected void Start()
        {
        }

        public void PlaySFXMarbleMarble(SettingsData settingsData)
        {
            if (settingsData.Vibrate)
                HapticPatterns.PlayPreset(HapticPatterns.PresetType.HeavyImpact);

            if (settingsData.SFX)
                m_audioSource.PlayOneShot(SFXMarbleMarble);
        }

        public void PlaySFXMarbleSlot(SettingsData settingsData)
        {
            if (settingsData.Vibrate)
                HapticPatterns.PlayPreset(HapticPatterns.PresetType.MediumImpact);

            if (settingsData.SFX)
                m_audioSource.PlayOneShot(SFXMarbleSlot);
        }

        public void PlaySFXMarbleInSlot(SettingsData settingsData)
        {
            if (settingsData.Vibrate)
                HapticPatterns.PlayPreset(HapticPatterns.PresetType.Failure);

            if (settingsData.SFX)
                m_audioSource.PlayOneShot(SFXMarbleInSlot);
        }

        public void PlaySFXScoring(SettingsData settingsData)
        {
            if (settingsData.Vibrate)
                HapticPatterns.PlayPreset(HapticPatterns.PresetType.SoftImpact);

            if (settingsData.SFX)
                m_audioSource.PlayOneShot(SFXScoring);
        }

        public void PlaySFXScoringTotal(SettingsData settingsData)
        {
            if (settingsData.Vibrate)
                HapticPatterns.PlayPreset(HapticPatterns.PresetType.MediumImpact);

            if (settingsData.SFX)
                m_audioSource.PlayOneShot(SFXScoringTotal);
        }

        public void PlaySFXMoney(SettingsData settingsData)
        {
            if (settingsData.Vibrate)
                HapticPatterns.PlayPreset(HapticPatterns.PresetType.SoftImpact);

            if (settingsData.SFX)
                m_audioSource.PlayOneShot(SFXMoney);
        }

        public void PlaySFXWheelSpin(SettingsData settingsData)
        {
            if (settingsData.Vibrate)
                HapticPatterns.PlayPreset(HapticPatterns.PresetType.SoftImpact);

            if (settingsData.SFX)
                m_audioSource.PlayOneShot(SFXWheelSpin);
        }

        public void PlaySFXButtonOK(SettingsData settingsData)
        {
            if (settingsData.Vibrate)
                HapticPatterns.PlayPreset(HapticPatterns.PresetType.SoftImpact);

            if (settingsData.SFX)
                m_audioSource.PlayOneShot(SFXButtonOK);
        }

        public void PlaySFXButtonCancel(SettingsData settingsData)
        {
            if (settingsData.Vibrate)
                HapticPatterns.PlayPreset(HapticPatterns.PresetType.SoftImpact);

            if (settingsData.SFX)
                m_audioSource.PlayOneShot(SFXButtonCancel);
        }

        public void PlaySFXGameOver(SettingsData settingsData)
        {
            if (settingsData.Vibrate)
                HapticPatterns.PlayPreset(HapticPatterns.PresetType.SoftImpact);

            if (settingsData.SFX)
                m_audioSource.PlayOneShot(SFXGameOver);
        }

        public void PlaySFXWinRound(SettingsData settingsData)
        {
            if (settingsData.Vibrate)
                HapticPatterns.PlayPreset(HapticPatterns.PresetType.MediumImpact);

            if (settingsData.SFX)
                m_audioSource.PlayOneShot(SFXWinRound);
        }

        public void PlaySFXWinGame(SettingsData settingsData)
        {
            if (settingsData.Vibrate)
                HapticPatterns.PlayPreset(HapticPatterns.PresetType.HeavyImpact);

            if (settingsData.SFX)
                m_audioSource.PlayOneShot(SFXWinGame);
        }

        public void PlaySFXGateOpen(SettingsData settingsData)
        {
            if (settingsData.Vibrate)
                HapticPatterns.PlayPreset(HapticPatterns.PresetType.HeavyImpact);

            if (settingsData.SFX)
                m_audioSource.PlayOneShot(SFXGateOpen);
        }

    }
}