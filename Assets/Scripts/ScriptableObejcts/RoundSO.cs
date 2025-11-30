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
    [CreateAssetMenu(fileName = "RoundSO", menuName = "Cardwheel/RoundSO", order = 1)]
    public class RoundSO : ScriptableObject
    {
        public int[] BaseChip;
        public float[] RoundChipMult;
        public int[] RoundReward;
    }
}