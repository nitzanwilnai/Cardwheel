using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonTools;
using System;
using System.IO;
using Unity.Collections.LowLevel.Unsafe;

namespace Cardwheel
{
    public enum SLOT_CHANGE_TYPE { RED, ORANGE, GREEN, BLUE, NONE };
    public enum RARITY { COMMON, UNCOMMON, RARE, LEGENDARY, MYTHIC };
    public enum BOSS_EFFECT
    {
        NONE = 0,
        BALLS_DEBUFFED = 1,
        SLOTS_DEBUFFED = 2,
        BALLS_DEBUFFED_FIRST_SPIN = 3,
        SLOTS_DEBUFFED_FIRST_SPIN = 4,
        BALL_EFFECTS_HIDDEN = 5,
        SLOT_EFFECTS_HIDDEN = 6,
        ONE_LESS_SPIN = 7,
        DOUBLE_GOAL = 8,
        ONLY_RED = 9,
        ONLY_YELLOW = 10,
        ONLY_GREEN = 11,
        ONLY_BLUE = 12,
        DIFFERENT_COLOR_EVERY_SPIN = 13,
        LOSE_MONEY_EVERY_SPIN = 14,
        NO_BASE_CHIPS = 15,
        NO_BASE_MULTIPLIER = 16,
        RANDOM_JOKE_DEBUFFED_PER_SPIN = 17,
        JUMBLE_BALLS = 18,
        JUMBLE_SLOTS = 19,
        NO_RED = 20,
        NO_YELLOW = 21,
        NO_GREEN = 22,
        NO_BLUE = 23,
        SWAP_COLORS = 24,
        ONLY_PLAY_MOST_USED_COLOR = 25,
        DEBUFF_MOST_USED_COLOR = 26,
        ONLY_RED_FIRST_SPIN = 27,
        ONLY_YELLOW_FIRST_SPIN = 28,
        ONLY_GREEN_FIRST_SPIN = 29,
        ONLY_BLUE_FIRST_SPIN = 30,

        ONLY_SCORE_SIX_BALLS = 31, // 8 // tested
        MOST_PLAYED_BASE_CHIPS_TO_FIVE = 32, // 7-8 // tested
        ONLY_SCORE_AT_LEAST_TWO_COLORS = 33, // 6-7 // tested
        ONLY_SCORE_AT_LEAST_THREE_COLORS = 34, // 8 // tested
        JUMBLE_SLOT_EFFECTS = 35, // 4-6 // NOT DONE
        ONLY_RED_ORANGE = 36, // tested
        ONLY_GREEN_BLUE = 37, // tested 
        ONLY_RED_GREEN = 38, // tested
        ONLY_BLUE_ORANGE = 39, // done
    };
    public enum VOUCHER_TYPE
    {
        PLUS_ONE_SPIN,
        SHOP_ITEM_DISCOUNT,
        EXTRA_SHOP_JOKER,
        CHEAP_SHOP_REROLLS,
        CHEAP_CARDPACK_REROLLS,
        CARPACK_MOST_PLAYED_COLOR,
        RAISE_INTEREST,
        REROLL_BOSS_TYPE,
        RARE_CARDS_WEIGHT,
        SLOT_CARDPACK_MOST_PLAYED_COLOR
    };

    public class SpinWheelBalance
    {
        public int NumSpinWheels;
        public string[] Description;
        public int[] Spins;
        public int[] StartingMoney;
        public float[] GoalMultiplier;
        public SLOT_TYPE[][] SlotType;
    }

    public class VoucherBalance
    {
        public int NumVouchers;
        public VOUCHER_TYPE[] Type;
        public string[] Description;
        public string[] SpriteName;
        public byte[] Repeatable;
    }

    public class BossBalance
    {
        public int NumBosses;
        public BOSS_EFFECT[] BossEffect;
        public string[] Description;
        public Vector2[] LevelRange;
    }

