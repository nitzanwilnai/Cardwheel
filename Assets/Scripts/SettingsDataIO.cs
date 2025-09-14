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
