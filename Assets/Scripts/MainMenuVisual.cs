using System;
using CommonTools;
#if !(PLATFORM_IOS || PLATFORM_ANDROID)
using Steamworks;
#endif
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Cardwheel
{
    public class MainMenuVisual : MonoBehaviour
    {
        public enum MENU_BUTTONS { NEW_GAME, CONTINUE };
        MENU_BUTTONS m_selectedButton;

        GameObject m_UI;
        TextMeshProUGUI m_title;

        float m_goToWheelSelectTimer = 0.0f;
        float m_goToWheelSelectionTime = 0.1f;
        float m_continueGametTimer = 0.0f;
        float m_continueGameTime = 0.1f;
        Animation m_animation;

        public Transform Joker1Parent;
        public Transform Joker2Parent;

        // public int NumJokers;

        GameObject[] m_jokersGO;
        Vector2[] m_jokerPos;
        float[] m_jokerSpeed;
        float[] m_jokerAngle;
        float[] m_jokerRotationSpeed;
        int[] m_shuffledJokerIdxs;

        GUIButtonData m_newGameButtonData;
        GUIButtonData m_continueButtonData;
        GUIButtonData m_privacyPolicyButtonData;

        public void Init(Camera camera, Balance balance)
        {
            m_UI = AssetManager.Instance.LoadMainMenuUI();
            m_UI.GetComponent<Canvas>().worldCamera = camera;

            GUIButtonRef guiButtonRef = m_UI.GetComponent<GUIButtonRef>();

            m_newGameButtonData = guiButtonRef.GetButtonData("Play");
            m_newGameButtonData.Button.onClick.AddListener(Game.Instance.AnimateGoToWheelSelection);
            CommonButtonVisual.AddSelectedBorder(m_newGameButtonData);

            m_continueButtonData = guiButtonRef.GetButtonData("Continue");
            m_continueButtonData.Button.onClick.AddListener(Game.Instance.AnimateContinueRun);
            CommonButtonVisual.AddSelectedBorder(m_continueButtonData);

            m_privacyPolicyButtonData = guiButtonRef.GetButtonData("PrivacyPolicy");
            m_privacyPolicyButtonData.Button.onClick.AddListener(Game.Instance.GoToPrivacyPolicy);
            CommonButtonVisual.AddSelectedBorder(m_privacyPolicyButtonData);

            GUIRef guiRef = m_UI.GetComponent<GUIRef>();

            m_animation = guiRef.GetAnimation("Animation");

            Joker1Parent = guiRef.GetGameObject("Jokers1").transform;
            Joker2Parent = guiRef.GetGameObject("Jokers2").transform;

            m_jokersGO = new GameObject[balance.JokerBalance.NumJokers];

            m_jokerPos = new Vector2[balance.JokerBalance.NumJokers];
            m_jokerSpeed = new float[balance.JokerBalance.NumJokers];
            m_jokerAngle = new float[balance.JokerBalance.NumJokers];
            m_jokerRotationSpeed = new float[balance.JokerBalance.NumJokers];
            m_shuffledJokerIdxs = new int[balance.JokerBalance.NumJokers];

            // pick NumJokers random jokers
            for (int i = 0; i < balance.JokerBalance.NumJokers; i++)
                m_shuffledJokerIdxs[i] = i;
            uint seed = (uint)(UInt16.MaxValue * UnityEngine.Random.value);
            Logic.ShuffleIntArray(ref seed, m_shuffledJokerIdxs);

            for (int i = 0; i < balance.JokerBalance.NumJokers; i++)
            {
                GameObject jokerGO = new GameObject("Joker" + (i + 1));
                m_jokersGO[i] = jokerGO;

                Image image = jokerGO.AddComponent<Image>();
                image.sprite = AssetManager.Instance.LoadJokerSprite(balance.JokerBalance.JokerSpritesNames[i]);

                RectTransform trans = jokerGO.GetComponent<RectTransform>();
                trans.transform.SetParent(UnityEngine.Random.value < 0.5f ? Joker1Parent : Joker2Parent); // setting parent
                trans.localScale = new Vector3(0.5f, 0.5f, 1.0f);

                trans.sizeDelta = new Vector2(256, 448); // custom size
            }

            for (int i = 0; i < balance.JokerBalance.NumJokers; i++)
                moveNewJoker(i);

            m_title = guiRef.GetTextGUI("Title");
            Hide();
        }

        void moveNewJoker(int jkrIdx)
        {
            float posX = -2176 - (1920.0f * 2.5f) * UnityEngine.Random.value;
            float posY = 720.0f * UnityEngine.Random.value - 360.0f;
            m_jokerPos[jkrIdx] = new Vector2(posX, posY);

            m_jokerSpeed[jkrIdx] = UnityEngine.Random.value * 100.0f + 500.0f;
            m_jokerAngle[jkrIdx] = UnityEngine.Random.value * 360.0f;
            m_jokerRotationSpeed[jkrIdx] = UnityEngine.Random.value * 50.0f + 50.0f;
        }

        public void Show(Balance balance, GAMEPAD_TYPE gamepadType, int availableInputs)
        {
            m_title.text = CommonVisual.ColorText(balance, "Cardwheel");

            MENU_STATE menuState = RunDataIO.LoadMenuStateOnly();
            m_continueButtonData.Button.gameObject.SetActive(menuState >= MENU_STATE.IN_GAME && menuState < MENU_STATE.GAME_OVER);

            CommonButtonVisual.UpdateButtonIcons(m_newGameButtonData, gamepadType);
            CommonButtonVisual.UpdateButtonIcons(m_continueButtonData, gamepadType);

            if (Logic.IsBitSet(availableInputs, (byte)INPUT_TYPES.GAMEPAD) || Logic.IsBitSet(availableInputs, (byte)INPUT_TYPES.KEYBOARD))
                selectButton(MENU_BUTTONS.NEW_GAME);
            else
            {
                m_newGameButtonData.SelectedGO.SetActive(false);
                m_continueButtonData.SelectedGO.SetActive(false);
            }

            m_UI.SetActive(true);
        }

        void selectButton(MENU_BUTTONS selectedButton)
        {
            m_selectedButton = selectedButton;
            m_newGameButtonData.SelectedGO.SetActive(m_selectedButton == MENU_BUTTONS.NEW_GAME);
            m_continueButtonData.SelectedGO.SetActive(m_selectedButton == MENU_BUTTONS.CONTINUE);
        }

        public void Tick(Balance balance, float dt, int availableInputs)
        {
            if (CommonVisual.AnimateCloseTick(ref m_goToWheelSelectTimer, dt))
                Game.Instance.GoToWheelSelection();
            if (CommonVisual.AnimateCloseTick(ref m_continueGametTimer, dt))
                Game.Instance.ContinueRun();

            for (int i = 0; i < balance.JokerBalance.NumJokers; i++)
            {
                m_jokerPos[i].x += dt * m_jokerSpeed[i];
                if (m_jokerPos[i].x > 2176.0f)
                    moveNewJoker(i);

                m_jokersGO[i].transform.localPosition = m_jokerPos[i];

                m_jokerAngle[i] += m_jokerRotationSpeed[i] * dt;
                if (m_jokerAngle[i] > 360.0f)
                    m_jokerAngle[i] -= 360.0f;
                m_jokersGO[i].transform.localRotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, m_jokerAngle[i]));
            }

            handleInput(availableInputs);

        }

        private void handleInput(int availableInputs)
        {
            if (CommonButtonVisual.NavigateGamepadButton(m_newGameButtonData, availableInputs))
                Game.Instance.AnimateGoToWheelSelection();

            if (CommonButtonVisual.NavigateGamepadButton(m_continueButtonData, availableInputs))
                Game.Instance.AnimateContinueRun();

            if (m_selectedButton == MENU_BUTTONS.NEW_GAME && CommonButtonVisual.NavigateEnter(availableInputs))
                Game.Instance.AnimateGoToWheelSelection();

            if (m_selectedButton == MENU_BUTTONS.CONTINUE && CommonButtonVisual.NavigateEnter(availableInputs))
                Game.Instance.AnimateContinueRun();

            if (m_selectedButton == MENU_BUTTONS.NEW_GAME && CommonButtonVisual.NavigateRight(availableInputs))
                selectButton(MENU_BUTTONS.CONTINUE);
            if (m_selectedButton == MENU_BUTTONS.CONTINUE && CommonButtonVisual.NavigateLeft(availableInputs))
                selectButton(MENU_BUTTONS.NEW_GAME);
        }

        public void Hide()
        {
            m_UI.SetActive(false);
        }

        public void AnimateGoToWheelSelection()
        {
            CommonVisual.AnimateClose(ref m_goToWheelSelectTimer, m_goToWheelSelectionTime, m_animation, "Main Menu Close");
        }

        public void AnimateContinueGame()
        {
            CommonVisual.AnimateClose(ref m_continueGametTimer, m_continueGameTime, m_animation, "Main Menu Close");
        }
    }
}
