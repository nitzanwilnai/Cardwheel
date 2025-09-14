using UnityEngine;

namespace Cardwheel
{
    [CreateAssetMenu(fileName = "JokerSO", menuName = "Cardwheel/JokerSO", order = 1)]
    public class JokerSO : ScriptableObject
    {
        [Header("Common")]
        public Sprite JokerSprite;
        public int Cost;
        public RARITY Rarity;

        public GameObject DescriptionGO;

        [Header("Modifiers")]
        public int BaseChipsAdd = 0;
        public float BaseMultiplierAdd = 0.0f; // 0000
        public float BaseMultiplierMult = 0.0f;

        public bool[] TypeExists = new bool[4];
        public bool[] TypeNotExists = new bool[4];
        public bool[] SizeExists = new bool[6];

        public float MultIncreaseForSize;
        public int MinTypes = 1;

        [Header("PER BALL")]
        public int ChipsPerBall;
        public int ChipsIncreasePerBall;

        [Header("REDUCE EVERY SPIN")]
        public Vector2 SubtractMultiplierAddPerRound = Vector2.zero;
        public Vector2 SubtractChipsAddPerSpin = Vector2.zero;

        [Header("PER JOKER")]
        public float PerJokerMultiplierAdd = 0.0f; // 0017
        public float PerNoJokerMultiplierAdd = 0.0f; //0018

        [Header("PER SPIN")]
        public float ChipsIncreasePerSpin = 0.0f;
        public float MultIncreasePerUnusedSpin = 0.0f;
        public float MultIncreasePerUsedSpin = 0.0f;
        public bool RetriggerBallsEverySpin = false;
        public bool RetriggerBallsLastSpin = false;

        [Header("RANDOM")]
        public Vector2 MultiplierAddRandomRange = Vector2.zero;

        [Header("MONEY")]
        public int EarnMoneyEveryRound = 0;
        public int IncreaseSellValueEveryRound = 0;
        public int GoIntoDebt = 0;
        public int InterestIncrease = 0;
        public float ChanceBallGivesMoney = 0.0f;
        public int MoneyPerSpin = 0;

        [Header("MONEY2")]
        public int ChipsPerDollar = 0;

        [Header("BALLS")]
        public float MultiplierMultForSpecialBall = 0.0f;
        public float MultiplierMultForNonSpecialBall = 0.0f;

        [Header("SLOTS")]
        public bool SortSlots = false;
        public CardPackSlotSO FirstBallConvertSlot;

        [Header("SPINS")]
        public int AddSpin;
        public float LastSpinMultiplierAdd = 0.0f;

        [Header("SLOTS")]
        public int BallIncreaseMultRemoveSlotMod = 0; // works
        public float AddMultipierMultRemoveAllSlotMod = 0.0f; // todo
        public int ChipsAddForEveryNonSlotMod = 0; // works
        public float MultiplierAddForEverySlotMod = 0.0f; // works
        public float BallMultiplierAddForSlotMod = 0.0f; // works
        public CardPackSlotSO StartRoundChangeSlotID; // works

        [Header("LEAST COLOR")]
        public float MultiplierAddForLeastPlayedColor = 0.0f; // works

        [Header("REROLL")]
        public float MultiplierMultEveryShopReroll = 0.0f;
        public float MultiplierMultEveryCardPackReroll = 0.0f;

        [Header("TODO")]

        public float MultiplierAddForCardpackAbandon = 0.0f; // todo

        public int TriggerEveryXSpins = 1; // todo
    }
}