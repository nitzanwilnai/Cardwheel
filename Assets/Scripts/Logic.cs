/*
  Cardwheel — Non-Commercial, No-Modification License
  Copyright © 2025 Nitzan Wilnai
  Source Code: https://github.com/nitzanwilnai/Cardwheel

  Permission is granted to view and run this code for non-commercial purposes only.
  Modification, redistribution of altered versions, and commercial use are strictly prohibited.

  See the LICENSE file for full legal terms.
*/

using System;
using Cardwheel;
using UnityEngine;
using UnityEngine.UIElements;

public static class Logic
{
    public static uint CustomRand(ref uint seed)
    {
        seed = (214013 * seed + 2531011);
        return (seed >> 16) & 0x7FFF;
    }

    public static int CustomRandInt(ref uint seed)
    {
        seed = (214013 * seed + 2531011);
        return (int)((seed >> 16) & 0x7FFF);
    }

    public static float CustomRandFloatRange(ref uint seed, float min, float max)
    {
        float randomValue = CustomRandFloat(ref seed);
        return (max - min) * randomValue + min;
    }

    public static float CustomRandFloat(ref uint seed)
    {
        seed = (214013 * seed + 2531011);
        return (float)((seed >> 16) & 0x7FFF) / (32768.0f);
    }

    public static bool IsFlagSet(int flags, int index)
    {
        return (flags & (1 << index)) > 0;
    }

    public static void SetFlag(ref int flags, int index)
    {
        flags |= 1 << index;
    }

    public static int GetFlag(int flags, int index)
    {
        return (flags & (1 << index)) > 0 ? 1 : 0;
    }

    public static int RemoveFlag(int array, int bit)
    {
        int mask = 1 << bit;
        int complementedMask = ~mask;
        return array & complementedMask;
    }

    public static void ShuffleSpanIntArray(ref uint seed, Span<int> array, int count)
    {
        for (int i = 0; i < count; i++)
        {
            int randomIdx = CustomRandInt(ref seed) % count;
            int v = array[randomIdx];
            array[randomIdx] = array[i];
            array[i] = v;
        }
    }

    public static void ShuffleIntArray(ref uint seed, int[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            int randomIdx = CustomRandInt(ref seed) % array.Length;
            int v = array[randomIdx];
            array[randomIdx] = array[i];
            array[i] = v;
        }
    }

    public static void ShuffleSlotTypeArray(ref uint seed, SLOT_TYPE[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            int randomIdx = CustomRandInt(ref seed) % array.Length;
            SLOT_TYPE v = array[randomIdx];
            array[randomIdx] = array[i];
            array[i] = v;
        }
    }

    public static void AllocateRunData(RunData runData, Balance balance)
    {
        runData.JokerCount = 0;
        runData.JokerTypes = new int[balance.MaxJokersInHand];
        runData.JokerSellValues = new int[balance.MaxJokersInHand];
        runData.JokerChips = new double[balance.MaxJokersInHand];
        runData.JokerMultiplierAdd = new double[balance.MaxJokersInHand];
        runData.JokerMultiplierMult = new double[balance.MaxJokersInHand];
        runData.JokerSpins = new int[balance.MaxJokersInHand];
        runData.JokerRounds = new int[balance.MaxJokersInHand];
        runData.JokerSkipCount = new int[balance.MaxJokersInHand];

        runData.BallTypes = new int[balance.MaxBalls];
        runData.BallTypesInGame = new int[balance.MaxBalls];
        runData.BallSnapVelocity = new float[balance.MaxBalls];
        runData.BallSnapTime = new float[balance.MaxBalls];
        runData.BallScoreIdxs = new int[balance.MaxBalls];
        runData.BallSlotIdx = new int[balance.MaxBalls];

        runData.BallScoresCount = new int[(int)SLOT_TYPE.LAST];

        runData.BaseChips = new double[(int)SLOT_TYPE.LAST];

        runData.SlotScored = new int[balance.NumSlots];
        runData.SlotType = new SLOT_TYPE[balance.NumSlots];
        runData.SlotTypeInGame = new SLOT_TYPE[balance.NumSlots];
        runData.SlotModType = new int[balance.NumSlots];

        runData.AvailableJokerTypes = new int[balance.JokerBalance.NumJokers];

        runData.ShopJokerIdxs = new int[balance.MaxShopJokers];
        runData.ShopCardPackIdxs = new int[balance.MaxShopCardPacks];
        runData.VoucherIdxs = new int[balance.VoucherBalance.NumVouchers];

        runData.CardPackBallSelected = new bool[balance.MaxBalls];
        runData.CardPackCardIdxs = new int[balance.MaxShopCardPackCards];

        runData.ColorCount = new int[(int)SLOT_TYPE.LAST];

        runData.SkipType = new int[balance.SkipBalance.NumSkips];
        runData.BossType = new int[balance.MaxRounds / 3];
        runData.UseSlot = new int[balance.NumSlots];
        runData.UseJoker = new int[balance.MaxJokersInHand];

        runData.RoundSeeds = new uint[balance.MaxRounds];
    }

    public static void AllocateGameData(GameData gameData, Balance balance)
    {
        gameData.RunCounter = 0;
        gameData.InitialVersion = GameDataIO.VERSION;
        gameData.SpinWheelWinCount = new int[balance.SpinWheelBalance.NumSpinWheels];
        gameData.AdsRemoved = false;
    }

    public static void StartNewGame(
        GameData gameData,
        RunData runData,
        Balance balance,
        int wheelIdx,
        uint seed
    )
    {
        gameData.RunCounter++;

        runData.WheelIdx = wheelIdx;
        runData.StartSeed = seed;
        runData.GameSeed = seed;
        runData.ShopSeed = seed;
        runData.SkipSeed = seed;
        runData.BossSeed = seed;

        uint roundSeed = seed;
        for (int i = 0; i < balance.MaxRounds; i++)
        {
            CustomRandInt(ref roundSeed);
            runData.RoundSeeds[i] = roundSeed;
        }

        runData.Money = balance.SpinWheelBalance.StartingMoney[runData.WheelIdx];
        runData.MaxJokersInHand = balance.StartMaxJokers;
        runData.MaxSpinsThisRound = balance.SpinWheelBalance.Spins[runData.WheelIdx];

        runData.JokerCount = 0;
        for (int i = 0; i < balance.MaxJokersInHand; i++)
        {
            runData.JokerTypes[i] = -1;
            runData.JokerSellValues[i] = 0;
            runData.JokerChips[i] = 0;
            runData.JokerMultiplierAdd[i] = 0.0f;
            runData.JokerMultiplierMult[i] = 0.0f;
        }

        for (int i = 0; i < balance.MaxBalls; i++)
        {
            runData.BallTypes[i] = 0;
            runData.BallTypesInGame[i] = 0;
            runData.BallScoreIdxs[i] = 0;
            runData.BallSlotIdx[i] = -1;
            runData.BallSnapVelocity[i] = 0.0f;
            runData.BallSnapTime[i] = 0.0f;
        }

        for (int i = 0; i < (int)SLOT_TYPE.LAST; i++)
        {
            runData.BallScoresCount[i] = 0;
            runData.BaseChips[i] = balance.BaseChips;
        }

        for (int slotIdx = 0; slotIdx < balance.NumSlots; slotIdx++)
            // runData.SlotTypeInGame[slotIdx] = runData.SlotType[slotIdx] = (SLOT_TYPE)(slotIdx / balance.SpinWheelBalance.SlotsPerColor[runData.WheelIdx] % 4);
            runData.SlotTypeInGame[slotIdx] = runData.SlotType[slotIdx] = balance
                .SpinWheelBalance
                .SlotType[runData.WheelIdx][slotIdx];

        for (int i = 0; i < balance.NumSlots; i++)
        {
            runData.SlotScored[i] = -1;
            runData.SlotModType[i] = -1;
        }

        for (int i = 0; i < balance.JokerBalance.NumJokers; i++)
            runData.AvailableJokerTypes[i] = i;
        runData.AvailableJokerCount = balance.JokerBalance.NumJokers;

        for (int i = 0; i < balance.MaxShopJokers; i++)
            runData.ShopJokerIdxs[i] = -1;
        runData.ShopJokerCount = balance.NumShopJokers;

        for (int i = 0; i < balance.MaxShopCardPacks; i++)
            runData.ShopCardPackIdxs[i] = -1;

        for (int i = 0; i < balance.VoucherBalance.NumVouchers; i++)
            runData.VoucherIdxs[i] = i;
        ShuffleIntArray(ref runData.ShopSeed, runData.VoucherIdxs);
        runData.VoucherPurchased = false;
        runData.VoucherSpins = 0;
        runData.VoucherMaxInterest = 0;
        runData.VoucherShopDiscount = 1.0f;
        runData.VoucherShopRerollsDiscount = 0;
        runData.VoucherCardPackRerollDiscount = 0;
        runData.VoucherCardPackMostPlayedColor = false;
        runData.VoucherRareJoker = 1.0f;
        runData.VoucherSlotMostPlayedColor = false;

        runData.SelectedShopCardPackIdx = -1;

        for (int i = 0; i < balance.MaxBalls; i++)
            runData.CardPackBallSelected[i] = false;
        for (int i = 0; i < balance.MaxShopCardPackCards; i++)
            runData.CardPackCardIdxs[i] = -1;

        runData.Round = 0;
        runData.CurrentSpin = 0;
        runData.ExtraSkipSpin = 0;
        runData.TotalSpins = 0;
        runData.SpinWheelAngle = 0.0f;

        runData.SpinsUsed = 0;
        runData.SpinsUnused = 0;

        runData.BossRerolls = 0;

        runData.BestSpin = 0;
        for (int i = 0; i < (int)SLOT_TYPE.LAST; i++)
            runData.ColorCount[i] = 0;

        initSkipsForNewGame(runData, balance);

        initBossesForNewGame(runData, balance);

        for (int i = 0; i < balance.MaxJokersInHand; i++)
            runData.UseJoker[i] = 1;

        runData.SkipShopUncommonJoker = 0;
        runData.SkipShopRareJoker = 0;

        runData.MoneyAfterBoss = 0;

        runData.UseBallsSpecial = 1;

        for (int i = 0; i < balance.NumSlots; i++)
            runData.UseSlot[i] = 1;

        for (int i = 0; i < (int)SLOT_TYPE.LAST; i++)
            Logic.SetFlag(ref runData.UseSlotBuffs, i);

        runData.UseBaseChips = 1;

        runData.ShopRerollCount = 0;
        runData.CardPackRerollCount = 0;

        runData.ShopRerollTotal = 0;
        runData.CardPackRerollTotal = 0;
        runData.CardPackAbandonTotal = 0;

#if UNITY_EDITOR
        // runData.Money = -20;

        // runData.SkipType[0] = 16;
        // runData.SkipType[1] = 16;
        // runData.SkipType[2] = 16;
        // runData.SkipType[3] = 16;
        // runData.SkipType[4] = 16;
        // runData.SkipType[5] = 16;

        AddJoker(runData, balance, 107);
        AddJoker(runData, balance, 108);
        AddJoker(runData, balance, 62);
        AddJoker(runData, balance, 63);

        // AddJoker(runData, balance, 90);
        // AddJoker(runData, balance, 91);
        // AddJoker(runData, balance, 92);
        // AddJoker(runData, balance, 20);
        // AddJoker(runData, balance, 73); // 74 and 30

        // for (int i = 0; i < runData.BallTypes.Length; i++)
        //     runData.BallTypes[i] = i;
        // runData.BallTypes[0] = 1;
        // runData.BallTypes[1] = 3;
        // runData.BallTypes[2] = 4;
        // runData.BallTypes[3] = 5;
        // runData.BallTypes[4] = 12;
        // runData.BallTypes[5] = 0;

        // int cnt = 0;
        // for (int i = 0; i < runData.SlotModType.Length; i++)
        //     if (i % 3 == 0)
        //         runData.SlotModType[i] = (cnt++ % 4 + 4);

        // int cnt = 0;
        // for (int i = 0; i < runData.SlotModType.Length; i++)
        //     if (i  / 6 == 0)
        //         runData.SlotModType[i] = (cnt++ % 4 + 4);

        // runData.SlotModType[8] = 0;
        // runData.SlotModType[9] = 0;
        // runData.SlotModType[10] = 0;

        // for (int i = 0; i < balance.BossBalance.NumBosses; i++)
        //     if (balance.BossBalance.BossEffect[i] == BOSS_EFFECT.ONLY_BLUE_ORANGE)
        //         runData.BossType[0] = i;

        // runData.VoucherSlotMostPlayedColor = true;

        // runData.Money = 0;
        // runData.VoucherShopDiscount *= 0.75f;

        // runData.ShopJokerCount = balance.MaxShopJokers;

        // runData.BossRerolls = 99;
#endif
    }

    private static void initBossesForNewGame(RunData runData, Balance balance)
    {
        int numBossRounds = balance.MaxRounds / 3;

        for (int i = 0; i < numBossRounds; i++)
            runData.BossType[i] = -1;

        for (int i = 0; i < numBossRounds; i++)
            setUniqueBossForNormalRound(runData, balance, i);

        /*
                // index bosses by difficulty
                Span<int> easyBosses;
                Span<int> mediumBosses;
                Span<int> hardBosses;
                int easyBossCount;
                int mediumBossCount;
                int hardBossCount;
                indexBossesByDifficulty(runData, balance, out easyBosses, out mediumBosses, out hardBosses, out easyBossCount, out mediumBossCount, out hardBossCount);

                int bossCount = 0;
                for (int i = 0; i < 3; i++)
                    runData.BossType[bossCount++] = easyBosses[i];
                for (int i = 0; i < 3; i++)
                    runData.BossType[bossCount++] = mediumBosses[i];
                for (int i = 0; i < 3; i++)
                    runData.BossType[bossCount++] = hardBosses[i];

                for (int i = bossCount; i < runData.BossType.Length; i++)
                    runData.BossType[i] = hardBosses[i % hardBossCount];
                    */
    }