    public class SkipBalance
    {
        public int NumSkips;
        public string[] SkipDescription;
        public int[] MoneyNow;
        public int[] MoneyAfterBoss;
        public RARITY[] JokerRarity;
        public bool[] ExtraSpin;
        public bool[] Change2SlotsToPlayedColor;
        public bool[] DoubleMoney;
        public bool[] SortSlots;
        public int[] MoneyForSpinsUsed;
        public int[] MoneyForSpinsUnused;
        public bool[] BossReroll;
        public int[] CardPackIdx;
        public bool[] CanShowFirstTwoRounds;
    }


    public class JokerBalance
    {
        public int NumSlots;
        public int NumJokers;
        public string[] JokerSpritesNames;

        public int[] Cost;
        public RARITY[] Rarity;
        public string[] DescriptionName;

        public float[] BaseMultiplierAdd;
        public int[] BaseChipsAdd;
        public float[] BaseMultiplierMult;

        public int[] TypeExists;
        public int[] TypeNotExists;
        public int[] SizeExists;

        public int[] ChipsPerBall;
        public int[] ChipsIncreasePerBall;
        public float[] MultIncreaseForSize;
        public int[] MinTypes;

        public Vector2[] SubtractMultiplierAddPerRound;
        public Vector2[] SubtractChipsPerSpin;

        public float[] PerJokerMultiplierAdd;
        public float[] PerNoJokerMultiplierAdd;

        public float[] ChipsIncreasePerSpin;
        public float[] MultIncreasePerUnusedSpin;
        public float[] MultIncreasePerUsedSpin;

        public Vector2[] MultiplierAddRandomRange;

        public int[] EarnMoneyEveryRound;
        public int[] IncreaseSellValueEveryRound;
        public int[] GoIntoDebt;
        public int[] InterestIncrease;
        public float[] ChanceBallGivesMoney;
        public int[] MoneyPerSpin;

        public int[] ChipsPerDollar;

        public float[] MultiplierMultForSpecialBall;
        public float[] MultiplierMultForNonSpecialBall;

        public bool[] SortSlots;
        public int[] FirstBallConvertSlotToID;

        public int[] AddSpin;
        public float[] LastSpinMultiplierAdd;

        public int[] BallIncMultRemoveSlotMod;
        public float[] AddMultipierMultRemoveAllSlotMod;
        public int[] ChipsAddForEveryNonSlotMod;
        public float[] MultiplierAddForEverySlotMod;
        public float[] BallMultiplierAddForSlotMod;

        public float[] MultiplierAddForLeastPlayedColor;

        public int[] StartRoundChangeSlotID;

        public bool[] RetriggerBallsEverySpin;
        public bool[] RetriggerBallsLastSpin;

        public float[] MultiplierMultEveryShopReroll;
        public float[] MultiplierMultEveryCardPackReroll;

        public float[] MultiplierAddForCardPackAbandon;

        public int[] TriggerEveryXSpins;
    }

    public class BallBalance
    {
        public String[] BallSprite;
        public String[] BallDescription;
        public float[] BallChips;
        public float[] BallMultiplierAdd;
        public float[] BallMultiplierMult;
        public int[] BallMoney;
        public float[] BallRevertChance;
        public float[][] BallColorMultiplier;

    }

    public class CardPackChipsBalance
    {
        public int[] Weights;
        public string[] DescriptionName;
        public SLOT_TYPE[] AffectedSlotType;
    }

    public class CardPackBallBalance
    {
        public int[] BallID;
        public int[] NumBalls;
        public int[] Weights;
        public string[] DescriptionName;
        public SLOT_TYPE[] AffectedSlotType;
    }

    public class CardPackSlotBalance
    {
        public SLOT_CHANGE_TYPE[] SlotChangeType;
        public int[] NumSlots;
        public int[] Weights;
        public string[] DescriptionName;
        public SLOT_TYPE[] AffectedSlotType;

        public int[] Chips;
        public int[] MultiplierAdd;
        public int[] MultiplierMult;
        public int[] Money;
    }

