using UnityEngine;
using System;

namespace Cardwheel
{
    [Serializable]
    public struct ShopCardPackInfo
    {
        public CARD_PACK_TYPE CardPackType;
        public int PickCards;
        public int MaxCards;
        public int Cost;
        public int Weight;
    }

    [CreateAssetMenu(fileName = "ShopCardPacksSO", menuName = "Cardwheel/ShopCardPacksSO", order = 1)]
    public class ShopCardPacksSO : ScriptableObject
    {
        public ShopCardPackInfo[] ShopCardPackInfo;
    }
}