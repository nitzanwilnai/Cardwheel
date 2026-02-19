/*
  Cardwheel — Non-Commercial, No-Modification License
  Copyright © 2025 Nitzan Wilnai
  Source Code: https://github.com/nitzanwilnai/Cardwheel

  Permission is granted to view and run this code for non-commercial purposes only.
  Modification, redistribution of altered versions, and commercial use are strictly prohibited.

  See the LICENSE file for full legal terms.
*/

using CommonTools;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Cardwheel
{
    public struct WheelSelectionSpinWheel
    {
        public GameObject SpinWheelGO;
        public ScoringSlot[] ScoringSlots;
        public SpinCircle SpinCircle;
    }

    public class WheelSelectionVisual : MonoBehaviour
    {
        public enum MENU_BUTTONS
        {
            PLAY,
            SEED,
            BACK,
        };

        MENU_BUTTONS m_selectedButton;

        GameObject m_UI;
        TextMeshProUGUI m_description;
        TextMeshProUGUI m_winCount;
        TextMeshProUGUI m_seedPlaceholder;
        TextMeshProUGUI m_seedText;

        int m_wheelSelectionIdx;

        float m_closeTimer = 0.0f;
        float m_closeTime = 0.1f;
        Animation m_animation;

        Transform m_spinWheelParent;

        WheelSelectionSpinWheel[] m_wheelSelectionSpinWheels;
        public float WheelSpeed;

        float spinWheelAngle = 0.0f;

        public float m_startX;
        public float m_targetX;
        public AnimationCurve SlideAnimCurve;
        float m_slideValue;
        public float SlideSpeed;

        GameObject m_playButtonGO;
        GameObject m_lockedGO;

        GameData gameData;
        Balance balance;

        GUIButtonData m_playButtonData;
        GUIButtonData m_prevButtonData;
        GUIButtonData m_nextButtonData;
        GUIButtonData m_backButtonData;
        GameObject m_seedSelected;

        uint m_seed = 0;
        Image m_seedGlyph;
        float m_seedChangedTime;

        TMP_InputField m_seedInputField;
        GameObject m_demoGO;

        int m_wheelOffset;

        public void Init(Camera camera, GameData gameData, Balance balance)
        {
            this.gameData = gameData;
            this.balance = balance;

            m_wheelOffset = Screen.width > Screen.height ? 1920 : 1080; //Screen.width;

            m_UI = AssetManager.Instance.LoadWheelSelectionUI();
            m_UI.GetComponent<Canvas>().worldCamera = camera;
            CommonVisual.ChangeCanvasScalerMatching(m_UI);

            GUIButtonRef guiButtonRef = m_UI.GetComponent<GUIButtonRef>();
            m_playButtonData = guiButtonRef.GetButtonData("Play");
            m_prevButtonData = guiButtonRef.GetButtonData("Prev");
            m_nextButtonData = guiButtonRef.GetButtonData("Next");
            m_backButtonData = guiButtonRef.GetButtonData("Back");

            CommonButtonVisual.AddSelectedBorder(m_playButtonData);
            CommonButtonVisual.AddSelectedBorder(m_prevButtonData);
            CommonButtonVisual.AddSelectedBorder(m_nextButtonData);
            CommonButtonVisual.AddSelectedBorder(m_backButtonData);

            m_playButtonData.Button.onClick.AddListener(animateClose);
            m_prevButtonData.Button.onClick.AddListener(prev);
            m_nextButtonData.Button.onClick.AddListener(next);
            m_backButtonData.Button.onClick.AddListener(Game.Instance.GoToMainMenu);

            GUIRef guiRef = m_UI.GetComponent<GUIRef>();
            m_description = guiRef.GetTextGUI("Description");
            m_winCount = guiRef.GetTextGUI("WinCount");

            m_animation = guiRef.GetAnimation("Animation");

            m_playButtonGO = guiRef.GetGameObject("Play");
            m_lockedGO = guiRef.GetGameObject("Locked");

            m_seedText = guiRef.GetTextGUI("Seed");
            m_seedPlaceholder = guiRef.GetTextGUI("Placeholder");
            m_seedInputField = guiRef
                .GetGameObject("Seed")
                .gameObject.GetComponent<TMP_InputField>();
            // m_seedInputField.onValueChanged.AddListener(changeSeed);
            m_seedInputField.onEndEdit.AddListener(changeSeed);
            m_seedSelected = guiRef.GetGameObject("SeedSelected");
            m_seedSelected.SetActive(false);

#if UNITY_STANDALONE
            m_seedGlyph = guiRef.GetImage("Seed");
            m_seedGlyph.gameObject.SetActive(false);
#endif

            m_spinWheelParent = guiRef.GetGameObject("SpinWheelParent").transform;
            m_wheelSelectionSpinWheels = new WheelSelectionSpinWheel[
                balance.SpinWheelBalance.NumSpinWheels
            ];
            m_wheelSelectionSpinWheels[0].SpinWheelGO = guiRef.GetGameObject("SpinWheel");
            for (int wheelIdx = 1; wheelIdx < balance.SpinWheelBalance.NumSpinWheels; wheelIdx++)
            {
                m_wheelSelectionSpinWheels[wheelIdx].SpinWheelGO = GameObject.Instantiate(
                    m_wheelSelectionSpinWheels[0].SpinWheelGO,
                    m_spinWheelParent
                );
                Vector3 pos = new Vector3(m_wheelOffset * wheelIdx, 0.0f, 0.0f);
                m_wheelSelectionSpinWheels[wheelIdx].SpinWheelGO.transform.localPosition = pos;
            }

            for (int wheelIdx = 0; wheelIdx < balance.SpinWheelBalance.NumSpinWheels; wheelIdx++)
            {
                SpinWheelRef spinWheelRef = m_wheelSelectionSpinWheels[wheelIdx]
                    .SpinWheelGO.GetComponent<SpinWheelRef>();
                spinWheelRef.SortingPopup.SetActive(false);
                m_wheelSelectionSpinWheels[wheelIdx].SpinCircle = spinWheelRef.SpinCircle;
                m_wheelSelectionSpinWheels[wheelIdx].ScoringSlots = new ScoringSlot[
                    spinWheelRef.SlotGO.Length
                ];
                for (int slotIdx = 0; slotIdx < spinWheelRef.SlotGO.Length; slotIdx++)
                {
                    m_wheelSelectionSpinWheels[wheelIdx].ScoringSlots[slotIdx] = spinWheelRef
                        .SlotGO[slotIdx]
                        .GetComponentInChildren<ScoringSlot>();
                    m_wheelSelectionSpinWheels[wheelIdx].ScoringSlots[slotIdx].Index = slotIdx;

                    // m_wheelSelectionSpinWheels[wheelIdx].ScoringSlots[slotIdx].SetSlotColor(balance.SlotColors[slotIdx / balance.SpinWheelBalance.SlotsPerColor[wheelIdx] % 4]);
                    m_wheelSelectionSpinWheels[wheelIdx]
                        .ScoringSlots[slotIdx]
                        .SetSlotColor(
                            balance.SlotColors[
                                (int)balance.SpinWheelBalance.SlotType[wheelIdx][slotIdx]
                            ]
                        );
                }

                m_wheelSelectionSpinWheels[wheelIdx].SpinWheelGO.SetActive(false);
            }

#if UNITY_STANDALONE
            m_demoGO = guiRef.GetGameObject("Demo");

            if (m_demoGO != null)
            {
#if DEMO
                m_demoGO.SetActive(true);
#else
                m_demoGO.SetActive(false);
#endif
            }
#endif

            Hide();
        }

        void changeSeed(string newText)
        {
            Debug.Log(Game.Instance.FrameCounter + " changeSeed(" + newText + ")");

            m_seed = Logic.DecodeSeed(newText.ToUpper());
            m_seedChangedTime = Time.realtimeSinceStartup;

            stopSeedEditing();
        }

        public void Show()
        {
            updateText();
            updateButton();

            GAMEPAD_TYPE gamepadType = Game.Instance.GetGamepadType();

            CommonButtonVisual.UpdateButtonIcons(m_playButtonData, gamepadType);
            CommonButtonVisual.UpdateButtonIcons(m_nextButtonData, gamepadType);
            CommonButtonVisual.UpdateButtonIcons(m_prevButtonData, gamepadType);
            CommonButtonVisual.UpdateButtonIcons(m_backButtonData, gamepadType);

            m_playButtonData.SelectedGO.SetActive(CommonButtonVisual.ShowSelected());

            autoSlideToLastUnlocked();

            m_seed = (uint)Mathf.FloorToInt(UnityEngine.Random.value * int.MaxValue);

            m_seedInputField.text = "";
            m_seedPlaceholder.text = Logic.EncodeSeed(m_seed);

            m_UI.SetActive(true);

#if UNITY_STANDALONE
            m_seedGlyph.gameObject.SetActive(gamepadType != GAMEPAD_TYPE.NONE);
            m_seedGlyph.sprite = AssetManager.Instance.GetGamepadGlyph(
                gamepadType,
                (int)GAMEPAD_BUTTON.NORTH - 1
            );
#endif

            m_seedChangedTime = Time.realtimeSinceStartup;

            selectButton(MENU_BUTTONS.PLAY);
        }

        void updateText()
        {
            m_description.text = balance.SpinWheelBalance.Description[m_wheelSelectionIdx];
            m_winCount.text = "Wins: " + gameData.SpinWheelWinCount[m_wheelSelectionIdx];
        }

        void updateButton()
        {
            m_prevButtonData.Button.interactable = (m_wheelSelectionIdx > 0);
            m_prevButtonData.Button.image.color =
                (m_wheelSelectionIdx > 0)
                    ? balance.ButtonColorEnabled
                    : balance.ButtonColorDisabled;
            m_nextButtonData.Button.interactable = (
                m_wheelSelectionIdx < gameData.SpinWheelWinCount.Length - 1
            );
            m_nextButtonData.Button.image.color =
                (m_wheelSelectionIdx < gameData.SpinWheelWinCount.Length - 1)
                    ? balance.ButtonColorEnabled
                    : balance.ButtonColorDisabled;

            bool canPlay = canPlayWheel();
            m_playButtonGO.SetActive(canPlay);
            m_lockedGO.SetActive(!canPlay);
        }

        public void Hide()
        {
            m_UI.SetActive(false);
        }

        public void Tick(float dt)
        {
            spinWheelAngle += dt * WheelSpeed;
            for (int i = 0; i < m_wheelSelectionSpinWheels.Length; i++)
                m_wheelSelectionSpinWheels[i].SpinCircle.Angle = spinWheelAngle;

            if (m_closeTimer > 0.0f)
            {
                m_closeTimer -= dt;
                if (m_closeTimer <= 0.0f)
                    Game.Instance.StartNewRun(m_wheelSelectionIdx, m_seed);
            }

            if (m_slideValue < 1.0f)
            {
                m_slideValue += dt * SlideSpeed;
                if (m_slideValue > 1.0f)
                {
                    m_slideValue = 1.0f;

                    updateText();
                    updateButton();

                    m_description.gameObject.SetActive(true);
                    m_winCount.gameObject.SetActive(true);
                }

                float posX =
                    (m_targetX - m_startX) * SlideAnimCurve.Evaluate(m_slideValue) + m_startX;
                Vector3 pos = m_spinWheelParent.localPosition;
                pos.x = posX;
                m_spinWheelParent.localPosition = pos;

                float negativeX = -posX;
                for (int i = 0; i < m_wheelSelectionSpinWheels.Length; i++)
                {
                    float localX = m_wheelSelectionSpinWheels[i]
                        .SpinWheelGO
                        .transform
                        .localPosition
                        .x;
                    if (localX >= negativeX - 1080.0f && localX <= negativeX + 1080.0f)
                    {
                        if (!m_wheelSelectionSpinWheels[i].SpinWheelGO.activeSelf)
                            m_wheelSelectionSpinWheels[i].SpinWheelGO.SetActive(true);
                    }
                    else
                    {
                        if (m_wheelSelectionSpinWheels[i].SpinWheelGO.activeSelf)
                            m_wheelSelectionSpinWheels[i].SpinWheelGO.SetActive(false);
                    }
                }
            }

            handleInput();
        }

        void handleInput()
        {
            if (m_seedInputField.isFocused && Keyboard.current.enterKey.wasPressedThisFrame)
            {
                stopSeedEditing();
                return;
            }

            if (!m_seedInputField.isFocused)
            {
                if (CommonButtonVisual.NavigateLeft())
                {
                    prev();
                    return;
                }

                if (CommonButtonVisual.NavigateRight())
                {
                    next();
                    return;
                }

                if (Time.realtimeSinceStartup - m_seedChangedTime >= 0.5f)
                {
                    if (canPlayWheel())
                    {
                        if (CommonButtonVisual.NavigateGamepadButton(m_playButtonData))
                        {
                            animateClose();
                            return;
                        }

                        if (
                            m_selectedButton == MENU_BUTTONS.PLAY
                            && CommonButtonVisual.NavigateEnter()
                        )
                        {
                            animateClose();
                            return;
                        }
                    }

                    if (Gamepad.current != null && Gamepad.current.buttonNorth.wasPressedThisFrame)
                    {
                        Debug.Log(Game.Instance.FrameCounter + " edit field");
                        m_seedInputField.Select();
                        m_seedInputField.ActivateInputField();
                        return;
                    }
                    if (m_selectedButton == MENU_BUTTONS.SEED && CommonButtonVisual.NavigateEnter())
                    {
                        Debug.Log(Game.Instance.FrameCounter + " edit field");
                        m_seedInputField.Select();
                        m_seedInputField.ActivateInputField();
                        return;
                    }
                }

                if (CommonButtonVisual.NavigateGamepadButton(m_backButtonData))
                {
                    Game.Instance.GoToMainMenu();
                    return;
                }

                if (m_selectedButton == MENU_BUTTONS.BACK && CommonButtonVisual.NavigateEnter())
                {
                    Game.Instance.GoToMainMenu();
                    return;
                }

                // down
                if (m_selectedButton == MENU_BUTTONS.PLAY && CommonButtonVisual.NavigateDown())
                {
                    selectButton(MENU_BUTTONS.SEED);
                    return;
                }
                if (m_selectedButton == MENU_BUTTONS.SEED && CommonButtonVisual.NavigateDown())
                {
                    selectButton(MENU_BUTTONS.BACK);
                    return;
                }
                // up
                if (m_selectedButton == MENU_BUTTONS.BACK && CommonButtonVisual.NavigateUp())
                {
                    selectButton(MENU_BUTTONS.SEED);
                    return;
                }
                if (m_selectedButton == MENU_BUTTONS.SEED && CommonButtonVisual.NavigateUp())
                {
                    selectButton(MENU_BUTTONS.PLAY);
                    return;
                }
            }
        }

        bool canPlayWheel()
        {
            bool canPlayWheel = m_wheelSelectionIdx == 0 ? true : false;
#if !DEMO
            if (m_wheelSelectionIdx > 0 && gameData.SpinWheelWinCount[m_wheelSelectionIdx - 1] > 0)
                canPlayWheel = true;
#endif
            return canPlayWheel;
        }

        public void animateClose()
        {
            Debug.Log(Game.Instance.FrameCounter + " animateClose()");
            SoundManager.Instance.PlaySFXButtonOK();

            CommonVisual.AnimateClose(
                ref m_closeTimer,
                m_closeTime,
                m_animation,
                "Wheel Selection Close"
            );
        }

        void prev()
        {
            if (m_wheelSelectionIdx > 0)
            {
                m_wheelSelectionIdx--;

                m_startX = m_targetX;
                m_targetX += m_wheelOffset;

                slideSpinWheel();
            }
        }

        void next()
        {
            if (m_wheelSelectionIdx < m_wheelSelectionSpinWheels.Length - 1)
            {
                m_wheelSelectionIdx++;

                m_startX = m_targetX;
                m_targetX -= m_wheelOffset;

                slideSpinWheel();
            }
        }

        void slideSpinWheel()
        {
            m_slideValue = 0.0f;
            m_description.gameObject.SetActive(false);
            m_winCount.gameObject.SetActive(false);
        }

        void autoSlideToLastUnlocked()
        {
            int lastWheel = 0;
            for (int i = 0; i < gameData.SpinWheelWinCount.Length; i++)
                if (gameData.SpinWheelWinCount[i] > 0)
                    lastWheel = i;
            m_wheelSelectionIdx = gameData.SpinWheelWinCount[0] > 0 ? lastWheel + 1 : 0;
            if (m_wheelSelectionIdx > gameData.SpinWheelWinCount.Length - 1)
                m_wheelSelectionIdx = gameData.SpinWheelWinCount.Length - 1;

            m_startX = 0.0f;
            m_targetX = -m_wheelOffset * m_wheelSelectionIdx;
        }

        void selectButton(MENU_BUTTONS selectedButton)
        {
            Debug.Log(
                Game.Instance.FrameCounter + " selectButton(" + selectedButton.ToString() + ")"
            );

            m_selectedButton = selectedButton;

            m_playButtonData.SelectedGO.SetActive(
                CommonButtonVisual.ShowSelected() && m_selectedButton == MENU_BUTTONS.PLAY
            );

            m_seedSelected.SetActive(
                CommonButtonVisual.ShowSelected() && m_selectedButton == MENU_BUTTONS.SEED
            );

            m_backButtonData.SelectedGO.SetActive(
                CommonButtonVisual.ShowSelected() && m_selectedButton == MENU_BUTTONS.BACK
            );
        }

        void stopSeedEditing()
        {
            Debug.Log(Game.Instance.FrameCounter + " handleInput() stopSeedEditing()");

            m_seedInputField.DeactivateInputField();

            EventSystem es = EventSystem.current;
            if (es != null && es.currentSelectedGameObject == m_seedInputField.gameObject)
                es.SetSelectedGameObject(null);
        }
    }
}
