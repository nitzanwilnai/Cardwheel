using UnityEngine;
using UnityEngine.UI;
using CommonTools;
using UnityEngine.InputSystem;

namespace Cardwheel
{
    public class ButtonNavigationIndices
    {
        public int[] Directions = new int[4];
        public string Key;
        public Button Button;
        public GameObject Selected;
    }

    public static class CommonButtonVisual
    {
        public static void InitButtonNavigationData(GUIButtonRef guiButtonRef, ref ButtonNavigationIndices[] m_buttonNavigationIndices)
        {
            m_buttonNavigationIndices = new ButtonNavigationIndices[guiButtonRef.Buttons.Length];
            for (int i = 0; i < guiButtonRef.Buttons.Length; i++)
            {
                m_buttonNavigationIndices[i] = new ButtonNavigationIndices();
                m_buttonNavigationIndices[i].Key = guiButtonRef.Buttons[i].Key;

                m_buttonNavigationIndices[i].Button = guiButtonRef.Buttons[i].Button;
                m_buttonNavigationIndices[i].Selected = AssetManager.Instance.GetButtonSelected(m_buttonNavigationIndices[i].Button.transform);
                m_buttonNavigationIndices[i].Selected.SetActive(false);

                m_buttonNavigationIndices[i].Directions[(int)NAV_DIRECTION.UP] = guiButtonRef.GetButtonIndex(guiButtonRef.Buttons[i].NavigationData.Up);
                m_buttonNavigationIndices[i].Directions[(int)NAV_DIRECTION.DOWN] = guiButtonRef.GetButtonIndex(guiButtonRef.Buttons[i].NavigationData.Down);
                m_buttonNavigationIndices[i].Directions[(int)NAV_DIRECTION.LEFT] = guiButtonRef.GetButtonIndex(guiButtonRef.Buttons[i].NavigationData.Left);
                m_buttonNavigationIndices[i].Directions[(int)NAV_DIRECTION.RIGHT] = guiButtonRef.GetButtonIndex(guiButtonRef.Buttons[i].NavigationData.Right);
            }
        }


        public static void handleInput(ButtonNavigationIndices[] buttonNavigationIndices, ref int selectedButtonIndex)
        {
            if (Keyboard.current[Key.UpArrow].wasReleasedThisFrame)
            {
                buttonNavigationIndices[selectedButtonIndex].Selected.SetActive(false);
                selectedButtonIndex = buttonNavigationIndices[selectedButtonIndex].Directions[(int)NAV_DIRECTION.UP];
                buttonNavigationIndices[selectedButtonIndex].Selected.SetActive(true);
            }
            if (Keyboard.current[Key.DownArrow].wasReleasedThisFrame)
            {
                buttonNavigationIndices[selectedButtonIndex].Selected.SetActive(false);
                selectedButtonIndex = buttonNavigationIndices[selectedButtonIndex].Directions[(int)NAV_DIRECTION.DOWN];
                buttonNavigationIndices[selectedButtonIndex].Selected.SetActive(true);
            }
            if (Keyboard.current[Key.LeftArrow].wasReleasedThisFrame)
            {
                buttonNavigationIndices[selectedButtonIndex].Selected.SetActive(false);
                selectedButtonIndex = buttonNavigationIndices[selectedButtonIndex].Directions[(int)NAV_DIRECTION.LEFT];
                buttonNavigationIndices[selectedButtonIndex].Selected.SetActive(true);
            }
            if (Keyboard.current[Key.RightArrow].wasReleasedThisFrame)
            {
                buttonNavigationIndices[selectedButtonIndex].Selected.SetActive(false);
                selectedButtonIndex = buttonNavigationIndices[selectedButtonIndex].Directions[(int)NAV_DIRECTION.RIGHT];
                buttonNavigationIndices[selectedButtonIndex].Selected.SetActive(true);
            }
        }
    }
}