    private static void initSkipsForNewGame(RunData runData, Balance balance)
    {
        Span<int> skipsFirstTwoRounds;
        Span<int> skipsAllRounds;
        int skipsFirstTwoRoundsCount = 0;
        int skipsAllRoundsCount = 0;
        skipsFirstTwoRounds = new int[balance.SkipBalance.NumSkips];
        skipsAllRounds = new int[balance.SkipBalance.NumSkips];

        for (int i = 0; i < balance.SkipBalance.NumSkips; i++)
        {
            if (balance.SkipBalance.CanShowFirstTwoRounds[i])
                skipsFirstTwoRounds[skipsFirstTwoRoundsCount++] = i;

            skipsAllRounds[skipsAllRoundsCount++] = i;
        }

        // shuffle
        ShuffleSpanIntArray(ref runData.SkipSeed, skipsFirstTwoRounds, skipsFirstTwoRoundsCount);
        ShuffleSpanIntArray(ref runData.SkipSeed, skipsAllRounds, skipsAllRoundsCount);

        // assign skips for first two rounds
        for (int skipIdx = 0; skipIdx < 3; skipIdx++)
        {
            int skipValue = skipsFirstTwoRounds[skipIdx];
            runData.SkipType[skipIdx] = skipValue;

            // remove selected skip from allSkips array
            int count = 0;
            for (int i = 0; i < skipsAllRoundsCount; i++)
                if (skipsAllRounds[i] != skipValue)
                    skipsAllRounds[count++] = skipsAllRounds[i];
            skipsAllRoundsCount = count;
        }

        // Debug.Log("balance.SkipBalance.NumSkips " + balance.SkipBalance.NumSkips + " skipsAllRoundsCount " + skipsAllRoundsCount);

        // assign the rest of the skips
        for (int skipIdx = 2; skipIdx < balance.SkipBalance.NumSkips; skipIdx++)
            runData.SkipType[skipIdx] = skipsAllRounds[skipIdx - 2];
    }

    public static void StartEndlessMode(RunData runData, Balance balance)
    {
        uint roundSeed = runData.RoundSeeds[balance.MaxRounds - 1];
        for (int i = 0; i < balance.MaxRounds; i++)
        {
            CustomRandInt(ref roundSeed);
            runData.RoundSeeds[i] = roundSeed;
        }

        // set endless mode bosses
        Span<int> endlessBosses = stackalloc int[balance.BossBalance.NumBosses];
        int endlessBossCount = 0;

        for (int i = 0; i < balance.BossBalance.NumBosses; i++)
            if (balance.BossBalance.EndlessMode[i])
                endlessBosses[endlessBossCount++] = i;

        ShuffleSpanIntArray(ref runData.BossSeed, endlessBosses, endlessBossCount);

        int numBossRounds = balance.MaxRounds / 3;
        for (int i = 0; i < numBossRounds; i++)
            runData.BossType[i] = endlessBosses[i];
    }

    public static void setUniqueBossForRound(RunData runData, Balance balance, int round)
    {
        if (round < balance.MaxRounds / 3)
            setUniqueBossForNormalRound(runData, balance, round);
        else
        {
            setUniqueBossForEndlessRound(runData, balance, round);
        }
    }

    private static void setUniqueBossForEndlessRound(RunData runData, Balance balance, int round)
    {
        Span<int> endlessBosses = stackalloc int[balance.BossBalance.NumBosses];
        int endlessBossCount = 0;

        for (int i = 0; i < balance.BossBalance.NumBosses; i++)
        {
            if (balance.BossBalance.EndlessMode[i])
            {
                bool bossExists = false;
                for (int r = 0; r < runData.BossType.Length; r++)
                {
                    if (runData.BossType[r] == i)
                    {
                        bossExists = true;
                        break;
                    }
                }

                if (!bossExists)
                    endlessBosses[endlessBossCount++] = i;
            }
        }
        int randomBossIdx = CustomRandInt(ref runData.BossSeed) % endlessBossCount;
        int roundIndex = round % runData.BossType.Length;
        runData.BossType[roundIndex] = endlessBosses[randomBossIdx];
    }

    public static void setUniqueBossForNormalRound(RunData runData, Balance balance, int round)
    {
        // if (runData.BossType[round - 1] > -1)
        //     Debug.Log("Prev boss runData.BossType[" + (round - 1) + "] " + balance.BossBalance.Description[runData.BossType[round - 1]]);

        Span<int> bossesForRound = stackalloc int[balance.BossBalance.NumBosses];
        int bossesForRoundCount = 0;
        for (int i = 0; i < balance.BossBalance.NumBosses; i++)
            if (
                round >= balance.BossBalance.LevelRange[i].x - 1
                && round <= balance.BossBalance.LevelRange[i].y - 1
            )
                bossesForRound[bossesForRoundCount++] = i;

        int numBosses = runData.BossType.Length;
        int count = 0;
        for (int bfrIdx = 0; bfrIdx < bossesForRoundCount; bfrIdx++)
        {
            bool bossAlreadyPicked = false;
            for (int bossIdx = 0; bossIdx < numBosses; bossIdx++)
            {
                if (runData.BossType[bossIdx] == bossesForRound[bfrIdx])
                    bossAlreadyPicked = true;
            }
            if (!bossAlreadyPicked)
                bossesForRound[count++] = bossesForRound[bfrIdx];
        }
        bossesForRoundCount = count;

        // get random boss
        int randomIdx = CustomRandInt(ref runData.BossSeed) % bossesForRoundCount;
        runData.BossType[round] = bossesForRound[randomIdx];

        int bossType = runData.BossType[round];
        // Debug.Log("setUniqueBossForRound round " + round + " bossesForRoundCount " + bossesForRoundCount + " runData.BossType[" + (round - 1) + "] " + bossType + " " + balance.BossBalance.Description[bossType] + " min " + balance.BossBalance.LevelRange[bossType].x + " max " + balance.BossBalance.LevelRange[bossType].y);
    }

    public static int GetBossTypeForRound(RunData runData)
    {
        return GetBossTypeForRound(runData, runData.Round);
    }

    public static int GetBossTypeForRound(RunData runData, int round)
    {
        int bossIdx = (round / 3) % runData.BossType.Length;

        int bossType = runData.BossType[bossIdx];

        return bossType;
    }

    public static bool TryUseBossRerolls(RunData runData, Balance balance)
    {
        if (runData.BossRerolls > 0)
        {
            runData.BossRerolls--;
            RerollBoss(runData, balance);
            return true;
        }
        return false;
    }

    public static void RerollBoss(RunData runData, Balance balance)
    {
        setUniqueBossForRound(runData, balance, runData.Round / 3);
    }

    public static void SetDataForNextRound(
        RunData runData,
        Balance balance,
        int[] affectedSlotsIdxs,
        ref int affectedSlotsCount
    )
    {
        affectedSlotsCount = 0;

        runData.CurrentSpin = 0;
        runData.TotalChips = 0.0d;

        runData.ShopRerollCount = 0;
        runData.CardPackRerollCount = 0;

        int spinsChange = 0;
        for (int jkrIdx = 0; jkrIdx < runData.JokerCount; jkrIdx++)
        {
            int jokerType = runData.JokerTypes[jkrIdx];
            spinsChange += balance.JokerBalance.AddSpin[jokerType];
        }

        for (int i = 0; i < balance.NumSlots; i++)
            runData.UseSlot[i] = 1;

        for (int i = 0; i < balance.MaxJokersInHand; i++)
            runData.UseJoker[i] = 1;

        for (int slotIdx = 0; slotIdx < balance.NumSlots; slotIdx++)
        {
            runData.SlotTypeInGame[slotIdx] = runData.SlotType[slotIdx];
        }

        for (int i = 0; i < runData.BallTypes.Length; i++)
            runData.BallTypesInGame[i] = runData.BallTypes[i];

        runData.UseBaseChips = 1;
        runData.UseBallsSpecial = 1;
        for (int i = 0; i < (int)SLOT_TYPE.LAST; i++)
            SetFlag(ref runData.UseSlotBuffs, i);

        if (InBossRound(runData))
        {
            int bossType = GetBossTypeForRound(runData);
            if (balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.ONE_LESS_SPIN)
                spinsChange -= 1;

            // TODO move to StartSpinBossEffect

            if (
                balance.BossBalance.BossEffect[bossType] >= BOSS_EFFECT.ONLY_RED
                && balance.BossBalance.BossEffect[bossType] <= BOSS_EFFECT.ONLY_BLUE
            )
            {
                int colorIdx =
                    (int)balance.BossBalance.BossEffect[bossType] - (int)BOSS_EFFECT.ONLY_RED;
                onlyUseOneSlotColor(
                    runData,
                    balance,
                    colorIdx,
                    affectedSlotsIdxs,
                    ref affectedSlotsCount
                );
            }

            if (
                balance.BossBalance.BossEffect[bossType] >= BOSS_EFFECT.ONLY_RED_FIRST_SPIN
                && balance.BossBalance.BossEffect[bossType] <= BOSS_EFFECT.ONLY_BLUE_FIRST_SPIN
            )
            {
                int colorIdx =
                    (int)balance.BossBalance.BossEffect[bossType]
                    - (int)BOSS_EFFECT.ONLY_RED_FIRST_SPIN;
                onlyUseOneSlotColor(
                    runData,
                    balance,
                    colorIdx,
                    affectedSlotsIdxs,
                    ref affectedSlotsCount
                );
            }

            if (
                balance.BossBalance.BossEffect[bossType] >= BOSS_EFFECT.NO_RED
                && balance.BossBalance.BossEffect[bossType] <= BOSS_EFFECT.NO_BLUE
            )
            {
                int colorIdx =
                    (int)balance.BossBalance.BossEffect[bossType] - (int)BOSS_EFFECT.NO_RED;
                turnOffOneColor(
                    runData,
                    balance,
                    colorIdx,
                    affectedSlotsIdxs,
                    ref affectedSlotsCount
                );
            }

            if (balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.ONLY_RED_GREEN)
            {
                turnOffOneColor(
                    runData,
                    balance,
                    (int)SLOT_TYPE.ORANGE,
                    affectedSlotsIdxs,
                    ref affectedSlotsCount
                );
                turnOffOneColor(
                    runData,
                    balance,
                    (int)SLOT_TYPE.BLUE,
                    affectedSlotsIdxs,
                    ref affectedSlotsCount
                );
            }

            if (balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.ONLY_RED_ORANGE)
            {
                turnOffOneColor(
                    runData,
                    balance,
                    (int)SLOT_TYPE.GREEN,
                    affectedSlotsIdxs,
                    ref affectedSlotsCount
                );
                turnOffOneColor(
                    runData,
                    balance,
                    (int)SLOT_TYPE.BLUE,
                    affectedSlotsIdxs,
                    ref affectedSlotsCount
                );
            }
            if (balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.ONLY_GREEN_BLUE)
            {
                turnOffOneColor(
                    runData,
                    balance,
                    (int)SLOT_TYPE.RED,
                    affectedSlotsIdxs,
                    ref affectedSlotsCount
                );
                turnOffOneColor(
                    runData,
                    balance,
                    (int)SLOT_TYPE.ORANGE,
                    affectedSlotsIdxs,
                    ref affectedSlotsCount
                );
            }
            if (balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.ONLY_BLUE_ORANGE)
            {
                turnOffOneColor(
                    runData,
                    balance,
                    (int)SLOT_TYPE.RED,
                    affectedSlotsIdxs,
                    ref affectedSlotsCount
                );
                turnOffOneColor(
                    runData,
                    balance,
                    (int)SLOT_TYPE.GREEN,
                    affectedSlotsIdxs,
                    ref affectedSlotsCount
                );
            }

            if (balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.ONLY_PLAY_MOST_USED_COLOR)
            {
                int colorIdx = (int)GetMostPlayedSlotType(runData);
                onlyUseOneSlotColor(
                    runData,
                    balance,
                    colorIdx,
                    affectedSlotsIdxs,
                    ref affectedSlotsCount
                );
            }

            if (balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.DISABLE_MOST_USED_COLOR)
            {
                int colorIdx = (int)GetMostPlayedSlotType(runData);
                turnOffOneColor(
                    runData,
                    balance,
                    colorIdx,
                    affectedSlotsIdxs,
                    ref affectedSlotsCount
                );
            }

            if (
                balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.JUMBLE_SLOTS
                || balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.JUMBLE_SLOTS_FIRST_SPIN
                || balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.JUMBLE_SLOTS_EVERY_SPIN
            )
            {
                for (int slotIdx = 0; slotIdx < balance.NumSlots; slotIdx++)
                {
                    runData.SlotTypeInGame[slotIdx] = runData.SlotType[slotIdx];
                    affectedSlotsIdxs[affectedSlotsCount++] = slotIdx;
                }

                ShuffleSlotTypeArray(ref runData.GameSeed, runData.SlotTypeInGame);
            }

            if (
                balance.BossBalance.BossEffect[bossType]
                    == BOSS_EFFECT.CHANGE_SLOTS_INTO_GROUPS_OF_THREE
                || balance.BossBalance.BossEffect[bossType]
                    == BOSS_EFFECT.CHANGE_SLOTS_INTO_GROUPS_OF_THREE_FIRST_SPIN
            )
            {
                for (int slotIdx = 0; slotIdx < balance.NumSlots; slotIdx++)
                {
                    int slotType = (slotIdx / 3) % 4;
                    runData.SlotTypeInGame[slotIdx] = (SLOT_TYPE)slotType;
                    affectedSlotsIdxs[affectedSlotsCount++] = slotIdx;
                }
            }

            if (
                balance.BossBalance.BossEffect[bossType]
                    == BOSS_EFFECT.CHANGE_SLOTS_INTO_GROUPS_OF_TWO
                || balance.BossBalance.BossEffect[bossType]
                    == BOSS_EFFECT.CHANGE_SLOTS_INTO_GROUPS_OF_TWO_FIRST_SPIN
            )
            {
                for (int slotIdx = 0; slotIdx < balance.NumSlots; slotIdx++)
                {
                    int slotType = (slotIdx / 2) % 4;
                    runData.SlotTypeInGame[slotIdx] = (SLOT_TYPE)slotType;
                    affectedSlotsIdxs[affectedSlotsCount++] = slotIdx;
                }
            }

            if (
                balance.BossBalance.BossEffect[bossType]
                    == BOSS_EFFECT.CHANGE_SLOTS_TO_ALTERNATING_COLORS
                || balance.BossBalance.BossEffect[bossType]
                    == BOSS_EFFECT.CHANGE_SLOTS_TO_ALTERNATING_COLORS_FIRST_SPIN
            )
            {
                for (int slotIdx = 0; slotIdx < balance.NumSlots; slotIdx++)
                {
                    runData.SlotTypeInGame[slotIdx] = (SLOT_TYPE)(slotIdx % 4);
                    affectedSlotsIdxs[affectedSlotsCount++] = slotIdx;
                }
            }

            if (balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.SWAP_COLORS)
            {
                SLOT_TYPE mostPlayedType = GetMostPlayedSlotType(runData);
                SLOT_TYPE leastPlayedType = GetLeastPlayedSlotType(runData);

                for (int slotIdx = 0; slotIdx < balance.NumSlots; slotIdx++)
                {
                    if (runData.SlotType[slotIdx] == mostPlayedType)
                    {
                        runData.SlotTypeInGame[slotIdx] = leastPlayedType;
                        affectedSlotsIdxs[affectedSlotsCount++] = slotIdx;
                    }
                    else if (runData.SlotType[slotIdx] == leastPlayedType)
                    {
                        runData.SlotTypeInGame[slotIdx] = mostPlayedType;
                        affectedSlotsIdxs[affectedSlotsCount++] = slotIdx;
                    }
                }
            }

            if (balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.JUMBLE_SLOT_EFFECTS)
            {
                for (int slotIdx = 0; slotIdx < balance.NumSlots; slotIdx++)
                {
                    int slotType = runData.SlotModType[slotIdx];
                    int randomIndex = CustomRandInt(ref runData.BossSeed) % balance.NumSlots;
                    runData.SlotModType[slotIdx] = runData.SlotModType[randomIndex];
                    runData.SlotModType[randomIndex] = slotType;
                }

                for (int slotIdx = 0; slotIdx < balance.NumSlots; slotIdx++)
                    if (runData.SlotModType[slotIdx] > -1)
                        affectedSlotsIdxs[affectedSlotsCount++] = slotIdx;
            }

            if (balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.SLOTS_DEBUFFED_FIRST_SPIN)
            {
                runData.UseSlotBuffs = 0;

                for (int slotIdx = 0; slotIdx < balance.NumSlots; slotIdx++)
                    if (runData.SlotModType[slotIdx] > -1)
                        affectedSlotsIdxs[affectedSlotsCount++] = slotIdx;
            }
        } // in boss round

        spinsChange += runData.ExtraSkipSpin + runData.VoucherSpins;
        runData.MaxSpinsThisRound = spinsChange + balance.SpinWheelBalance.Spins[runData.WheelIdx];

        if (InBossRound(runData))
        {
            int bossType = GetBossTypeForRound(runData);
            if (balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.ONLY_ONE_SPIN)
                runData.MaxSpinsThisRound = 1;
        }

        for (int jkrIdx = 0; jkrIdx < runData.JokerCount; jkrIdx++)
        {
            int jokerType = runData.JokerTypes[jkrIdx];
            if (balance.JokerBalance.MultMultButBallsDisabled[jokerType] > 0)
                runData.UseBallsSpecial = 0;

            runData.JokerRounds[jkrIdx]++;
        }

        runData.LeastPlayedColorAtRoundStart = GetLeastPlayedSlotType(runData);

        // check if boss round
        if (InBossRound(runData))
        {
            startBossRound(runData, balance);
        }
    }