    public class Balance
    {
        public Color[] SlotColors;
        public Color SlotOffColor;
        public Color[] RarityColors;
        public int StartingMoney;
        public int MaxBalls;
        public int NumSlots = 24;
        public int MaxRounds;
        public int MaxJokersInHand;
        public int StartMaxJokers;
        public int BaseChips;
        public float BaseMultiplier;
        public int ShopRerollBaseCost;
        public int CardPackRerollBaseCost;

        public int NumShopJokers;
        public int MaxShopJokers;
        public int MaxShopCardPacks;
        public int MaxShopCardPackCards;
        public int VoucherCost;

        public Color ButtonColorEnabled;
        public Color RerollColorEnabled;
        public Color ButtonColorDisabled;

        public float UISpinWheelSpeed;

        public int InterestEveryXDollars;
        public int InterestEarnedPerXDollars;
        public int InterestMax;

        [Header("RoundInfo")]
        public int[] BaseChip;
        public float[] RoundChipMult;
        public int[] RoundReward;

        public CARD_PACK_TYPE[] CardPackType;
        public SLOT_TYPE CardPackAffectedSlotType;
        public int[] CardPackPickCards;
        public int[] CardPackMaxCards;
        public int[] CardPackCost;
        public int[] CardPackWeight;

        public JokerBalance JokerBalance = new JokerBalance();
        public BallBalance BallBalance = new BallBalance();
        public CardPackBallBalance CardPackBallBalance = new CardPackBallBalance();
        public CardPackSlotBalance CardPackSlotBalance = new CardPackSlotBalance();
        public CardPackChipsBalance CardPackChipsBalance = new CardPackChipsBalance();
        public SkipBalance SkipBalance = new SkipBalance();
        public BossBalance BossBalance = new BossBalance();
        public VoucherBalance VoucherBalance = new VoucherBalance();
        public SpinWheelBalance SpinWheelBalance = new SpinWheelBalance();

        public void LoadBalance()
        {
            TextAsset asset = Resources.Load("balance") as TextAsset;
            LoadBalance(asset.bytes);
        }

