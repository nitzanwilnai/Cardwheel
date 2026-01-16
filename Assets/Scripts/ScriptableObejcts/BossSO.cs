/*
  Cardwheel — Non-Commercial, No-Modification License
  Copyright © 2025 Nitzan Wilnai
  Source Code: https://github.com/nitzanwilnai/Cardwheel

  Permission is granted to view and run this code for non-commercial purposes only.
  Modification, redistribution of altered versions, and commercial use are strictly prohibited.

  See the LICENSE file for full legal terms.
*/

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
        public bool EndlessMode;
    }
}