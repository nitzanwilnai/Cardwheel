/*
  Cardwheel — Non-Commercial, No-Modification License
  Copyright © 2025 Nitzan Wilnai
  Source Code: https://github.com/nitzanwilnai/Cardwheel

  Permission is granted to view and run this code for non-commercial purposes only.
  Modification, redistribution of altered versions, and commercial use are strictly prohibited.

  See the LICENSE file for full legal terms.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


namespace Cardwheel
{
    public static class GameDataIO
    {
        public static int VERSION = 5;

        public static void SaveGameData(GameData gameData)
        {
            Debug.LogFormat("SaveGame()");

            string fileName = Application.persistentDataPath + "/gamedata_v" + VERSION + ".dat";
            using (FileStream fs = File.Create(fileName))
            using (BinaryWriter bw = new BinaryWriter(fs))
            {
                bw.Write(VERSION);
                bw.Write(gameData.InitialVersion);
                bw.Write(gameData.SpinWheelWinCount.Length);
                for (int i = 0; i < gameData.SpinWheelWinCount.Length; i++)
                    bw.Write(gameData.SpinWheelWinCount[i]);

                bw.Write(gameData.MenuTutorialFlags);

                bw.Write(gameData.RunCounter);
                bw.Write(gameData.AdsRemoved);
            }
        }

        public static bool LoadGameData(GameData gameData, Balance balance)
        {
            string fileName = Application.persistentDataPath + "/gamedata_v" + VERSION + ".dat";
            bool gameDataLoaded = false;
            if (File.Exists(fileName))
            {
                using (var stream = File.Open(fileName, FileMode.Open))
                {
                    using (BinaryReader br = new BinaryReader(stream))
                    {
                        int version = br.ReadInt32();
                        gameData.InitialVersion = br.ReadInt32();
                        int savedNumSpinWheels = br.ReadInt32();
                        Span<int> tempArray = stackalloc int[savedNumSpinWheels];
                        for (int i = 0; i < savedNumSpinWheels; i++)
                            tempArray[i] = br.ReadInt32();

                        gameData.SpinWheelWinCount = new int[balance.SpinWheelBalance.NumSpinWheels];
                        for (int i = 0; i < gameData.SpinWheelWinCount.Length && i < savedNumSpinWheels; i++)
                            gameData.SpinWheelWinCount[i] = tempArray[i];

                        gameData.MenuTutorialFlags = br.ReadInt32();

                        gameData.RunCounter = br.ReadInt32();
                        gameData.AdsRemoved = br.ReadBoolean();

                        gameDataLoaded = true;
                    }
                }
            }
#if UNITY_EDITOR
            // gameData.MenuTutorialFlags = 0;
            for (int i = 0; i < gameData.SpinWheelWinCount.Length; i++)
                gameData.SpinWheelWinCount[i] = 1;
#endif
            // for (int i = 0; i < gameData.SpinWheelWinCount.Length; i++)
            //     gameData.SpinWheelWinCount[i]++;

            return gameDataLoaded;
        }

    }
}
