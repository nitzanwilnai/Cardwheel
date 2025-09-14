using UnityEngine;
using UnityEngine.UI;
using CommonTools;
using TMPro;

namespace Cardwheel
{
    public class BallScreenVisual : MonoBehaviour
    {
        GameObject m_UI;

        TopBarGUI m_topBarGUI;

        UIBallMoveData m_uiBallMoveData = new UIBallMoveData();
        UIBallVisualData m_uiBallVisualData = new UIBallVisualData();

        float m_closeTimer = 0.0f;
        float m_closeTime = 0.1f;
        Animation m_animation;

        public void Init(Balance balance, Camera camera)
        {
            m_UI = AssetManager.Instance.LoadBallScreenUI();
            m_UI.GetComponent<Canvas>().worldCamera = camera;

            GUIRef guiRef = m_UI.GetComponent<GUIRef>();
            CommonVisual.InitTopBarGUI(guiRef.GetGameObject("TopBar"), ref m_topBarGUI);

            m_animation = guiRef.GetAnimation("Animation");

            CommonBallVisual.InitBallsMoveData(balance, guiRef, m_uiBallMoveData);
            CommonBallVisual.InitBallsVisualData(balance, guiRef, m_uiBallVisualData);

            m_UI.SetActive(false);

        }

        public void Show(RunData runData, Balance balance)
        {

            m_UI.SetActive(true);

            CommonVisual.ShowTopBar(runData, m_topBarGUI, "Balls");

            CommonBallVisual.PositionBalls(runData, balance, m_uiBallMoveData);
            CommonBallVisual.ShowBalls(runData.BallTypes, balance, m_uiBallVisualData);

            GUIRef guiRef = m_UI.GetComponent<GUIRef>();
            guiRef.GetButton("Close").onClick.RemoveAllListeners();
            guiRef.GetButton("Close").onClick.AddListener(Game.Instance.CloseBallScreen);

            Canvas.ForceUpdateCanvases();
        }

        public void Hide()
        {
            m_UI.SetActive(false);
        }

        public void Tick(RunData runData, Camera camera, float dt)
        {
            CommonBallVisual.TickMoveBalls(dt, m_uiBallMoveData);

            CommonBallVisual.HanleInput(runData, m_uiBallMoveData, camera, false);

            // Debug.Log("m_ballIdx " + m_ballIdx + " m_ballIdx + 1" + (m_ballIdx + 1));
            CommonBallVisual.TickCheckSwapBalls(runData, m_uiBallMoveData, m_uiBallVisualData, false);

            if (CommonVisual.AnimateCloseTick(ref m_closeTimer, dt))
                Game.Instance.SetMenuState(runData.PrevMenuState);
        }

        public void AnimateClose()
        {
            CommonVisual.AnimateClose(ref m_closeTimer, m_closeTime, m_animation, "Ball Screen Close");
        }
    }


}