using UnityEngine;

namespace Cardwheel
{
    [CreateAssetMenu(fileName = "CardPackBallSO", menuName = "Cardwheel/CardPackBallSO", order = 1)]
    public class CardPackBallSO : ScriptableObject
    {
        public BallSO[] BallSO;
        public int Weight;
        public GameObject DescriptionGO;
        public SLOT_TYPE AffectSlotType = SLOT_TYPE.NONE;
    }
}