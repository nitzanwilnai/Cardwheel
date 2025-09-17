using System.Collections;
using System.Collections.Generic;
using CommonTools;
using TMPro;
using UnityEngine;
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

        Button prevButtonGO;
        Button nextButtonGO;
        GameObject playButtonGO;
        GameObject lockedGO;

        public void Init(Camera camera, Balance balance)
        {
            m_UI = AssetManager.Instance.LoadWheelSelectionUI();
            m_UI.GetComponent<Canvas>().worldCamera = camera;

            GUIRef guiRef = m_UI.GetComponent<GUIRef>();
            guiRef.GetButton("Play").onClick.AddListener(Game.Instance.CloseWheelSelection);
            guiRef.GetButton("Prev").onClick.AddListener(prev);
            guiRef.GetButton("Next").onClick.AddListener(next);
            prevButtonGO = guiRef.GetButton("Prev");
            nextButtonGO = guiRef.GetButton("Next");

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
                Vector3 pos = new Vector3(Screen.width * wheelIdx, 0.0f, 0.0f);
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

            Hide();
        }

        public void Show(GameData gameData, Balance balance)
        {
            updateText(gameData, balance);
            updateButton(gameData, balance);

            m_UI.SetActive(true);
        }

        void updateText(GameData gameData, Balance balance)
        {
            m_description.text = balance.SpinWheelBalance.Description[m_wheelSelectionIdx];
            m_winCount.text = "Wins: " + gameData.SpinWheelWinCount[m_wheelSelectionIdx];
        }

        void updateButton(GameData gameData, Balance balance)
        {
            prevButtonGO.interactable = (m_wheelSelectionIdx > 0);
            prevButtonGO.image.color = (m_wheelSelectionIdx > 0) ? balance.ButtonColorEnabled : balance.ButtonColorDisabled;
            nextButtonGO.interactable = (m_wheelSelectionIdx < gameData.SpinWheelWinCount.Length - 1);
            nextButtonGO.image.color = (m_wheelSelectionIdx < gameData.SpinWheelWinCount.Length - 1) ? balance.ButtonColorEnabled : balance.ButtonColorDisabled;

            if (m_wheelSelectionIdx == 0)
            {
                playButtonGO.SetActive(true);
                lockedGO.SetActive(false);
            }
            else if (gameData.SpinWheelWinCount[m_wheelSelectionIdx - 1] > 0)
            {
                playButtonGO.SetActive(true);
                lockedGO.SetActive(false);
            }
            else
            {
                playButtonGO.SetActive(false);
                lockedGO.SetActive(true);
            }
        }

        public void Hide()
        {
            m_UI.SetActive(false);
        }

        public void Tick(GameData gameData, Balance balance, float dt)
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

                    updateText(gameData, balance);
                    updateButton(gameData, balance);

                    m_description.gameObject.SetActive(true);
                    m_winCount.gameObject.SetActive(true);
                }

                float posX = (m_targetX - m_startX) * SlideAnimCurve.Evaluate(m_slideValue) + m_startX;
                Vector3 pos = m_spinWheelParent.localPosition;
                pos.x = posX;
                m_spinWheelParent.localPosition = pos;
            }

            // if (m_nextWheelTimer > 0.0f)
            // {
            //     m_nextWheelTimer -= dt;
            //     if (m_nextWheelTimer <= 0.0f)
            //     {
            //         m_animation.Play("Wheel Selection Next 2");
            //     }
            // }

            // if (m_prevWheelTimer > 0.0f)
            // {
            //     m_prevWheelTimer -= dt;
            //     if (m_prevWheelTimer <= 0.0f)
            //     {
            //         m_animation.Play("Wheel Selection Prev 2");
            //     }
            // }
        }

        public void AnimateClose()
        {
            CommonVisual.AnimateClose(ref m_closeTimer, m_closeTime, m_animation, "Wheel Selection Close");
        }

        void prev()
        {
            if (m_wheelSelectionIdx > 0)
            {
                m_wheelSelectionIdx--;

                m_startX = m_targetX;
                m_targetX += Screen.width;

                slideSpinWheel();
            }
        }

        void next()
        {
            if (m_wheelSelectionIdx < m_wheelSelectionSpinWheels.Length - 1)
            {
                m_wheelSelectionIdx++;

                m_startX = m_targetX;
                m_targetX -= Screen.width;

                slideSpinWheel();
            }
        }

        void slideSpinWheel()
        {
            m_slideValue = 0.0f;
            m_description.gameObject.SetActive(false);
            m_winCount.gameObject.SetActive(false);
        }

    }
}
