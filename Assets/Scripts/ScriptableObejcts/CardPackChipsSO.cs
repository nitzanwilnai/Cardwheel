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
    [CreateAssetMenu(fileName = "CardPackChipsSO", menuName = "Cardwheel/CardPackChipsSO", order = 1)]
    public class CardPackChipsSO : ScriptableObject
    {
        public int Weight;
        public GameObject DescriptionGO;        
        public SLOT_TYPE AffectSlotType = SLOT_TYPE.NONE;
    }
}