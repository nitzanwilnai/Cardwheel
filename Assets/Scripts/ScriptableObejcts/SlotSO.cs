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
    [CreateAssetMenu(fileName = "SlotSO", menuName = "Cardwheel/SlotSO", order = 1)]
    public class SlotSO : ScriptableObject
    {
        [HideInInspector] public int ID;
        public SLOT_TYPE SlotType;
        public int SlotChips = 0; // how many chips does this ball add?
        public int SlotMultiplierAdd = 0; // how much multiplier does this ball add?
        public float BallMultiplierMult = 1.0f; // by how much do we multiply the multiplier
        public int BallMoney = 0; // how much money does this ball add?
        public float BallRevertChance = 0.0f; // chance ball loses its specialty
        public int BallOnlyScoresOnColor = -1; // ball only scores if it lands on this color
    }

    // what can slots do?
    // money
    // chips
    // multiplier add
    // multiplier mult
}
