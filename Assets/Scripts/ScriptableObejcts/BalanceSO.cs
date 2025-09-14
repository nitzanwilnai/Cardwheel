using System;
using UnityEngine;

namespace Cardwheel
{
    [Serializable]
    public struct BallHandType
    {
        public int[] Balls;
        public string HandName;
        public int Chips;
        public float Multiplier;
    }

    [CreateAssetMenu(fileName = "BalanceSO", menuName = "Cardwheel/BalanceSO", order = 1)]
    public class BalanceSO : ScriptableObject
    {
        public int NumSlots;
        public Color[] SlotColors;
        public Color SlotOffColor;
        public Color[] RarityColors;
        public int StartingMoney;
        public int MaxBalls;
        public int MaxRounds;
        public int MaxJokers;
        public int StartMaxJokers;
        public int BaseChips;
        public float BaseMultiplier;
        public int ShopRerollBaseCost;
        public int CardPackRerollBaseCost;
        
        [Header("Interest")]
        public int InterestEveryXDollars;
        public int InterestEarnedPerXDollars;
        public int InterestMax;


        [Header("Shop")]
        public int NumShopJokers;
        public int MaxShopJokers;
        public int MaxShopCardPacks;
        public int MaxShopCardPackCards;
        public int VoucherCost;

        [Header("UI")]
        public Color ButtonColorEnabled;
        public Color RerollColorEnabled;
        public Color ButtonColorDisabled;
        public float UISpinWheelSpeed;

    }
}