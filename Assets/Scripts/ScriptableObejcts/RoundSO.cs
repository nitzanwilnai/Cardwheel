using UnityEngine;

namespace Cardwheel
{
    [CreateAssetMenu(fileName = "RoundSO", menuName = "Cardwheel/RoundSO", order = 1)]
    public class RoundSO : ScriptableObject
    {
        public int[] BaseChip;
        public float[] RoundChipMult;
        public int[] RoundReward;
    }
}