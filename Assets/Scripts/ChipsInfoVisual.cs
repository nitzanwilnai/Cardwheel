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

        float m_closeTimer = 0.0f;
        float m_closeTime = 0.1f;
        Animation m_animation;

        public void Init(Camera camera)
        {

            m_UI = AssetManager.Instance.LoadChipsInfoUI();
            m_UI.GetComponent<Canvas>().worldCamera = camera;
            GUIRef guiRef = m_UI.GetComponent<GUIRef>();

            guiRef.GetButton("Close").onClick.AddListener(Game.Instance.CloseChipsInfo);
            m_animation = guiRef.GetAnimation("Animation");

            m_baseChipsText = new TextMeshProUGUI[(int)SLOT_TYPE.LAST];
            CommonChipsVisual.InitChipsInfo(guiRef, m_baseChipsText);

            m_UI.SetActive(false);
        }

        public void Show(RunData runData)
        {
            m_UI.SetActive(true);

            CommonChipsVisual.Show(runData, m_baseChipsText);
        }

        public void Tick(RunData runData, float dt)
        {
            if (CommonVisual.AnimateCloseTick(ref m_closeTimer, dt))
                Game.Instance.SetMenuState(runData.PrevMenuState);
        }

        public void Hide()
        {
            m_UI.SetActive(false);
        }

        public void AnimateClose()
        {
            CommonVisual.AnimateClose(ref m_closeTimer, m_closeTime, m_animation, "Chips Info Close");
        }
    }
}