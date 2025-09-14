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
    }
}