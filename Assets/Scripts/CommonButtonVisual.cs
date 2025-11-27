using UnityEngine;
using CommonTools;
using UnityEngine.InputSystem;

namespace Cardwheel
{
    public enum GAMEPAD_TYPE { NONE, STEAM, PS5, SWITCH, XBOX };
    public enum COMMON_BUTTONS
    {
        SETTINGS = 10,
        WHEEL = 11,
        BALLS = 12,
        JOKER_1 = 20,
        JOKER_2 = 21,
        JOKER_3 = 22,
        JOKER_4 = 23,
        JOKER_5 = 24,
        BALL_1 = 30,
        BALL_2 = 31,
        BALL_3 = 32,
        BALL_4 = 33,
        BALL_5 = 34,
        BALL_6 = 35,
    }

    public static class CommonButtonVisual
    {
        public static double LastKeyboardInputTime = 0.0d;

        public static void UpdateButtonIcons(GUIButtonData guiButtonData, GAMEPAD_TYPE gamepadType)
        {
            if (guiButtonData.GlyphImage != null)
            {
                guiButtonData.GlyphImage.gameObject.SetActive(gamepadType != GAMEPAD_TYPE.NONE);
                guiButtonData.GlyphImage.sprite = AssetManager.Instance.GetGamepadGlyph(gamepadType, guiButtonData.GamepadButton - 1);
            }
        }

        public static void AddSelectedBorder(GUIButtonData gUIButtonData)
        {
            gUIButtonData.SelectedGO = AssetManager.Instance.GetButtonSelected(gUIButtonData.Button.transform);
            gUIButtonData.SelectedGO.SetActive(false);
        }

        public static bool NavigateEnter()
        {
            if (Gamepad.current != null)
                if (Gamepad.current.buttonSouth.wasReleasedThisFrame)
                {
                    LastKeyboardInputTime = Time.realtimeSinceStartupAsDouble;
                    return true;
                }


            if (Keyboard.current != null)
                if (Keyboard.current.enterKey.wasReleasedThisFrame)
                {
                    LastKeyboardInputTime = Time.realtimeSinceStartupAsDouble;
                    return true;
                }

            return false;
        }

        public static bool NavigateEnterHold()
        {
            if (Gamepad.current != null)
                if (Gamepad.current.buttonSouth.isPressed)
                {
                    LastKeyboardInputTime = Time.realtimeSinceStartupAsDouble;
                    return true;
                }


            if (Keyboard.current != null)
                if (Keyboard.current.enterKey.isPressed)
                {
                    LastKeyboardInputTime = Time.realtimeSinceStartupAsDouble;
                    return true;
                }

            return false;
        }

        public static bool NavigateUp()
        {
            if (Gamepad.current != null)
                if (Gamepad.current.dpad.up.wasPressedThisFrame || Gamepad.current.leftStick.up.wasPressedThisFrame)
                {
                    LastKeyboardInputTime = Time.realtimeSinceStartupAsDouble;
                    return true;
                }

            if (Keyboard.current != null)
                if (Keyboard.current.upArrowKey.wasPressedThisFrame)
                {
                    LastKeyboardInputTime = Time.realtimeSinceStartupAsDouble;
                    return true;
                }

            return false;
        }

        public static bool NavigateDown()
        {
            if (Gamepad.current != null)
                if (Gamepad.current.dpad.down.wasPressedThisFrame || Gamepad.current.leftStick.down.wasPressedThisFrame)
                {
                    LastKeyboardInputTime = Time.realtimeSinceStartupAsDouble;
                    return true;
                }

            if (Keyboard.current != null)
                if (Keyboard.current.downArrowKey.wasPressedThisFrame)
                {
                    LastKeyboardInputTime = Time.realtimeSinceStartupAsDouble;
                    return true;
                }

            return false;
        }

        public static bool NavigateRight()
        {
            if (Gamepad.current != null)
                if (Gamepad.current.dpad.right.wasPressedThisFrame || Gamepad.current.leftStick.right.wasPressedThisFrame)
                {
                    LastKeyboardInputTime = Time.realtimeSinceStartupAsDouble;
                    return true;
                }


            if (Keyboard.current != null)
                if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
                {
                    LastKeyboardInputTime = Time.realtimeSinceStartupAsDouble;
                    return true;
                }

            return false;
        }

