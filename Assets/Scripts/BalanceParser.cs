using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;


namespace Cardwheel
{
    public class BalanceParser : MonoBehaviour
    {
#if UNITY_EDITOR
        [MenuItem("Cardwheel/Balance/Parse Local")]
        public static void ParseLocal()
        {
            Debug.Log("Parse balance started!");

            AssignIDs();
            byte[] array = parse();
            // save array
            string path = "Assets/Resources/balance.bytes";
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            using (FileStream fs = File.Create(path))
            using (BinaryWriter bw = new BinaryWriter(fs))
                bw.Write(array);

            Debug.Log("Parse balance finished!");

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        public static void AssignIDs()
        {
            List<Object> objects = new List<Object>();
            int numObjects;

            objects.Clear();
            AddObjectsFromDirectory("Assets/Data/Balls", objects, typeof(BallSO));
            numObjects = objects.Count;
            for (int i = 0; i < numObjects; i++)
            {
                BallSO ball = (BallSO)objects[i];
                ball.ID = i;
                EditorUtility.SetDirty(ball);
            }

            objects.Clear();
            AddObjectsFromDirectory("Assets/Data/CardPackSlots", objects, typeof(CardPackSlotSO));
            numObjects = objects.Count;
            for (int i = 0; i < numObjects; i++)
            {
                CardPackSlotSO slot = (CardPackSlotSO)objects[i];
                slot.ID = i;
                EditorUtility.SetDirty(slot);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        static byte[] parse()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter bw = new BinaryWriter(stream))
                {
                    int version = 1;
                    bw.Write(version);

                    BalanceSO balanceSO = (BalanceSO)AssetDatabase.LoadAssetAtPath("Assets/Data/BalanceSO.asset", typeof(BalanceSO));
                    bw.Write(balanceSO.NumSlots);
                    for (int i = 0; i < balanceSO.SlotColors.Length; i++)
                    {
                        bw.Write(balanceSO.SlotColors[i].r);
                        bw.Write(balanceSO.SlotColors[i].g);
                        bw.Write(balanceSO.SlotColors[i].b);
                        bw.Write(balanceSO.SlotColors[i].a);
                    }

                    bw.Write(balanceSO.SlotOffColor.r);
                    bw.Write(balanceSO.SlotOffColor.g);
                    bw.Write(balanceSO.SlotOffColor.b);
                    bw.Write(balanceSO.SlotOffColor.a);

                    bw.Write(balanceSO.RarityColors.Length);
                    for (int i = 0; i < balanceSO.RarityColors.Length; i++)
                    {
                        bw.Write(balanceSO.RarityColors[i].r);
                        bw.Write(balanceSO.RarityColors[i].g);
                        bw.Write(balanceSO.RarityColors[i].b);
                        bw.Write(balanceSO.RarityColors[i].a);
                    }
                    bw.Write(balanceSO.StartingMoney);
                    bw.Write(balanceSO.MaxBalls);
                    bw.Write(balanceSO.MaxRounds);
                    bw.Write(balanceSO.MaxJokers);
                    bw.Write(balanceSO.StartMaxJokers);
                    bw.Write(balanceSO.BaseChips);
                    bw.Write(balanceSO.BaseMultiplier);
                    bw.Write(balanceSO.ShopRerollBaseCost);
                    bw.Write(balanceSO.CardPackRerollBaseCost);

                    bw.Write(balanceSO.InterestEveryXDollars);
                    bw.Write(balanceSO.InterestEarnedPerXDollars);
                    bw.Write(balanceSO.InterestMax);

                    bw.Write(balanceSO.NumShopJokers);
                    bw.Write(balanceSO.MaxShopJokers);
                    bw.Write(balanceSO.MaxShopCardPacks);
                    bw.Write(balanceSO.MaxShopCardPackCards);
                    bw.Write(balanceSO.VoucherCost);

                    bw.Write(balanceSO.ButtonColorEnabled.r);
                    bw.Write(balanceSO.ButtonColorEnabled.g);
                    bw.Write(balanceSO.ButtonColorEnabled.b);
                    bw.Write(balanceSO.ButtonColorEnabled.a);

                    bw.Write(balanceSO.RerollColorEnabled.r);
                    bw.Write(balanceSO.RerollColorEnabled.g);
                    bw.Write(balanceSO.RerollColorEnabled.b);
                    bw.Write(balanceSO.RerollColorEnabled.a);

                    bw.Write(balanceSO.ButtonColorDisabled.r);
                    bw.Write(balanceSO.ButtonColorDisabled.g);
                    bw.Write(balanceSO.ButtonColorDisabled.b);
                    bw.Write(balanceSO.ButtonColorDisabled.a);

                    bw.Write(balanceSO.UISpinWheelSpeed);

                    ShopCardPacksSO shopCardPacksSO = (ShopCardPacksSO)AssetDatabase.LoadAssetAtPath("Assets/Data/ShopCardPacksSO.asset", typeof(ShopCardPacksSO));
                    bw.Write(shopCardPacksSO.ShopCardPackInfo.Length);
                    for (int i = 0; i < shopCardPacksSO.ShopCardPackInfo.Length; i++)
                    {
                        bw.Write((byte)shopCardPacksSO.ShopCardPackInfo[i].CardPackType);
                        bw.Write(shopCardPacksSO.ShopCardPackInfo[i].PickCards);
                        bw.Write(shopCardPacksSO.ShopCardPackInfo[i].MaxCards);
                        bw.Write(shopCardPacksSO.ShopCardPackInfo[i].Cost);
                        bw.Write(shopCardPacksSO.ShopCardPackInfo[i].Weight);
                    }

                    RoundSO roundSO = (RoundSO)AssetDatabase.LoadAssetAtPath("Assets/Data/RoundSO.asset", typeof(RoundSO));
                    for (int i = 0; i < 8; i++)
                        bw.Write(roundSO.BaseChip[i]);
                    for (int i = 0; i < 3; i++)
                        bw.Write(roundSO.RoundChipMult[i]);
                    for (int i = 0; i < 3; i++)
                        bw.Write(roundSO.RoundReward[i]);

                    List<Object> objects = new List<Object>();
                    AddObjectsFromDirectory("Assets/Data/Jokers", objects, typeof(JokerSO));
                    int numJokers = objects.Count;
                    Debug.Log("numJokers " + numJokers);
                    bw.Write(numJokers);
                    for (int jkrIdx = 0; jkrIdx < numJokers; jkrIdx++)
                    {
                        JokerSO jokerSO = (JokerSO)objects[jkrIdx];
                        bw.Write(jokerSO.JokerSprite.name);
                        bw.Write(jokerSO.Cost);
                        bw.Write((byte)jokerSO.Rarity);
                        bw.Write(jokerSO.DescriptionGO.name);

                        bw.Write(jokerSO.BaseChipsAdd);
                        bw.Write(jokerSO.BaseMultiplierAdd);
                        bw.Write(jokerSO.BaseMultiplierMult);

                        for (int i = 0; i < 4; i++)
                            bw.Write(jokerSO.TypeExists[i]);

                        for (int i = 0; i < 4; i++)
                            bw.Write(jokerSO.TypeNotExists[i]);

                        for (int i = 0; i < 6; i++)
                            bw.Write(jokerSO.SizeExists[i]);

                        bw.Write(jokerSO.ChipsPerBall);
                        bw.Write(jokerSO.ChipsIncreasePerBall);
                        bw.Write(jokerSO.MultIncreaseForSize);
                        bw.Write(jokerSO.MinTypes);

                        bw.Write(jokerSO.SubtractMultiplierAddPerRound.x);
                        bw.Write(jokerSO.SubtractMultiplierAddPerRound.y);
                        bw.Write(jokerSO.SubtractChipsAddPerSpin.x);
                        bw.Write(jokerSO.SubtractChipsAddPerSpin.y);

                        bw.Write(jokerSO.PerJokerMultiplierAdd);
                        bw.Write(jokerSO.PerNoJokerMultiplierAdd);

                        bw.Write(jokerSO.ChipsIncreasePerSpin);
                        bw.Write(jokerSO.MultIncreasePerUnusedSpin);
                        bw.Write(jokerSO.MultIncreasePerUsedSpin);

                        bw.Write(jokerSO.MultiplierAddRandomRange.x);
                        bw.Write(jokerSO.MultiplierAddRandomRange.y);

                        bw.Write(jokerSO.EarnMoneyEveryRound);
                        bw.Write(jokerSO.IncreaseSellValueEveryRound);
                        bw.Write(jokerSO.GoIntoDebt);
                        bw.Write(jokerSO.InterestIncrease);
                        bw.Write(jokerSO.ChanceBallGivesMoney);
                        bw.Write(jokerSO.MoneyPerSpin);

                        bw.Write(jokerSO.ChipsPerDollar);

                        bw.Write(jokerSO.MultiplierMultForSpecialBall);
                        bw.Write(jokerSO.MultiplierMultForNonSpecialBall);

                        bw.Write(jokerSO.SortSlots);
                        int slotID = jokerSO.FirstBallConvertSlot == null ? -1 : jokerSO.FirstBallConvertSlot.ID;
                        bw.Write(slotID);

                        bw.Write(jokerSO.AddSpin);
                        bw.Write(jokerSO.LastSpinMultiplierAdd);

                        bw.Write(jokerSO.BallIncreaseMultRemoveSlotMod);
                        bw.Write(jokerSO.AddMultipierMultRemoveAllSlotMod);
                        bw.Write(jokerSO.ChipsAddForEveryNonSlotMod);
                        bw.Write(jokerSO.MultiplierAddForEverySlotMod);
                        bw.Write(jokerSO.BallMultiplierAddForSlotMod);

                        bw.Write(jokerSO.MultiplierAddForLeastPlayedColor);

                        slotID = jokerSO.StartRoundChangeSlotID == null ? -1 : jokerSO.StartRoundChangeSlotID.ID;
                        bw.Write(slotID);

                        bw.Write(jokerSO.RetriggerBallsEverySpin);
                        bw.Write(jokerSO.RetriggerBallsLastSpin);

                        bw.Write(jokerSO.MultiplierMultEveryShopReroll);
                        bw.Write(jokerSO.MultiplierMultEveryCardPackReroll);

                        bw.Write(jokerSO.MultiplierAddForCardpackAbandon);

                        bw.Write(jokerSO.TriggerEveryXSpins);

                    }

                    objects.Clear();
                    AddObjectsFromDirectory("Assets/Data/Balls", objects, typeof(BallSO));
                    int numBalls = objects.Count;
                    Debug.Log("numBalls " + numBalls);
                    bw.Write(numBalls);
                    for (int ballIdx = 0; ballIdx < numBalls; ballIdx++)
                    {
                        BallSO ballSO = (BallSO)objects[ballIdx];
                        bw.Write(ballSO.BallSprite.name);
                        bw.Write(ballSO.BallDescription);
                        bw.Write(ballSO.BallChips); // how many chips does this ball add?
                        bw.Write(ballSO.BallMultiplierAdd); // how much multiplier does this ball add?
                        bw.Write(ballSO.BallMultiplierMult); // by how much do we multiply the multiplier
                        bw.Write(ballSO.BallMoney); // how much money does this ball add?
                        bw.Write(ballSO.BallRevertChance); // chance ball loses its specialty
                        for (int i = 0; i < 4; i++)
                            bw.Write((float)ballSO.BallColorMultiplier[i]);
                    }

                    objects.Clear();
                    AddObjectsFromDirectory("Assets/Data/CardPackBalls", objects, typeof(CardPackBallSO));
                    int numCardPackBalls = objects.Count;
                    Debug.Log("numCardPackBalls " + numCardPackBalls);
                    bw.Write(numCardPackBalls);
                    for (int cardPackIdx = 0; cardPackIdx < numCardPackBalls; cardPackIdx++)
                    {
                        CardPackBallSO cardPackBallSO = (CardPackBallSO)objects[cardPackIdx];
                        int numBallsInPack = cardPackBallSO.BallSO.Length;
                        bw.Write(numBallsInPack);
                        bw.Write(cardPackBallSO.BallSO[0].ID);
                        bw.Write(cardPackBallSO.Weight);
                        bw.Write(cardPackBallSO.DescriptionGO.name);
                        bw.Write((byte)cardPackBallSO.AffectSlotType);
                    }

                    objects.Clear();
                    AddObjectsFromDirectory("Assets/Data/CardPackSlots", objects, typeof(CardPackSlotSO));
                    int numCardPackSlots = objects.Count;
                    Debug.Log("numCardPackSlots " + numCardPackSlots);
                    bw.Write(numCardPackSlots);
                    for (int cardPackIdx = 0; cardPackIdx < numCardPackSlots; cardPackIdx++)
                    {
                        CardPackSlotSO cardPackSlotSO = (CardPackSlotSO)objects[cardPackIdx];
                        bw.Write((byte)cardPackSlotSO.SlotChangeType);
                        bw.Write(cardPackSlotSO.NumSlots);
                        bw.Write(cardPackSlotSO.Weight);
                        bw.Write(cardPackSlotSO.DescriptionGO.name);
                        bw.Write((byte)cardPackSlotSO.AffectSlotType);

                        bw.Write(cardPackSlotSO.Chips);
                        bw.Write(cardPackSlotSO.MultiplierAdd);
                        bw.Write(cardPackSlotSO.MultiplierMult);
                        bw.Write(cardPackSlotSO.Money);
                    }

                    objects.Clear();
                    AddObjectsFromDirectory("Assets/Data/CardPackChips", objects, typeof(CardPackChipsSO));
                    int numCardPackChips = objects.Count;
                    Debug.Log("numCardPackChips " + numCardPackChips);
                    bw.Write(numCardPackChips);
                    for (int cardPackIdx = 0; cardPackIdx < numCardPackChips; cardPackIdx++)
                    {
                        CardPackChipsSO cardPackChipsSO = (CardPackChipsSO)objects[cardPackIdx];
                        bw.Write(cardPackChipsSO.Weight);
                        bw.Write(cardPackChipsSO.DescriptionGO.name);
                        bw.Write((byte)cardPackChipsSO.AffectSlotType);
                    }

                    objects.Clear();
                    AddObjectsFromDirectory("Assets/Data/Skips", objects, typeof(SkipSO));
                    int numSkips = objects.Count;
                    Debug.Log("numSkips " + numSkips);
                    bw.Write(numSkips);
                    for (int skipIdx = 0; skipIdx < numSkips; skipIdx++)
                    {
                        SkipSO skipSO = (SkipSO)objects[skipIdx];
                        bw.Write(skipSO.SkipDescription);
                        bw.Write(skipSO.MoneyNow);
                        bw.Write(skipSO.MoneyAfterBoss);
                        bw.Write((byte)skipSO.JokerRarity);
                        bw.Write(skipSO.ExtraSpin);
                        bw.Write(skipSO.Change2SlotsToPlayedColor);
                        bw.Write(skipSO.DoubleMoney);
                        bw.Write(skipSO.SortSlots);
                        bw.Write(skipSO.MoneyForSpinsUsed);
                        bw.Write(skipSO.MoneyForSpinsUnused);
                        bw.Write(skipSO.BossReroll);
                        bw.Write(skipSO.CardPackIdx);
                        bw.Write(skipSO.CanShowFirstTwoRounds);
                    }

                    objects.Clear();
                    AddObjectsFromDirectory("Assets/Data/Bosses", objects, typeof(BossSO));
                    int numBosses = objects.Count;
                    Debug.Log("numBosses " + numBosses);
                    bw.Write(numBosses);
                    for (int bossIdx = 0; bossIdx < numBosses; bossIdx++)
                    {
                        BossSO bossSO = (BossSO)objects[bossIdx];
                        bw.Write((byte)bossSO.bossEffects);
                        bw.Write(bossSO.Description);
                        bw.Write((int)bossSO.LevelRange.x);
                        bw.Write((int)bossSO.LevelRange.y);
                    }

                    objects.Clear();
                    AddObjectsFromDirectory("Assets/Data/Vouchers", objects, typeof(VoucherSO));
                    int numVouchers = objects.Count;
                    Debug.Log("numVouchers " + numVouchers);
                    bw.Write(numVouchers);
                    for (int voucherIdx = 0; voucherIdx < numVouchers; voucherIdx++)
                    {
                        VoucherSO voucherSO = (VoucherSO)objects[voucherIdx];
                        bw.Write((byte)voucherSO.VoucherType);
                        bw.Write(voucherSO.Description);
                        bw.Write(voucherSO.VoucherSprite.name);
                        bw.Write(voucherSO.Repeatable ? (byte)1 : (byte)0);
                    }

                    objects.Clear();
                    AddObjectsFromDirectory("Assets/Data/SpinWheels", objects, typeof(SpinWheelSO));
                    int numSpinWheels = objects.Count;
                    Debug.Log("numSpinWheels " + numSpinWheels);
                    bw.Write(numSpinWheels);
                    for (int swIdx = 0; swIdx < numSpinWheels; swIdx++)
                    {
                        SpinWheelSO spinWheelSO = (SpinWheelSO)objects[swIdx];
                        bw.Write(spinWheelSO.Description);
                        bw.Write(spinWheelSO.Spins);
                        bw.Write(spinWheelSO.StartingMoney);
                        bw.Write(spinWheelSO.GoalMultiplier);
                        bw.Write(spinWheelSO.SlotType.Length);
                        for (int i = 0; i < spinWheelSO.SlotType.Length; i++)
                            bw.Write((byte)spinWheelSO.SlotType[i]);
                    }
                }
                return stream.ToArray();
            }
        }

        public static void AddObjectsFromDirectory(string path, List<Object> items, System.Type type)
        {
            if (Directory.Exists(path))
            {
                string[] assets = Directory.GetFiles(path);
                foreach (string assetPath in assets)
                    if (assetPath.Contains(".asset") && !assetPath.Contains(".meta"))
                        items.Add(AssetDatabase.LoadAssetAtPath(assetPath, type));

                string[] directories = Directory.GetDirectories(path);
                foreach (string directory in directories)
                    if (Directory.Exists(directory))
                        AddObjectsFromDirectory(directory, items, type);
            }
        }
#endif
    }
}
