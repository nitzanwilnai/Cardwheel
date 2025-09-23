using UnityEngine;
using UnityEngine.UI;
using CommonTools;
using UnityEngine.InputSystem;
using TMPro;

namespace Cardwheel
{
    public enum GAMEPAD_TYPE { NONE, STEAM, PS5, SWITCH, XBOX };

    public static class CommonButtonVisual
    {
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
                if (Gamepad.current.buttonSouth.wasPressedThisFrame)
                    return true;


            if (Logic.IsBitSet(availableInputs, (byte)INPUT_TYPES.KEYBOARD))
                if (Keyboard.current.enterKey.wasPressedThisFrame)
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

            return false;
        }
    }
}