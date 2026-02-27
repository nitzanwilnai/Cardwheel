/*
  Cardwheel — Non-Commercial, No-Modification License
  Copyright © 2025 Nitzan Wilnai
  Source Code: https://github.com/nitzanwilnai/Cardwheel

  Permission is granted to view and run this code for non-commercial purposes only.
  Modification, redistribution of altered versions, and commercial use are strictly prohibited.

  See the LICENSE file for full legal terms.
*/

using System;
using System.IO;
using UnityEngine;

namespace Cardwheel
{
    public static class GameDataIOV2
    {
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
                        Span<int> tempArray = stackalloc int[savedNumSpinWheels];
                        for (int i = 0; i < savedNumSpinWheels; i++)
                            tempArray[i] = br.ReadInt32();

                        gameData.SpinWheelWinCount = new int[
                            balance.SpinWheelBalance.NumSpinWheels
                        ];
                        for (int i = 0; i < gameData.SpinWheelWinCount.Length && i < savedNumSpinWheels; i++)
                            gameData.SpinWheelWinCount[i] = tempArray[i];

                        if (version > 1)
                            gameData.MenuTutorialFlags = br.ReadInt32();

                        gameDataLoaded = true;
                    }
                }
            }
#if UNITY_EDITOR
            // gameData.MenuTutorialFlags = 0;
#endif

            return gameDataLoaded;
        }
    }
}
