/*
  Cardwheel — Non-Commercial, No-Modification License
  Copyright © 2025 Nitzan Wilnai
  Source Code: https://github.com/nitzanwilnai/Cardwheel

  Permission is granted to view and run this code for non-commercial purposes only.
  Modification, redistribution of altered versions, and commercial use are strictly prohibited.

  See the LICENSE file for full legal terms.
*/

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