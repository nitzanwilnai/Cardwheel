using UnityEngine;

namespace Cardwheel
{
    [CreateAssetMenu(fileName = "VoucherSO", menuName = "Cardwheel/VoucherSO", order = 1)]
    public class VoucherSO : ScriptableObject
    {
        public VOUCHER_TYPE VoucherType;
        [TextArea] public string Description;
        public Sprite VoucherSprite;
        public bool Repeatable;
    }
}