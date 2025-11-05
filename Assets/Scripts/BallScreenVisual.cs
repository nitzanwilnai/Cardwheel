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

        GameObject m_descriptionTouch;
        GameObject m_descriptionKeyboard;
        GameObject m_descriptionGamepad;

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

            m_descriptionTouch = guiRef.GetGameObject("TextTouch");
            m_descriptionKeyboard = guiRef.GetGameObject("TextKeyboard");
            m_descriptionGamepad = guiRef.GetGameObject("TextGamepad");

            m_animation = guiRef.GetAnimation("Animation");

            GUIButtonRef guiButtonRef = m_UI.GetComponent<GUIButtonRef>();
            m_closeButtonData = guiButtonRef.GetButtonData("Close");
            m_closeButtonData.Button.onClick.AddListener(animateClose);
            CommonButtonVisual.AddSelectedBorder(m_closeButtonData);

            CommonBallVisual.InitBallsMoveData(balance, guiRef, m_uiBallMoveData);
            CommonBallVisual.InitBallsVisualData(balance, guiRef, m_uiBallVisualData);

            m_UI.SetActive(false);
        }

        public void Show()
        {

            m_UI.SetActive(true);

            CommonVisual.ShowTopBarNoSettings(runData, m_topBarGUI, "Balls");

            CommonBallVisual.PositionBalls(balance, m_uiBallMoveData);
            CommonBallVisual.ShowBalls(runData.BallTypes, balance, m_uiBallVisualData);

            selectButton(MENU_BUTTONS.CLOSE);

            m_descriptionTouch.SetActive(false);
            m_descriptionKeyboard.SetActive(false);
            m_descriptionGamepad.SetActive(false);
            if (Logic.IsBitSet(Game.Instance.GetAvailableInputs(), (byte)INPUT_TYPES.GAMEPAD))
                m_descriptionGamepad.SetActive(true);
            else if (Logic.IsBitSet(Game.Instance.GetAvailableInputs(), (byte)INPUT_TYPES.KEYBOARD))
                m_descriptionKeyboard.SetActive(true);
            else
                m_descriptionTouch.SetActive(true);

            Canvas.ForceUpdateCanvases();

        }

        public void Hide()
        {
            m_UI.SetActive(false);
            CommonBallVisual.HideBalls(balance, m_uiBallMoveData);
        }

        public void Tick(float dt)
        {
            CommonBallVisual.TickMoveBalls(dt, m_uiBallMoveData);

            handleInput();

            if (m_uiBallMoveData.BallIdx > -1)
                CommonBallVisual.TickCheckSwapBalls(runData, m_uiBallMoveData, m_uiBallVisualData, false);

            if (CommonVisual.AnimateCloseTick(ref m_closeTimer, dt))
                Game.Instance.SetMenuState(runData.PrevMenuState);


        }

        void selectButton(MENU_BUTTONS selectedButton)
        {
            m_selectedButton = selectedButton;

            m_closeButtonData.SelectedGO.SetActive(CommonButtonVisual.ShowSelected() && m_selectedButton == MENU_BUTTONS.CLOSE);
            for (int i = 0; i < m_uiBallMoveData.BallSelectedGO.Length; i++)
                m_uiBallMoveData.BallSelectedGO[i].SetActive(CommonButtonVisual.ShowSelected() && m_selectedButton == MENU_BUTTONS.BALL_1 + i);
        }

        void handleInput()
        {
            if (Logic.IsBitSet(Game.Instance.GetAvailableInputs(), (byte)INPUT_TYPES.GAMEPAD) || Logic.IsBitSet(Game.Instance.GetAvailableInputs(), (byte)INPUT_TYPES.KEYBOARD))
            {
                MENU_BUTTONS newSelectedButton = (MENU_BUTTONS)CommonBallVisual.HandleInputGamepadKeyboard(runData, m_uiBallMoveData, m_uiBallVisualData, (COMMON_BUTTONS)m_selectedButton, false, Game.Instance.GetAvailableInputs());
                selectButton(newSelectedButton);
            }
            else
                CommonBallVisual.HanleInputTouchMove(runData, m_uiBallMoveData, mainCamera, false, Game.Instance.GetAvailableInputs());

            if (m_selectedButton == MENU_BUTTONS.CLOSE && CommonButtonVisual.NavigateEnter(Game.Instance.GetAvailableInputs()) || CommonButtonVisual.NavigateGamepadButton(m_closeButtonData, Game.Instance.GetAvailableInputs()))
            {
                animateClose();
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.CLOSE && CommonButtonVisual.NavigateUp(Game.Instance.GetAvailableInputs()))
            {
                selectButton(MENU_BUTTONS.BALL_1);
                return;
            }

            if (m_selectedButton >= MENU_BUTTONS.BALL_1 && m_selectedButton <= MENU_BUTTONS.BALL_6 && CommonButtonVisual.NavigateDown(Game.Instance.GetAvailableInputs()))
            {
                m_uiBallMoveData.BallIdx = -1;
                selectButton(MENU_BUTTONS.CLOSE);
                return;
            }
        }

        void animateClose()
        {
            SoundManager.Instance.PlaySFXButtonOK();

            CommonVisual.AnimateClose(ref m_closeTimer, m_closeTime, m_animation, "Ball Screen Close");
        }
    }


}