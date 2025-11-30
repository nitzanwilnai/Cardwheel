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
using UnityEngine;
using UnityEngine.UI;
using CommonTools;
using TMPro;
using System.Xml;

namespace Cardwheel
{

    public class GameInfoVisual : MonoBehaviour
    {

        GameObject m_UI;

        TextMeshProUGUI[] m_baseChipsText;
        TextMeshProUGUI m_mostFrequent;
        TextMeshProUGUI m_leastFrequest;

        UIBallVisualData m_uiBallVisualData = new UIBallVisualData();

        float m_closeTimer = 0.0f;
        float m_closeTime = 0.1f;
        Animation m_animation;

        GUIButtonData m_closeButtonData;

        public void Init(Camera camera, Balance balance)
        {
            m_UI = AssetManager.Instance.LoadGameInfoUI();
            m_UI.GetComponent<Canvas>().worldCamera = camera;
            CommonVisual.ChangeCanvasScalerMatching(m_UI);

            GUIRef guiRef = m_UI.GetComponent<GUIRef>();
            m_animation = guiRef.GetAnimation("Animation");

            GUIButtonRef guiButtonRef = m_UI.GetComponent<GUIButtonRef>();
            m_closeButtonData = guiButtonRef.GetButtonData("Close");
            m_closeButtonData.Button.onClick.AddListener(animateClose);
            CommonButtonVisual.AddSelectedBorder(m_closeButtonData);

            CommonBallVisual.InitBallsVisualData(balance, guiRef.GetGameObject("Balls").GetComponent<GUIRef>(), m_uiBallVisualData);

            m_baseChipsText = new TextMeshProUGUI[(int)SLOT_TYPE.LAST];
            CommonChipsVisual.InitChipsInfo(guiRef, m_baseChipsText, ref m_mostFrequent, ref m_leastFrequest);

            m_UI.SetActive(false);
        }

        public void Show(RunData runData, Balance balance)
        {
            m_UI.SetActive(true);

            CommonChipsVisual.Show(runData, balance, m_baseChipsText, m_mostFrequent, m_leastFrequest);

            CommonBallVisual.ShowBalls(runData.BallTypesInGame, balance, m_uiBallVisualData);

            m_closeButtonData.SelectedGO.SetActive(CommonButtonVisual.ShowSelected());
            CommonButtonVisual.UpdateButtonIcons(m_closeButtonData, Game.Instance.GetGamepadType());
        }

        public void Tick(RunData runData, float dt)
        {
            if (CommonVisual.AnimateCloseTick(ref m_closeTimer, dt))
                Game.Instance.SetMenuState(runData.PrevMenuState);

            if (CommonButtonVisual.NavigateEnter() || CommonButtonVisual.NavigateGamepadButton(m_closeButtonData))
                animateClose();
        }

        public void Hide()
        {
            m_UI.SetActive(false);
        }

        void animateClose()
        {
            SoundManager.Instance.PlaySFXButtonOK();

            CommonVisual.AnimateClose(ref m_closeTimer, m_closeTime, m_animation, "In Game Info Close");
        }
    }
}