        public static bool NavigateLeft()
        {
            if (Gamepad.current != null)
                if (Gamepad.current.dpad.left.wasPressedThisFrame || Gamepad.current.leftStick.left.wasPressedThisFrame)
                {
                    LastKeyboardInputTime = Time.realtimeSinceStartupAsDouble;
                    return true;
                }

            if (Keyboard.current != null)
                if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
                {
                    LastKeyboardInputTime = Time.realtimeSinceStartupAsDouble;
                    return true;
                }

            return false;
        }

        public static bool NavigateGamepadButton(GUIButtonData guiButtonData)
        {
            if (guiButtonData.GamepadButton == GAMEPAD_BUTTON.NORTH)
                if (Gamepad.current != null)
                    if (Gamepad.current.buttonNorth.wasPressedThisFrame)
                    {
                        LastKeyboardInputTime = Time.realtimeSinceStartupAsDouble;
                        return true;
                    }

            if (guiButtonData.GamepadButton == GAMEPAD_BUTTON.SOUTH)
                if (Gamepad.current != null)
                    if (Gamepad.current.buttonSouth.wasPressedThisFrame)
                    {
                        LastKeyboardInputTime = Time.realtimeSinceStartupAsDouble;
                        return true;
                    }

            if (guiButtonData.GamepadButton == GAMEPAD_BUTTON.WEST)
                if (Gamepad.current != null)
                    if (Gamepad.current.buttonWest.wasPressedThisFrame)
                    {
                        LastKeyboardInputTime = Time.realtimeSinceStartupAsDouble;
                        return true;
                    }

            if (guiButtonData.GamepadButton == GAMEPAD_BUTTON.EAST)
                if (Gamepad.current != null)
                    if (Gamepad.current.buttonEast.wasPressedThisFrame)
                    {
                        LastKeyboardInputTime = Time.realtimeSinceStartupAsDouble;
                        return true;
                    }

            if (guiButtonData.GamepadButton == GAMEPAD_BUTTON.OPTIONS)
                if (Gamepad.current != null)
                    if (Gamepad.current.startButton.wasPressedThisFrame)
                    {
                        LastKeyboardInputTime = Time.realtimeSinceStartupAsDouble;
                        return true;
                    }

            if (guiButtonData.GamepadButton == GAMEPAD_BUTTON.R1)
                if (Gamepad.current != null)
                    if (Gamepad.current.rightTrigger.wasPressedThisFrame)
                    {
                        LastKeyboardInputTime = Time.realtimeSinceStartupAsDouble;
                        return true;
                    }

            if (guiButtonData.GamepadButton == GAMEPAD_BUTTON.L1)
                if (Gamepad.current != null)
                    if (Gamepad.current.leftTrigger.wasPressedThisFrame)
                    {
                        LastKeyboardInputTime = Time.realtimeSinceStartupAsDouble;
                        return true;
                    }

            if (guiButtonData.GamepadButton == GAMEPAD_BUTTON.R2)
                if (Gamepad.current != null)
                    if (Gamepad.current.rightShoulder.wasPressedThisFrame)
                    {
                        LastKeyboardInputTime = Time.realtimeSinceStartupAsDouble;
                        return true;
                    }

            if (guiButtonData.GamepadButton == GAMEPAD_BUTTON.L2)
                if (Gamepad.current != null)
                    if (Gamepad.current.leftShoulder.wasPressedThisFrame)
                    {
                        LastKeyboardInputTime = Time.realtimeSinceStartupAsDouble;
                        return true;
                    }

            return false;
        }

        public static void UpdateCommonButtonIcons(TopBarGUI topBarGUI, CardsBallsSpinWheelGUI cardsBallsSpinWheelGUI, GAMEPAD_TYPE gamepadType)
        {
            UpdateButtonIcons(topBarGUI.SettingsButtonData, gamepadType);
            UpdateButtonIcons(cardsBallsSpinWheelGUI.BallsButtonData, gamepadType);
            UpdateButtonIcons(cardsBallsSpinWheelGUI.SpinwheelButtonData, gamepadType);
        }

        public static bool CommonHandleInput(TopBarGUI topBarGUI, CardsBallsSpinWheelGUI cardsBallsSpinWheelGUI, COMMON_BUTTONS selectedButton)
        {
            if ((selectedButton == COMMON_BUTTONS.SETTINGS && NavigateEnter()) || NavigateGamepadButton(topBarGUI.SettingsButtonData))
            {
                Game.Instance.GoToSettings();
                return true;
            }

            if (CommonHandleInputNoTopBar(cardsBallsSpinWheelGUI, selectedButton))
                return true;

            return false;
        }

