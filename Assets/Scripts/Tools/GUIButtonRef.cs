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


    [Serializable]
    public class GUIButtonData
    {
        public string Key;
        public Button Button;
        public GUIButtonNavigationData NavigationData;
    }

    public enum NAV_DIRECTION { UP, DOWN, LEFT, RIGHT };

    public class GUIButtonRef : MonoBehaviour
    {
        public GUIButtonData[] Buttons;

#if UNITY_EDITOR
        // validate
        public void Start()
        {
            for (int i = 0; i < Buttons.Length; i++)
            {
                GetButtonData(Buttons[i].NavigationData.Up);
                GetButtonData(Buttons[i].NavigationData.Down);
                GetButtonData(Buttons[i].NavigationData.Left);
                GetButtonData(Buttons[i].NavigationData.Right);
            }
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