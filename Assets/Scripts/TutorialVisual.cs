using UnityEngine;
using CommonTools;
using TMPro;

namespace Cardwheel
{

    public class TutorialVisual
    {
        GameObject m_UI;

        TextMeshProUGUI m_text;

        GUIButtonData m_closeButtonData;

        GameData gameData;
        RunData runData;
        Balance balance;

        public void Init(GameData gameData, RunData runData, Balance balance, Camera camera)
        {
            this.gameData = gameData;
            this.runData = runData;
            this.balance = balance;

            m_UI = AssetManager.Instance.LoadTutorialUI();
            Canvas canvas = m_UI.GetComponent<Canvas>();
            canvas.worldCamera = camera;
            CommonVisual.ChangeCanvasScalerMatching(m_UI);

            GUIButtonRef guiButtonRef = m_UI.GetComponent<GUIButtonRef>();
            m_closeButtonData = guiButtonRef.GetButtonData("Close");
            m_closeButtonData.Button.onClick.AddListener(close);
            CommonButtonVisual.AddSelectedBorder(m_closeButtonData);

            GUIRef guiRef = m_UI.GetComponent<GUIRef>();
            m_text = guiRef.GetTextGUI("Text");

            m_UI.SetActive(false);
        }

        public void Show(int availableInputs)
        {
            m_UI.SetActive(true);

            m_closeButtonData.SelectedGO.SetActive(Logic.IsBitSet(availableInputs, (byte)INPUT_TYPES.GAMEPAD) || Logic.IsBitSet(availableInputs, (byte)INPUT_TYPES.KEYBOARD));

            m_text.text = balance.MenuTutorialText[(int)runData.MenuState];
        }

        public void HandleInput(int availableInputs)
        {
            if (CommonButtonVisual.NavigateEnter(availableInputs) || CommonButtonVisual.NavigateGamepadButton(m_closeButtonData, availableInputs))
                close();
        }

        public void Hide()
        {
            gameData.MenuTutorialFlags = Logic.SetBit(gameData.MenuTutorialFlags, (int)runData.MenuState);
            GameDataIO.SaveGameData(gameData);
            m_UI.SetActive(false);
        }

        void close()
        {
            SoundManager.Instance.PlaySFXButtonOK();

            Hide();
        }
    }
}