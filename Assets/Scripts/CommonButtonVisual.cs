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
                guiButtonData.GlyphImage.sprite = AssetManager.Instance.GetGamepadGlyph(gamepadType, guiButtonData.GamepadButton);
            }
        }

        public static void AddSelectedBorder(GUIButtonData gUIButtonData)
        {
            gUIButtonData.SelectedGO = AssetManager.Instance.GetButtonSelected(gUIButtonData.Button.transform);
            gUIButtonData.SelectedGO.SetActive(false);
        }

        public static bool NavigateEnter(int availableInputs)
        {
            if (Logic.IsBitSet(availableInputs, (byte)INPUT_TYPES.GAMEPAD))
                if (Gamepad.current.buttonSouth.wasReleasedThisFrame)
                    return true;


            if (Logic.IsBitSet(availableInputs, (byte)INPUT_TYPES.KEYBOARD))
                if (Keyboard.current.enterKey.wasReleasedThisFrame)
                {
                    LastKeyboardInputTime = Time.realtimeSinceStartupAsDouble;
                    return true;
                }

            return false;
        }

        public static bool NavigateEnterHold(int availableInputs)
        {
            if (Logic.IsBitSet(availableInputs, (byte)INPUT_TYPES.GAMEPAD))
                if (Gamepad.current.buttonSouth.isPressed)
                    return true;


            if (Logic.IsBitSet(availableInputs, (byte)INPUT_TYPES.KEYBOARD))
                if (Keyboard.current.enterKey.isPressed)
                    return true;

            return false;
        }

        public static bool NavigateUp(int availableInputs)
        {
            if (Logic.IsBitSet(availableInputs, (byte)INPUT_TYPES.GAMEPAD))
                if (Gamepad.current.dpad.up.wasPressedThisFrame || Gamepad.current.leftStick.up.wasPressedThisFrame)
                    return true;

            if (Logic.IsBitSet(availableInputs, (byte)INPUT_TYPES.KEYBOARD))
                if (Keyboard.current.upArrowKey.wasPressedThisFrame)
                    return true;

            return false;
        }

        public static bool NavigateDown(int availableInputs)
        {
            if (Logic.IsBitSet(availableInputs, (byte)INPUT_TYPES.GAMEPAD))
                if (Gamepad.current.dpad.down.wasPressedThisFrame || Gamepad.current.leftStick.down.wasPressedThisFrame)
                    return true;

            if (Logic.IsBitSet(availableInputs, (byte)INPUT_TYPES.KEYBOARD))
                if (Keyboard.current.downArrowKey.wasPressedThisFrame)
                    return true;

            return false;
        }

        public static bool NavigateRight(int availableInputs)
        {
            if (Logic.IsBitSet(availableInputs, (byte)INPUT_TYPES.GAMEPAD))
                if (Gamepad.current.dpad.right.wasPressedThisFrame || Gamepad.current.leftStick.right.wasPressedThisFrame)
                    return true;


            if (Logic.IsBitSet(availableInputs, (byte)INPUT_TYPES.KEYBOARD))
                if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
                    return true;

            return false;
        }

        public static bool NavigateLeft(int availableInputs)
        {
            if (Logic.IsBitSet(availableInputs, (byte)INPUT_TYPES.GAMEPAD))
                if (Gamepad.current.dpad.left.wasPressedThisFrame || Gamepad.current.leftStick.left.wasPressedThisFrame)
                    return true;

            if (Logic.IsBitSet(availableInputs, (byte)INPUT_TYPES.KEYBOARD))
                if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
                    return true;

            return false;
        }

        public static bool NavigateGamepadButton(GUIButtonData guiButtonData, int availableInputs)
        {
            if (guiButtonData.GamepadButton == GAMEPAD_BUTTON.NORTH)
                if (Logic.IsBitSet(availableInputs, (byte)INPUT_TYPES.GAMEPAD))
                    if (Gamepad.current.buttonNorth.wasPressedThisFrame)
                        return true;

            if (guiButtonData.GamepadButton == GAMEPAD_BUTTON.SOUTH)
                if (Logic.IsBitSet(availableInputs, (byte)INPUT_TYPES.GAMEPAD))
                    if (Gamepad.current.buttonSouth.wasPressedThisFrame)
                        return true;

            if (guiButtonData.GamepadButton == GAMEPAD_BUTTON.WEST)
                if (Logic.IsBitSet(availableInputs, (byte)INPUT_TYPES.GAMEPAD))
                    if (Gamepad.current.buttonWest.wasPressedThisFrame)
                        return true;

            if (guiButtonData.GamepadButton == GAMEPAD_BUTTON.EAST)
                if (Logic.IsBitSet(availableInputs, (byte)INPUT_TYPES.GAMEPAD))
                    if (Gamepad.current.buttonEast.wasPressedThisFrame)
                        return true;

            if (guiButtonData.GamepadButton == GAMEPAD_BUTTON.OPTIONS)
                if (Logic.IsBitSet(availableInputs, (byte)INPUT_TYPES.GAMEPAD))
                    if (Gamepad.current.startButton.wasPressedThisFrame)
                        return true;

            if (guiButtonData.GamepadButton == GAMEPAD_BUTTON.R1)
                if (Logic.IsBitSet(availableInputs, (byte)INPUT_TYPES.GAMEPAD))
                    if (Gamepad.current.rightTrigger.wasPressedThisFrame)
                        return true;

            if (guiButtonData.GamepadButton == GAMEPAD_BUTTON.L1)
                if (Logic.IsBitSet(availableInputs, (byte)INPUT_TYPES.GAMEPAD))
                    if (Gamepad.current.leftTrigger.wasPressedThisFrame)
                        return true;

            if (guiButtonData.GamepadButton == GAMEPAD_BUTTON.R2)
                if (Logic.IsBitSet(availableInputs, (byte)INPUT_TYPES.GAMEPAD))
                    if (Gamepad.current.rightShoulder.wasPressedThisFrame)
                        return true;

            if (guiButtonData.GamepadButton == GAMEPAD_BUTTON.L2)
                if (Logic.IsBitSet(availableInputs, (byte)INPUT_TYPES.GAMEPAD))
                    if (Gamepad.current.leftShoulder.wasPressedThisFrame)
                        return true;

            return false;
        }

        public static void UpdateCommonButtonIcons(TopBarGUI topBarGUI, CardsBallsSpinWheelGUI cardsBallsSpinWheelGUI, GAMEPAD_TYPE gamepadType)
        {
            UpdateButtonIcons(topBarGUI.SettingsButtonData, gamepadType);
            UpdateButtonIcons(cardsBallsSpinWheelGUI.BallsButtonData, gamepadType);
            UpdateButtonIcons(cardsBallsSpinWheelGUI.SpinwheelButtonData, gamepadType);
        }

        public static bool CommonHandleInput(TopBarGUI topBarGUI, CardsBallsSpinWheelGUI cardsBallsSpinWheelGUI, int availableInputs, COMMON_BUTTONS selectedButton)
        {
            if ((selectedButton == COMMON_BUTTONS.SETTINGS && NavigateEnter(availableInputs)) || NavigateGamepadButton(topBarGUI.SettingsButtonData, availableInputs))
            {
                Game.Instance.GoToSettings();
                return true;
            }

            if (CommonHandleInputNoTopBar(cardsBallsSpinWheelGUI, availableInputs, selectedButton))
                return true;

            return false;
        }

        public static bool CommonHandleInputNoTopBar(CardsBallsSpinWheelGUI cardsBallsSpinWheelGUI, int availableInputs, COMMON_BUTTONS selectedButton)
        {
            if ((selectedButton == COMMON_BUTTONS.BALLS && NavigateEnter(availableInputs)) || NavigateGamepadButton(cardsBallsSpinWheelGUI.BallsButtonData, availableInputs))
            {
                Game.Instance.GoToBallScreen();
                return true;
            }

            if ((selectedButton == COMMON_BUTTONS.WHEEL && NavigateEnter(availableInputs)) || NavigateGamepadButton(cardsBallsSpinWheelGUI.SpinwheelButtonData, availableInputs))
            {
                Game.Instance.GoToChipsInfo();
                return true;
            }

            if ((selectedButton >= COMMON_BUTTONS.JOKER_1 && selectedButton <= COMMON_BUTTONS.JOKER_5) && NavigateEnter(availableInputs))
            {
                Game.Instance.ShowJokerInfoPopup((int)selectedButton - (int)COMMON_BUTTONS.JOKER_1);
                return true;
            }

            return false;
        }

        public static int CommonNavigation(RunData runData, int availableInputs, COMMON_BUTTONS selectedButton)
        {
            if (selectedButton == COMMON_BUTTONS.WHEEL && CommonButtonVisual.NavigateUp(availableInputs))
                return (int)COMMON_BUTTONS.BALLS;

            if (selectedButton == COMMON_BUTTONS.BALLS && CommonButtonVisual.NavigateDown(availableInputs))
                return (int)COMMON_BUTTONS.WHEEL;

            if (selectedButton == COMMON_BUTTONS.BALLS && CommonButtonVisual.NavigateUp(availableInputs))
                return (int)COMMON_BUTTONS.JOKER_1;

            if (selectedButton >= COMMON_BUTTONS.JOKER_1 && selectedButton <= COMMON_BUTTONS.JOKER_5 && NavigateDown(availableInputs))
                return (int)COMMON_BUTTONS.BALLS;

            if ((selectedButton >= COMMON_BUTTONS.JOKER_1 && selectedButton < (COMMON_BUTTONS.JOKER_1 + runData.JokerCount - 1)) && NavigateRight(availableInputs))
                return ((int)selectedButton + 1);
            if ((selectedButton >= COMMON_BUTTONS.JOKER_2 && selectedButton < (COMMON_BUTTONS.JOKER_1 + runData.JokerCount)) && NavigateLeft(availableInputs))
                return ((int)selectedButton - 1);
            return -1;
        }

        public static void CommonSelectButton(TopBarGUI topBarGUI, CardsBallsSpinWheelGUI cardsBallsSpinWheelGUI, COMMON_BUTTONS m_selectedButton)
        {
            topBarGUI.SettingsButtonData.SelectedGO.SetActive(m_selectedButton == COMMON_BUTTONS.SETTINGS);

            CommonSelectButtonNoTopBar(cardsBallsSpinWheelGUI, m_selectedButton);
        }

        public static void CommonSelectButtonNoTopBar(CardsBallsSpinWheelGUI cardsBallsSpinWheelGUI, COMMON_BUTTONS m_selectedButton)
        {
            cardsBallsSpinWheelGUI.BallsButtonData.SelectedGO.SetActive(m_selectedButton == COMMON_BUTTONS.BALLS);
            cardsBallsSpinWheelGUI.SpinwheelButtonData.SelectedGO.SetActive(m_selectedButton == COMMON_BUTTONS.WHEEL);

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
    }
}