        public static bool CommonHandleInputNoTopBar(CardsBallsSpinWheelGUI cardsBallsSpinWheelGUI, COMMON_BUTTONS selectedButton)
        {
            if ((selectedButton == COMMON_BUTTONS.BALLS && NavigateEnter()) || NavigateGamepadButton(cardsBallsSpinWheelGUI.BallsButtonData))
            {
                Game.Instance.GoToBallScreen();
                return true;
            }

            if ((selectedButton == COMMON_BUTTONS.WHEEL && NavigateEnter()) || NavigateGamepadButton(cardsBallsSpinWheelGUI.SpinwheelButtonData))
            {
                Game.Instance.GoToChipsInfo();
                return true;
            }

            if ((selectedButton >= COMMON_BUTTONS.JOKER_1 && selectedButton <= COMMON_BUTTONS.JOKER_5) && NavigateEnter())
            {
                Game.Instance.ShowJokerInfoPopup((int)selectedButton - (int)COMMON_BUTTONS.JOKER_1);
                return true;
            }

            return false;
        }

        public static int CommonNavigation(RunData runData, COMMON_BUTTONS selectedButton)
        {
            if (selectedButton == COMMON_BUTTONS.WHEEL && NavigateUp())
                return (int)COMMON_BUTTONS.BALLS;

            if (selectedButton == COMMON_BUTTONS.BALLS && NavigateDown())
                return (int)COMMON_BUTTONS.WHEEL;

            if (selectedButton == COMMON_BUTTONS.BALLS && NavigateUp() && runData.JokerCount > 0)
                return (int)COMMON_BUTTONS.JOKER_1;

            if (selectedButton >= COMMON_BUTTONS.JOKER_1 && selectedButton <= COMMON_BUTTONS.JOKER_5 && NavigateDown())
                return (int)COMMON_BUTTONS.BALLS;

            if ((selectedButton >= COMMON_BUTTONS.JOKER_1 && selectedButton < (COMMON_BUTTONS.JOKER_1 + runData.JokerCount - 1)) && NavigateRight())
                return ((int)selectedButton + 1);
            if ((selectedButton >= COMMON_BUTTONS.JOKER_2 && selectedButton < (COMMON_BUTTONS.JOKER_1 + runData.JokerCount)) && NavigateLeft())
                return ((int)selectedButton - 1);
            return -1;
        }

        public static void CommonSelectButton(TopBarGUI topBarGUI, CardsBallsSpinWheelGUI cardsBallsSpinWheelGUI, COMMON_BUTTONS m_selectedButton)
        {
            topBarGUI.SettingsButtonData.SelectedGO.SetActive(ShowSelected() && m_selectedButton == COMMON_BUTTONS.SETTINGS);

            CommonSelectButtonNoTopBar(cardsBallsSpinWheelGUI, m_selectedButton);
        }

        public static void CommonSelectButtonNoTopBar(CardsBallsSpinWheelGUI cardsBallsSpinWheelGUI, COMMON_BUTTONS m_selectedButton)
        {
            cardsBallsSpinWheelGUI.BallsButtonData.SelectedGO.SetActive(ShowSelected() && m_selectedButton == COMMON_BUTTONS.BALLS);
            cardsBallsSpinWheelGUI.SpinwheelButtonData.SelectedGO.SetActive(ShowSelected() && m_selectedButton == COMMON_BUTTONS.WHEEL);

            if (m_selectedButton >= COMMON_BUTTONS.JOKER_1 && m_selectedButton <= COMMON_BUTTONS.JOKER_5)
                CommonVisual.SelectJoker((int)m_selectedButton - (int)COMMON_BUTTONS.JOKER_1);
        }

        public static void HideAllButtonSelections(TopBarGUI topBarGUI, CardsBallsSpinWheelGUI cardsBallsSpinWheelGUI)
        {
            topBarGUI.SettingsButtonData.SelectedGO.SetActive(false);
            cardsBallsSpinWheelGUI.BallsButtonData.SelectedGO.SetActive(false);
            cardsBallsSpinWheelGUI.SpinwheelButtonData.SelectedGO.SetActive(false);
            CommonVisual.UnselectAllJokers();

        }

        public static bool ShowSelected()
        {
            return (Gamepad.current != null ||
            Keyboard.current != null) && (Time.realtimeSinceStartupAsDouble - LastKeyboardInputTime < 5.0d);
        }
    }
}