    public static bool InBossRound(RunData runData)
    {
        return InBossRound(runData.Round);
    }

    public static bool InBossRound(int round)
    {
        return round % 3 == 2;
    }

    private static void onlyUseOneSlotColor(
        RunData runData,
        Balance balance,
        int colorIdx,
        int[] affectedSlotsIdxs,
        ref int affectedSlotsCount
    )
    {
        for (int slotIdx = 0; slotIdx < balance.NumSlots; slotIdx++)
        {
            int slotType = (int)runData.SlotTypeInGame[slotIdx];
            runData.UseSlot[slotIdx] = slotType == colorIdx ? 1 : 0;

            if (slotType == colorIdx)
                affectedSlotsIdxs[affectedSlotsCount++] = slotIdx;
        }
    }

    private static void turnOffOneColor(
        RunData runData,
        Balance balance,
        int colorIdx,
        int[] affectedSlotsIdxs,
        ref int affectedSlotsCount
    )
    {
        for (int slotIdx = 0; slotIdx < balance.NumSlots; slotIdx++)
        {
            int slotType = (int)runData.SlotTypeInGame[slotIdx];
            if (slotType == colorIdx)
                runData.UseSlot[slotIdx] = 0;

            if (slotType == colorIdx)
                affectedSlotsIdxs[affectedSlotsCount++] = slotIdx;
        }
    }

    static void startBossRound(RunData runData, Balance balance)
    {
        int bossType = GetBossTypeForRound(runData);
        if (
            balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.BALLS_DEBUFFED
            || balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.BALLS_DEBUFFED_FIRST_SPIN
        )
            runData.UseBallsSpecial = 0;

        runData.UseSlotBuffs = GetSlotBuffsForBoss(runData, balance);

        if (balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.NO_BASE_CHIPS)
            runData.UseBaseChips = 0;
    }

    public static int GetSlotBuffsForBoss(RunData runData, Balance balance)
    {
        int useSlotBuffs = 15;

        int bossType = GetBossTypeForRound(runData);

        if (
            balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.SLOTS_DEBUFFED
            || balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.SLOTS_DEBUFFED_FIRST_SPIN
        )
            useSlotBuffs = 0;

        if (
            balance.BossBalance.BossEffect[bossType] >= BOSS_EFFECT.RED_SLOTS_DEBUFFED
            && balance.BossBalance.BossEffect[bossType] <= BOSS_EFFECT.BLUE_SLOTS_DEBUFFED
        )
        {
            int slotType = (int)(
                balance.BossBalance.BossEffect[bossType] - BOSS_EFFECT.RED_SLOTS_DEBUFFED
            );
            useSlotBuffs = RemoveFlag(useSlotBuffs, slotType);
        }

        return useSlotBuffs;
    }

    public static void PostRoundBossEffect(RunData runData, Balance balance, out bool moneyChanged)
    {
        moneyChanged = false;

        int bossType = GetBossTypeForRound(runData);
        BOSS_EFFECT bossEffect = balance.BossBalance.BossEffect[bossType];
        if (bossEffect == BOSS_EFFECT.LOSE_MONEY_EVERY_SPIN)
        {
            runData.Money--;
            moneyChanged = true;
        }

        if (
            bossEffect >= BOSS_EFFECT.LOSE_MONEY_EVERY_BALL_RED
            && bossEffect <= BOSS_EFFECT.LOSE_MONEY_EVERY_BALL_BLUE
        )
            for (int ballIdx = 0; ballIdx < balance.MaxBalls; ballIdx++)
            {
                int slotIdx = runData.BallSlotIdx[ballIdx];
                for (SLOT_TYPE slotType = SLOT_TYPE.RED; slotType < SLOT_TYPE.LAST; slotType++)
                    if (
                        runData.SlotType[slotIdx] == slotType
                        && bossEffect
                            == (BOSS_EFFECT)(
                                (int)BOSS_EFFECT.LOSE_MONEY_EVERY_BALL_RED + (int)slotType
                            )
                    )
                    {
                        runData.Money--;
                        moneyChanged = true;
                    }
            }

        if (bossEffect == BOSS_EFFECT.LOSE_MONEY_EVERY_BALL_MOST_COMMON_COLOR)
        {
            SLOT_TYPE slotType = GetMostPlayedSlotType(runData);
            for (int ballIdx = 0; ballIdx < balance.MaxBalls; ballIdx++)
            {
                int slotIdx = runData.BallSlotIdx[ballIdx];
                if (runData.SlotType[slotIdx] == slotType)
                {
                    runData.Money--;
                    moneyChanged = true;
                }
            }
        }
    }

    public static void StartSpin(RunData runData, Balance balance)
    {
        for (int i = 0; i < balance.NumSlots; i++)
            runData.SlotScored[i] = -1;

        for (int i = 0; i < balance.MaxBalls; i++)
        {
            runData.BallSlotIdx[i] = -1;
            runData.BallSnapVelocity[i] = 0.0f;
            runData.BallSnapTime[i] = 0.0f;
        }

        for (int jkrIdx = 0; jkrIdx < runData.JokerCount; jkrIdx++)
            runData.JokerSpins[jkrIdx]++;

        runData.JokerBallTriggerIdx = 0;
    }

    public static void DropBalls(RunData runData) { }

    public static void JumbleBalls(RunData runData, Balance balance)
    {
        for (int i = 0; i < balance.MaxBalls; i++)
        {
            int ballType = runData.BallTypesInGame[i];
            int randomIndex = CustomRandInt(ref runData.BossSeed) % balance.MaxBalls;
            runData.BallTypesInGame[i] = runData.BallTypesInGame[randomIndex];
            runData.BallTypesInGame[randomIndex] = ballType;
        }
    }

    public static void StartSpinBossEffect(
        RunData runData,
        Balance balance,
        int[] affectedSlotsIdxs,
        ref int affectedSlotsCount
    )
    {
        if (InBossRound(runData))
        {
            int bossType = GetBossTypeForRound(runData);
            if (balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.DIFFERENT_COLOR_EVERY_SPIN)
            {
                uint seed = runData.StartSeed;
                int randomOffset = CustomRandInt(ref seed);
                int colorIdx = (randomOffset + runData.CurrentSpin) % (int)SLOT_TYPE.LAST;
                onlyUseOneSlotColor(
                    runData,
                    balance,
                    colorIdx,
                    affectedSlotsIdxs,
                    ref affectedSlotsCount
                );

                affectedSlotsCount = 0;
                for (int slotIdx = 0; slotIdx < balance.NumSlots; slotIdx++)
                    if (runData.UseSlot[slotIdx] == 1)
                        affectedSlotsIdxs[affectedSlotsCount++] = slotIdx;
            }

            if (
                runData.CurrentSpin == 1
                && (
                    balance.BossBalance.BossEffect[bossType] >= BOSS_EFFECT.ONLY_RED_FIRST_SPIN
                    && balance.BossBalance.BossEffect[bossType] <= BOSS_EFFECT.ONLY_BLUE_FIRST_SPIN
                )
            )
            {
                for (int slotIdx = 0; slotIdx < balance.NumSlots; slotIdx++)
                {
                    runData.UseSlot[slotIdx] = 1;
                    affectedSlotsIdxs[affectedSlotsCount++] = slotIdx;
                }
            }

            if (
                balance.BossBalance.BossEffect[bossType]
                == BOSS_EFFECT.RANDOM_JOKE_DEBUFFED_PER_SPIN
            )
            {
                int prevDebuffedJokerIdx = -1;
                for (int i = 0; i < runData.JokerCount; i++)
                    if (runData.UseJoker[i] == 0)
                        prevDebuffedJokerIdx = i;

                for (int i = 0; i < runData.JokerCount; i++)
                    runData.UseJoker[i] = 1;

                if (runData.JokerCount > 0)
                {
                    int randomJokerIdx = CustomRandInt(ref runData.BossSeed) % runData.JokerCount;
                    if (randomJokerIdx == prevDebuffedJokerIdx)
                        randomJokerIdx = (prevDebuffedJokerIdx + 1) % runData.JokerCount;
                    runData.UseJoker[randomJokerIdx] = 0;

                    Debug.Log("Debuffed joker " + randomJokerIdx);
                }
            }

            if (
                runData.CurrentSpin == 1
                && balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.SLOTS_DEBUFFED_FIRST_SPIN
            )
            {
                runData.UseSlotBuffs = 15;

                for (int slotIdx = 0; slotIdx < balance.NumSlots; slotIdx++)
                    if (runData.SlotModType[slotIdx] > -1)
                        affectedSlotsIdxs[affectedSlotsCount++] = slotIdx;
            }

            if (
                runData.CurrentSpin == 1
                && balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.JUMBLE_SLOTS_FIRST_SPIN
            )
            {
                for (int slotIdx = 0; slotIdx < balance.NumSlots; slotIdx++)
                {
                    runData.SlotTypeInGame[slotIdx] = runData.SlotType[slotIdx];
                    affectedSlotsIdxs[affectedSlotsCount++] = slotIdx;
                }
            }

            if (
                runData.CurrentSpin > 0
                && balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.JUMBLE_SLOTS_EVERY_SPIN
            )
            {
                for (int slotIdx = 0; slotIdx < balance.NumSlots; slotIdx++)
                {
                    runData.SlotTypeInGame[slotIdx] = runData.SlotType[slotIdx];
                    affectedSlotsIdxs[affectedSlotsCount++] = slotIdx;
                }
                ShuffleSlotTypeArray(ref runData.GameSeed, runData.SlotTypeInGame);
            }

            if (
                runData.CurrentSpin == 1
                && balance.BossBalance.BossEffect[bossType]
                    == BOSS_EFFECT.CHANGE_SLOTS_INTO_GROUPS_OF_THREE_FIRST_SPIN
            )
            {
                for (int slotIdx = 0; slotIdx < balance.NumSlots; slotIdx++)
                {
                    runData.SlotTypeInGame[slotIdx] = runData.SlotType[slotIdx];
                    affectedSlotsIdxs[affectedSlotsCount++] = slotIdx;
                }
            }

            if (
                runData.CurrentSpin == 1
                && balance.BossBalance.BossEffect[bossType]
                    == BOSS_EFFECT.CHANGE_SLOTS_INTO_GROUPS_OF_TWO_FIRST_SPIN
            )
            {
                for (int slotIdx = 0; slotIdx < balance.NumSlots; slotIdx++)
                {
                    runData.SlotTypeInGame[slotIdx] = runData.SlotType[slotIdx];
                    affectedSlotsIdxs[affectedSlotsCount++] = slotIdx;
                }
            }

            if (
                runData.CurrentSpin == 1
                && balance.BossBalance.BossEffect[bossType]
                    == BOSS_EFFECT.CHANGE_SLOTS_TO_ALTERNATING_COLORS_FIRST_SPIN
            )
            {
                for (int slotIdx = 0; slotIdx < balance.NumSlots; slotIdx++)
                {
                    runData.SlotTypeInGame[slotIdx] = runData.SlotType[slotIdx];
                    affectedSlotsIdxs[affectedSlotsCount++] = slotIdx;
                }
            }
        }
    }

