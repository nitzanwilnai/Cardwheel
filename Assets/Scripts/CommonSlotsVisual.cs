using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using Cardwheel;
using UnityEngine;
using UnityEngine.Video;

public static class CommonSlotsVisual
{
    public static int[] AffectedSlotsIdxs;
    public static int AffectedSlotsCount;

    public static float SortingTime = 1.0f;
    public static float SortingTimer = 0.0f;

    public static void Init(Balance balance)
    {
        AffectedSlotsIdxs = new int[balance.NumSlots];
        AffectedSlotsCount = 0;
    }

    public static void TickHighlightChangedSlots(float value, AnimationCurve SlotScaleCurve, ScoringSlot[] scoringSlots, SLOT_TYPE[] slotTypes, Color[] slotColors)
    {
        if (value > 1.0f)
            value = 1.0f;
        float colorMult = SlotScaleCurve.Evaluate(value) * 0.5f;
        for (int i = 0; i < AffectedSlotsCount; i++)
        {
            int slotIdx = AffectedSlotsIdxs[i];
            int slotType = (int)slotTypes[slotIdx];
            scoringSlots[slotIdx].SpriteRenderer.color = slotColors[slotType] + Color.white * colorMult;
        }
        if (value >= 1.0f)
            AffectedSlotsCount = 0;

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
        Logic.SetDataForNextRound(runData, balance);
        cardsBallsSpinWheelGUI.SortingPopup.SetActive(true);
        SortingTimer = SortingTime;
        ShowSpinWheelUI(runData, balance, cardsBallsSpinWheelGUI.ScoringSlots, runData.SlotType);
    }

    public static void ShowSpinWheelUI(RunData runData, Balance balance, ScoringSlot[] scoringSlots, SLOT_TYPE[] slotType)
    {
        ShowSpinWheel(runData, balance, scoringSlots, slotType, true, false);
    }
    public static void ShowSpinWheel(RunData runData, Balance balance, ScoringSlot[] scoringSlots, SLOT_TYPE[] slotType, bool showSlotEffects, bool slotsDebuffed)
    {
        for (int i = 0; i < scoringSlots.Length; i++)
        {
            scoringSlots[i].SetSlotColor(runData.SlotColors[(int)slotType[i]]);

            int slotModType = runData.SlotModType[i];
            scoringSlots[i].ChipsGO.SetActive(showSlotEffects && slotModType > -1 && balance.CardPackSlotBalance.Chips[slotModType] > 0);
            scoringSlots[i].MultGO.SetActive(showSlotEffects && slotModType > -1 && balance.CardPackSlotBalance.MultiplierAdd[slotModType] > 0);
            scoringSlots[i].MoneyGO.SetActive(showSlotEffects && slotModType > -1 && balance.CardPackSlotBalance.Money[slotModType] > 0);
            scoringSlots[i].BonusGO.SetActive(showSlotEffects && slotModType > -1 && balance.CardPackSlotBalance.MultiplierMult[slotModType] > 0);
            scoringSlots[i].DebuffedGO.SetActive(slotsDebuffed && slotModType > -1);
        }
    }

    public static void ShowSpinWheelForRound(RunData runData, Balance balance, ScoringSlot[] scoringSlots, int round)
    {
        bool showSlotEffects;
        bool slotsDebuffed;

        CheckSpinWheelDebuffForNewRound(runData, balance, round, out showSlotEffects, out slotsDebuffed);

        ShowSpinWheel(runData, balance, scoringSlots, runData.SlotTypeInGame, showSlotEffects, slotsDebuffed);
    }

    public static void CheckSpinWheelDebuffForNewRound(RunData runData, Balance balance, int round, out bool showSlotEffects, out bool slotsDebuffed)
    {
        showSlotEffects = true;
        slotsDebuffed = false;

        if (round % 3 == 2)
        {
            int bossType = Logic.GetBossTypeForRound(runData);

            if (balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.SLOT_EFFECTS_HIDDEN)
                showSlotEffects = false;

            if (balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.SLOTS_DEBUFFED ||
                balance.BossBalance.BossEffect[bossType] == BOSS_EFFECT.SLOTS_DEBUFFED_FIRST_SPIN)
                slotsDebuffed = true;
        }
    }
}
