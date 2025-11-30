/*
  Cardwheel — Non-Commercial, No-Modification License
  Copyright © 2025 Nitzan Wilnai
  Source Code: https://github.com/nitzanwilnai/Cardwheel

  Permission is granted to view and run this code for non-commercial purposes only.
  Modification, redistribution of altered versions, and commercial use are strictly prohibited.

  See the LICENSE file for full legal terms.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CommonTools
{
    [Serializable]
    public class GUIButtonNavigationData
    {
        public string Up;
        public string Down;
        public string Left;
        public string Right;
    }


    public enum GAMEPAD_BUTTON { NONE, NORTH, SOUTH, WEST, EAST, UP, DOWN, LEFT, RIGHT, R1, R2, L1, L2, OPTIONS };

    [Serializable]
    public class GUIButtonData
    {
        public string Key;
        public Button Button;
        public Image GlyphImage;
        public GAMEPAD_BUTTON GamepadButton;
        public GameObject SelectedGO;
    }

    public enum NAV_DIRECTION { UP, DOWN, LEFT, RIGHT };

    public class GUIButtonRef : MonoBehaviour
    {
        public GUIButtonData[] Buttons;

#if UNITY_EDITOR
        // validate
        public void Start()
        {
        }
#endif

        public int GetButtonIndex(string key)
        {
            for (int i = 0; i < Buttons.Length; i++)
                if (Buttons[i].Key == key)
                    return i;
            Debug.LogError("GUIButtonData GetButtonIndex(" + key + ")  missing!");
            return -1;
        }

        public GUIButtonData GetButtonData(string key)
        {
            for (int i = 0; i < Buttons.Length; i++)
                if (Buttons[i].Key == key)
                    return Buttons[i];
            Debug.LogError("GUIButtonData GetButton(" + key + ")  missing!");
            return null;
        }
    }
}