/*
  Cardwheel — Non-Commercial, No-Modification License
  Copyright © 2025 Nitzan Wilnai
  Source Code: https://github.com/nitzanwilnai/Cardwheel

  Permission is granted to view and run this code for non-commercial purposes only.
  Modification, redistribution of altered versions, and commercial use are strictly prohibited.

  See the LICENSE file for full legal terms.
*/

using UnityEngine;

namespace Cardwheel
{

    [CreateAssetMenu(fileName = "SpinWheelSO", menuName = "Cardwheel/SpinWheelSO", order = 1)]
    public class SpinWheelSO : ScriptableObject
    {
        [TextArea] public string Description;
        public int Spins = 4;
        public int StartingMoney = 4;
        public float GoalMultiplier = 1.0f;
        public SLOT_TYPE[] SlotType;
    }
}