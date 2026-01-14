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
    [CreateAssetMenu(fileName = "SkipSO", menuName = "Cardwheel/SkipSO", order = 1)]
    public class SkipSO : ScriptableObject
    {
        public string SkipDescription;
        public int MoneyNow;
        public int MoneyAfterBoss;
        public RARITY JokerRarity;
        public bool ExtraSpin;
        public bool Change2SlotsToPlayedColor;
        public bool DoubleMoney;
        public bool SortSlots;
        public int MoneyForSpinsUsed;
        public int MoneyForSpinsUnused;
        public bool BossReroll;
        public int CardPackIdx = -1;
        public bool CanShowFirstTwoRounds;
        public int IncreaseJokerSellValue;
        public int AddCommonRandomJoker;
        public int AddUncommonRandomJoker;
    }
}