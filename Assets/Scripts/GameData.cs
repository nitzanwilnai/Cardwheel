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
using UnityEngine;
using UnityEngine.Animations;

namespace Cardwheel
{
    public class GameData
    {
        public int InitialVersion;
        public int[] SpinWheelWinCount;
        public int MenuTutorialFlags;
        public int RunCounter;
        public bool AdsRemoved;
    }
}