    public static int JokerPreRoundTryModifySlot(RunData runData, Balance balance, int jokerType)
    {
        int modifiedSlotIdx = -1;

        if (balance.JokerBalance.StartRoundChangeSlotID[jokerType] > -1)
        {
            int availableSlotCount = 0;
            Span<int> avaiableSlots = stackalloc int[balance.NumSlots];

            for (int slotIdx = 0; slotIdx < balance.NumSlots; slotIdx++)
                if (runData.SlotModType[slotIdx] == -1)
                    avaiableSlots[availableSlotCount++] = slotIdx;

            modifiedSlotIdx = avaiableSlots[CustomRandInt(ref runData.GameSeed) % availableSlotCount];
            runData.SlotModType[modifiedSlotIdx] = balance.JokerBalance.StartRoundChangeSlotID[jokerType];
        }
        if (balance.JokerBalance.StartRoundChangeSlotColor[jokerType] != SLOT_TYPE.NONE)
        {
            int availableSlotCount = 0;
            Span<int> avaiableSlots = stackalloc int[balance.NumSlots];

            for (int slotIdx = 0; slotIdx < balance.NumSlots; slotIdx++)
                if (runData.SlotTypeInGame[slotIdx] != balance.JokerBalance.StartRoundChangeSlotColor[jokerType])
                    avaiableSlots[availableSlotCount++] = slotIdx;

            modifiedSlotIdx = avaiableSlots[CustomRandInt(ref runData.GameSeed) % availableSlotCount];
            Debug.Log("Changing slot runData.SlotType[" + modifiedSlotIdx + "] " + runData.SlotType[modifiedSlotIdx] + " runData.SlotTypeInGame[" + modifiedSlotIdx + "] " + runData.SlotTypeInGame[modifiedSlotIdx] + " color to " + balance.JokerBalance.StartRoundChangeSlotColor[jokerType]);
            runData.SlotType[modifiedSlotIdx] = runData.SlotTypeInGame[modifiedSlotIdx] = balance.JokerBalance.StartRoundChangeSlotColor[jokerType];
        }

        return modifiedSlotIdx;
    }

    public static bool BallInSlot(
        RunData runData,
        Balance balance,
        int ballIdx,
        int slotIdx,
        out int slotChangedIdx,
        out int slotChangeJokerIdx,
        out int jokerMultIncIdx,
        out int jokerMultInc
    )
    {
        slotChangedIdx = -1;
        slotChangeJokerIdx = -1;
        jokerMultIncIdx = -1;
        jokerMultInc = 0;

        if (runData.BallSlotIdx[ballIdx] == -1 && runData.SlotScored[slotIdx] == -1)
        {
            runData.SlotScored[slotIdx] = ballIdx;
            runData.BallSlotIdx[ballIdx] = slotIdx;
            runData.BallSnapVelocity[ballIdx] = 0.0f;
            runData.BallSnapTime[ballIdx] = 0.0f;
            int slotType = (int)runData.SlotTypeInGame[slotIdx];
            runData.ColorCount[slotType]++;

            if (runData.CurrentSpin == 0 && ballIdx == 0)
                for (int jokerIdx = 0; jokerIdx < runData.JokerCount; jokerIdx++)
                {
                    int jokerType = runData.JokerTypes[jokerIdx];
                    if (balance.JokerBalance.FirstBallConvertSlotToID[jokerType] > 0)
                    {
                        slotChangedIdx = slotIdx;
                        slotChangeJokerIdx = jokerIdx;
                        runData.SlotModType[slotIdx] = balance
                            .JokerBalance
                            .FirstBallConvertSlotToID[jokerType];
                    }
                }

            for (int jokerIdx = 0; jokerIdx < runData.JokerCount; jokerIdx++)
            {
                int jokerType = runData.JokerTypes[jokerIdx];

                if (
                    runData.SlotModType[slotIdx] > -1
                    && balance.JokerBalance.BallIncMultRemoveSlotMod[jokerType] > 0
                )
                {
                    slotChangedIdx = slotIdx;
                    slotChangeJokerIdx = jokerIdx;
                    runData.SlotModType[slotIdx] = -1;
                    runData.JokerMultiplierAdd[jokerIdx] += balance
                        .JokerBalance
                        .BallIncMultRemoveSlotMod[jokerType];

                    jokerMultIncIdx = jokerIdx;
                    jokerMultInc = balance.JokerBalance.BallIncMultRemoveSlotMod[jokerType];
                }
            }

            return true;
        }
        return false;
    }

    public static bool AllBallsInSlot(RunData runData)
    {
        for (int ballIdx = 0; ballIdx < runData.BallSlotIdx.Length; ballIdx++)
            if (runData.BallSlotIdx[ballIdx] == -1)
                return false;
        return true;
    }

    public static void StartScoring(RunData runData, Balance balance)
    {
        runData.SpinMultiplier = balance.BaseMultiplier;
        runData.SpinChips = 0;

        for (int i = 0; i < (int)(SLOT_TYPE.LAST); i++)
        {
            runData.BallScoresCount[i] = 0;
        }

        int ballCount = 0;
        for (int i = runData.SlotScored.Length - 1; i >= 0; i--)
            if (runData.SlotScored[i] > -1)
                runData.BallScoreIdxs[ballCount++] = runData.SlotScored[i];
    }

    public static double CalculateSlotBallChips(RunData runData, Balance balance, int ballIdx)
    {
        int slotIdx = runData.BallSlotIdx[ballIdx];
        int slotType = (int)runData.SlotTypeInGame[slotIdx];

        runData.BallScoresCount[slotType]++;

        double baseChips = runData.BaseChips[slotType];

        if (InBossRound(runData))
            if (
                balance.BossBalance.BossEffect[GetBossTypeForRound(runData)]
                == BOSS_EFFECT.MOST_PLAYED_BASE_CHIPS_TO_FIVE
            )
                if (GetMostPlayedSlotType(runData) == runData.SlotTypeInGame[slotIdx])
                    baseChips = 5;

        // Debug.Log("slotType " + slotType + " m_ballScoresCount " + runData.BallScoresCount.ToString());
        double chips = runData.BallScoresCount[slotType] * baseChips * runData.UseBaseChips;
        int slotModType = runData.SlotModType[slotIdx];
        if (slotModType > -1)
            chips +=
                balance.CardPackSlotBalance.Chips[slotModType]
                * GetFlag(runData.UseSlotBuffs, slotType);

        chips *= runData.UseSlot[slotIdx];

        if (!scoringBossCheck(runData, balance))
            chips = 0.0d;

        runData.SpinChips += chips;

        return chips;
    }

    public static float CalculateSlotBallMultiplierAdd(
        RunData runData,
        Balance balance,
        int ballIdx
    )
    {
        int slotIdx = runData.BallSlotIdx[ballIdx];
        int slotType = (int)runData.SlotTypeInGame[slotIdx];

        // only add mult if special slot
        // runData.BallMultipliersCount[slotType]++;
        // Debug.Log("slotType " + slotType + " m_ballMultipliersCount " + runData.BallMultipliersCount.ToString());
        float multiplier = 0.0f;
        int slotModType = runData.SlotModType[slotIdx];
        if (slotModType > -1)
        {
            multiplier +=
                balance.CardPackSlotBalance.MultiplierAdd[slotModType]
                * GetFlag(runData.UseSlotBuffs, slotType);
        }

        multiplier *= runData.UseSlot[slotIdx];

        if (!scoringBossCheck(runData, balance))
            multiplier = 0;

        runData.SpinMultiplier += multiplier;

        return multiplier;
    }

    public static int CalculateSlotBallMultiplierMult(RunData runData, Balance balance, int ballIdx)
    {
        int slotIdx = runData.BallSlotIdx[ballIdx];
        int slotType = (int)runData.SlotTypeInGame[slotIdx];
        int slotModType = runData.SlotModType[slotIdx];
        int multiplier = 0;
        if (slotModType > -1)
            multiplier +=
                balance.CardPackSlotBalance.MultiplierMult[slotModType]
                * GetFlag(runData.UseSlotBuffs, slotType);

        multiplier *= runData.UseSlot[slotIdx];

        if (!scoringBossCheck(runData, balance))
            multiplier = 0;

        if (multiplier > 0)
            runData.SpinMultiplier *= multiplier;

        return multiplier;
    }

    public static void AddJoker(RunData runData, Balance balance, int jokerType)
    {
        // remove this joker from the available joker types so we don't show it in the shop again
        int count = 0;
        for (int jkrIdx = 0; jkrIdx < runData.AvailableJokerCount; jkrIdx++)
            if (runData.AvailableJokerTypes[jkrIdx] != jokerType)
                runData.AvailableJokerTypes[count++] = runData.AvailableJokerTypes[jkrIdx];
        runData.AvailableJokerCount = count;

        int jokerIdx = runData.JokerCount;

        runData.JokerTypes[jokerIdx] = jokerType;
        runData.JokerSellValues[jokerIdx] = balance.JokerBalance.Cost[jokerType] / 2;
        runData.JokerChips[jokerIdx] = Mathf.RoundToInt(
            balance.JokerBalance.SubtractChipsPerSpin[jokerType].x
        );
        runData.JokerMultiplierAdd[jokerIdx] = balance
            .JokerBalance
            .SubtractMultiplierAddPerRound[jokerType]
            .x;
        runData.JokerRounds[jokerIdx] = 0;
        runData.JokerSpins[jokerIdx] = 0;
        runData.JokerSkipCount[jokerIdx] = 0;

        runData.JokerCount++;
    }

    public static void RemoveJoker(RunData runData, int jokerRemovedIdx)
    {
        int count = 0;
        for (int jokerIdx = 0; jokerIdx < runData.JokerCount; jokerIdx++)
        {
            if (jokerIdx != jokerRemovedIdx)
            {
                runData.JokerTypes[count] = runData.JokerTypes[jokerIdx];
                runData.JokerSellValues[count] = runData.JokerSellValues[jokerIdx];
                runData.JokerChips[count] = runData.JokerChips[jokerIdx];
                runData.JokerMultiplierAdd[count] = runData.JokerMultiplierAdd[jokerIdx];
                runData.JokerMultiplierMult[count] = runData.JokerMultiplierMult[jokerIdx];
                count++;
            }
        }
        runData.JokerCount = count;
    }

    public static int CalculateBallChips(RunData runData, Balance balance, int ballIdx)
    {
        int chips = Mathf.FloorToInt(
            CalculateBallCommon(
                runData,
                ballIdx,
                balance.BallBalance.BallChips,
                balance.BallBalance.BallColorMultiplier
            )
        );

        if (!scoringBossCheck(runData, balance))
            chips = 0;

        runData.SpinChips += chips;

        return chips;
    }

    public static float CalculateBallMultiplierAdd(RunData runData, Balance balance, int ballIdx)
    {
        float mult = CalculateBallCommon(
            runData,
            ballIdx,
            balance.BallBalance.BallMultiplierAdd,
            balance.BallBalance.BallColorMultiplier
        );

        if (!scoringBossCheck(runData, balance))
            mult = 0;

        runData.SpinMultiplier += mult;

        return mult;
    }

    public static float CalculateBallMultiplierMult(RunData runData, Balance balance, int ballIdx)
    {
        float mult = CalculateBallCommon(
            runData,
            ballIdx,
            balance.BallBalance.BallMultiplierMult,
            balance.BallBalance.BallColorMultiplier
        );

        if (!scoringBossCheck(runData, balance))
            mult = 0;

        if (mult >= 1.0f)
            runData.SpinMultiplier *= mult;

        return mult;
    }

    public static float CalculateBallCommon(
        RunData runData,
        int ballIdx,
        float[] perBallArray,
        float[][] perColorArray
    )
    {
        float value = 0;
        int slotIdx = runData.BallSlotIdx[ballIdx];
        int ballType = runData.BallTypesInGame[ballIdx];
        value += perBallArray[ballType] * runData.UseBallsSpecial;
        int slotType = (int)runData.SlotTypeInGame[slotIdx];
        value = value * perColorArray[ballType][slotType];

        value *= runData.UseSlot[slotIdx];

        return value;
    }

