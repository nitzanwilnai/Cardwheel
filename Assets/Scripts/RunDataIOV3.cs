/*
  Cardwheel — Non-Commercial, No-Modification License
  Copyright © 2025 Nitzan Wilnai
  Source Code: https://github.com/nitzanwilnai/Cardwheel

  Permission is granted to view and run this code for non-commercial purposes only.
  Modification, redistribution of altered versions, and commercial use are strictly prohibited.

  See the LICENSE file for full legal terms.
*/

using System.IO;
using UnityEngine;


namespace Cardwheel
{
    public static class RunDataIOV3
    {
        public static void LoadRun(RunData runData, Balance balance)
        {
            string fileName = Application.persistentDataPath + "/Cardwheel/save.dat";
            if (File.Exists(fileName))
            {
                using (var stream = File.Open(fileName, FileMode.Open))
                {
                    using (BinaryReader br = new BinaryReader(stream))
                    {
                        int version = br.ReadInt32();

                        runData.MenuState = (MENU_STATE)br.ReadByte();
                        runData.PrevMenuState = (MENU_STATE)br.ReadByte();

                        runData.Money = br.ReadInt32();
                        runData.StartSeed = br.ReadUInt32();
                        runData.GameSeed = br.ReadUInt32();
                        runData.ShopSeed = br.ReadUInt32();
                        runData.SkipSeed = br.ReadUInt32();
                        runData.BossSeed = br.ReadUInt32();
                        for (int i = 0; i < runData.RoundSeeds.Length; i++)
                            runData.RoundSeeds[i] = br.ReadUInt32();

                        runData.TotalChips = br.ReadInt32();
                        runData.SpinChips = br.ReadInt32();
                        runData.SpinMultiplier = br.ReadSingle();
                        runData.Round = br.ReadInt32();
                        runData.CurrentSpin = br.ReadInt32();
                        runData.ExtraSkipSpin = br.ReadInt32();
                        runData.MaxSpinsThisRound = br.ReadInt32();
                        runData.TotalSpins = br.ReadInt32();

                        runData.SpinsUsed = br.ReadInt32();
                        runData.SpinsUnused = br.ReadInt32();

                        runData.RotationSpeed = br.ReadSingle();
                        runData.SpinWheelAngle = br.ReadSingle();

                        int numSlots = br.ReadInt32();
                        for (int i = 0; i < numSlots; i++)
                        {
                            runData.SlotScored[i] = br.ReadInt32();
                            runData.SlotType[i] = (SLOT_TYPE)br.ReadByte();
                            runData.SlotTypeInGame[i] = (SLOT_TYPE)br.ReadByte();
                            runData.SlotModType[i] = br.ReadInt32();
                        }

                        int maxBalls = br.ReadInt32();
                        for (int i = 0; i < maxBalls; i++)
                        {
                            runData.BallTypes[i] = br.ReadInt32();
                            runData.BallTypesInGame[i] = br.ReadInt32();
                            runData.BallSnapVelocity[i] = br.ReadSingle();
                            runData.BallSnapTime[i] = br.ReadSingle();
                            runData.BallScoreIdxs[i] = br.ReadInt32();
                            runData.BallSlotIdx[i] = br.ReadInt32();
                            runData.CardPackBallSelected[i] = br.ReadBoolean();
                        }

                        for (int i = 0; i < (int)SLOT_TYPE.LAST; i++)
                        {
                            runData.SlotColors[i].r = br.ReadSingle();
                            runData.SlotColors[i].g = br.ReadSingle();
                            runData.SlotColors[i].b = br.ReadSingle();
                            runData.SlotColors[i].a = br.ReadSingle();
                            runData.BallScoresCount[i] = br.ReadInt32();
                            runData.BaseChips[i] = br.ReadInt32();
                            runData.ColorCount[i] = br.ReadInt32();
                            runData.UseSlotType[i] = br.ReadInt32();
                        }

                        runData.MoneyAfterBoss = br.ReadInt32();
                        runData.BossRerolls = br.ReadInt32();
                        runData.LeastPlayedColorAtRoundStart = (SLOT_TYPE)br.ReadByte();
                        runData.BestSpin = br.ReadInt32();

                        runData.JokerBallTriggerIdx = br.ReadInt32();

                        int maxJokersInHand = br.ReadInt32();
                        for (int i = 0; i < maxJokersInHand; i++)
                        {
                            runData.JokerTypes[i] = br.ReadInt32();
                            runData.JokerSellValues[i] = br.ReadInt32();
                            runData.JokerChips[i] = br.ReadInt32();
                            runData.JokerMultiplierAdd[i] = br.ReadSingle();
                            runData.UseJoker[i] = br.ReadInt32();
                            runData.JokerSpins[i] = br.ReadInt32();
                            runData.JokerRounds[i] = br.ReadInt32();
                            runData.JokerSkipCount[i] = br.ReadInt32();
                        }

                        runData.JokerCount = br.ReadInt32();
                        runData.MaxJokersInHand = br.ReadInt32();

                        runData.ShopJokerCount = br.ReadInt32();
                        runData.ShopRerollCount = br.ReadInt32();
                        runData.CardPackRerollCount = br.ReadInt32();
                        runData.ShopRerollTotal = br.ReadInt32();
                        runData.CardPackRerollTotal = br.ReadInt32();
                        runData.CardPackAbandonTotal = br.ReadInt32();

                        runData.SelectedShopCardPackIdx = br.ReadInt32();

                        runData.VoucherPurchased = br.ReadBoolean();
                        runData.VoucherSpins = br.ReadInt32();
                        runData.VoucherMaxInterest = br.ReadInt32();
                        runData.VoucherShopDiscount = br.ReadSingle();
                        runData.VoucherShopRerollsDiscount = br.ReadInt32();
                        runData.VoucherCardPackRerollDiscount = br.ReadInt32();
                        runData.VoucherCardPackMostPlayedColor = br.ReadBoolean();
                        runData.VoucherRareJoker = br.ReadSingle();
                        runData.VoucherSlotMostPlayedColor = br.ReadBoolean();

                        runData.AvailableJokerCount = br.ReadInt32();
                        int numSavedJokers = br.ReadInt32();
                        for (int i = 0; i < numSavedJokers; i++)
                            runData.AvailableJokerTypes[i] = br.ReadInt32();

                        int maxShopJokers = br.ReadInt32();
                        for (int i = 0; i < maxShopJokers; i++)
                            runData.ShopJokerIdxs[i] = br.ReadInt32();

                        int maxShopCardPacks = br.ReadInt32();
                        for (int i = 0; i < maxShopCardPacks; i++)
                            runData.ShopCardPackIdxs[i] = br.ReadInt32();

                        int maxShopCardPackCards = br.ReadInt32();
                        for (int i = 0; i < maxShopCardPackCards; i++)
                            runData.CardPackCardIdxs[i] = br.ReadInt32();

                        int numVouchers = br.ReadInt32();
                        for (int i = 0; i < numVouchers; i++)
                            runData.VoucherIdxs[i] = br.ReadInt32();

                        int numSkips = br.ReadInt32();
                        for (int i = 0; i < numSkips; i++)
                            runData.SkipType[i] = br.ReadInt32();
                        runData.SkipShopUncommonJoker = br.ReadInt32();
                        runData.SkipShopRareJoker = br.ReadInt32();

                        int numBosses = br.ReadInt32();
                        for (int i = 0; i < numBosses; i++)
                            runData.BossType[i] = br.ReadInt32();
                        runData.UseBallsSpecial = br.ReadInt32();
                        runData.UseSlotsSpecial = br.ReadInt32();
                        runData.UseBaseChips = br.ReadInt32();

                        if (version >= 3)
                        {
                            runData.SkipCount = br.ReadInt32();
                        }

                        Debug.Log("RunDataIO.LoadRun " + br.ReadInt32());

                    }
                }
            }
        }

    }
}
