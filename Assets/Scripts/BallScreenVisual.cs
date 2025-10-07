using UnityEngine;
using CommonTools;

namespace Cardwheel
{
    public class BallScreenVisual : MonoBehaviour
    {
        enum MENU_BUTTONS
        {
            CLOSE,
            BALL_1 = 30,
            BALL_2 = 31,
            BALL_3 = 32,
            BALL_4 = 33,
            BALL_5 = 34,
            BALL_6 = 35,
        }
        MENU_BUTTONS m_selectedButton;

        GameObject m_UI;

        TopBarGUI m_topBarGUI;

        UIBallMoveData m_uiBallMoveData = new UIBallMoveData();
        UIBallVisualData m_uiBallVisualData = new UIBallVisualData();

        float m_closeTimer = 0.0f;
        float m_closeTime = 0.1f;
        Animation m_animation;

        GUIButtonData m_closeButtonData;

        RunData runData;
        Balance balance;
        Camera mainCamera;

        public void Init(RunData runData, Balance balance, Camera mainCamera)
        {
            this.runData = runData;
            this.balance = balance;
            this.mainCamera = mainCamera;

            m_UI = AssetManager.Instance.LoadBallScreenUI();
            m_UI.GetComponent<Canvas>().worldCamera = mainCamera;
            CommonVisual.ChangeCanvasScalerMatching(m_UI);

            GUIRef guiRef = m_UI.GetComponent<GUIRef>();
            CommonVisual.InitTopBarGUI(guiRef.GetGameObject("TopBar"), ref m_topBarGUI);

            m_animation = guiRef.GetAnimation("Animation");

            GUIButtonRef guiButtonRef = m_UI.GetComponent<GUIButtonRef>();
            m_closeButtonData = guiButtonRef.GetButtonData("Close");
            m_closeButtonData.Button.onClick.AddListener(animateClose);
            CommonButtonVisual.AddSelectedBorder(m_closeButtonData);

            CommonBallVisual.InitBallsMoveData(balance, guiRef, m_uiBallMoveData);
            CommonBallVisual.InitBallsVisualData(balance, guiRef, m_uiBallVisualData);

            m_UI.SetActive(false);
        }

        public void Show(int availableInputs)
        {

            m_UI.SetActive(true);

            CommonVisual.ShowTopBar(runData, m_topBarGUI, "Balls");

            CommonBallVisual.PositionBalls(runData, balance, m_uiBallMoveData);
            CommonBallVisual.ShowBalls(runData.BallTypes, balance, m_uiBallVisualData);

            if (Logic.IsBitSet(availableInputs, (byte)INPUT_TYPES.GAMEPAD) || Logic.IsBitSet(availableInputs, (byte)INPUT_TYPES.KEYBOARD))
                CommonBallVisual.HideBallSelected(m_uiBallMoveData);

            m_closeButtonData.SelectedGO.SetActive(Logic.IsBitSet(availableInputs, (byte)INPUT_TYPES.GAMEPAD) || Logic.IsBitSet(availableInputs, (byte)INPUT_TYPES.KEYBOARD));
            m_selectedButton = MENU_BUTTONS.CLOSE;

            Canvas.ForceUpdateCanvases();

        }

        public void Hide()
        {
            m_UI.SetActive(false);
        }

        public void Tick(float dt, int availableInputs)
        {
            CommonBallVisual.TickMoveBalls(dt, m_uiBallMoveData);

            handleInput(dt, availableInputs);

            // Debug.Log("m_ballIdx " + m_ballIdx + " m_ballIdx + 1" + (m_ballIdx + 1));
            CommonBallVisual.TickCheckSwapBalls(runData, m_uiBallMoveData, m_uiBallVisualData, false);

            if (CommonVisual.AnimateCloseTick(ref m_closeTimer, dt))
                Game.Instance.SetMenuState(runData.PrevMenuState);


        }

        void selectButton(MENU_BUTTONS selectedButton)
        {
            m_selectedButton = selectedButton;
            m_closeButtonData.SelectedGO.SetActive(m_selectedButton == MENU_BUTTONS.CLOSE);
            for (int i = 0; i < m_uiBallMoveData.BallSelectedGO.Length; i++)
                m_uiBallMoveData.BallSelectedGO[i].SetActive(m_selectedButton == MENU_BUTTONS.BALL_1 + i);
        }

        void handleInput(float dt, int availableInputs)
        {
            if (Logic.IsBitSet(availableInputs, (byte)INPUT_TYPES.GAMEPAD) || Logic.IsBitSet(availableInputs, (byte)INPUT_TYPES.KEYBOARD))
            CommonBallVisual.HandleInputGamepadKeyboard(runData, m_uiBallMoveData, m_uiBallVisualData, (COMMON_BUTTONS)m_selectedButton, false, availableInputs);
            else
            CommonBallVisual.HanleInputTouchMove(runData, m_uiBallMoveData, mainCamera, false, availableInputs);

            if (Logic.IsBitSet(availableInputs, (byte)INPUT_TYPES.GAMEPAD) || Logic.IsBitSet(availableInputs, (byte)INPUT_TYPES.KEYBOARD))
            {
                if (m_selectedButton == MENU_BUTTONS.CLOSE && CommonButtonVisual.NavigateUp(availableInputs))
                {
                    selectButton(MENU_BUTTONS.BALL_1);
                    return;
                }

                if (m_selectedButton >= MENU_BUTTONS.BALL_1 && m_selectedButton <= MENU_BUTTONS.BALL_6 && CommonButtonVisual.NavigateDown(availableInputs))
                {
                    selectButton(MENU_BUTTONS.CLOSE);
                    return;
                }

                if (m_selectedButton >= MENU_BUTTONS.BALL_1 && m_selectedButton < MENU_BUTTONS.BALL_6 && CommonButtonVisual.NavigateLeft(availableInputs))
                {
                    selectButton(m_selectedButton + 1);
                    return;
                }

                if (m_selectedButton > MENU_BUTTONS.BALL_1 && m_selectedButton <= MENU_BUTTONS.BALL_6 && CommonButtonVisual.NavigateRight(availableInputs))
                {
                    selectButton(m_selectedButton - 1);
                    return;
                }
            }
        }

        void animateClose()
        {
            SoundManager.Instance.PlaySFXButtonOK();

            CommonVisual.AnimateClose(ref m_closeTimer, m_closeTime, m_animation, "Ball Screen Close");
        }
    }


}