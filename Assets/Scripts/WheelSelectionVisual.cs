/*
  Cardwheel — Non-Commercial, No-Modification License
  Copyright © 2025 Nitzan Wilnai
  Source Code: https://github.com/nitzanwilnai/Cardwheel

  Permission is granted to view and run this code for non-commercial purposes only.
  Modification, redistribution of altered versions, and commercial use are strictly prohibited.

  See the LICENSE file for full legal terms.
*/

using System.Collections;
using System.Collections.Generic;
using CommonTools;
using TMPro;
using UnityEngine;
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
        GameObject m_UI;
        TextMeshProUGUI m_description;
        TextMeshProUGUI m_winCount;

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

        GameObject playButtonGO;
        GameObject lockedGO;

        GameData gameData;
        Balance balance;

        GUIButtonData m_playButtonData;
        GUIButtonData m_prevButtonData;
        GUIButtonData m_nextButtonData;

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

            CommonButtonVisual.AddSelectedBorder(m_playButtonData);
            CommonButtonVisual.AddSelectedBorder(m_prevButtonData);
            CommonButtonVisual.AddSelectedBorder(m_nextButtonData);

            m_playButtonData.Button.onClick.AddListener(animateClose);
            m_prevButtonData.Button.onClick.AddListener(prev);
            m_nextButtonData.Button.onClick.AddListener(next);

            GUIRef guiRef = m_UI.GetComponent<GUIRef>();
            m_description = guiRef.GetTextGUI("Description");
            m_winCount = guiRef.GetTextGUI("WinCount");

            m_animation = guiRef.GetAnimation("Animation");

            playButtonGO = guiRef.GetGameObject("Play");
            lockedGO = guiRef.GetGameObject("Locked");

            m_spinWheelParent = guiRef.GetGameObject("SpinWheelParent").transform;
            m_wheelSelectionSpinWheels = new WheelSelectionSpinWheel[balance.SpinWheelBalance.NumSpinWheels];
            m_wheelSelectionSpinWheels[0].SpinWheelGO = guiRef.GetGameObject("SpinWheel");
            for (int wheelIdx = 1; wheelIdx < balance.SpinWheelBalance.NumSpinWheels; wheelIdx++)
            {
                m_wheelSelectionSpinWheels[wheelIdx].SpinWheelGO = GameObject.Instantiate(m_wheelSelectionSpinWheels[0].SpinWheelGO, m_spinWheelParent);
                Vector3 pos = new Vector3(m_wheelOffset * wheelIdx, 0.0f, 0.0f);
                m_wheelSelectionSpinWheels[wheelIdx].SpinWheelGO.transform.localPosition = pos;
            }

            for (int wheelIdx = 0; wheelIdx < balance.SpinWheelBalance.NumSpinWheels; wheelIdx++)
            {
                SpinWheelRef spinWheelRef = m_wheelSelectionSpinWheels[wheelIdx].SpinWheelGO.GetComponent<SpinWheelRef>();
                spinWheelRef.SortingPopup.SetActive(false);
                m_wheelSelectionSpinWheels[wheelIdx].SpinCircle = spinWheelRef.SpinCircle;
                m_wheelSelectionSpinWheels[wheelIdx].ScoringSlots = new ScoringSlot[spinWheelRef.SlotGO.Length];
                for (int slotIdx = 0; slotIdx < spinWheelRef.SlotGO.Length; slotIdx++)
                {
                    m_wheelSelectionSpinWheels[wheelIdx].ScoringSlots[slotIdx] = spinWheelRef.SlotGO[slotIdx].GetComponentInChildren<ScoringSlot>();
                    m_wheelSelectionSpinWheels[wheelIdx].ScoringSlots[slotIdx].Index = slotIdx;

                    // m_wheelSelectionSpinWheels[wheelIdx].ScoringSlots[slotIdx].SetSlotColor(balance.SlotColors[slotIdx / balance.SpinWheelBalance.SlotsPerColor[wheelIdx] % 4]);
                    m_wheelSelectionSpinWheels[wheelIdx].ScoringSlots[slotIdx].SetSlotColor(balance.SlotColors[(int)balance.SpinWheelBalance.SlotType[wheelIdx][slotIdx]]);
                }
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

        public void Show()
        {
            updateText();
            updateButton();

            CommonButtonVisual.UpdateButtonIcons(m_playButtonData, Game.Instance.GetGamepadType());
            CommonButtonVisual.UpdateButtonIcons(m_nextButtonData, Game.Instance.GetGamepadType());
            CommonButtonVisual.UpdateButtonIcons(m_prevButtonData, Game.Instance.GetGamepadType());

            m_playButtonData.SelectedGO.SetActive(CommonButtonVisual.ShowSelected());

            autoSlideToLastUnlocked();

            m_UI.SetActive(true);
        }

        void updateText()
        {
            m_description.text = balance.SpinWheelBalance.Description[m_wheelSelectionIdx];
            m_winCount.text = "Wins: " + gameData.SpinWheelWinCount[m_wheelSelectionIdx];
        }

        void updateButton()
        {
            m_prevButtonData.Button.interactable = (m_wheelSelectionIdx > 0);
            m_prevButtonData.Button.image.color = (m_wheelSelectionIdx > 0) ? balance.ButtonColorEnabled : balance.ButtonColorDisabled;
            m_nextButtonData.Button.interactable = (m_wheelSelectionIdx < gameData.SpinWheelWinCount.Length - 1);
            m_nextButtonData.Button.image.color = (m_wheelSelectionIdx < gameData.SpinWheelWinCount.Length - 1) ? balance.ButtonColorEnabled : balance.ButtonColorDisabled;

            bool canPlay = canPlayWheel();
            playButtonGO.SetActive(canPlay);
            lockedGO.SetActive(!canPlay);
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
                    Game.Instance.StartNewRun(m_wheelSelectionIdx);
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

                float posX = (m_targetX - m_startX) * SlideAnimCurve.Evaluate(m_slideValue) + m_startX;
                Vector3 pos = m_spinWheelParent.localPosition;
                pos.x = posX;
                m_spinWheelParent.localPosition = pos;
            }

            handleInput();
        }

        void handleInput()
        {

            if (CommonButtonVisual.NavigateLeft())
                prev();

            if (CommonButtonVisual.NavigateRight())
                next();


            if (canPlayWheel())
            {
                if (CommonButtonVisual.NavigateGamepadButton(m_playButtonData))
                    animateClose();

                if (CommonButtonVisual.NavigateEnter())
                    animateClose();
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
            SoundManager.Instance.PlaySFXButtonOK();

            CommonVisual.AnimateClose(ref m_closeTimer, m_closeTime, m_animation, "Wheel Selection Close");
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
            m_wheelSelectionIdx = lastWheel + 1;
            if (m_wheelSelectionIdx > gameData.SpinWheelWinCount.Length - 1)
                m_wheelSelectionIdx = gameData.SpinWheelWinCount.Length - 1;

            m_startX = 0.0f;
            m_targetX = -m_wheelOffset * m_wheelSelectionIdx;

        }

    }
}
