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

        UIBallVisualData m_uiBallVisualData = new UIBallVisualData();

        float m_closeTimer = 0.0f;
        float m_closeTime = 0.1f;
        Animation m_animation;

        public void Init(Camera camera, Balance balance)
        {
            m_UI = AssetManager.Instance.LoadGameInfoUI();
            m_UI.GetComponent<Canvas>().worldCamera = camera;
            GUIRef guiRef = m_UI.GetComponent<GUIRef>();

            guiRef.GetButton("Close").onClick.AddListener(Game.Instance.CloseGameInfo);
            m_animation = guiRef.GetAnimation("Animation");

            CommonBallVisual.InitBallsVisualData(balance, guiRef.GetGameObject("Balls").GetComponent<GUIRef>(), m_uiBallVisualData);

            m_baseChipsText = new TextMeshProUGUI[(int)SLOT_TYPE.LAST];
            CommonChipsVisual.InitChipsInfo(guiRef, m_baseChipsText);

            m_UI.SetActive(false);
        }

        public void Show(RunData runData, Balance balance)
        {
            m_UI.SetActive(true);

            CommonChipsVisual.Show(runData, m_baseChipsText);

            CommonBallVisual.ShowBalls(runData.BallTypesInGame, balance, m_uiBallVisualData);
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
            CommonVisual.AnimateClose(ref m_closeTimer, m_closeTime, m_animation, "In Game Info Close");
        }
    }
}