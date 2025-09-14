using UnityEngine;

namespace Cardwheel
{
    [CreateAssetMenu(fileName = "CardPackChipsSO", menuName = "Cardwheel/CardPackChipsSO", order = 1)]
    public class CardPackChipsSO : ScriptableObject
    {
        public int Weight;
        public GameObject DescriptionGO;        
        public SLOT_TYPE AffectSlotType = SLOT_TYPE.NONE;
    }
}