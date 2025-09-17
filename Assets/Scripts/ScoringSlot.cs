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