    public static int CalculateBallMoney(
        RunData runData,
        Balance balance,
        int ballIdx,
        Span<int> jokerIdxs,
        ref int jokerCount
    )
    {
        int money = 0;
        int ballType = runData.BallTypesInGame[ballIdx];

        int slotIdx = runData.BallSlotIdx[ballIdx];
        int slotType = (int)runData.SlotTypeInGame[slotIdx];

        money +=
            balance.BallBalance.BallMoney[ballType]
            * runData.UseBallsSpecial
            * runData.UseSlot[slotIdx];

        for (int jkrIdx = 0; jkrIdx < runData.JokerCount; jkrIdx++)
        {
            int jokerType = runData.JokerTypes[jkrIdx];
            if (
                CustomRandFloat(ref runData.GameSeed)
                < balance.JokerBalance.ChanceBallGivesMoney[jokerType]
            )
            {
                jokerIdxs[jokerCount++] = jkrIdx;
                money++;
            }

            if (
                ballType > 0
                && runData.UseSlot[slotIdx] > 0
                && balance.JokerBalance.MoneyForSpecialBallOnColor[jokerType][slotType] > 0
            )
            {
                jokerIdxs[jokerCount++] = jkrIdx;
                money += balance.JokerBalance.MoneyForSpecialBallOnColor[jokerType][slotType];
            }
        }

        if (!scoringBossCheck(runData, balance))
            money = 0;

        runData.Money += money;

        return money;
    }

    public static int CalculateSlotMoney(RunData runData, Balance balance, int ballIdx)
    {
        int money = 0;
        int slotIdx = runData.BallSlotIdx[ballIdx];
        int slotType = (int)runData.SlotTypeInGame[slotIdx];

        int slotModType = runData.SlotModType[slotIdx];
        if (slotModType > -1)
            money +=
                balance.CardPackSlotBalance.Money[slotModType]
                * GetFlag(runData.UseSlotBuffs, slotType);

        money *= runData.UseSlot[slotIdx];

        runData.Money += money;

        return money;
    }

    public static int GetNumNonModedSlots(RunData runData, Balance balance)
    {
        int numNonModedSlots = 0;
        for (int i = 0; i < balance.NumSlots; i++)
            if (runData.SlotModType[i] == -1)
                numNonModedSlots++;
        return numNonModedSlots;
    }

    public static int GetNumModedSlots(RunData runData, Balance balance)
    {
        int numModedSlots = 0;
        for (int i = 0; i < balance.NumSlots; i++)
            if (runData.SlotModType[i] > -1)
                numModedSlots++;
        return numModedSlots;
    }

    static bool scoringBossCheck(RunData runData, Balance balance)
    {
        if (InBossRound(runData))
        {
            Span<int> slotTypeCount = stackalloc int[4];
            CountNumBallsOnSlotType(runData, balance.MaxBalls, slotTypeCount);

            int bossType = GetBossTypeForRound(runData);
            if (balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.ONLY_SCORE_SIX_BALLS)
            {
                bool sizeFound = false;
                for (int slotType = 0; slotType < 4; slotType++)
                    if (slotTypeCount[slotType] == 6)
                        sizeFound = true;
                if (!sizeFound)
                    return false;
            }

            if (
                balance.BossBalance.BossEffect[bossType]
                    == BOSS_EFFECT.ONLY_SCORE_AT_LEAST_TWO_COLORS
                || balance.BossBalance.BossEffect[bossType]
                    == BOSS_EFFECT.ONLY_SCORE_AT_LEAST_THREE_COLORS
            )
            {
                int numTypes = 0;
                for (int slotType = 0; slotType < 4; slotType++)
                    if (slotTypeCount[slotType] > 0)
                        numTypes++;

                if (
                    balance.BossBalance.BossEffect[bossType]
                        == BOSS_EFFECT.ONLY_SCORE_AT_LEAST_TWO_COLORS
                    && numTypes < 2
                )
                    return false;
                if (
                    balance.BossBalance.BossEffect[bossType]
                        == BOSS_EFFECT.ONLY_SCORE_AT_LEAST_THREE_COLORS
                    && numTypes < 3
                )
                    return false;
            }
        }
        return true;
    }

    static bool jokerBaseCheck(RunData runData, Balance balance, int jokerIdx, int jokerType)
    {
        Span<int> slotTypeCount = stackalloc int[4];
        CountNumBallsOnSlotType(runData, balance.MaxBalls, slotTypeCount);

        bool useBaseType = checkTypeReqs(runData, balance, jokerType, slotTypeCount);
        bool useBaseNotExist = checkTypeNotExistReqs(runData, balance, jokerType, slotTypeCount);
        bool useBaseSize = checkSizeReqs(runData, balance, jokerType, slotTypeCount);
        bool numTypesOk = checkNumTypes(runData, balance, jokerType, slotTypeCount);

        return (numTypesOk && useBaseType && useBaseSize && useBaseNotExist);
    }

    public static int CheckJokerRetriggerBalls(RunData runData, Balance balance)
    {
        for (int jokerIdx = runData.JokerBallTriggerIdx; jokerIdx < runData.JokerCount; jokerIdx++)
        {
            int jokerType = runData.JokerTypes[jokerIdx];
            if (
                balance.JokerBalance.RetriggerBallsEverySpin[jokerType]
                || (
                    runData.CurrentSpin + 1 == runData.MaxSpinsThisRound
                    && balance.JokerBalance.RetriggerBallsLastSpin[jokerType]
                )
            )
            {
                runData.JokerBallTriggerIdx = jokerIdx + 1;
                return jokerIdx;
            }
        }
        return -1;
    }

    public static double CalculateJokerChipsAdd(
        RunData runData,
        Balance balance,
        int jokerIdx,
        int jokerType
    )
    {
        double chips = 0;

        Span<int> slotTypeCount = stackalloc int[4];
        CountNumBallsOnSlotType(runData, balance.MaxBalls, slotTypeCount);

        if (jokerBaseCheck(runData, balance, jokerIdx, jokerType))
        {
            chips += balance.JokerBalance.BaseChipsAdd[jokerType];

            runData.JokerChips[jokerIdx] += (int)
                balance.JokerBalance.ChipsIncreasePerSpin[jokerType];

            for (int slotType = 0; slotType < 4; slotType++)
                if (IsFlagSet(balance.JokerBalance.TypeExists[jokerType], slotType))
                    runData.JokerChips[jokerIdx] +=
                        balance.JokerBalance.ChipsIncreasePerBall[jokerType]
                        * slotTypeCount[slotType];

            // add chips per ball
            for (int slotType = 0; slotType < 4; slotType++)
                if (IsFlagSet(balance.JokerBalance.TypeExists[jokerType], slotType))
                    chips += slotTypeCount[slotType] * balance.JokerBalance.ChipsPerBall[jokerType];

            chips += (int)GetValueForTriggerSpins(
                runData,
                balance.JokerBalance.ChipsIncreasePerXSpins[jokerType],
                jokerIdx
            );
        }

        chips += balance.JokerBalance.ChipsPerDollar[jokerType] * runData.Money;

        chips += runData.JokerChips[jokerIdx];

        int numNonSlotMods = GetNumNonModedSlots(runData, balance);
        chips += numNonSlotMods * balance.JokerBalance.ChipsAddForEveryNonSlotMod[jokerType];

        chips +=
            runData.JokerSkipCount[jokerIdx] * balance.JokerBalance.RoundSkippedChipsAdd[jokerType];

        chips +=
            balance.JokerBalance.ChipsAddForCardPackAbandon[jokerType]
            * runData.CardPackAbandonTotal;

        chips *= runData.UseJoker[jokerIdx];

        if (!scoringBossCheck(runData, balance))
            chips = 0.0d;

        // Debug.Log("jokerType " + jokerType + " chips " + chips);

        runData.SpinChips += chips;

        return chips;
    }

    public static double CalculateJokerMultiplierAdd(
        RunData runData,
        Balance balance,
        int jokerIdx,
        int jokerType
    )
    {
        double mult = 0.0d;

        Span<int> slotTypeCount = stackalloc int[4];
        CountNumBallsOnSlotType(runData, balance.MaxBalls, slotTypeCount);

        if (jokerBaseCheck(runData, balance, jokerIdx, jokerType))
        {
            mult += balance.JokerBalance.BaseMultiplierAdd[jokerType];
            runData.JokerMultiplierAdd[jokerIdx] += balance.JokerBalance.MultIncreaseForSize[
                jokerType
            ];

            if (runData.CurrentSpin + 1 == runData.MaxSpinsThisRound)
                mult += balance.JokerBalance.LastSpinMultiplierAdd[jokerType];

            for (int slotType = 0; slotType < 4; slotType++)
                if (IsFlagSet(balance.JokerBalance.TypeExists[jokerType], slotType))
                    mult +=
                        slotTypeCount[slotType] * balance.JokerBalance.MultAddPerBall[jokerType];

            for (int slotType = 0; slotType < 4; slotType++)
                if (IsFlagSet(balance.JokerBalance.TypeExists[jokerType], slotType))
                    runData.JokerMultiplierAdd[jokerIdx] +=
                        slotTypeCount[slotType]
                        * balance.JokerBalance.MultAddIncreasePerBall[jokerType];

            mult += GetValueForTriggerSpins(
                runData,
                balance.JokerBalance.MultAddIncreasePerXSpins[jokerType],
                jokerIdx
            );
        }

        int numNoJokers = runData.MaxJokersInHand - runData.JokerCount;
        mult += balance.JokerBalance.PerJokerMultiplierAdd[jokerType] * runData.JokerCount;
        mult += balance.JokerBalance.PerNoJokerMultiplierAdd[jokerType] * numNoJokers;

        mult += runData.JokerMultiplierAdd[jokerIdx];

        float randomValue = CustomRandFloatRange(
            ref runData.GameSeed,
            balance.JokerBalance.MultiplierAddRandomRange[jokerType].x,
            balance.JokerBalance.MultiplierAddRandomRange[jokerType].y + 1.0f
        );
        mult += Mathf.Floor(randomValue);

        int numSlotMods = GetNumModedSlots(runData, balance);
        mult += numSlotMods * balance.JokerBalance.MultiplierAddForEverySlotMod[jokerType];

        mult +=
            runData.JokerSkipCount[jokerIdx]
            * balance.JokerBalance.RoundSkippedMultiplierAdd[jokerType];

        int numBallsInModedSlots = 0;
        for (int ballIdx = 0; ballIdx < balance.MaxBalls; ballIdx++)
        {
            int slotIdx = runData.BallSlotIdx[ballIdx];
            if (runData.SlotModType[slotIdx] > -1)
                numBallsInModedSlots++;
        }
        mult += balance.JokerBalance.BallMultiplierAddForSlotMod[jokerType] * numBallsInModedSlots;

        SLOT_TYPE leastPlayedSlotType = runData.LeastPlayedColorAtRoundStart;
        for (int ballIdx = 0; ballIdx < balance.MaxBalls; ballIdx++)
        {
            int slotIdx = runData.BallSlotIdx[ballIdx];
            if (leastPlayedSlotType == runData.SlotType[slotIdx])
                mult += balance.JokerBalance.MultiplierAddForLeastPlayedColor[jokerType];
        }

        if (balance.JokerBalance.AddAllSellValueToMult[jokerType])
        {
            int totalSellValue = 0;
            for (int jkrIdx = 0; jkrIdx < runData.JokerCount; jkrIdx++)
                totalSellValue += runData.JokerSellValues[jkrIdx];
            mult += totalSellValue;
        }

        // branchless use or don't use
        mult *= runData.UseJoker[jokerIdx];

        if (!scoringBossCheck(runData, balance))
            mult = 0;

        // Debug.Log("jokerType " + jokerType + " mult " + mult);

        runData.SpinMultiplier += mult;

        return mult;
    }

    public static float GetValueForTriggerSpins(RunData runData, Vector2 data, int jokerIdx)
    {
        int numTriggerSpins = Mathf.FloorToInt(runData.JokerSpins[jokerIdx] / data.y);
        return (float)numTriggerSpins * data.x;
    }

    public static double CalculateJokerMultiplierMult(
        RunData runData,
        Balance balance,
        int jokerIdx,
        int jokerType
    )
    {
        double mult = 0.0f;

        // Debug.Log("jokerType " + jokerType + " mult " + mult);

        Span<int> slotTypeCount = stackalloc int[4];
        CountNumBallsOnSlotType(runData, balance.MaxBalls, slotTypeCount);

        if (jokerBaseCheck(runData, balance, jokerIdx, jokerType))
        {
            mult += balance.JokerBalance.BaseMultiplierMult[jokerType];

            for (int slotType = 0; slotType < 4; slotType++)
                if (IsFlagSet(balance.JokerBalance.TypeExists[jokerType], slotType))
                    mult +=
                        slotTypeCount[slotType] * balance.JokerBalance.MultMultPerBall[jokerType];

            for (int slotType = 0; slotType < 4; slotType++)
                if (IsFlagSet(balance.JokerBalance.TypeExists[jokerType], slotType))
                    runData.JokerMultiplierMult[jokerIdx] +=
                        slotTypeCount[slotType]
                        * balance.JokerBalance.MultMultIncreasePerBall[jokerType];

            mult += GetValueForTriggerSpins(
                runData,
                balance.JokerBalance.MultMultIncreasePerXSpins[jokerType],
                jokerIdx
            );
        }

        mult += runData.JokerMultiplierMult[jokerIdx];

        int numSpecialBalls = 0;
        for (int ballIdx = 0; ballIdx < balance.MaxBalls; ballIdx++)
            if (runData.BallTypesInGame[ballIdx] > 0)
                numSpecialBalls++;

        if (runData.JokerCount == 1)
            mult += balance.JokerBalance.NoJokersMultMult[jokerType];

        mult += balance.JokerBalance.MultiplierMultForSpecialBall[jokerType] * numSpecialBalls;
        mult +=
            balance.JokerBalance.MultiplierMultForNonSpecialBall[jokerType]
            * (balance.MaxBalls - numSpecialBalls);
        mult +=
            balance.JokerBalance.MultiplierMultEveryShopReroll[jokerType] * runData.ShopRerollTotal;
        mult +=
            balance.JokerBalance.MultiplierMultEveryCardPackReroll[jokerType]
            * runData.CardPackRerollTotal;
        mult += balance.JokerBalance.MultMultButBallsDisabled[jokerType];
        mult +=
            balance.JokerBalance.MultiplierMultForCardPackAbandon[jokerType]
            * runData.CardPackAbandonTotal;

        mult +=
            runData.JokerSkipCount[jokerIdx]
            * balance.JokerBalance.RoundSkippedMultiplierMult[jokerType];

        mult *= runData.UseJoker[jokerIdx];

        mult += 1.0d;

        if (!scoringBossCheck(runData, balance))
            mult = 0;

        if (mult > 1.0d)
            runData.SpinMultiplier *= mult;

        return mult;
    }

