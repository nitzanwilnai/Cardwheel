using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

namespace Cardwheel
{
    public class RunData
    {
        public int WheelIdx;

        // user
        public int Money; //
        public uint StartSeed; //
        public uint GameSeed; //
        public uint ShopSeed; //
        public uint SkipSeed; //
        public uint BossSeed; //
        public uint[] RoundSeeds; //

        public MENU_STATE MenuState; //
        public MENU_STATE PrevMenuState; //

        // in game
        public int TotalChips; //
        public int SpinChips; //
        public float SpinMultiplier; //
        public int Round; //
        public int CurrentSpin; //
        public int ExtraSkipSpin; //
        public int MaxSpinsThisRound; //
        public int TotalSpins; //

        public int SpinsUsed; //
        public int SpinsUnused; //

        public float RotationSpeed; // 
        public float SpinWheelAngle; //

        public int[] BallTypes; //
        public int[] BallTypesInGame; //

        // gameplay
        public int[] SlotScored; //
        public SLOT_TYPE[] SlotType; //
        public SLOT_TYPE[] SlotTypeInGame; //
        public int[] SlotModType; //
        
        public int[] BallSlotIdx; //
        public float[] BallSnapVelocity; //
        public float[] BallSnapTime; //
        public int MoneyAfterBoss; //
        public Color[] SlotColors; //
        public int BossRerolls; //

        public SLOT_TYPE LeastPlayedColorAtRoundStart; //

        public int JokerBallTriggerIdx;

        // gameover data
        public int BestSpin; //
        public int[] ColorCount; //

        // scoring
        public int[] BaseChips; //
        public int[] BallScoreIdxs; //
        public int[] BallScoresCount; //

        // jokers
        public int JokerCount; //
        public int[] JokerTypes; //
        public int MaxJokersInHand; //
        public int[] JokerSellValues; //
        public int[] JokerChips; //
        public float[] JokerMultiplierAdd; //

        public int[] AvailableJokerTypes; //
        public int AvailableJokerCount; //

        // shop
        public int[] ShopJokerIdxs; //
        public int ShopJokerCount; //
        public int[] VoucherIdxs; //
        public int[] ShopCardPackIdxs; //
        public int ShopRerollCount; // 
        public int CardPackRerollCount; //
        public int ShopRerollTotal; //
        public int CardPackRerollTotal; //

        // vouchers
        public bool VoucherPurchased; //
        public int VoucherSpins; //
        public int VoucherMaxInterest; //
        public float VoucherShopDiscount; //
        public int VoucherShopRerollsDiscount; //
        public int VoucherCardPackRerollDiscount; //
        public bool VoucherCardPackMostPlayedColor; //
        public float VoucherRareJoker; //
        public bool VoucherSlotMostPlayedColor; //

        // card packs
        public int SelectedShopCardPackIdx; //
        public bool[] CardPackBallSelected; //
        public int[] CardPackCardIdxs; //

        // skips
        public int[] SkipType; //
        public int SkipShopUncommonJoker; //
        public int SkipShopRareJoker; //

        // bosses
        public int[] BossType; //
        public int UseBallsSpecial; //
        public int UseSlotsSpecial; //
        public int[] UseSlotType; //
        public int UseBaseChips; //
        public int[] UseJoker; //
    }
}