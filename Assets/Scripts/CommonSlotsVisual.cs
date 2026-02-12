/*
  Cardwheel — Non-Commercial, No-Modification License
  Copyright © 2025 Nitzan Wilnai
  Source Code: https://github.com/nitzanwilnai/Cardwheel

  Permission is granted to view and run this code for non-commercial purposes only.
  Modification, redistribution of altered versions, and commercial use are strictly prohibited.

  See the LICENSE file for full legal terms.
*/

using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using Cardwheel;
using UnityEngine;
using UnityEngine.Video;

public static class CommonSlotsVisual
{
    public static int[] ChangedSlotsIdxs;
    public static int ChangedSlotsCount;

    public static float SortingTime = 1.0f;
    public static float SortingTimer = 0.0f;


    public static void Init(Balance balance)
    {
        ChangedSlotsIdxs = new int[balance.NumSlots];
        ChangedSlotsCount = 0;
    }

    public static void TickHighlightChangedSlots(RunData runData, Balance balance, float value, AnimationCurve SlotScaleCurve, ScoringSlot[] scoringSlots, SLOT_TYPE[] slotTypes)
    {
        if (value > 1.0f)
            value = 1.0f;
        float colorMult = SlotScaleCurve.Evaluate(value) * 0.5f;
        for (int i = 0; i < ChangedSlotsCount; i++)
        {
            int slotIdx = ChangedSlotsIdxs[i];
            int slotType = (int)slotTypes[slotIdx];
            Color color = runData.UseSlot[slotIdx] == 1 ? balance.SlotColors[slotType] : balance.SlotOffColor;
            scoringSlots[slotIdx].SpriteRenderer.color = color + Color.white * colorMult;
        }
        if (value >= 1.0f)
            ChangedSlotsCount = 0;
    }

    public static void TickSpinWheelUI(RunData runData, float rotationSpeed, float dt, CardsBallsSpinWheelGUI m_cardsBallsSpinWheelGUI)
    {
        runData.SpinWheelAngle += rotationSpeed * dt;
        m_cardsBallsSpinWheelGUI.SpinCircle.Angle = runData.SpinWheelAngle;
    }

    public static void TickSortingPopup(float dt, CardsBallsSpinWheelGUI m_cardsBallsSpinWheelGUI)
    {
        if (SortingTimer > 0.0f)
        {
            SortingTimer -= dt;
            if (SortingTimer <= 0.0f)
                m_cardsBallsSpinWheelGUI.SortingPopup.SetActive(false);
        }

    }

    public static void TrySortSlots(RunData runData, Balance balance, CardsBallsSpinWheelGUI cardsBallsSpinWheelGUI)
    {
        if (!Logic.AreSlotsSorted(runData))
        {
            SortSlots(runData, balance, cardsBallsSpinWheelGUI);

            for (int slotIdx = 0; slotIdx < balance.NumSlots; slotIdx++)
                runData.SlotTypeInGame[slotIdx] = runData.SlotType[slotIdx];
        }
    }

    public static void SortSlots(RunData runData, Balance balance, CardsBallsSpinWheelGUI cardsBallsSpinWheelGUI)
    {
        Logic.SortSlots(runData);
        cardsBallsSpinWheelGUI.SortingPopup.SetActive(true);
        SortingTimer = SortingTime;
        ShowSpinWheelUI(runData, balance, cardsBallsSpinWheelGUI.ScoringSlots, runData.SlotType);
    }

    public static void SortSlotsRoundSelection(RunData runData, Balance balance, CardsBallsSpinWheelGUI cardsBallsSpinWheelGUI)
    {
        Logic.SortSlots(runData);
        Logic.SetDataForNextRound(runData, balance, ChangedSlotsIdxs, ref ChangedSlotsCount);
        cardsBallsSpinWheelGUI.SortingPopup.SetActive(true);
        SortingTimer = SortingTime;
        ShowSpinWheelUI(runData, balance, cardsBallsSpinWheelGUI.ScoringSlots, runData.SlotType);
    }

    public static void ShowSpinWheelUI(RunData runData, Balance balance, ScoringSlot[] scoringSlots, SLOT_TYPE[] slotType)
    {
        ShowSpinWheel(runData, balance, scoringSlots, slotType, true, 15);
    }

    public static void ShowSpinWheel(RunData runData, Balance balance, ScoringSlot[] scoringSlots, SLOT_TYPE[] slotTypes, bool showSlotEffects, int slotsDebuffed)
    {
        for (int slotIdx = 0; slotIdx < scoringSlots.Length; slotIdx++)
        {
            int slotType = (int)slotTypes[slotIdx];
            Color color = runData.UseSlot[slotIdx] == 1 ? balance.SlotColors[slotType] : balance.SlotOffColor;
            scoringSlots[slotIdx].SetSlotColor(color);

            int slotModType = runData.SlotModType[slotIdx];
            scoringSlots[slotIdx].ChipsGO.SetActive(showSlotEffects && slotModType > -1 && balance.CardPackSlotBalance.Chips[slotModType] > 0);
            scoringSlots[slotIdx].MultGO.SetActive(showSlotEffects && slotModType > -1 && balance.CardPackSlotBalance.MultiplierAdd[slotModType] > 0);
            scoringSlots[slotIdx].MoneyGO.SetActive(showSlotEffects && slotModType > -1 && balance.CardPackSlotBalance.Money[slotModType] > 0);
            scoringSlots[slotIdx].BonusGO.SetActive(showSlotEffects && slotModType > -1 && balance.CardPackSlotBalance.MultiplierMult[slotModType] > 0);

            bool slotDebuffed = !Logic.IsFlagSet(slotsDebuffed, slotType);
            scoringSlots[slotIdx].DebuffedGO.SetActive(slotDebuffed && slotModType > -1);
        }
    }

    public static void ShowSpinWheelForRound(RunData runData, Balance balance, ScoringSlot[] scoringSlots, int round)
    {
        bool showSlotBuffs = ShowSlotBuffsForRound(runData, balance, round);

        int useSlotBuffs = 15; // (1111)
        if (Logic.InBossRound(runData))
            useSlotBuffs = Logic.GetSlotBuffsForBoss(runData, balance);

        ShowSpinWheel(runData, balance, scoringSlots, runData.SlotTypeInGame, showSlotBuffs, useSlotBuffs);
    }

    public static bool ShowSlotBuffsForRound(RunData runData, Balance balance, int round)
    {
        bool showSlotEffects = true;

        if (Logic.InBossRound(round))
        {
            int bossType = Logic.GetBossTypeForRound(runData, round);

            if (balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.SLOT_EFFECTS_HIDDEN)
                showSlotEffects = false;
        }

        return showSlotEffects;
    }
}