    private static bool checkTypeReqs(
        RunData runData,
        Balance balance,
        int jokerType,
        Span<int> slotTypeCount
    )
    {
        bool use = false;
        for (int slotType = 0; slotType < 4; slotType++)
            if (
                slotTypeCount[slotType] > 0
                && IsFlagSet(balance.JokerBalance.TypeExists[jokerType], slotType)
            )
                use = true;
        return use;
    }

    private static bool checkTypeNotExistReqs(
        RunData runData,
        Balance balance,
        int jokerType,
        Span<int> slotTypeCount
    )
    {
        bool use = true;
        for (int slotType = 0; slotType < 4; slotType++)
            if (
                slotTypeCount[slotType] > 0
                && IsFlagSet(balance.JokerBalance.TypeNotExists[jokerType], slotType)
            )
                use = false;
        return use;
    }

    public static bool checkSizeReqs(
        RunData runData,
        Balance balance,
        int jokerType,
        Span<int> slotTypeCount
    )
    {
        bool use = true;
        for (int size = 0; size < 6; size++)
        {
            if (IsFlagSet(balance.JokerBalance.SizeExists[jokerType], size))
            {
                bool sizeFound = false;
                for (int slotType = 0; slotType < 4; slotType++)
                    if (slotTypeCount[slotType] == (size + 1))
                        sizeFound = true;
                if (!sizeFound)
                    use = false;
            }

            if (IsFlagSet(balance.JokerBalance.SizeNotExists[jokerType], size))
            {
                bool sizeFound = false;
                for (int slotType = 0; slotType < 4; slotType++)
                    if (slotTypeCount[slotType] == (size + 1))
                        sizeFound = true;
                if (sizeFound)
                    use = false;
            }
        }
        return use;
    }

    private static bool checkNumTypes(
        RunData runData,
        Balance balance,
        int jokerType,
        Span<int> slotTypeCount
    )
    {
        bool numTypesOk;
        int numTypes = 0;
        for (int slotType = 0; slotType < 4; slotType++)
            if (slotTypeCount[slotType] > 0)
                numTypes++;
        numTypesOk = numTypes >= balance.JokerBalance.MinTypes[jokerType];
        return numTypesOk;
    }

    public static void CountNumBallsOnSlotType(
        RunData runData,
        int maxBalls,
        Span<int> slotTypeCount
    )
    {
        for (int i = 0; i < (int)SLOT_TYPE.LAST; i++)
            slotTypeCount[i] = 0;

        for (int ballIdx = 0; ballIdx < maxBalls; ballIdx++)
        {
            int slotIdx = runData.BallSlotIdx[ballIdx];
            int slotType = (int)runData.SlotTypeInGame[slotIdx];
            if (runData.UseSlot[slotIdx] == 1)
                slotTypeCount[slotType]++;
        }
    }

    public static double CalculateTotalScore(RunData runData)
    {
        double roundTotalScore = runData.SpinChips * runData.SpinMultiplier;

        if (runData.BestSpin < roundTotalScore)
            runData.BestSpin = roundTotalScore;
        runData.TotalChips += roundTotalScore;

        return roundTotalScore;
    }

    public static void JokerPostSpin(RunData runData, Balance balance)
    {
        for (int jokerIdx = 0; jokerIdx < runData.JokerCount; jokerIdx++)
        {
            int jokerType = runData.JokerTypes[jokerIdx];
            runData.JokerChips[jokerIdx] -= Mathf.RoundToInt(
                balance.JokerBalance.SubtractChipsPerSpin[jokerType].y
            );
            if (runData.JokerChips[jokerIdx] < 0)
                runData.JokerChips[jokerIdx] = 0;

            runData.Money += balance.JokerBalance.MoneyPerSpin[jokerType];
        }
    }

    public static void SpinComplete(
        RunData runData,
        Balance balance,
        int[] affectedSlotsIdxs,
        ref int affectedSlotsCount
    )
    {
        affectedSlotsCount = 0;
        runData.CurrentSpin++;
        runData.TotalSpins++;
        runData.SpinsUsed++;

        if (InBossRound(runData))
        {
            int bossType = GetBossTypeForRound(runData);
            if (
                balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.BALLS_DEBUFFED_FIRST_SPIN
                && runData.CurrentSpin > 0
            )
                runData.UseBallsSpecial = 1;

            if (balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.PLAYED_SLOTS_DISABLED)
            {
                for (int slotIdx = 0; slotIdx < balance.NumSlots; slotIdx++)
                    if (runData.SlotScored[slotIdx] > -1)
                    {
                        runData.UseSlot[slotIdx] = 0;
                        affectedSlotsIdxs[affectedSlotsCount++] = slotIdx;
                    }
            }
            if (balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.PLAYED_COLORS_DISABLED)
            {
                for (int slotIdx = 0; slotIdx < balance.NumSlots; slotIdx++)
                    if (runData.SlotScored[slotIdx] > -1)
                    {
                        SLOT_TYPE slotType = runData.SlotType[slotIdx];
                        for (int slotIdx2 = 0; slotIdx2 < balance.NumSlots; slotIdx2++)
                            if (runData.SlotType[slotIdx2] == slotType)
                            {
                                runData.UseSlot[slotIdx2] = 0;

                                bool alreadyExists = false;
                                for (int i = 0; i < affectedSlotsCount; i++)
                                    if (affectedSlotsIdxs[i] == slotIdx2)
                                        alreadyExists = true;
                                if (!alreadyExists)
                                    affectedSlotsIdxs[affectedSlotsCount++] = slotIdx2;
                            }
                    }
            }
        }
    }

    public static double Term(int n)
    {
        int k = (n - 1) >> 1; // floor((n-1)/2)
        int isEven = (n & 1) ^ 1; // 1 if n is even, 0 if odd   (branchless)

        // factor = 1 for odd, 2.5 for even
        double factor = 1.0 + 1.5 * isEven;

        return 1_000_000.0 * Math.Pow(10.0, k) * factor;
    }

    public static double GetRoundGoal(RunData runData, Balance balance)
    {
        return GetRoundGoal(runData, balance, runData.Round / 3, runData.Round % 3);
    }

    public static double GetRoundGoal(
        RunData runData,
        Balance balance,
        int bigRound,
        int smallRound
    )
    {
        double goal = GetGoalForRound(runData.WheelIdx, balance, bigRound, smallRound);

        if (InBossRound(runData))
        {
            int bossType = GetBossTypeForRound(runData);
            if (balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.DOUBLE_GOAL)
                goal *= 2;
        }
        return goal;
    }

    public static double GetGoalForRound(
        int wheelIdx,
        Balance balance,
        int bigRound,
        int smallRound
    )
    {
        double goal;
        if (bigRound >= balance.RoundBaseChips.Length)
        {
            int diff = bigRound - balance.RoundBaseChips.Length + 1;
            int k = (diff - 1) >> 1; // floor((n-1)/2)
            int isEven = (diff & 1) ^ 1; // 1 if n is even, 0 if odd   (branchless)

            // factor = 1 for odd, 2.5 for even
            double factor = 1.0 + 1.5 * isEven;

            goal = 1_000_000.0 * Math.Pow(10.0, k) * factor;
        }
        else
            goal = balance.RoundBaseChips[bigRound];
        return goal
            * balance.RoundChipMult[smallRound]
            * balance.SpinWheelBalance.GoalMultiplier[wheelIdx];
    }

    public static bool CheckRoundComplete(RunData runData, Balance balance)
    {
        double goal = GetRoundGoal(runData, balance);
        return (runData.TotalChips >= goal);
    }

    public static bool CheckGameOver(RunData runData)
    {
        return runData.CurrentSpin >= runData.MaxSpinsThisRound;
    }

    public static bool CheckWin(RunData runData, Balance balance)
    {
        return CheckRoundComplete(runData, balance) && runData.Round == balance.MaxRounds - 1;
    }

    public static void WinGame(GameData gameData, RunData runData)
    {
        gameData.SpinWheelWinCount[runData.WheelIdx]++;
    }

    public static void GameOver(GameData gameData)
    {
        // do nothing
    }

    public static void SetMenuState(RunData runData, MENU_STATE newMenuState)
    {
        runData.PrevMenuState = runData.MenuState;
        runData.MenuState = newMenuState;
    }

    public static void RoundComplete(RunData runData, Balance balance)
    {
        for (int jokerIdx = 0; jokerIdx < runData.JokerCount; jokerIdx++)
        {
            int jokerType = runData.JokerTypes[jokerIdx];
            runData.JokerMultiplierAdd[jokerIdx] +=
                (int)balance.JokerBalance.MultIncreasePerUnusedSpin[jokerType]
                * (runData.MaxSpinsThisRound - runData.CurrentSpin);
            runData.JokerMultiplierAdd[jokerIdx] +=
                (int)balance.JokerBalance.MultIncreasePerUsedSpin[jokerType] * runData.CurrentSpin;

            runData.JokerMultiplierAdd[jokerIdx] -= balance
                .JokerBalance
                .SubtractMultiplierAddPerRound[jokerType]
                .y;
            if (runData.JokerMultiplierAdd[jokerIdx] < 0)
                runData.JokerMultiplierAdd[jokerIdx] = 0;
        }

        runData.SpinsUnused += runData.MaxSpinsThisRound - runData.CurrentSpin;

        int moneyFromJokers = GetRoundCompleteMoneyFromJokers(runData, balance);
        runData.Money += moneyFromJokers;

        for (int jkrIdx = 0; jkrIdx < runData.JokerCount; jkrIdx++)
        {
            int jokerType = runData.JokerTypes[jkrIdx];
            runData.JokerSellValues[jkrIdx] += balance.JokerBalance.IncreaseSellValueEveryRound[
                jokerType
            ];
            runData.UseJoker[jkrIdx] = 1;
        }

        int sellValueAddition = 0;
        for (int jkrIdx = 0; jkrIdx < runData.JokerCount; jkrIdx++)
        {
            int jokerType = runData.JokerTypes[jkrIdx];
            sellValueAddition += balance.JokerBalance.IncreaseSellValueAllJokersEveryRound[
                jokerType
            ];
        }
        for (int jkrIdx = 0; jkrIdx < runData.JokerCount; jkrIdx++)
            runData.JokerSellValues[jkrIdx] += sellValueAddition;

        if (InBossRound(runData))
            runData.Money += runData.MoneyAfterBoss;

        runData.ExtraSkipSpin = 0;
        runData.LeastPlayedColorAtRoundStart = GetLeastPlayedSlotType(runData);
    }

    public static int GetRoundCompleteMoneyFromJokers(RunData runData, Balance balance)
    {
        int moneyFromJokers = 0;
        for (int jkrIdx = 0; jkrIdx < runData.JokerCount; jkrIdx++)
        {
            int jokerType = runData.JokerTypes[jkrIdx];
            moneyFromJokers += balance.JokerBalance.EarnMoneyEveryRound[jokerType];
        }
        return moneyFromJokers;
    }

    public static int GetInterestForRound(RunData runData, Balance balance)
    {
        int interestIncrease = 0;
        for (int jkrIdx = 0; jkrIdx < runData.JokerCount; jkrIdx++)
        {
            int jokerType = runData.JokerTypes[jkrIdx];
            interestIncrease += balance.JokerBalance.InterestIncrease[jokerType];
        }

        int interest =
            runData.Money > 0
                ? runData.Money / balance.InterestEveryXDollars * balance.InterestEarnedPerXDollars
                : 0;
        interest += interestIncrease;
        if (interest > balance.InterestMax + runData.VoucherMaxInterest)
            interest = balance.InterestMax + runData.VoucherMaxInterest;
        return interest;
    }

    public static int GetRoundCompleteMoneyFromSpins(RunData runData)
    {
        return runData.MaxSpinsThisRound - runData.CurrentSpin;
    }

    public static void ClaimRoundReward(RunData runData, Balance balance)
    {
        int interest = GetInterestForRound(runData, balance);
        runData.Money += interest;

        int reward = balance.RoundReward[runData.Round % 3];
        runData.Money += reward;

        int spinMoney = GetRoundCompleteMoneyFromSpins(runData);
        runData.Money += spinMoney;

        runData.Round++;
        if (runData.Round % 3 == 0)
        {
            runData.MoneyAfterBoss = 0;
            runData.VoucherPurchased = false;
        }

        if (runData.Round < balance.MaxRounds)
            setRoundSeeds(runData, balance);

        // reset slots
        ResetSlots(runData, balance);
    }

    static void setRoundSeeds(RunData runData, Balance balance)
    {
        runData.ShopSeed = runData.RoundSeeds[runData.Round % balance.MaxRounds];
        runData.SkipSeed = runData.RoundSeeds[runData.Round % balance.MaxRounds];
        runData.BossSeed = runData.RoundSeeds[runData.Round % balance.MaxRounds];
    }

