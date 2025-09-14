using UnityEngine;

namespace Cardwheel
{
    [CreateAssetMenu(fileName = "BallSO", menuName = "Cardwheel/BallSO", order = 1)]
    public class BallSO : ScriptableObject
    {
        [HideInInspector] public int ID;
        public Sprite BallSprite;
        [TextArea] public string BallDescription;
        public int BallChips = 0; // how many chips does this ball add?
        public int BallMultiplierAdd = 0; // how much multiplier does this ball add?
        public float BallMultiplierMult = 1.0f; // by how much do we multiply the multiplier
        public int BallMoney = 0; // how much money does this ball add?
        public float BallRevertChance = 0.0f; // chance ball loses its specialty
        public int[] BallColorMultiplier = { 1, 1, 1, 1 }; // ball only scores if it lands on this color
    }
}