        public void LoadBalance(byte[] array)
        {
            Stream s = new MemoryStream(array);
            using (BinaryReader br = new BinaryReader(s))
            {
                int version = br.ReadInt32();

                NumSlots = br.ReadInt32();
                SlotColors = new Color[4];
                for (int i = 0; i < SlotColors.Length; i++)
                    SlotColors[i] = new Color(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());

                SlotOffColor = new Color(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());

                int numRarityColors = br.ReadInt32();
                RarityColors = new Color[numRarityColors];
                for (int i = 0; i < numRarityColors; i++)
                    RarityColors[i] = new Color(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());

                StartingMoney = br.ReadInt32();
                MaxBalls = br.ReadInt32();
                MaxRounds = br.ReadInt32();
                MaxJokersInHand = br.ReadInt32();
                StartMaxJokers = br.ReadInt32();
                BaseChips = br.ReadInt32();
                BaseMultiplier = br.ReadSingle();

                ShopRerollBaseCost = br.ReadInt32();
                CardPackRerollBaseCost = br.ReadInt32();

                InterestEveryXDollars = br.ReadInt32();
                InterestEarnedPerXDollars = br.ReadInt32();
                InterestMax = br.ReadInt32();

                NumShopJokers = br.ReadInt32();
                MaxShopJokers = br.ReadInt32();
                MaxShopCardPacks = br.ReadInt32();
                MaxShopCardPackCards = br.ReadInt32();
                VoucherCost = br.ReadInt32();

                ButtonColorEnabled = new Color(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                RerollColorEnabled = new Color(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());
                ButtonColorDisabled = new Color(br.ReadSingle(), br.ReadSingle(), br.ReadSingle(), br.ReadSingle());

                UISpinWheelSpeed = br.ReadSingle();

                int numCardPacks = br.ReadInt32();
                CardPackType = new CARD_PACK_TYPE[numCardPacks];
                CardPackPickCards = new int[numCardPacks];
                CardPackMaxCards = new int[numCardPacks];
                CardPackCost = new int[numCardPacks];
                CardPackWeight = new int[numCardPacks];
                for (int packIdx = 0; packIdx < numCardPacks; packIdx++)
                {
                    CardPackType[packIdx] = (CARD_PACK_TYPE)br.ReadByte();
                    CardPackPickCards[packIdx] = br.ReadInt32();
                    CardPackMaxCards[packIdx] = br.ReadInt32();
                    CardPackCost[packIdx] = br.ReadInt32();
                    CardPackWeight[packIdx] = br.ReadInt32();
                }

                BaseChip = new int[8];
                RoundChipMult = new float[3];
                RoundReward = new int[3];
                for (int i = 0; i < 8; i++)
                    BaseChip[i] = br.ReadInt32();
                for (int i = 0; i < 3; i++)
                    RoundChipMult[i] = br.ReadSingle();
                for (int i = 0; i < 3; i++)
                    RoundReward[i] = br.ReadInt32();


                int numJokers = br.ReadInt32();
                JokerBalance.NumJokers = numJokers;
                JokerBalance.JokerSpritesNames = new string[numJokers];
                JokerBalance.Cost = new int[numJokers];
                JokerBalance.Rarity = new RARITY[numJokers];
                JokerBalance.DescriptionName = new string[numJokers];

                JokerBalance.BaseChipsAdd = new int[numJokers];
                JokerBalance.BaseMultiplierAdd = new float[numJokers];
                JokerBalance.BaseMultiplierMult = new float[numJokers];

                JokerBalance.TypeExists = new int[numJokers];
                JokerBalance.TypeNotExists = new int[numJokers];
                JokerBalance.SizeExists = new int[numJokers];

                JokerBalance.ChipsPerBall = new int[numJokers];
                JokerBalance.ChipsIncreasePerBall = new int[numJokers];
                JokerBalance.MultIncreaseForSize = new float[numJokers];
                JokerBalance.MinTypes = new int[numJokers];

                JokerBalance.SubtractMultiplierAddPerRound = new Vector2[numJokers];
                JokerBalance.SubtractChipsPerSpin = new Vector2[numJokers];

                JokerBalance.PerJokerMultiplierAdd = new float[numJokers];
                JokerBalance.PerNoJokerMultiplierAdd = new float[numJokers];

                JokerBalance.ChipsIncreasePerSpin = new float[numJokers];
                JokerBalance.MultIncreasePerUnusedSpin = new float[numJokers];
                JokerBalance.MultIncreasePerUsedSpin = new float[numJokers];

                JokerBalance.MultiplierAddRandomRange = new Vector2[numJokers];

                JokerBalance.EarnMoneyEveryRound = new int[numJokers];
                JokerBalance.IncreaseSellValueEveryRound = new int[numJokers];
                JokerBalance.GoIntoDebt = new int[numJokers];
                JokerBalance.InterestIncrease = new int[numJokers];
                JokerBalance.ChanceBallGivesMoney = new float[numJokers];
                JokerBalance.MoneyPerSpin = new int[numJokers];

                JokerBalance.ChipsPerDollar = new int[numJokers];

                JokerBalance.MultiplierMultForSpecialBall = new float[numJokers];
                JokerBalance.MultiplierMultForNonSpecialBall = new float[numJokers];

                JokerBalance.SortSlots = new bool[numJokers];
                JokerBalance.FirstBallConvertSlotToID = new int[numJokers];

                JokerBalance.AddSpin = new int[numJokers];
                JokerBalance.LastSpinMultiplierAdd = new float[numJokers];

                JokerBalance.BallIncMultRemoveSlotMod = new int[numJokers];
                JokerBalance.AddMultipierMultRemoveAllSlotMod = new float[numJokers];
                JokerBalance.ChipsAddForEveryNonSlotMod = new int[numJokers];
                JokerBalance.MultiplierAddForEverySlotMod = new float[numJokers];
                JokerBalance.BallMultiplierAddForSlotMod = new float[numJokers];

                JokerBalance.MultiplierAddForLeastPlayedColor = new float[numJokers];

                JokerBalance.StartRoundChangeSlotID = new int[numJokers];

                JokerBalance.RetriggerBallsEverySpin = new bool[numJokers];
                JokerBalance.RetriggerBallsLastSpin = new bool[numJokers];

                JokerBalance.MultiplierMultEveryShopReroll = new float[numJokers];
                JokerBalance.MultiplierMultEveryCardPackReroll = new float[numJokers];

                JokerBalance.MultiplierAddForCardPackAbandon = new float[numJokers];

                JokerBalance.TriggerEveryXSpins = new int[numJokers];

                for (int jkrIdx = 0; jkrIdx < numJokers; jkrIdx++)
                {
                    JokerBalance.JokerSpritesNames[jkrIdx] = br.ReadString();
                    JokerBalance.Cost[jkrIdx] = br.ReadInt32();
                    JokerBalance.Rarity[jkrIdx] = (RARITY)br.ReadByte();
                    JokerBalance.DescriptionName[jkrIdx] = br.ReadString();

                    JokerBalance.BaseChipsAdd[jkrIdx] = br.ReadInt32();
                    JokerBalance.BaseMultiplierAdd[jkrIdx] = br.ReadSingle();
                    JokerBalance.BaseMultiplierMult[jkrIdx] = br.ReadSingle();

                    for (int i = 0; i < 4; i++)
                        if (br.ReadBoolean())
                            Logic.SetFlag(ref JokerBalance.TypeExists[jkrIdx], i);
                    for (int i = 0; i < 4; i++)
                        if (br.ReadBoolean())
                            Logic.SetFlag(ref JokerBalance.TypeNotExists[jkrIdx], i);
                    for (int i = 0; i < 6; i++)
                        if (br.ReadBoolean())
                            Logic.SetFlag(ref JokerBalance.SizeExists[jkrIdx], i);

                    JokerBalance.ChipsPerBall[jkrIdx] = br.ReadInt32();
                    JokerBalance.ChipsIncreasePerBall[jkrIdx] = br.ReadInt32();
                    JokerBalance.MultIncreaseForSize[jkrIdx] = br.ReadSingle();
                    JokerBalance.MinTypes[jkrIdx] = br.ReadInt32();

                    JokerBalance.SubtractMultiplierAddPerRound[jkrIdx].x = br.ReadSingle();
                    JokerBalance.SubtractMultiplierAddPerRound[jkrIdx].y = br.ReadSingle();
                    JokerBalance.SubtractChipsPerSpin[jkrIdx].x = br.ReadSingle();
                    JokerBalance.SubtractChipsPerSpin[jkrIdx].y = br.ReadSingle();

                    JokerBalance.PerJokerMultiplierAdd[jkrIdx] = br.ReadSingle();
                    JokerBalance.PerNoJokerMultiplierAdd[jkrIdx] = br.ReadSingle();

                    JokerBalance.ChipsIncreasePerSpin[jkrIdx] = br.ReadSingle();
                    JokerBalance.MultIncreasePerUnusedSpin[jkrIdx] = br.ReadSingle();
                    JokerBalance.MultIncreasePerUsedSpin[jkrIdx] = br.ReadSingle();

                    JokerBalance.MultiplierAddRandomRange[jkrIdx] = new Vector2(br.ReadSingle(), br.ReadSingle());

                    JokerBalance.EarnMoneyEveryRound[jkrIdx] = br.ReadInt32();
                    JokerBalance.IncreaseSellValueEveryRound[jkrIdx] = br.ReadInt32();
                    JokerBalance.GoIntoDebt[jkrIdx] = br.ReadInt32();
                    JokerBalance.InterestIncrease[jkrIdx] = br.ReadInt32();
                    JokerBalance.ChanceBallGivesMoney[jkrIdx] = br.ReadSingle();
                    JokerBalance.MoneyPerSpin[jkrIdx] = br.ReadInt32();

                    JokerBalance.ChipsPerDollar[jkrIdx] = br.ReadInt32();

                    JokerBalance.MultiplierMultForSpecialBall[jkrIdx] = br.ReadSingle();
                    JokerBalance.MultiplierMultForNonSpecialBall[jkrIdx] = br.ReadSingle();

                    JokerBalance.SortSlots[jkrIdx] = br.ReadBoolean();
                    JokerBalance.FirstBallConvertSlotToID[jkrIdx] = br.ReadInt32();

                    JokerBalance.AddSpin[jkrIdx] = br.ReadInt32();
                    JokerBalance.LastSpinMultiplierAdd[jkrIdx] = br.ReadSingle();

                    JokerBalance.BallIncMultRemoveSlotMod[jkrIdx] = br.ReadInt32();
                    JokerBalance.AddMultipierMultRemoveAllSlotMod[jkrIdx] = br.ReadSingle();
                    JokerBalance.ChipsAddForEveryNonSlotMod[jkrIdx] = br.ReadInt32();
                    JokerBalance.MultiplierAddForEverySlotMod[jkrIdx] = br.ReadSingle();
                    JokerBalance.BallMultiplierAddForSlotMod[jkrIdx] = br.ReadSingle();

                    JokerBalance.MultiplierAddForLeastPlayedColor[jkrIdx] = br.ReadSingle();

                    JokerBalance.StartRoundChangeSlotID[jkrIdx] = br.ReadInt32();

                    JokerBalance.RetriggerBallsEverySpin[jkrIdx] = br.ReadBoolean();
                    JokerBalance.RetriggerBallsLastSpin[jkrIdx] = br.ReadBoolean();

                    JokerBalance.MultiplierMultEveryShopReroll[jkrIdx] = br.ReadSingle();
                    JokerBalance.MultiplierMultEveryCardPackReroll[jkrIdx] = br.ReadSingle();

                    JokerBalance.MultiplierAddForCardPackAbandon[jkrIdx] = br.ReadSingle();

                    JokerBalance.TriggerEveryXSpins[jkrIdx] = br.ReadInt32();
                }

                int numBalls = br.ReadInt32();
                BallBalance.BallSprite = new String[numBalls];
                BallBalance.BallDescription = new String[numBalls];
                BallBalance.BallChips = new float[numBalls];
                BallBalance.BallMultiplierAdd = new float[numBalls];
                BallBalance.BallMultiplierMult = new float[numBalls];
                BallBalance.BallMoney = new int[numBalls];
                BallBalance.BallRevertChance = new float[numBalls];
                BallBalance.BallColorMultiplier = new float[numBalls][];

                for (int ballIdx = 0; ballIdx < numBalls; ballIdx++)
                {
                    BallBalance.BallSprite[ballIdx] = br.ReadString();
                    BallBalance.BallDescription[ballIdx] = br.ReadString();
                    BallBalance.BallChips[ballIdx] = br.ReadInt32();
                    BallBalance.BallMultiplierAdd[ballIdx] = br.ReadInt32();
                    BallBalance.BallMultiplierMult[ballIdx] = br.ReadSingle();
                    BallBalance.BallMoney[ballIdx] = br.ReadInt32();
                    BallBalance.BallRevertChance[ballIdx] = br.ReadSingle();
                    BallBalance.BallColorMultiplier[ballIdx] = new float[5];
                    for (int i = 0; i < 4; i++)
                        BallBalance.BallColorMultiplier[ballIdx][i] = br.ReadSingle();
                }

                int numCardPackBalls = br.ReadInt32();
                CardPackBallBalance.BallID = new int[numCardPackBalls];
                CardPackBallBalance.NumBalls = new int[numCardPackBalls];
                CardPackBallBalance.Weights = new int[numCardPackBalls];
                CardPackBallBalance.DescriptionName = new string[numCardPackBalls];
                CardPackBallBalance.AffectedSlotType = new SLOT_TYPE[numCardPackBalls];
                for (int packIdx = 0; packIdx < numCardPackBalls; packIdx++)
                {
                    CardPackBallBalance.NumBalls[packIdx] = br.ReadInt32();
                    CardPackBallBalance.BallID[packIdx] = br.ReadInt32();
                    CardPackBallBalance.Weights[packIdx] = br.ReadInt32();
                    CardPackBallBalance.DescriptionName[packIdx] = br.ReadString();
                    CardPackBallBalance.AffectedSlotType[packIdx] = (SLOT_TYPE)br.ReadByte();
                }

                int numCardPackSlots = br.ReadInt32();
                CardPackSlotBalance.SlotChangeType = new SLOT_CHANGE_TYPE[numCardPackSlots];
                CardPackSlotBalance.NumSlots = new int[numCardPackSlots];
                CardPackSlotBalance.Weights = new int[numCardPackSlots];
                CardPackSlotBalance.DescriptionName = new string[numCardPackSlots];
                CardPackSlotBalance.AffectedSlotType = new SLOT_TYPE[numCardPackSlots];

                CardPackSlotBalance.Chips = new int[numCardPackSlots];
                CardPackSlotBalance.MultiplierAdd = new int[numCardPackSlots];
                CardPackSlotBalance.MultiplierMult = new int[numCardPackSlots];
                CardPackSlotBalance.Money = new int[numCardPackSlots];
                for (int packIdx = 0; packIdx < numCardPackSlots; packIdx++)
                {
                    CardPackSlotBalance.SlotChangeType[packIdx] = (SLOT_CHANGE_TYPE)br.ReadByte();
                    CardPackSlotBalance.NumSlots[packIdx] = br.ReadInt32();
                    CardPackSlotBalance.Weights[packIdx] = br.ReadInt32();
                    CardPackSlotBalance.DescriptionName[packIdx] = br.ReadString();
                    CardPackSlotBalance.AffectedSlotType[packIdx] = (SLOT_TYPE)br.ReadByte();

                    CardPackSlotBalance.Chips[packIdx] = br.ReadInt32();
                    CardPackSlotBalance.MultiplierAdd[packIdx] = br.ReadInt32();
                    CardPackSlotBalance.MultiplierMult[packIdx] = br.ReadInt32();
                    CardPackSlotBalance.Money[packIdx] = br.ReadInt32();
                }

                int numCardPackChips = br.ReadInt32();
                CardPackChipsBalance.Weights = new int[numCardPackChips];
                CardPackChipsBalance.DescriptionName = new string[numCardPackChips];
                CardPackChipsBalance.AffectedSlotType = new SLOT_TYPE[numCardPackChips];
                for (int packIdx = 0; packIdx < numCardPackChips; packIdx++)
                {
                    CardPackChipsBalance.Weights[packIdx] = br.ReadInt32();
                    CardPackChipsBalance.DescriptionName[packIdx] = br.ReadString();
                    CardPackChipsBalance.AffectedSlotType[packIdx] = (SLOT_TYPE)br.ReadByte();
                }

                int numSkips = br.ReadInt32();
                SkipBalance.NumSkips = numSkips;
                SkipBalance.SkipDescription = new string[numSkips];
                SkipBalance.MoneyNow = new int[numSkips];
                SkipBalance.MoneyAfterBoss = new int[numSkips];
                SkipBalance.JokerRarity = new RARITY[numSkips];
                SkipBalance.ExtraSpin = new bool[numSkips];
                SkipBalance.Change2SlotsToPlayedColor = new bool[numSkips];
                SkipBalance.DoubleMoney = new bool[numSkips];
                SkipBalance.SortSlots = new bool[numSkips];
                SkipBalance.MoneyForSpinsUsed = new int[numSkips];
                SkipBalance.MoneyForSpinsUnused = new int[numSkips];
                SkipBalance.BossReroll = new bool[numSkips];
                SkipBalance.CardPackIdx = new int[numSkips];
                SkipBalance.CanShowFirstTwoRounds = new bool[numSkips];
                for (int skipIdx = 0; skipIdx < numSkips; skipIdx++)
                {
                    SkipBalance.SkipDescription[skipIdx] = br.ReadString();
                    SkipBalance.MoneyNow[skipIdx] = br.ReadInt32();
                    SkipBalance.MoneyAfterBoss[skipIdx] = br.ReadInt32();
                    SkipBalance.JokerRarity[skipIdx] = (RARITY)br.ReadByte();
                    SkipBalance.ExtraSpin[skipIdx] = br.ReadBoolean();
                    SkipBalance.Change2SlotsToPlayedColor[skipIdx] = br.ReadBoolean();
                    SkipBalance.DoubleMoney[skipIdx] = br.ReadBoolean();
                    SkipBalance.SortSlots[skipIdx] = br.ReadBoolean();
                    SkipBalance.MoneyForSpinsUsed[skipIdx] = br.ReadInt32();
                    SkipBalance.MoneyForSpinsUnused[skipIdx] = br.ReadInt32();
                    SkipBalance.BossReroll[skipIdx] = br.ReadBoolean();
                    SkipBalance.CardPackIdx[skipIdx] = br.ReadInt32();
                    SkipBalance.CanShowFirstTwoRounds[skipIdx] = br.ReadBoolean();
                }

                int numBosses = br.ReadInt32();
                BossBalance.NumBosses = numBosses;
                BossBalance.BossEffect = new BOSS_EFFECT[numBosses];
                BossBalance.Description = new string[numBosses];
                BossBalance.LevelRange = new Vector2[numBosses];
                for (int i = 0; i < numBosses; i++)
                {
                    BossBalance.BossEffect[i] = (BOSS_EFFECT)br.ReadByte();
                    BossBalance.Description[i] = br.ReadString();
                    BossBalance.LevelRange[i].x = br.ReadInt32();
                    BossBalance.LevelRange[i].y = br.ReadInt32();
                }

                int numVouchers = br.ReadInt32();
                VoucherBalance.NumVouchers = numVouchers;
                VoucherBalance.Type = new VOUCHER_TYPE[numVouchers];
                VoucherBalance.Description = new string[numVouchers];
                VoucherBalance.SpriteName = new string[numVouchers];
                VoucherBalance.Repeatable = new byte[numVouchers];
                for (int i = 0; i < numVouchers; i++)
                {
                    VoucherBalance.Type[i] = (VOUCHER_TYPE)br.ReadByte();
                    VoucherBalance.Description[i] = br.ReadString();
                    VoucherBalance.SpriteName[i] = br.ReadString();
                    VoucherBalance.Repeatable[i] = br.ReadByte();
                }

                int numSpinWheels = br.ReadInt32();
                SpinWheelBalance.NumSpinWheels = numSpinWheels;
                SpinWheelBalance.Description = new string[numSpinWheels];
                SpinWheelBalance.Spins = new int[numSpinWheels];
                SpinWheelBalance.StartingMoney = new int[numSpinWheels];
                SpinWheelBalance.GoalMultiplier = new float[numSpinWheels];
                SpinWheelBalance.SlotType = new SLOT_TYPE[numSpinWheels][];
                for (int i = 0; i < numSpinWheels; i++)
                {
                    SpinWheelBalance.Description[i] = br.ReadString();
                    SpinWheelBalance.Spins[i] = br.ReadInt32();
                    SpinWheelBalance.StartingMoney[i] = br.ReadInt32();
                    SpinWheelBalance.GoalMultiplier[i] = br.ReadSingle();
                    SpinWheelBalance.SlotType[i] = new SLOT_TYPE[br.ReadInt32()];
                    for (int j = 0; j < SpinWheelBalance.SlotType[i].Length; j++)
                        SpinWheelBalance.SlotType[i][j] = (SLOT_TYPE)br.ReadByte();
                }
            }
        }
    }
}