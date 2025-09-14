using UnityEngine;

namespace Cardwheel
{
    public enum BOSS_DIFFICULTY { EASY, MEDIUM, HARD };

    [CreateAssetMenu(fileName = "BossSO", menuName = "Cardwheel/BossSO", order = 1)]
    public class BossSO : ScriptableObject
    {

        public BOSS_EFFECT bossEffects;
        public string Description;
        public Vector2 LevelRange;
    }
}