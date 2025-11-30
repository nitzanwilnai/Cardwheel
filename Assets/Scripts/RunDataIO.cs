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
    public static class RunDataIO
    {
        public static int VERSION = 5;

        public static void SaveRun(RunData runData, Balance balance)
        {
            Debug.LogFormat("SaveGame()");

            if (!Directory.Exists(Application.persistentDataPath + "/Cardwheel"))
                Directory.CreateDirectory(Application.persistentDataPath + "/Cardwheel");

            string fileName = Application.persistentDataPath + "/Cardwheel/save.dat";
            using (FileStream fs = File.Create(fileName))
            using (BinaryWriter bw = new BinaryWriter(fs))
            {
                bw.Write(VERSION);

                bw.Write((byte)runData.MenuState);
                bw.Write((byte)runData.PrevMenuState);

                bw.Write(runData.Money);
                bw.Write(runData.StartSeed);
                bw.Write(runData.GameSeed);
                bw.Write(runData.ShopSeed);
                bw.Write(runData.SkipSeed);
                bw.Write(runData.BossSeed);
                for (int i = 0; i < runData.RoundSeeds.Length; i++)
                    bw.Write(runData.RoundSeeds[i]);

                bw.Write(runData.TotalChips);
                bw.Write(runData.SpinChips);
                bw.Write(runData.SpinMultiplier);
                bw.Write(runData.Round);
                bw.Write(runData.CurrentSpin);
                bw.Write(runData.ExtraSkipSpin);
                bw.Write(runData.MaxSpinsThisRound);
                bw.Write(runData.TotalSpins);

                bw.Write(runData.SpinsUsed);
                bw.Write(runData.SpinsUnused);

                bw.Write(runData.RotationSpeed);
                bw.Write(runData.SpinWheelAngle);

                bw.Write(balance.NumSlots);
                for (int i = 0; i < balance.NumSlots; i++)
                {
                    bw.Write(runData.SlotScored[i]);
                    bw.Write((byte)runData.SlotType[i]);
                    bw.Write((byte)runData.SlotTypeInGame[i]);
                    bw.Write(runData.SlotModType[i]);
                }

                bw.Write(balance.MaxBalls);
                for (int i = 0; i < balance.MaxBalls; i++)
                {
                    bw.Write(runData.BallTypes[i]);
                    bw.Write(runData.BallTypesInGame[i]);
                    bw.Write(runData.BallSnapVelocity[i]);
                    bw.Write(runData.BallSnapTime[i]);
                    bw.Write(runData.BallScoreIdxs[i]);
                    bw.Write(runData.BallSlotIdx[i]);
                    bw.Write(runData.CardPackBallSelected[i]);
                }

                for (int i = 0; i < (int)SLOT_TYPE.LAST; i++)
                {
                    bw.Write(runData.SlotColors[i].r);
                    bw.Write(runData.SlotColors[i].g);
                    bw.Write(runData.SlotColors[i].b);
                    bw.Write(runData.SlotColors[i].a);
                    bw.Write(runData.BallScoresCount[i]);
                    bw.Write(runData.BaseChips[i]);
                    bw.Write(runData.ColorCount[i]);
                    bw.Write(runData.UseSlotType[i]);
                }

                bw.Write(runData.MoneyAfterBoss);
                bw.Write(runData.BossRerolls);
                bw.Write((byte)runData.LeastPlayedColorAtRoundStart);
                bw.Write(runData.BestSpin);

                bw.Write(runData.JokerBallTriggerIdx);

                bw.Write(balance.MaxJokersInHand);
                for (int i = 0; i < balance.MaxJokersInHand; i++)
                {
                    bw.Write(runData.JokerTypes[i]);
                    bw.Write(runData.JokerSellValues[i]);
                    bw.Write(runData.JokerChips[i]);
                    bw.Write(runData.JokerMultiplierAdd[i]);
                    bw.Write(runData.UseJoker[i]);
                    bw.Write(runData.JokerSpins[i]);
                    bw.Write(runData.JokerRounds[i]);
                    bw.Write(runData.JokerSkipCount[i]);
                }

                bw.Write(runData.JokerCount);
                bw.Write(runData.MaxJokersInHand);

                bw.Write(runData.ShopJokerCount);
                bw.Write(runData.ShopRerollCount);
                bw.Write(runData.CardPackRerollCount);
                bw.Write(runData.ShopRerollTotal);
                bw.Write(runData.CardPackRerollTotal);
                bw.Write(runData.CardPackAbandonTotal);

                bw.Write(runData.SelectedShopCardPackIdx);

                bw.Write(runData.VoucherPurchased);
                bw.Write(runData.VoucherSpins);
                bw.Write(runData.VoucherMaxInterest);
                bw.Write(runData.VoucherShopDiscount);
                bw.Write(runData.VoucherShopRerollsDiscount);
                bw.Write(runData.VoucherCardPackRerollDiscount);
                bw.Write(runData.VoucherCardPackMostPlayedColor);
                bw.Write(runData.VoucherRareJoker);
                bw.Write(runData.VoucherSlotMostPlayedColor);

                bw.Write(runData.AvailableJokerCount);
                bw.Write(balance.JokerBalance.NumJokers);
                for (int i = 0; i < balance.JokerBalance.NumJokers; i++)
                    bw.Write(runData.AvailableJokerTypes[i]);

                bw.Write(balance.MaxShopJokers);
                for (int i = 0; i < balance.MaxShopJokers; i++)
                    bw.Write(runData.ShopJokerIdxs[i]);

                bw.Write(balance.MaxShopCardPacks);
                for (int i = 0; i < balance.MaxShopCardPacks; i++)
                    bw.Write(runData.ShopCardPackIdxs[i]);

                bw.Write(balance.MaxShopCardPackCards);
                for (int i = 0; i < balance.MaxShopCardPackCards; i++)
                    bw.Write(runData.CardPackCardIdxs[i]);

                bw.Write(balance.VoucherBalance.NumVouchers);
                for (int i = 0; i < balance.VoucherBalance.NumVouchers; i++)
                    bw.Write(runData.VoucherIdxs[i]);

                bw.Write(balance.SkipBalance.NumSkips);
                for (int i = 0; i < balance.SkipBalance.NumSkips; i++)
                    bw.Write(runData.SkipType[i]);
                bw.Write(runData.SkipShopUncommonJoker);
                bw.Write(runData.SkipShopRareJoker);

                bw.Write(balance.BossBalance.NumBosses);
                for (int i = 0; i < balance.BossBalance.NumBosses; i++)
                    bw.Write(runData.BossType[i]);
                bw.Write(runData.UseBallsSpecial);
                bw.Write(runData.UseSlotsSpecial);
                bw.Write(runData.UseBaseChips);

                bw.Write(runData.SkipCount);

                bw.Write(runData.WheelIdx);

                bw.Write((int)123456);
            }
        }

        public static MENU_STATE LoadMenuStateOnly()
        {
            if (File.Exists(Application.persistentDataPath + "/Cardwheel/save_v"+VERSION+".dat"))
                return LoadMenuStateFromFile("save_v"+VERSION+".dat");
            else if (File.Exists(Application.persistentDataPath + "/Cardwheel/save.dat"))
                return LoadMenuStateFromFile("save.dat");
            else
                return MENU_STATE.NONE;
        }

        public static MENU_STATE LoadMenuStateFromFile(string filename)
        {
            MENU_STATE menuState = MENU_STATE.NONE;
            string path = Application.persistentDataPath + "/Cardwheel/" + filename;
            if (File.Exists(path))
            {
                using (var stream = File.Open(path, FileMode.Open))
                {
                    using (BinaryReader br = new BinaryReader(stream))
                    {
                        int version = br.ReadInt32();

                        if (version >= 2)
                            menuState = (MENU_STATE)br.ReadByte();
                    }
                }
            }
            return menuState;
        }

        public static bool LoadRun(RunData runData)
        {
            string fileName = Application.persistentDataPath + "/Cardwheel/save_v"+VERSION+".dat";
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

                        runData.SkipCount = br.ReadInt32();

                        runData.WheelIdx = br.ReadInt32();

                        Debug.Log("RunDataIO.LoadRun " + br.ReadInt32());

                        return true;
                    }
                }
            }
            return false;
        }

    }
}
