/*
  Cardwheel — Non-Commercial, No-Modification License
  Copyright © 2025 Nitzan Wilnai
  Source Code: https://github.com/nitzanwilnai/Cardwheel

  Permission is granted to view and run this code for non-commercial purposes only.
  Modification, redistribution of altered versions, and commercial use are strictly prohibited.

  See the LICENSE file for full legal terms.
*/

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using CommonTools;
using System.Xml;

namespace Cardwheel
{
    public class MusicManager : Singleton<MusicManager>
    {
        public AudioClip MusicClip;
        public float BeatsPerMinute;
        public float SecondsPerBeat;
        public double MusicStartTime;
        public float MusicTime;

        AudioSource m_audioSource;
        SettingsData settingsData;

        public void Init(SettingsData settingsData)
        {
            this.settingsData = settingsData;
            m_audioSource = GetComponent<AudioSource>();
            m_audioSource.loop = true;
            Mute();

            SecondsPerBeat = 60.0f / BeatsPerMinute;
        }

        public void Mute()
        {
            m_audioSource.mute = !settingsData.Music;
        }

        public void FixedUpdate()
        {
            MusicTime += Time.deltaTime;
        }

        public float TimeSinceLastBeat()
        {
            float timeSinceLastBeat = MusicTime - (Mathf.Floor(MusicTime / SecondsPerBeat) * SecondsPerBeat);
            return timeSinceLastBeat;
        }

        // // clipIdnex -1 means random clip
        public void PlayMusic()
        {
            if (MusicClip != null)
            {
                m_audioSource.clip = MusicClip;
                m_audioSource.Play();
                MusicStartTime = Time.realtimeSinceStartupAsDouble;
            }
        }
    }
}