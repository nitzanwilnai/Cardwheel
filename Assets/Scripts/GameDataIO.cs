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
        public static void SaveGameData(GameData gameData)
        {
            Debug.LogFormat("SaveGame()");

            string fileName = Application.persistentDataPath + "/gamedata.dat";
            using (FileStream fs = File.Create(fileName))
            using (BinaryWriter bw = new BinaryWriter(fs))
            {
                bw.Write(1);
                bw.Write(gameData.SpinWheelWinCount.Length);
                for (int i = 0; i < gameData.SpinWheelWinCount.Length; i++)
                    bw.Write(gameData.SpinWheelWinCount[i]);
            }
        }

        public static bool LoadGameData(GameData gameData, Balance balance)
        {
            string fileName = Application.persistentDataPath + "/gamedata.dat";
            bool gameDataLoaded = false;
            if (File.Exists(fileName))
            {
                using (var stream = File.Open(fileName, FileMode.Open))
                {
                    using (BinaryReader br = new BinaryReader(stream))
                    {
                        int version = br.ReadInt32();
                        int savedNumSpinWheels = br.ReadInt32();
                        Span<int> tempArray = new int[savedNumSpinWheels];
                        for (int i = 0; i < savedNumSpinWheels; i++)
                            tempArray[i] = br.ReadInt32();

                        gameData.SpinWheelWinCount = new int[balance.SpinWheelBalance.NumSpinWheels];
                        for (int i = 0; i < gameData.SpinWheelWinCount.Length && i < savedNumSpinWheels; i++)
                            gameData.SpinWheelWinCount[i] = tempArray[i];

                        gameDataLoaded = true;
                    }
                }
            }
            return gameDataLoaded;
        }

    }
}
