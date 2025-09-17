using System;
using Cardwheel;
using UnityEngine;

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

    public static void AllocateRunData(RunData runData, Balance balance)
    {
        runData.JokerCount = 0;
        runData.JokerTypes = new int[balance.MaxJokersInHand];
        runData.JokerSellValues = new int[balance.MaxJokersInHand];
        runData.JokerChips = new int[balance.MaxJokersInHand];
        runData.JokerMultiplierAdd = new float[balance.MaxJokersInHand];

        runData.BallTypes = new int[balance.MaxBalls];
        runData.BallTypesInGame = new int[balance.MaxBalls];
        runData.BallSnapVelocity = new float[balance.MaxBalls];
        runData.BallSnapTime = new float[balance.MaxBalls];
        runData.BallScoreIdxs = new int[balance.MaxBalls];
        runData.BallSlotIdx = new int[balance.MaxBalls];

        runData.SlotColors = new Color[(int)SLOT_TYPE.LAST];

        runData.BallScoresCount = new int[(int)SLOT_TYPE.LAST];

        runData.BaseChips = new int[(int)SLOT_TYPE.LAST];

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
        runData.BossType = new int[balance.BossBalance.NumBosses];
        runData.UseSlotType = new int[(int)SLOT_TYPE.LAST];
        runData.UseJoker = new int[balance.MaxJokersInHand];

        runData.RoundSeeds = new uint[balance.MaxRounds];
    }

    public static void AllocateGameData(GameData gameData, Balance balance)
    {
        gameData.SpinWheelWinCount = new int[balance.SpinWheelBalance.NumSpinWheels];
    }

    public static void StartNewGame(RunData runData, Balance balance, int wheelIdx, uint seed)
    {
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
            runData.SlotTypeInGame[slotIdx] = runData.SlotType[slotIdx] = balance.SpinWheelBalance.SlotType[runData.WheelIdx][slotIdx];

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

        for (int i = 0; i < (int)SLOT_TYPE.LAST; i++)
            runData.SlotColors[i] = balance.SlotColors[i];

        initSkipsForNewGame(runData, balance);

        initBossesForNewGame(runData, balance);

        for (int i = 0; i < balance.MaxJokersInHand; i++)
            runData.UseJoker[i] = 1;

        runData.SkipShopUncommonJoker = 0;
        runData.SkipShopRareJoker = 0;

        runData.MoneyAfterBoss = 0;

        runData.UseBallsSpecial = 1;
        runData.UseSlotsSpecial = 1;
        for (int i = 0; i < (int)SLOT_TYPE.LAST; i++)
            runData.UseSlotType[i] = 1;
        runData.UseBaseChips = 1;

        runData.ShopRerollCount = 0;
        runData.CardPackRerollCount = 0;

        runData.ShopRerollTotal = 0;
        runData.CardPackRerollTotal = 0;


        // TEST
#if UNITY_EDITOR
        // runData.Money = -20;

        // runData.SkipType[0] = 6;
        // runData.SkipType[1] = 12;
        // runData.SkipType[3] = 13;
        // runData.SkipType[1] = 7;

        // AddJoker(runData, balance, 67);
        // AddJoker(runData, balance, 70);
        // AddJoker(runData, balance, 5);
        // AddJoker(runData, balance, 24);
        // AddJoker(runData, balance, 11);

        // for (int i = 0; i < runData.BallTypes.Length; i++)
        //     runData.BallTypes[i] = i;
        // runData.BallTypes[0] = 1;
        // runData.BallTypes[1] = 2;
        // runData.BallTypes[2] = 8;
        // runData.BallTypes[3] = 8;
        // runData.BallTypes[4] = 8;
        // runData.BallTypes[5] = 8;

        // int cnt = 0;
        // for (int i = 0; i < runData.SlotModType.Length; i++)
        //     if (i % 3 == 0)
        //         runData.SlotModType[i] = (cnt++ % 4 + 4);

        // runData.SlotModType[8] = 0;
        // runData.SlotModType[9] = 0;
        // runData.SlotModType[10] = 0;

        // for (int i = 0; i < balance.BossBalance.NumBosses; i++)
        //     if (balance.BossBalance.BossEffect[i] == BOSS_EFFECT.JUMBLE_BALLS)
        //         runData.BossType[0] = i;

        // runData.Money = 0;
        // runData.VoucherShopDiscount *= 0.75f;

        // runData.BossRerolls = 99;
#endif
    }

    private static void initBossesForNewGame(RunData runData, Balance balance)
    {
        for (int i = 0; i < balance.BossBalance.NumBosses; i++)
            runData.BossType[i] = -1;

        int numBossRounds = balance.MaxRounds / 3;
        for (int i = 0; i < numBossRounds; i++)
            setUniqueBossForRound(runData, balance, i + 1);
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

        Debug.Log("balance.SkipBalance.NumSkips " + balance.SkipBalance.NumSkips + " skipsAllRoundsCount " + skipsAllRoundsCount);

        // assign the rest of the skips
        for (int skipIdx = 2; skipIdx < balance.SkipBalance.NumSkips; skipIdx++)
            runData.SkipType[skipIdx] = skipsAllRounds[skipIdx - 2];
    }

    public static void setUniqueBossForRound(RunData runData, Balance balance, int round)
    {
        if (runData.BossType[round - 1] > -1)
            Debug.Log("Prev boss runData.BossType[" + (round - 1) + "] " + balance.BossBalance.Description[runData.BossType[round - 1]]);

        Span<int> bossesForRound = new int[balance.BossBalance.NumBosses];
        int bossesForRoundCount = 0;
        for (int i = 0; i < balance.BossBalance.NumBosses; i++)
            if (round >= balance.BossBalance.LevelRange[i].x && round <= balance.BossBalance.LevelRange[i].y)
                bossesForRound[bossesForRoundCount++] = i;

        int count = 0;
        for (int bfrIdx = 0; bfrIdx < bossesForRoundCount; bfrIdx++)
        {
            bool bossAlreadyPicked = false;
            for (int bossIdx = 0; bossIdx < balance.BossBalance.NumBosses; bossIdx++)
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
        runData.BossType[round - 1] = bossesForRound[randomIdx];

        int bossType = runData.BossType[round - 1];
        Debug.Log("setUniqueBossForRound round " + round + " bossesForRoundCount " + bossesForRoundCount + " runData.BossType[" + (round - 1) + "] " + bossType + " " + balance.BossBalance.Description[bossType] + " min " + balance.BossBalance.LevelRange[bossType].x + " max " + balance.BossBalance.LevelRange[bossType].y);
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

    public static int GetBossTypeForRound(RunData runData)
    {
        int bossIdx = (runData.Round / 3) % runData.BossType.Length;
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
        setUniqueBossForRound(runData, balance, (runData.Round / 3) + 1);
    }

    public static void SetDataForNextRound(RunData runData, Balance balance)
    {
        runData.CurrentSpin = 0;
        runData.TotalChips = 0;

        runData.ShopRerollCount = 0;
        runData.CardPackRerollCount = 0;

        int spinsChange = 0;
        for (int jkrIdx = 0; jkrIdx < runData.JokerCount; jkrIdx++)
        {
            int jokerType = runData.JokerTypes[jkrIdx];
            spinsChange += balance.JokerBalance.AddSpin[jokerType];
        }

        for (int i = 0; i < (int)SLOT_TYPE.LAST; i++)
        {
            runData.SlotColors[i] = balance.SlotColors[i];
            runData.UseSlotType[i] = 1;
        }

        for (int i = 0; i < balance.MaxJokersInHand; i++)
            runData.UseJoker[i] = 1;

        for (int slotIdx = 0; slotIdx < balance.NumSlots; slotIdx++)
        {
            runData.SlotTypeInGame[slotIdx] = runData.SlotType[slotIdx];
        }

        for (int i = 0; i < runData.BallTypes.Length; i++)
            runData.BallTypesInGame[i] = runData.BallTypes[i];

        if (InBossRound(runData))
        {
            int bossType = GetBossTypeForRound(runData);
            if (balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.ONE_LESS_SPIN)
                spinsChange -= 1;

            if (balance.BossBalance.BossEffect[bossType] >= BOSS_EFFECT.ONLY_RED &&
                balance.BossBalance.BossEffect[bossType] <= BOSS_EFFECT.ONLY_BLUE)
            {
                int colorIdx = (int)balance.BossBalance.BossEffect[bossType] - (int)BOSS_EFFECT.ONLY_RED;
                onlyUseOneSlotColor(runData, balance, colorIdx);
            }

            if (balance.BossBalance.BossEffect[bossType] >= BOSS_EFFECT.NO_RED &&
                balance.BossBalance.BossEffect[bossType] <= BOSS_EFFECT.NO_BLUE)
            {
                int colorIdx = (int)balance.BossBalance.BossEffect[bossType] - (int)BOSS_EFFECT.NO_RED;
                debuffOneColor(runData, balance, colorIdx);
            }

            if (balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.ONLY_RED_GREEN)
            {
                turnOffOneColor(runData, balance, (int)SLOT_TYPE.ORANGE);
                turnOffOneColor(runData, balance, (int)SLOT_TYPE.BLUE);

            }
            if (balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.ONLY_RED_ORANGE)
            {
                turnOffOneColor(runData, balance, (int)SLOT_TYPE.GREEN);
                turnOffOneColor(runData, balance, (int)SLOT_TYPE.BLUE);
            }
            if (balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.ONLY_GREEN_BLUE)
            {
                turnOffOneColor(runData, balance, (int)SLOT_TYPE.RED);
                turnOffOneColor(runData, balance, (int)SLOT_TYPE.ORANGE);

            }
            if (balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.ONLY_BLUE_ORANGE)
            {
                turnOffOneColor(runData, balance, (int)SLOT_TYPE.RED);
                turnOffOneColor(runData, balance, (int)SLOT_TYPE.GREEN);
            }

            if (balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.ONLY_PLAY_MOST_USED_COLOR)
            {
                int colorIdx = (int)GetMostPlayedSlotType(runData);
                onlyUseOneSlotColor(runData, balance, colorIdx);
            }

            if (balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.DEBUFF_MOST_USED_COLOR)
            {
                int colorIdx = (int)GetMostPlayedSlotType(runData);
                debuffOneColor(runData, balance, colorIdx);
            }

            if (balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.SWAP_COLORS)
            {
                SLOT_TYPE mostPlayedType = GetMostPlayedSlotType(runData);
                SLOT_TYPE leastPlayedType = GetLeastPlayedSlotType(runData);

                for (int i = 0; i < balance.NumSlots; i++)
                {
                    if (runData.SlotType[i] == mostPlayedType)
                        runData.SlotTypeInGame[i] = leastPlayedType;
                    if (runData.SlotType[i] == leastPlayedType)
                        runData.SlotTypeInGame[i] = mostPlayedType;
                }
            }

            if (balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.JUMBLE_SLOTS)
            {
                for (int i = 0; i < balance.NumSlots; i++)
                {
                    SLOT_TYPE slotType = runData.SlotTypeInGame[i];
                    int randomIndex = CustomRandInt(ref runData.BossSeed) % balance.NumSlots;
                    runData.SlotTypeInGame[i] = runData.SlotTypeInGame[randomIndex];
                    runData.SlotTypeInGame[randomIndex] = slotType;
                }
            }

            if (balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.JUMBLE_SLOT_EFFECTS)
            {
                for (int i = 0; i < balance.NumSlots; i++)
                {
                    int slotType = runData.SlotModType[i];
                    int randomIndex = CustomRandInt(ref runData.BossSeed) % balance.NumSlots;
                    runData.SlotModType[i] = runData.SlotModType[randomIndex];
                    runData.SlotModType[randomIndex] = slotType;
                }
            }
        }


        spinsChange += runData.ExtraSkipSpin + runData.VoucherSpins;
        runData.MaxSpinsThisRound = spinsChange + balance.SpinWheelBalance.Spins[runData.WheelIdx];

        runData.UseBaseChips = 1;
        runData.UseBallsSpecial = 1;
        runData.UseSlotsSpecial = 1;

        runData.LeastPlayedColorAtRoundStart = GetLeastPlayedSlotType(runData);

        // check if boss round
        if (InBossRound(runData))
        {
            startBossRound(runData, balance);
        }
    }

    public static bool InBossRound(RunData runData)
    {
        return runData.Round % 3 == 2;
    }

    private static void onlyUseOneSlotColor(RunData runData, Balance balance, int colorIdx)
    {
        for (int i = 0; i < (int)SLOT_TYPE.LAST; i++)
        {
            runData.SlotColors[i] = (i == colorIdx) ? balance.SlotColors[i] : balance.SlotOffColor;
            runData.UseSlotType[i] = (i == colorIdx) ? 1 : 0;
        }
    }

    private static void debuffOneColor(RunData runData, Balance balance, int colorIdx)
    {
        for (int i = 0; i < (int)SLOT_TYPE.LAST; i++)
        {
            runData.SlotColors[i] = (i == colorIdx) ? balance.SlotOffColor : balance.SlotColors[i];
            runData.UseSlotType[i] = (i == colorIdx) ? 0 : 1;
        }
    }

    private static void turnOffOneColor(RunData runData, Balance balance, int colorIdx)
    {
        runData.SlotColors[colorIdx] = balance.SlotOffColor;
        runData.UseSlotType[colorIdx] = 0;
    }

    static void startBossRound(RunData runData, Balance balance)
    {
        int bossType = GetBossTypeForRound(runData);
        if (balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.BALLS_DEBUFFED || balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.BALLS_DEBUFFED_FIRST_SPIN)
            runData.UseBallsSpecial = 0;
        if (balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.SLOTS_DEBUFFED || balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.SLOTS_DEBUFFED_FIRST_SPIN)
            runData.UseSlotsSpecial = 0;
        if (balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.NO_BASE_CHIPS)
            runData.UseBaseChips = 0;
    }

    public static void PostRoundBossEffect(RunData runData, Balance balance)
    {
        int bossType = GetBossTypeForRound(runData);
        if (balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.LOSE_MONEY_EVERY_SPIN)
        {
            runData.Money--;
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

        runData.JokerBallTriggerIdx = 0;

        runData.CurrentSpin++;
        runData.TotalSpins++;
        runData.SpinsUsed++;
    }

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

    public static void StartSpinBossEffect(RunData runData, Balance balance, out bool slotsChanged, int[] affectedSlotsIdxs, ref int affectedSlotsCount)
    {
        slotsChanged = false;

        if (InBossRound(runData))
        {
            int bossType = GetBossTypeForRound(runData);
            if (balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.DIFFERENT_COLOR_EVERY_SPIN)
            {
                uint seed = runData.StartSeed;
                int randomOffset = CustomRandInt(ref seed);
                int colorIdx = (randomOffset + runData.CurrentSpin) % (int)SLOT_TYPE.LAST;
                onlyUseOneSlotColor(runData, balance, colorIdx);
                slotsChanged = true;

                affectedSlotsCount = 0;
                for (int slotIdx = 0; slotIdx < balance.NumSlots; slotIdx++)
                {
                    int slotType = (int)runData.SlotTypeInGame[slotIdx];
                    if (runData.UseSlotType[slotType] == 1)
                        affectedSlotsIdxs[affectedSlotsCount++] = slotIdx;
                }
            }
            if (balance.BossBalance.BossEffect[bossType] >= BOSS_EFFECT.ONLY_RED_FIRST_SPIN &&
                balance.BossBalance.BossEffect[bossType] <= BOSS_EFFECT.ONLY_BLUE_FIRST_SPIN)
            {
                if (runData.CurrentSpin > 1)
                {
                    for (int i = 0; i < (int)SLOT_TYPE.LAST; i++)
                    {
                        runData.SlotColors[i] = balance.SlotColors[i];
                        runData.UseSlotType[i] = 1;
                    }
                }
                else
                {
                    int colorIdx = (int)(balance.BossBalance.BossEffect[bossType] - BOSS_EFFECT.ONLY_RED_FIRST_SPIN);
                    onlyUseOneSlotColor(runData, balance, colorIdx);
                }
                slotsChanged = true;
            }


            if (balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.RANDOM_JOKE_DEBUFFED_PER_SPIN)
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
        }
    }

    public static int JokerPreRoundTryModifySlot(RunData runData, Balance balance, int jokerType)
    {
        int slotIdx = -1;

        if (balance.JokerBalance.StartRoundChangeSlotID[jokerType] > -1)
        {
            int availableSlotCount = 0;
            Span<int> avaiableSlots = new int[balance.NumSlots];

            for (int j = 0; j < balance.NumSlots; j++)
                if (runData.SlotModType[j] == -1)
                    avaiableSlots[availableSlotCount++] = j;

            slotIdx = CustomRandInt(ref runData.GameSeed) % availableSlotCount;
            runData.SlotModType[slotIdx] = balance.JokerBalance.StartRoundChangeSlotID[jokerType];
        }

        return slotIdx;
    }

    public static bool BallInSlot(RunData runData, Balance balance, int ballIdx, int slotIdx, out int slotChangedIdx, out int slotChangeJokerIdx, out int jokerMultIncIdx, out int jokerMultInc)
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

            if (runData.CurrentSpin == 1 && ballIdx == 0)
                for (int jokerIdx = 0; jokerIdx < runData.JokerCount; jokerIdx++)
                {
                    int jokerType = runData.JokerTypes[jokerIdx];
                    if (balance.JokerBalance.FirstBallConvertSlotToID[jokerType] > 0)
                    {
                        slotChangedIdx = slotIdx;
                        slotChangeJokerIdx = jokerIdx;
                        runData.SlotModType[slotIdx] = balance.JokerBalance.FirstBallConvertSlotToID[jokerType];
                    }
                }

            for (int jokerIdx = 0; jokerIdx < runData.JokerCount; jokerIdx++)
            {
                int jokerType = runData.JokerTypes[jokerIdx];

                if (runData.SlotModType[slotIdx] > -1 && balance.JokerBalance.BallIncMultRemoveSlotMod[jokerType] > 0)
                {
                    slotChangedIdx = slotIdx;
                    slotChangeJokerIdx = jokerIdx;
                    runData.SlotModType[slotIdx] = -1;
                    runData.JokerMultiplierAdd[jokerIdx] += balance.JokerBalance.BallIncMultRemoveSlotMod[jokerType];

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
        {
            if (runData.SlotScored[i] > -1)
            {
                runData.BallScoreIdxs[ballCount++] = runData.SlotScored[i];
            }
        }
    }

    public static int CalculateSlotBallChips(RunData runData, Balance balance, int ballIdx)
    {
        int slotIdx = runData.BallSlotIdx[ballIdx];
        int slotType = (int)runData.SlotTypeInGame[slotIdx];

        runData.BallScoresCount[slotType]++;

        int baseChips = runData.BaseChips[slotType];

        if (InBossRound(runData))
            if (balance.BossBalance.BossEffect[GetBossTypeForRound(runData)] == BOSS_EFFECT.MOST_PLAYED_BASE_CHIPS_TO_FIVE)
                if (GetMostPlayedSlotType(runData) == runData.SlotTypeInGame[slotIdx])
                    baseChips = 5;

        // Debug.Log("slotType " + slotType + " m_ballScoresCount " + runData.BallScoresCount.ToString());
        int chips = runData.BallScoresCount[slotType] * baseChips * runData.UseBaseChips;
        int slotModType = runData.SlotModType[slotIdx];
        if (slotModType > -1)
            chips += balance.CardPackSlotBalance.Chips[slotModType] * runData.UseSlotsSpecial;

        chips *= runData.UseSlotType[slotType];

        if (!scoringBossCheck(runData, balance))
            chips = 0;

        runData.SpinChips += chips;

        return chips;
    }

    public static float CalculateSlotBallMultiplierAdd(RunData runData, Balance balance, int ballIdx)
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
            multiplier += balance.CardPackSlotBalance.MultiplierAdd[slotModType] * runData.UseSlotsSpecial;
        }

        multiplier *= runData.UseSlotType[slotType];

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
            multiplier += balance.CardPackSlotBalance.MultiplierMult[slotModType] * runData.UseSlotsSpecial;

        multiplier *= runData.UseSlotType[slotType];

        if (!scoringBossCheck(runData, balance))
            multiplier = 0;

        if (multiplier > 0)
            runData.SpinMultiplier *= multiplier;

        return multiplier;
    }

    public static void AddJoker(RunData runData, Balance balance, int jokerType)
    {
        if (runData.JokerCount < balance.MaxJokersInHand)
        {
            int jkrIdx = runData.JokerCount;

            runData.JokerTypes[jkrIdx] = jokerType;
            runData.JokerSellValues[jkrIdx] = balance.JokerBalance.Cost[jokerType] / 2;
            runData.JokerChips[jkrIdx] = Mathf.RoundToInt(balance.JokerBalance.SubtractChipsPerSpin[jokerType].x);
            runData.JokerMultiplierAdd[jkrIdx] = balance.JokerBalance.SubtractMultiplierAddPerRound[jokerType].x;

            runData.JokerCount++;
        }
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
                count++;
            }
        }
        runData.JokerCount = count;
    }

    public static int CalculateBallChips(RunData runData, Balance balance, int ballIdx)
    {
        int chips = Mathf.FloorToInt(CalculateBallCommon(runData, ballIdx, balance.BallBalance.BallChips, balance.BallBalance.BallColorMultiplier));

        if (!scoringBossCheck(runData, balance))
            chips = 0;

        runData.SpinChips += chips;

        return chips;
    }

    public static float CalculateBallMultiplierAdd(RunData runData, Balance balance, int ballIdx)
    {
        float mult = CalculateBallCommon(runData, ballIdx, balance.BallBalance.BallMultiplierAdd, balance.BallBalance.BallColorMultiplier);

        if (!scoringBossCheck(runData, balance))
            mult = 0;

        runData.SpinMultiplier += mult;

        return mult;
    }

    public static float CalculateBallMultiplierMult(RunData runData, Balance balance, int ballIdx)
    {
        float mult = CalculateBallCommon(runData, ballIdx, balance.BallBalance.BallMultiplierMult, balance.BallBalance.BallColorMultiplier);

        if (!scoringBossCheck(runData, balance))
            mult = 0;

        if (mult >= 1.0f)
            runData.SpinMultiplier *= mult;

        return mult;
    }

    public static float CalculateBallCommon(RunData runData, int ballIdx, float[] perBallArray, float[][] perColorArray)
    {
        float value = 0;
        int slotIdx = runData.BallSlotIdx[ballIdx];
        int ballType = runData.BallTypesInGame[ballIdx];
        value += perBallArray[ballType] * runData.UseBallsSpecial;
        int slotType = (int)runData.SlotTypeInGame[slotIdx];
        value = value * perColorArray[ballType][slotType];

        value *= runData.UseSlotType[slotType];

        return value;
    }

    public static int CalculateBallMoney(RunData runData, Balance balance, int ballIdx, Span<int> jokerIdxs, ref int jokerCount)
    {
        int money = 0;
        int ballType = runData.BallTypesInGame[ballIdx];

        int slotIdx = runData.BallSlotIdx[ballIdx];
        int slotType = (int)runData.SlotTypeInGame[slotIdx];

        money += balance.BallBalance.BallMoney[ballType] * runData.UseBallsSpecial * runData.UseSlotType[slotType];

        for (int jkrIdx = 0; jkrIdx < runData.JokerCount; jkrIdx++)
        {
            int jokerType = runData.JokerTypes[jkrIdx];
            if (CustomRandFloat(ref runData.GameSeed) < balance.JokerBalance.ChanceBallGivesMoney[jokerType])
            {
                jokerIdxs[jokerCount++] = jkrIdx;
                money++;
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

        int slotModType = runData.SlotModType[slotIdx];
        if (slotModType > -1)
            money += balance.CardPackSlotBalance.Money[slotModType] * runData.UseSlotsSpecial;

        int slotType = (int)runData.SlotTypeInGame[slotIdx];
        money *= runData.UseSlotType[slotType];

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
            Span<int> slotTypeCount = new int[4];
            CountNumBallsOnSlotType(runData, balance.MaxBalls, slotTypeCount);

            int bossType = GetBossTypeForRound(runData);
            if (balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.ONLY_SCORE_SIX_BALLS)
            {
                bool sizeFound = false;
                for (int slotType = 0; slotType < 4; slotType++)
                    if (runData.UseSlotType[slotType] == 1 && slotTypeCount[slotType] == 6)
                        sizeFound = true;
                if (!sizeFound)
                    return false;
            }

            if (balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.ONLY_SCORE_AT_LEAST_TWO_COLORS ||
            balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.ONLY_SCORE_AT_LEAST_THREE_COLORS)
            {
                int numTypes = 0;
                for (int slotType = 0; slotType < 4; slotType++)
                    if (runData.UseSlotType[slotType] == 1 && slotTypeCount[slotType] > 0)
                        numTypes++;

                if (balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.ONLY_SCORE_AT_LEAST_TWO_COLORS && numTypes < 2)
                    return false;
                if (balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.ONLY_SCORE_AT_LEAST_THREE_COLORS && numTypes < 3)
                    return false;

            }
        }
        return true;
    }

    static bool jokerBaseCheck(RunData runData, Balance balance, int jokerIdx, int jokerType)
    {
        Span<int> slotTypeCount = new int[4];
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
            if (balance.JokerBalance.RetriggerBallsEverySpin[jokerType] ||
            (runData.CurrentSpin == runData.MaxSpinsThisRound && balance.JokerBalance.RetriggerBallsLastSpin[jokerType]))
            {
                runData.JokerBallTriggerIdx = jokerIdx + 1;
                return jokerIdx;
            }
        }
        return -1;
    }

    public static int CalculateJokerChipsAdd(RunData runData, Balance balance, int jokerIdx, int jokerType)
    {
        int chips = 0;

        Span<int> slotTypeCount = new int[4];
        CountNumBallsOnSlotType(runData, balance.MaxBalls, slotTypeCount);

        if (jokerBaseCheck(runData, balance, jokerIdx, jokerType))
        {
            chips += balance.JokerBalance.BaseChipsAdd[jokerType];

            runData.JokerChips[jokerIdx] += (int)balance.JokerBalance.ChipsIncreasePerSpin[jokerType];

            for (int slotType = 0; slotType < 4; slotType++)
                if (IsFlagSet(balance.JokerBalance.TypeExists[jokerType], slotType))
                    runData.JokerChips[jokerIdx] += balance.JokerBalance.ChipsIncreasePerBall[jokerType] * slotTypeCount[slotType];
        }

        chips += balance.JokerBalance.ChipsPerDollar[jokerType] * runData.Money;

        chips += runData.JokerChips[jokerIdx];

        int numNonSlotMods = GetNumNonModedSlots(runData, balance);
        chips += (numNonSlotMods * balance.JokerBalance.ChipsAddForEveryNonSlotMod[jokerType]);

        chips *= runData.UseJoker[jokerIdx];

        // add chips per ball
        for (int slotType = 0; slotType < 4; slotType++)
            if (runData.UseSlotType[slotType] == 1 && slotTypeCount[slotType] > 0 && IsFlagSet(balance.JokerBalance.TypeExists[jokerType], slotType))
                chips += slotTypeCount[slotType] * balance.JokerBalance.ChipsPerBall[jokerType];

        if (!scoringBossCheck(runData, balance))
            chips = 0;

        Debug.Log("jokerType " + jokerType + " chips " + chips);

        runData.SpinChips += chips;

        return chips;
    }

    public static float CalculateJokerMultiplierAdd(RunData runData, Balance balance, int jokerIdx, int jokerType)
    {
        float mult = 0.0f;

        if (jokerBaseCheck(runData, balance, jokerIdx, jokerType))
        {
            mult += balance.JokerBalance.BaseMultiplierAdd[jokerType];
            runData.JokerMultiplierAdd[jokerIdx] += balance.JokerBalance.MultIncreaseForSize[jokerType];
        }

        int numNoJokers = runData.MaxJokersInHand - runData.JokerCount;
        mult += balance.JokerBalance.PerJokerMultiplierAdd[jokerType] * runData.JokerCount;
        mult += balance.JokerBalance.PerNoJokerMultiplierAdd[jokerType] * numNoJokers;

        if (runData.CurrentSpin == runData.MaxSpinsThisRound)
            mult += balance.JokerBalance.LastSpinMultiplierAdd[jokerType];

        // runData.JokerMultiplierAdd[jokerIdx] += (int)balance.JokerBalance.ChipsIncreasePerSpin[jokerType];
        mult += runData.JokerMultiplierAdd[jokerIdx];

        float randomValue = CustomRandFloatRange(ref runData.GameSeed, balance.JokerBalance.MultiplierAddRandomRange[jokerType].x, balance.JokerBalance.MultiplierAddRandomRange[jokerType].y + 1.0f);
        mult += Mathf.Floor(randomValue);

        int numSlotMods = GetNumModedSlots(runData, balance);
        mult += (numSlotMods * balance.JokerBalance.MultiplierAddForEverySlotMod[jokerType]);

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

        // branchless use or don't use
        mult *= runData.UseJoker[jokerIdx];

        if (!scoringBossCheck(runData, balance))
            mult = 0;

        Debug.Log("jokerType " + jokerType + " mult " + mult);

        runData.SpinMultiplier += mult;


        return mult;
    }

    public static float CalculateJokerMultiplierMult(RunData runData, Balance balance, int jokerIdx, int jokerType)
    {
        float mult = 0.0f;

        Debug.Log("jokerType " + jokerType + " mult " + mult);

        if (jokerBaseCheck(runData, balance, jokerIdx, jokerType))
            mult += balance.JokerBalance.BaseMultiplierMult[jokerType];

        int numSpecialBalls = 0;
        for (int ballIdx = 0; ballIdx < balance.MaxBalls; ballIdx++)
            if (runData.BallTypesInGame[ballIdx] > 0)
                numSpecialBalls++;

        mult += balance.JokerBalance.MultiplierMultForSpecialBall[jokerType] * numSpecialBalls;
        mult += balance.JokerBalance.MultiplierMultForNonSpecialBall[jokerType] * (balance.MaxBalls - numSpecialBalls);
        mult += balance.JokerBalance.MultiplierMultEveryShopReroll[jokerType] * runData.ShopRerollTotal;
        mult += balance.JokerBalance.MultiplierMultEveryCardPackReroll[jokerType] * runData.CardPackRerollTotal;

        mult *= runData.UseJoker[jokerIdx];

        mult += 1.0f;

        if (!scoringBossCheck(runData, balance))
            mult = 0;

        if (mult > 1.0f)
            runData.SpinMultiplier *= mult;


        return mult;
    }

    private static bool checkTypeReqs(RunData runData, Balance balance, int jokerType, Span<int> slotTypeCount)
    {
        bool use = false;
        for (int slotType = 0; slotType < 4; slotType++)
            if (runData.UseSlotType[slotType] == 1 && slotTypeCount[slotType] > 0 && IsFlagSet(balance.JokerBalance.TypeExists[jokerType], slotType))
                use = true;
        return use;
    }

    private static bool checkTypeNotExistReqs(RunData runData, Balance balance, int jokerType, Span<int> slotTypeCount)
    {
        bool use = true;
        for (int slotType = 0; slotType < 4; slotType++)
            if (runData.UseSlotType[slotType] == 1 && slotTypeCount[slotType] > 0 && IsFlagSet(balance.JokerBalance.TypeNotExists[jokerType], slotType))
                use = false;
        return use;
    }


    public static bool checkSizeReqs(RunData runData, Balance balance, int jokerType, Span<int> slotTypeCount)
    {
        bool use = true;
        for (int size = 0; size < 6; size++)
        {
            if (IsFlagSet(balance.JokerBalance.SizeExists[jokerType], size))
            {
                bool sizeFound = false;
                for (int slotType = 0; slotType < 4; slotType++)
                    if (runData.UseSlotType[slotType] == 1 && slotTypeCount[slotType] == (size + 1))
                        sizeFound = true;
                if (!sizeFound)
                    use = false;
            }
        }
        return use;
    }

    private static bool checkNumTypes(RunData runData, Balance balance, int jokerType, Span<int> slotTypeCount)
    {
        bool numTypesOk;
        int numTypes = 0;
        for (int slotType = 0; slotType < 4; slotType++)
            if (runData.UseSlotType[slotType] == 1 && slotTypeCount[slotType] > 0)
                numTypes++;
        numTypesOk = numTypes >= balance.JokerBalance.MinTypes[jokerType];
        return numTypesOk;
    }

    public static void CountNumBallsOnSlotType(RunData runData, int maxBalls, Span<int> slotTypeCount)
    {
        for (int i = 0; i < (int)SLOT_TYPE.LAST; i++)
            slotTypeCount[i] = 0;

        for (int ballIdx = 0; ballIdx < maxBalls; ballIdx++)
        {
            int slotIdx = runData.BallSlotIdx[ballIdx];
            int slotType = (int)runData.SlotTypeInGame[slotIdx];
            slotTypeCount[slotType]++;
        }
    }

    public static void JokerPostSpin(RunData runData, Balance balance)
    {
        for (int jokerIdx = 0; jokerIdx < runData.JokerCount; jokerIdx++)
        {
            int jokerType = runData.JokerTypes[jokerIdx];
            runData.JokerChips[jokerIdx] -= Mathf.RoundToInt(balance.JokerBalance.SubtractChipsPerSpin[jokerType].y);
            if (runData.JokerChips[jokerIdx] < 0)
                runData.JokerChips[jokerIdx] = 0;

            runData.Money += balance.JokerBalance.MoneyPerSpin[jokerType];
        }
    }

    public static void SpinComplete(RunData runData, Balance balance)
    {
        if (InBossRound(runData))
        {
            int bossType = GetBossTypeForRound(runData);
            if (balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.BALLS_DEBUFFED_FIRST_SPIN && runData.CurrentSpin > 0)
                runData.UseBallsSpecial = 1;
            if (balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.SLOTS_DEBUFFED_FIRST_SPIN && runData.CurrentSpin > 0)
                runData.UseSlotsSpecial = 1;
        }
    }

    public static int GetRoundGoal(RunData runData, Balance balance)
    {
        return GetRoundGoal(runData, balance, runData.Round / 3, runData.Round % 3);
    }
    public static int GetRoundGoal(RunData runData, Balance balance, int bigRound, int smallRound)
    {
        if (bigRound >= balance.BaseChip.Length)
            bigRound = balance.BaseChip.Length - 1;

        int goal = Mathf.FloorToInt(balance.BaseChip[bigRound] * balance.RoundChipMult[smallRound] * (balance.SpinWheelBalance.GoalMultiplier[runData.WheelIdx] + 1.0f));
        if (InBossRound(runData))
        {
            int bossType = GetBossTypeForRound(runData);
            if (balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.DOUBLE_GOAL)
                goal *= 2;
        }
        return goal;
    }

    public static bool CheckRoundComplete(RunData runData, Balance balance)
    {
        int goal = GetRoundGoal(runData, balance);
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

    public static void RoundComplete(RunData runData, Balance balance)
    {
        for (int jokerIdx = 0; jokerIdx < runData.JokerCount; jokerIdx++)
        {
            int jokerType = runData.JokerTypes[jokerIdx];
            runData.JokerMultiplierAdd[jokerIdx] += (int)balance.JokerBalance.MultIncreasePerUnusedSpin[jokerType] * (runData.MaxSpinsThisRound - runData.CurrentSpin);
            runData.JokerMultiplierAdd[jokerIdx] += (int)balance.JokerBalance.MultIncreasePerUsedSpin[jokerType] * runData.CurrentSpin;

            runData.JokerMultiplierAdd[jokerIdx] -= balance.JokerBalance.SubtractMultiplierAddPerRound[jokerType].y;
            if (runData.JokerMultiplierAdd[jokerIdx] < 0)
                runData.JokerMultiplierAdd[jokerIdx] = 0;
        }

        runData.SpinsUnused += runData.MaxSpinsThisRound - runData.CurrentSpin;


        int moneyFromJokers = GetRoundCompleteMoneyFromJokers(runData, balance);
        runData.Money += moneyFromJokers;

        for (int jkrIdx = 0; jkrIdx < runData.JokerCount; jkrIdx++)
        {
            int jokerType = runData.JokerTypes[jkrIdx];
            runData.JokerSellValues[jkrIdx] += balance.JokerBalance.IncreaseSellValueEveryRound[jokerType];
            runData.UseJoker[jkrIdx] = 1;
        }

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

        int interest = runData.Money > 0 ? runData.Money / balance.InterestEveryXDollars * balance.InterestEarnedPerXDollars : 0;
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

        setRoundSeeds(runData);

        // reset slots
        ResetSlots(runData, balance);
    }

    static void setRoundSeeds(RunData runData)
    {
        runData.ShopSeed = runData.RoundSeeds[runData.Round];
        runData.SkipSeed = runData.RoundSeeds[runData.Round];
        runData.BossSeed = runData.RoundSeeds[runData.Round];
    }

    public static void ResetSlots(RunData runData, Balance balance)
    {
        for (int i = 0; i < (int)SLOT_TYPE.LAST; i++)
        {
            runData.SlotColors[i] = balance.SlotColors[i];
            runData.UseSlotType[i] = 1;
        }
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
        int cost = (runData.ShopRerollCount + balance.ShopRerollBaseCost) - runData.VoucherShopRerollsDiscount; ;
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
        Span<int> commonJokerTypes = new int[runData.AvailableJokerCount];
        Span<int> uncommonJokerTypes = new int[runData.AvailableJokerCount];
        Span<int> rareJokerTypes = new int[runData.AvailableJokerCount];
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

        Debug.Log("commonJokerCount " + commonJokerCount + " uncommonJokerCount " + uncommonJokerCount + " rareJokerCount " + rareJokerCount);

        float rareWeight = 0.05f * runData.VoucherRareJoker;

        for (int shopJokerIdx = 0; shopJokerIdx < balance.MaxShopJokers; shopJokerIdx++)
        {
            // 5% Rare
            // 25% Uncommon
            // 70% Common
            float rarityRandom = CustomRandFloat(ref runData.ShopSeed);

            Debug.Log("rarityRandom " + rarityRandom);

            if (runData.SkipShopRareJoker > 0)
            {
                Debug.Log("runData.SkipShopRareJoker " + runData.SkipShopRareJoker);

                // force rare joker if available
                runData.SkipShopRareJoker--;
                rarityRandom = 0.0f;
            }
            else if (runData.SkipShopUncommonJoker > 0)
            {
                Debug.Log("runData.SkipShopUncommonJoker " + runData.SkipShopUncommonJoker);

                // force uncommon joker if available
                runData.SkipShopUncommonJoker--;
                rarityRandom = rareWeight;
            }

            if (rarityRandom < rareWeight && rareJokerCount > 0)
                AssignRandomJokerToShop(runData, rareJokerTypes, ref rareJokerCount, shopJokerIdx);
            else if (rarityRandom < 0.3 && uncommonJokerCount > 0)
                AssignRandomJokerToShop(runData, uncommonJokerTypes, ref uncommonJokerCount, shopJokerIdx);
            else
                AssignRandomJokerToShop(runData, commonJokerTypes, ref commonJokerCount, shopJokerIdx);

        }
    }

    private static bool AssignRandomJokerToShop(RunData runData, Span<int> availableJokerTypes, ref int availableJokerCount, int shopJokerIdx)
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
        int cost = (runData.CardPackRerollCount + balance.CardPackRerollBaseCost) - runData.VoucherCardPackRerollDiscount;
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

    public static void GetCardPackCards(RunData runData, Balance balance, int[] weights, SLOT_TYPE[] slotTypes)
    {
        int maxCards = balance.CardPackMaxCards[runData.SelectedShopCardPackIdx];

        int availableCardCount = weights.Length;
        Span<int> availableCardIdxs = new int[availableCardCount];
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

        for (int i = 0; i < maxCards; i++)
            Debug.Log("GetCardPackCards runData.CardPackCardIdxs[" + i + "] " + runData.CardPackCardIdxs[i]);
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
        int jokerType = runData.ShopJokerIdxs[shopJokerIdx];
        runData.ShopJokerIdxs[shopJokerIdx] = -1;
        runData.Money -= GetJokerShopCost(runData, balance, jokerType);

        Debug.Log("Logic.BuyShopJoker(shopJokerIdx + " + shopJokerIdx + ") jokerIdx" + jokerType);

        int count = 0;
        for (int jkrIdx = 0; jkrIdx < runData.AvailableJokerCount; jkrIdx++)
            if (runData.AvailableJokerTypes[jkrIdx] != jokerType)
                runData.AvailableJokerTypes[count++] = runData.AvailableJokerTypes[jkrIdx];
        runData.AvailableJokerCount = count;


        AddJoker(runData, balance, jokerType);
        return jokerType;
    }

    public static void BuyCardPack(RunData runData, Balance balance, int shopPackIdx)
    {
        runData.Money -= GetCardPackShopCost(runData, balance, runData.ShopCardPackIdxs[shopPackIdx]);

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

    public static bool TryRerollCardPack(RunData runData, Balance balance)
    {
        int cost = GetCardPackRerollCost(runData, balance);
        if (CanBuy(runData, balance, cost))
        {
            runData.Money -= cost;
            runData.CardPackRerollCount++;
            runData.CardPackRerollTotal++;
            return true;
        }
        return false;
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

    public static void UseCardPackSlotCard(RunData runData, Balance balance, int cardIdx, int[] affectedSlotsIdxs, ref int affectedSlotsCount)
    {
        affectedSlotsCount = 0;

        int cardType = runData.CardPackCardIdxs[cardIdx];
        for (int i = 0; i < balance.CardPackSlotBalance.NumSlots[cardType]; i++)
        {
            int availableSlotCount = 0;
            Span<int> avaiableSlots = new int[balance.NumSlots];

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
                    if (runData.SlotType[j] != (SLOT_TYPE)balance.CardPackSlotBalance.SlotChangeType[cardType] && runData.SlotModType[j] == -1)
                        avaiableSlots[availableSlotCount++] = j;
            }
            int randomIdx = CustomRandInt(ref runData.ShopSeed) % availableSlotCount;
            int randomSlotIdx = avaiableSlots[randomIdx];
            affectedSlotsIdxs[affectedSlotsCount++] = randomSlotIdx;

            Debug.Log("availableSlotCount " + availableSlotCount + " runData.SlotType[" + randomSlotIdx + "] " + runData.SlotType[randomSlotIdx] + " (int) " + (int)runData.SlotType[randomSlotIdx] + " chagnging to " + (SLOT_TYPE)balance.CardPackSlotBalance.SlotChangeType[cardType] + " (int) " + (int)(SLOT_TYPE)balance.CardPackSlotBalance.SlotChangeType[cardType]);

            if (balance.CardPackSlotBalance.SlotChangeType[cardType] == SLOT_CHANGE_TYPE.NONE)
                runData.SlotModType[randomSlotIdx] = cardType;
            else
                runData.SlotTypeInGame[randomSlotIdx] = runData.SlotType[randomSlotIdx] = (SLOT_TYPE)balance.CardPackSlotBalance.SlotChangeType[cardType];
        }
    }


    public static void UseCardPackChipsCard(RunData runData, Balance balance, int cardIdx)
    {
        runData.BaseChips[runData.CardPackCardIdxs[cardIdx]] += balance.BaseChips;
    }

    public static bool CheckForSortSlotsJoker(RunData runData, Balance balance, Span<int> jokerIdxs, ref int jokerCount)
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
        }
        while (!AreSlotsSorted(runData));
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

    public static void Skip(RunData runData, Balance balance, int[] affectedSlotsIdxs, ref int affectedSlotsCount)
    {
        affectedSlotsCount = 0;

        int skipIdx = runData.Round % balance.SkipBalance.NumSkips;
        int skipType = runData.SkipType[skipIdx];

        Debug.Log("Skip " + balance.SkipBalance.SkipDescription[skipType]);

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
        return runData.VoucherIdxs[(runData.Round / 3) % runData.VoucherIdxs.Length];
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
        if (balance.VoucherBalance.Type[voucherIdx] == VOUCHER_TYPE.PLUS_ONE_SPIN)
        {
            runData.VoucherSpins++;
        }
        if (balance.VoucherBalance.Type[voucherIdx] == VOUCHER_TYPE.SHOP_ITEM_DISCOUNT)
        {
            runData.VoucherShopDiscount *= 0.75f;
        }
        if (balance.VoucherBalance.Type[voucherIdx] == VOUCHER_TYPE.EXTRA_SHOP_JOKER)
        {
            runData.ShopJokerCount = balance.MaxShopJokers;
        }
        if (balance.VoucherBalance.Type[voucherIdx] == VOUCHER_TYPE.CHEAP_SHOP_REROLLS)
        {
            runData.VoucherShopRerollsDiscount += 2;
        }
        if (balance.VoucherBalance.Type[voucherIdx] == VOUCHER_TYPE.CHEAP_CARDPACK_REROLLS)
        {
            runData.VoucherCardPackRerollDiscount += 2;
        }
        if (balance.VoucherBalance.Type[voucherIdx] == VOUCHER_TYPE.CARPACK_MOST_PLAYED_COLOR)
        {
            runData.VoucherCardPackMostPlayedColor = true;
        }
        if (balance.VoucherBalance.Type[voucherIdx] == VOUCHER_TYPE.RAISE_INTEREST)
        {
            runData.VoucherMaxInterest += 5;
        }
        if (balance.VoucherBalance.Type[voucherIdx] == VOUCHER_TYPE.REROLL_BOSS_TYPE)
        {
            runData.BossRerolls += 3;
        }
        if (balance.VoucherBalance.Type[voucherIdx] == VOUCHER_TYPE.RARE_CARDS_WEIGHT)
        {
            runData.VoucherRareJoker = 2.0f;
        }
        if (balance.VoucherBalance.Type[voucherIdx] == VOUCHER_TYPE.SLOT_CARDPACK_MOST_PLAYED_COLOR)
        {
            runData.VoucherSlotMostPlayedColor = true;
        }
    }

    private const string Digits = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    public static string EncodeSeed(uint value)
    {
        string encoded = "";
        do
            encoded = Digits[(int)(value % Digits.Length)] + encoded;
        while ((value /= (uint)Digits.Length) != 0);
        return encoded;
    }

    public static uint DecodeSeed(string value)
    {
        uint decoded = 0;
        for (var i = 0; i < value.Length; ++i)
            decoded += (uint)Digits.IndexOf(value[i]) * (uint)(Mathf.Pow(Digits.Length, value.Length - i - 1));
        return decoded;
    }
}


