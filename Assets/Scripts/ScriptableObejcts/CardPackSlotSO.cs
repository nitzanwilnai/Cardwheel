using UnityEngine;

namespace Cardwheel
{
    [CreateAssetMenu(fileName = "CardPackSlotSO", menuName = "Cardwheel/CardPackSlotSO", order = 1)]
    public class CardPackSlotSO : ScriptableObject
    {
        [HideInInspector] public int ID;
        public SLOT_CHANGE_TYPE SlotChangeType;
        public int NumSlots;
        public int Weight;
        public GameObject DescriptionGO;
        public SLOT_TYPE AffectSlotType = SLOT_TYPE.NONE;

        public int Chips;
        public int MultiplierAdd;
        public int MultiplierMult;
        public int Money;
        
    }
}