    public static void ResetSlots(RunData runData, Balance balance)
    {
        for (int slotIdx = 0; slotIdx < balance.NumSlots; slotIdx++)
            runData.UseSlot[slotIdx] = 1;

        for (int slotIdx = 0; slotIdx < balance.NumSlots; slotIdx++)
            runData.SlotTypeInGame[slotIdx] = runData.SlotType[slotIdx];
    }

    public static void PopulateShop(RunData runData, Balance balance)
    {
        runData.SelectedShopCardPackIdx = -1;
        GetJokersForShop(runData, balance);
        GetCardPacksForShop(runData, balance);
    }

    public static int GetShopRerollCost(RunData runData, Balance balance)
    {
        int cost =
            (runData.ShopRerollCount + balance.ShopRerollBaseCost)
            - runData.VoucherShopRerollsDiscount;
        ;
        if (cost < 0)
            cost = 0;
        return cost;
    }

    public static bool TryRerollShop(RunData runData, Balance balance)
    {
        int cost = GetShopRerollCost(runData, balance);
        if (CanBuy(runData, balance, cost))
        {
            runData.Money -= cost;
            runData.ShopRerollCount++;
            runData.ShopRerollTotal++;
            PopulateShop(runData, balance);
            return true;
        }
        return false;
    }

    public static void GetJokersForShop(RunData runData, Balance balance)
    {
        Span<int> commonJokerTypes = stackalloc int[runData.AvailableJokerCount];
        Span<int> uncommonJokerTypes = stackalloc int[runData.AvailableJokerCount];
        Span<int> rareJokerTypes = stackalloc int[runData.AvailableJokerCount];
        int commonJokerCount = 0;
        int uncommonJokerCount = 0;
        int rareJokerCount = 0;

        for (int i = 0; i < runData.AvailableJokerCount; i++)
        {
            int jokerType = runData.AvailableJokerTypes[i];
            if (balance.JokerBalance.Rarity[jokerType] == RARITY.COMMON)
                commonJokerTypes[commonJokerCount++] = jokerType;
            else if (balance.JokerBalance.Rarity[jokerType] == RARITY.UNCOMMON)
                uncommonJokerTypes[uncommonJokerCount++] = jokerType;
            else if (balance.JokerBalance.Rarity[jokerType] == RARITY.RARE)
                rareJokerTypes[rareJokerCount++] = jokerType;
        }

        // dont allow rare jokers in first 3 rounds
        if (runData.Round < 4 && runData.SkipShopRareJoker == 0)
            rareJokerCount = 0;
        // dont allow uncommon jokers in first 2 rounds
        if (runData.Round < 3 && runData.SkipShopUncommonJoker == 0)
            uncommonJokerCount = 0;

        // Debug.Log("commonJokerCount " + commonJokerCount + " uncommonJokerCount " + uncommonJokerCount + " rareJokerCount " + rareJokerCount);

        float rareWeight = 0.05f * runData.VoucherRareJoker;

        for (int shopJokerIdx = 0; shopJokerIdx < balance.MaxShopJokers; shopJokerIdx++)
        {
            // 5% Rare
            // 25% Uncommon
            // 70% Common
            float rarityRandom = CustomRandFloat(ref runData.ShopSeed);

            // Debug.Log("rarityRandom " + rarityRandom);

            if (runData.SkipShopRareJoker > 0)
            {
                // Debug.Log("runData.SkipShopRareJoker " + runData.SkipShopRareJoker);

                // force rare joker if available
                runData.SkipShopRareJoker--;
                rarityRandom = 0.0f;
            }
            else if (runData.SkipShopUncommonJoker > 0)
            {
                // Debug.Log("runData.SkipShopUncommonJoker " + runData.SkipShopUncommonJoker);

                // force uncommon joker if available
                runData.SkipShopUncommonJoker--;
                rarityRandom = rareWeight;
            }

            if (rarityRandom < rareWeight && rareJokerCount > 0)
                AssignRandomJokerToShop(runData, rareJokerTypes, ref rareJokerCount, shopJokerIdx);
            else if (rarityRandom < 0.3 && uncommonJokerCount > 0)
                AssignRandomJokerToShop(
                    runData,
                    uncommonJokerTypes,
                    ref uncommonJokerCount,
                    shopJokerIdx
                );
            else
                AssignRandomJokerToShop(
                    runData,
                    commonJokerTypes,
                    ref commonJokerCount,
                    shopJokerIdx
                );
        }

        // #if UNITY_EDITOR
        //         runData.ShopJokerIdxs[0] = 34;
        // #endif
    }

    private static bool AssignRandomJokerToShop(
        RunData runData,
        Span<int> availableJokerTypes,
        ref int availableJokerCount,
        int shopJokerIdx
    )
    {
        int randomIdx = CustomRandInt(ref runData.ShopSeed) % availableJokerCount;
        int jokerType = availableJokerTypes[randomIdx];

        runData.ShopJokerIdxs[shopJokerIdx] = jokerType;
        int count = 0;
        for (int jkrIdx = 0; jkrIdx < availableJokerCount; jkrIdx++)
            if (availableJokerTypes[jkrIdx] != jokerType)
                availableJokerTypes[count++] = availableJokerTypes[jkrIdx];
        availableJokerCount = count;

        return true;
    }

    public static int GetCardPackRerollCost(RunData runData, Balance balance)
    {
        int cost =
            (runData.CardPackRerollCount + balance.CardPackRerollBaseCost)
            - runData.VoucherCardPackRerollDiscount;
        if (cost < 0)
            cost = 0;
        return cost;
    }

    public static int GetRandomCardPackIdx(RunData runData, Balance balance)
    {
        int totalWeight = 0;
        for (int i = 0; i < balance.CardPackWeight.Length; i++)
            totalWeight += balance.CardPackWeight[i];

        int randomWeight = CustomRandInt(ref runData.ShopSeed) % totalWeight;

        int currentWeight = 0;
        for (int i = 0; i < balance.CardPackWeight.Length; i++)
        {
            currentWeight += balance.CardPackWeight[i];
            if (randomWeight < currentWeight)
            {
                return i;
            }
        }
        return 0;
    }

    public static void GetCardPacksForShop(RunData runData, Balance balance)
    {
        runData.ShopCardPackIdxs[0] = GetRandomCardPackIdx(runData, balance);
        runData.ShopCardPackIdxs[1] = GetRandomCardPackIdx(runData, balance);
    }

    public static void GetCardPackCards(
        RunData runData,
        Balance balance,
        Span<int> weights,
        SLOT_TYPE[] slotTypes
    )
    {
        int maxCards = balance.CardPackMaxCards[runData.SelectedShopCardPackIdx];

        int availableCardCount = weights.Length;
        Span<int> availableCardIdxs = stackalloc int[availableCardCount];
        for (int i = 0; i < availableCardCount; i++)
            availableCardIdxs[i] = i;

        int cardCount = 0;
        while (cardCount < maxCards)
        {
            int totalWeight = 0;
            for (int i = 0; i < availableCardCount; i++)
            {
                int cardIdx = availableCardIdxs[i];
                totalWeight += weights[cardIdx];
            }
            int randomWeight = CustomRandInt(ref runData.ShopSeed) % totalWeight;
            int currentWeight = 0;
            int randomCardIdx = availableCardIdxs[0];
            for (int i = 0; i < availableCardCount; i++)
            {
                int cardIdx = availableCardIdxs[i];
                currentWeight += weights[cardIdx];
                if (randomWeight < currentWeight)
                {
                    randomCardIdx = cardIdx;
                    break;
                }
            }

            runData.CardPackCardIdxs[cardCount++] = randomCardIdx;
            int count = 0;
            for (int i = 0; i < availableCardCount; i++)
                if (availableCardIdxs[i] != randomCardIdx)
                    availableCardIdxs[count++] = availableCardIdxs[i];
            availableCardCount = count;
        }

        if (runData.VoucherCardPackMostPlayedColor)
        {
            bool mostPlayedColorExists = false;
            SLOT_TYPE mostCommonSlotType = GetMostPlayedSlotType(runData);
            for (int i = 0; i < maxCards; i++)
            {
                int idx = runData.CardPackCardIdxs[i];
                if (slotTypes[idx] == mostCommonSlotType)
                    mostPlayedColorExists = true;
            }
            if (!mostPlayedColorExists)
            {
                for (int idx = 0; idx < slotTypes.Length; idx++)
                {
                    if (slotTypes[idx] == mostCommonSlotType)
                    {
                        runData.CardPackCardIdxs[0] = idx;
                        break;
                    }
                }
            }
        }
    }

    public static void SellJoker(RunData runData, int jokerIdx)
    {
        int jokerType = runData.JokerTypes[jokerIdx];
        runData.Money += runData.JokerSellValues[jokerIdx];
        runData.AvailableJokerTypes[runData.AvailableJokerCount++] = jokerType;

        RemoveJoker(runData, jokerIdx);
    }

    public static int BuyShopJoker(RunData runData, Balance balance, int shopJokerIdx)
    {
        // no need to check because we already checked before calling this

        int jokerType = runData.ShopJokerIdxs[shopJokerIdx];
        runData.ShopJokerIdxs[shopJokerIdx] = -1;
        runData.Money -= GetJokerShopCost(runData, balance, jokerType);

        AddJoker(runData, balance, jokerType);
        return jokerType;
    }

    public static void BuyCardPack(RunData runData, Balance balance, int shopPackIdx)
    {
        runData.Money -= GetCardPackShopCost(
            runData,
            balance,
            runData.ShopCardPackIdxs[shopPackIdx]
        );

        runData.SelectedShopCardPackIdx = runData.ShopCardPackIdxs[shopPackIdx];
        // todo need to remember which cardPackType we are on in GameData
        runData.ShopCardPackIdxs[shopPackIdx] = -1;

        runData.CardPackRerollCount = 0;
    }

    public static void OpenCardPack(RunData runData, Balance balance, int cardPackIdx)
    {
        runData.SelectedShopCardPackIdx = cardPackIdx;
        runData.CardPackRerollCount = 0;
    }

    public static bool TryRerollCardPack(
        RunData runData,
        Balance balance,
        int[] weights,
        SLOT_TYPE[] slotTypes
    )
    {
        int cost = GetCardPackRerollCost(runData, balance);
        if (CanBuy(runData, balance, cost))
        {
            runData.Money -= cost;
            runData.CardPackRerollCount++;
            runData.CardPackRerollTotal++;

            Span<int> tempWeights = stackalloc int[weights.Length];
            for (int i = 0; i < weights.Length; i++)
                tempWeights[i] = weights[i] * 10;

            for (int i = 0; i < balance.CardPackMaxCards[runData.SelectedShopCardPackIdx]; i++)
            {
                int index = runData.CardPackCardIdxs[i];
                tempWeights[index] = 1;
            }

            GetCardPackCards(runData, balance, tempWeights, slotTypes);
            return true;
        }
        return false;
    }

    public static void AbandonCardPack(RunData runData)
    {
        runData.CardPackAbandonTotal++;
    }

    public static bool RoomForJokerInHand(RunData runData)
    {
        return runData.JokerCount < runData.MaxJokersInHand;
    }

    public static bool CanBuy(RunData runData, Balance balance, int amount)
    {
        int debtAmount = 0;
        for (int jkrIdx = 0; jkrIdx < runData.JokerCount; jkrIdx++)
        {
            int jokerType = runData.JokerTypes[jkrIdx];
            debtAmount += balance.JokerBalance.GoIntoDebt[jokerType];
        }
        return (runData.Money + debtAmount >= amount);
    }

    public static void UnSelectAllCardPacksBalls(RunData runData)
    {
        for (int i = 0; i < runData.CardPackBallSelected.Length; i++)
            runData.CardPackBallSelected[i] = false;
    }

    public static void ToggleCardPackBallSelection(RunData runData, int ballIdx)
    {
        bool alreadySelected = runData.CardPackBallSelected[ballIdx];
        UnSelectAllCardPacksBalls(runData);
        if (!alreadySelected)
            runData.CardPackBallSelected[ballIdx] = true;
    }

    public static void UseCardPackBallCard(RunData runData, Balance balance, int cardIdx)
    {
        int cardType = runData.CardPackCardIdxs[cardIdx];
        for (int ballIdx = 0; ballIdx < runData.CardPackBallSelected.Length; ballIdx++)
            if (runData.CardPackBallSelected[ballIdx])
                runData.BallTypes[ballIdx] = balance.CardPackBallBalance.BallID[cardType];
    }

    public static void SwapBalls(RunData runData, int ballIdx1, int ballIdx2)
    {
        int ballType1 = runData.BallTypes[ballIdx1];
        int ballType2 = runData.BallTypes[ballIdx2];

        runData.BallTypes[ballIdx1] = ballType2;
        runData.BallTypes[ballIdx2] = ballType1;

        bool ballSelected1 = runData.CardPackBallSelected[ballIdx1];
        bool ballSelected2 = runData.CardPackBallSelected[ballIdx2];
        runData.CardPackBallSelected[ballIdx1] = ballSelected2;
        runData.CardPackBallSelected[ballIdx2] = ballSelected1;
    }

