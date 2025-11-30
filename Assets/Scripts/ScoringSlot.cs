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
    public class ScoringSlot : MonoBehaviour
    {
        public int Index;
        public SpriteRenderer SpriteRenderer;
        public GameObject ChipsGO;
        public GameObject MultGO;
        public GameObject BonusGO;
        public GameObject MoneyGO;
        public GameObject DebuffedGO;
        public GameObject LockGO;

        public void SetSlotColor(Color color)
        {
            SpriteRenderer.color = color;
        }

        void OnTriggerEnter2D(Collider2D col)
        {
            int ballIdx = int.Parse(col.name);
            Game.Instance.BallInSlot(ballIdx, Index);
        }
    }
}