/*
  Cardwheel — Non-Commercial, No-Modification License
  Copyright © 2025 Nitzan Wilnai
  Source Code: https://github.com/nitzanwilnai/Cardwheel

  Permission is granted to view and run this code for non-commercial purposes only.
  Modification, redistribution of altered versions, and commercial use are strictly prohibited.

  See the LICENSE file for full legal terms.
*/

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonTools
{
    public class NotchAutoPad : MonoBehaviour
    {
        public float Amount;

        private void Awake()
        {
            RectTransform rectTransform = GetComponent<RectTransform>();
            if ((float)Screen.width / (float)Screen.height < 0.5f)
                rectTransform.sizeDelta = new Vector2(0.0f, Amount);
            else
                rectTransform.sizeDelta = Vector2.zero;

        }
    }
}