using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CommonTools;
using TMPro;

namespace Cardwheel
{

    public class ChipsInfoVisual : MonoBehaviour
    {
        GameObject m_UI;

        TextMeshProUGUI[] m_baseChipsText;
        TextMeshProUGUI m_mostFrequent;
        TextMeshProUGUI m_leastFrequest;

        float m_closeTimer = 0.0f;
        float m_closeTime = 0.1f;
        Animation m_animation;

        GUIButtonData m_closeButtonData;

        public void Init(Camera camera)
        {
            m_UI = AssetManager.Instance.LoadChipsInfoUI();
            m_UI.GetComponent<Canvas>().worldCamera = camera;
            CommonVisual.ChangeCanvasScalerMatching(m_UI);

            GUIRef guiRef = m_UI.GetComponent<GUIRef>();
            m_animation = guiRef.GetAnimation("Animation");

            GUIButtonRef guiButtonRef = m_UI.GetComponent<GUIButtonRef>();
            m_closeButtonData = guiButtonRef.GetButtonData("Close");
            m_closeButtonData.Button.onClick.AddListener(animateClose);
            CommonButtonVisual.AddSelectedBorder(m_closeButtonData);

            m_baseChipsText = new TextMeshProUGUI[(int)SLOT_TYPE.LAST];
            CommonChipsVisual.InitChipsInfo(guiRef, m_baseChipsText, ref m_mostFrequent, ref m_leastFrequest);

            m_UI.SetActive(false);
        }

        public void Show(RunData runData, Balance balance)
        {
            m_UI.SetActive(true);

            CommonChipsVisual.Show(runData, balance, m_baseChipsText, m_mostFrequent, m_leastFrequest);

            m_closeButtonData.SelectedGO.SetActive(CommonButtonVisual.ShowSelected());
            CommonButtonVisual.UpdateButtonIcons(m_closeButtonData, Game.Instance.GetGamepadType());
        }

        public void Tick(RunData runData, float dt)
        {
            if (CommonVisual.AnimateCloseTick(ref m_closeTimer, dt))
                Game.Instance.SetMenuState(runData.PrevMenuState);

            if (m_closeTimer <= 0.0f && CommonButtonVisual.NavigateEnter(Game.Instance.GetAvailableInputs()) || CommonButtonVisual.NavigateGamepadButton(m_closeButtonData, Game.Instance.GetAvailableInputs()))
                animateClose();
        }

        public void Hide()
        {
            m_UI.SetActive(false);
        }

        void animateClose()
        {
            SoundManager.Instance.PlaySFXButtonOK();

            CommonVisual.AnimateClose(ref m_closeTimer, m_closeTime, m_animation, "Chips Info Close");
        }
    }
}