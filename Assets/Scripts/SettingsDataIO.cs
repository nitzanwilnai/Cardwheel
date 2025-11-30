/*
  Cardwheel — Non-Commercial, No-Modification License
  Copyright © 2025 Nitzan Wilnai
  Source Code: https://github.com/nitzanwilnai/Cardwheel

  Permission is granted to view and run this code for non-commercial purposes only.
  Modification, redistribution of altered versions, and commercial use are strictly prohibited.

  See the LICENSE file for full legal terms.
*/

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


namespace Cardwheel
{
    public static class SettingsDataIO
    {
        public static void SaveSettings(SettingsData settingsData)
        {
            Debug.LogFormat("SaveGame()");

            string fileName = Application.persistentDataPath + "/settings.dat";
            using (FileStream fs = File.Create(fileName))
            using (BinaryWriter bw = new BinaryWriter(fs))
            {
                bw.Write(2);
                bw.Write(settingsData.SFX);
                bw.Write(settingsData.Music);
                bw.Write(settingsData.Vibrate);
                bw.Write(settingsData.Speed);
                bw.Write(settingsData.SkipRound1);
            }
        }

        public static void LoadSettings(SettingsData settingsData)
        {
            string fileName = Application.persistentDataPath + "/settings.dat";
            if (File.Exists(fileName))
            {
                using (var stream = File.Open(fileName, FileMode.Open))
                {
                    using (BinaryReader br = new BinaryReader(stream))
                    {
                        int version = br.ReadInt32();
                        if (version >= 2)
                        {
                            settingsData.SFX = br.ReadBoolean();
                            settingsData.Music = br.ReadBoolean();
                            settingsData.Vibrate = br.ReadBoolean();
                            settingsData.Speed = br.ReadSingle();
                            settingsData.SkipRound1 = br.ReadBoolean();
                        }
                    }
                }
            }
        }

    }
}
