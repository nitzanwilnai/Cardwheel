using UnityEngine;

namespace Cardwheel
{

    [CreateAssetMenu(fileName = "SpinWheelSO", menuName = "Cardwheel/SpinWheelSO", order = 1)]
    public class SpinWheelSO : ScriptableObject
    {
        [TextArea] public string Description;
        public int Spins = 4;
        public int StartingMoney = 4;
        public float GoalMultiplier = 1.0f;
        public SLOT_TYPE[] SlotType;
    }
}