    public static void UseCardPackSlotCard(
        RunData runData,
        Balance balance,
        int cardIdx,
        int[] affectedSlotsIdxs,
        ref int affectedSlotsCount
    )
    {
        affectedSlotsCount = 0;

        int cardType = runData.CardPackCardIdxs[cardIdx];
        Span<int> avaiableSlots = stackalloc int[balance.NumSlots];
        for (int i = 0; i < balance.CardPackSlotBalance.NumSlots[cardType]; i++)
        {
            int availableSlotCount = 0;

            if (balance.CardPackSlotBalance.SlotChangeType[cardType] == SLOT_CHANGE_TYPE.NONE)
            {
                if (runData.VoucherSlotMostPlayedColor)
                {
                    SLOT_TYPE mostUsedSlotType = GetMostPlayedSlotType(runData);
                    for (int j = 0; j < balance.NumSlots; j++)
                        if (runData.SlotModType[j] == -1 && runData.SlotType[j] == mostUsedSlotType)
                            avaiableSlots[availableSlotCount++] = j;
                }

                if (availableSlotCount == 0)
                    for (int j = 0; j < balance.NumSlots; j++)
                        if (runData.SlotModType[j] == -1)
                            avaiableSlots[availableSlotCount++] = j;
            }
            else
            {
                for (int j = 0; j < balance.NumSlots; j++)
                {
                    bool okToChangeSlot = false;
                    if (
                        balance.CardPackSlotBalance.AffectedSlotType[cardType] < SLOT_TYPE.LAST
                        && runData.SlotType[j]
                            != (SLOT_TYPE)balance.CardPackSlotBalance.SlotChangeType[cardType]
                    )
                        okToChangeSlot = true;
                    else if (
                        balance.CardPackSlotBalance.AffectedSlotType[cardType] == SLOT_TYPE.NONE
                        && runData.SlotModType[j] == -1
                    )
                        okToChangeSlot = true;

                    if (okToChangeSlot)
                        avaiableSlots[availableSlotCount++] = j;
                }
            }
            if (availableSlotCount > 0)
            {
                int randomIdx = CustomRandInt(ref runData.ShopSeed) % availableSlotCount;
                int randomSlotIdx = avaiableSlots[randomIdx];
                affectedSlotsIdxs[affectedSlotsCount++] = randomSlotIdx;

                if (balance.CardPackSlotBalance.SlotChangeType[cardType] == SLOT_CHANGE_TYPE.NONE)
                    runData.SlotModType[randomSlotIdx] = cardType;
                else
                    runData.SlotTypeInGame[randomSlotIdx] = runData.SlotType[randomSlotIdx] =
                        (SLOT_TYPE)balance.CardPackSlotBalance.SlotChangeType[cardType];
            }
        }
    }

    public static void UseCardPackChipsCard(RunData runData, Balance balance, int cardIdx)
    {
        runData.BaseChips[runData.CardPackCardIdxs[cardIdx]] += balance.BaseChips;
    }

    public static bool CheckForSortSlotsJoker(
        RunData runData,
        Balance balance,
        Span<int> jokerIdxs,
        ref int jokerCount
    )
    {
        jokerCount = 0;

        for (int jokerIdx = 0; jokerIdx < runData.JokerCount; jokerIdx++)
        {
            int jokerType = runData.JokerTypes[jokerIdx];
            if (balance.JokerBalance.SortSlots[jokerType])
            {
                jokerIdxs[jokerCount++] = jokerIdx;
                return true;
            }
        }
        return false;
    }

    public static bool AreSlotsSorted(RunData runData)
    {
        for (int slotType = 0; slotType < 4; slotType++)
        {
            for (int slotIdx = 0; slotIdx < runData.SlotType.Length - 1; slotIdx++)
            {
                if (runData.SlotType[slotIdx] > runData.SlotType[slotIdx + 1])
                {
                    return false;
                }
            }
        }
        return true;
    }

    public static void SortSlots(RunData runData)
    {
        do
        {
            for (int slotType = 0; slotType < 4; slotType++)
            {
                for (int slotIdx = 0; slotIdx < runData.SlotType.Length - 1; slotIdx++)
                {
                    if (runData.SlotType[slotIdx] > runData.SlotType[slotIdx + 1])
                    {
                        // swap the slots
                        SLOT_TYPE slotType1 = runData.SlotType[slotIdx];
                        SLOT_TYPE slotType2 = runData.SlotType[slotIdx + 1];
                        runData.SlotType[slotIdx] = slotType2;
                        runData.SlotType[slotIdx + 1] = slotType1;

                        int slotMod1 = runData.SlotModType[slotIdx];
                        int slotMod2 = runData.SlotModType[slotIdx + 1];
                        runData.SlotModType[slotIdx] = slotMod2;
                        runData.SlotModType[slotIdx + 1] = slotMod1;
                    }
                }
            }
        } while (!AreSlotsSorted(runData));
    }

    public static int GetDoubleMoneyLimit20(RunData runData)
    {
        int money = runData.Money;
        if (money > 20)
            money = 20;
        else if (money < 0)
            money = 0;
        return money;
    }

    public static int GetSkipIndexForRound(int round)
    {
        return round - Mathf.FloorToInt((float)round / 3.0f);
    }

    public static int GetSkipTypeForRound(RunData runData, Balance balance, int round)
    {
        int skipIdx = GetSkipIndexForRound(round) % balance.SkipBalance.NumSkips;
        return runData.SkipType[skipIdx];
    }

    public static void Skip(
        RunData runData,
        Balance balance,
        int[] affectedSlotsIdxs,
        ref int affectedSlotsCount,
        out int addedJokerCount
    )
    {
        affectedSlotsCount = 0;
        addedJokerCount = 0;

        int skipType = GetSkipTypeForRound(runData, balance, runData.Round);

        if (balance.SkipBalance.DoubleMoney[skipType])
        {
            int money = GetDoubleMoneyLimit20(runData);

            runData.Money += money;
        }

        runData.Money += balance.SkipBalance.MoneyForSpinsUsed[skipType] * runData.SpinsUsed;
        runData.Money += balance.SkipBalance.MoneyForSpinsUnused[skipType] * runData.SpinsUnused;

        if (balance.SkipBalance.Change2SlotsToPlayedColor[skipType])
        {
            SLOT_TYPE slotType = GetMostPlayedSlotType(runData);

            Span<int> avaiableSlots = new int[balance.NumSlots];

            for (int i = 0; i < 2; i++)
            {
                int availableSlotCount = 0;
                for (int slotIdx = 0; slotIdx < balance.NumSlots; slotIdx++)
                    if (runData.SlotType[slotIdx] != slotType)
                        avaiableSlots[availableSlotCount++] = slotIdx;

                int randomIdx = CustomRandInt(ref runData.SkipSeed) % availableSlotCount;
                int randomSlotIdx = avaiableSlots[randomIdx];
                affectedSlotsIdxs[affectedSlotsCount++] = randomSlotIdx;
                runData.SlotType[randomSlotIdx] = slotType;
            }
        }

        if (balance.SkipBalance.JokerRarity[skipType] == RARITY.UNCOMMON)
            runData.SkipShopUncommonJoker++;
        if (balance.SkipBalance.JokerRarity[skipType] == RARITY.RARE)
            runData.SkipShopRareJoker++;
        if (balance.SkipBalance.ExtraSpin[skipType])
            runData.ExtraSkipSpin = 1;

        if (balance.SkipBalance.BossReroll[skipType])
            runData.BossRerolls++;

        runData.Money += balance.SkipBalance.MoneyNow[skipType];

        runData.MoneyAfterBoss += balance.SkipBalance.MoneyAfterBoss[skipType];

        runData.SkipCount++;
        for (int jokerIdx = 0; jokerIdx < runData.JokerCount; jokerIdx++)
            runData.JokerSkipCount[jokerIdx]++;

        for (int jkrIdx = 0; jkrIdx < runData.JokerCount; jkrIdx++)
            runData.JokerSellValues[jkrIdx] += balance.SkipBalance.IncreaseJokerSellValue[skipType];

        for (
            int jkrIdx = 0;
            jkrIdx < balance.SkipBalance.AddCommonRandomJoker[skipType]
                && runData.JokerCount < balance.MaxJokersInHand;
            jkrIdx++
        )
        {
            if (runData.JokerCount < balance.MaxJokersInHand)
            {
                int commonJokerCount = 0;
                Span<int> commonJokerTypes = stackalloc int[runData.AvailableJokerCount];
                for (int i = 0; i < runData.AvailableJokerCount; i++)
                {
                    int availableJokerType = runData.AvailableJokerTypes[i];
                    if (balance.JokerBalance.Rarity[availableJokerType] == RARITY.COMMON)
                        commonJokerTypes[commonJokerCount++] = availableJokerType;
                }

                int randomIndex = CustomRandInt(ref runData.SkipSeed) % balance.NumSlots;
                int jokerType = commonJokerTypes[randomIndex];
                AddJoker(runData, balance, jokerType);
                addedJokerCount++;
            }
        }

        for (
            int jkrIdx = 0;
            jkrIdx < balance.SkipBalance.AddUncommonRandomJoker[skipType]
                && runData.JokerCount < balance.MaxJokersInHand;
            jkrIdx++
        )
        {
            int uncommonJokerCount = 0;
            Span<int> uncommonJokerTypes = stackalloc int[runData.AvailableJokerCount];
            for (int i = 0; i < runData.AvailableJokerCount; i++)
            {
                int availableJokerType = runData.AvailableJokerTypes[i];
                if (balance.JokerBalance.Rarity[availableJokerType] == RARITY.UNCOMMON)
                    uncommonJokerTypes[uncommonJokerCount++] = availableJokerType;
            }

            if (runData.JokerCount < balance.MaxJokersInHand)
            {
                int randomIndex = CustomRandInt(ref runData.SkipSeed) % balance.NumSlots;
                int jokerType = uncommonJokerTypes[randomIndex];
                AddJoker(runData, balance, jokerType);
                addedJokerCount++;
            }
        }

        runData.Round++;
    }

    public static SLOT_TYPE GetMostPlayedSlotType(RunData runData)
    {
        int colorIdx = 0;
        int mostPlayed = 0;
        for (int i = 0; i < 4; i++)
            if (runData.ColorCount[i] > mostPlayed)
            {
                mostPlayed = runData.ColorCount[i];
                colorIdx = i;
            }
        return (SLOT_TYPE)colorIdx;
    }

    public static SLOT_TYPE GetLeastPlayedSlotType(RunData runData)
    {
        int colorIdx = 0;
        int leastPlayed = int.MaxValue;
        for (int i = 0; i < 4; i++)
            if (runData.ColorCount[i] < leastPlayed)
            {
                leastPlayed = runData.ColorCount[i];
                colorIdx = i;
            }
        return (SLOT_TYPE)colorIdx;
    }

    public static int GetVoucherForRound(RunData runData)
    {
        int index = runData.Round / 3;
        return index < runData.VoucherIdxs.Length ? runData.VoucherIdxs[index] : -1;
    }

    public static int GetJokerShopCost(RunData runData, Balance balance, int jokerType)
    {
        return Mathf.FloorToInt(balance.JokerBalance.Cost[jokerType] * runData.VoucherShopDiscount);
    }

    public static int GetCardPackShopCost(RunData runData, Balance balance, int cardPackIdx)
    {
        return Mathf.FloorToInt(balance.CardPackCost[cardPackIdx] * runData.VoucherShopDiscount);
    }

    public static int GetVoucherCost(RunData runData, Balance balance)
    {
        return Mathf.FloorToInt(balance.VoucherCost * runData.VoucherShopDiscount);
    }

    public static void BuyVoucher(RunData runData, Balance balance)
    {
        runData.VoucherPurchased = true;

        int cost = GetVoucherCost(runData, balance);
        runData.Money -= cost;

        int voucherIdx = GetVoucherForRound(runData);
        if (voucherIdx == -1)
        {
            // do nothing
        }
        else if (balance.VoucherBalance.Type[voucherIdx] == VOUCHER_TYPE.PLUS_ONE_SPIN)
        {
            runData.VoucherSpins++;
        }
        else if (balance.VoucherBalance.Type[voucherIdx] == VOUCHER_TYPE.SHOP_ITEM_DISCOUNT)
        {
            runData.VoucherShopDiscount *= 0.75f;
        }
        else if (balance.VoucherBalance.Type[voucherIdx] == VOUCHER_TYPE.EXTRA_SHOP_JOKER)
        {
            runData.ShopJokerCount = balance.MaxShopJokers;
        }
        else if (balance.VoucherBalance.Type[voucherIdx] == VOUCHER_TYPE.CHEAP_SHOP_REROLLS)
        {
            runData.VoucherShopRerollsDiscount += 2;
        }
        else if (balance.VoucherBalance.Type[voucherIdx] == VOUCHER_TYPE.CHEAP_CARDPACK_REROLLS)
        {
            runData.VoucherCardPackRerollDiscount += 2;
        }
        else if (balance.VoucherBalance.Type[voucherIdx] == VOUCHER_TYPE.CARPACK_MOST_PLAYED_COLOR)
        {
            runData.VoucherCardPackMostPlayedColor = true;
        }
        else if (balance.VoucherBalance.Type[voucherIdx] == VOUCHER_TYPE.RAISE_INTEREST)
        {
            runData.VoucherMaxInterest += 5;
        }
        else if (balance.VoucherBalance.Type[voucherIdx] == VOUCHER_TYPE.REROLL_BOSS_TYPE)
        {
            runData.BossRerolls += 3;
        }
        else if (balance.VoucherBalance.Type[voucherIdx] == VOUCHER_TYPE.RARE_CARDS_WEIGHT)
        {
            runData.VoucherRareJoker = 2.0f;
        }
        else if (
            balance.VoucherBalance.Type[voucherIdx] == VOUCHER_TYPE.SLOT_CARDPACK_MOST_PLAYED_COLOR
        )
        {
            runData.VoucherSlotMostPlayedColor = true;
        }
    }

    private const string Digits = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    public static string EncodeSeed(uint value)
    {
        string encoded = "";
        do encoded = Digits[(int)(value % Digits.Length)] + encoded;
        while ((value /= (uint)Digits.Length) != 0);
        return encoded;
    }

    public static uint DecodeSeed(string value)
    {
        uint decoded = 0;
        for (var i = 0; i < value.Length; ++i)
            decoded +=
                (uint)Digits.IndexOf(value[i])
                * (uint)(Mathf.Pow(Digits.Length, value.Length - i - 1));
        return decoded;
    }
}
