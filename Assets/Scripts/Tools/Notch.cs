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
    public class Notch : MonoBehaviour
    {
        public float yOffset;

        private void Awake()
        {
            if ((float)Screen.width / (float)Screen.height < 0.5f)
            {
                Vector3 position = gameObject.transform.localPosition;
                position.y += yOffset;
                gameObject.transform.localPosition = position;
            }

        }
    }
}