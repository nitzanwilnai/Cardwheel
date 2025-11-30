/*
  Cardwheel — Non-Commercial, No-Modification License
  Copyright © 2025 Nitzan Wilnai
  Source Code: https://github.com/nitzanwilnai/Cardwheel

  Permission is granted to view and run this code for non-commercial purposes only.
  Modification, redistribution of altered versions, and commercial use are strictly prohibited.

  See the LICENSE file for full legal terms.
*/

using UnityEngine;
using CommonTools;
using TMPro;
using UnityEngine.UI;

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

            if ((float)Screen.width / (float)Screen.height > 1.0f)
                m_UI.GetComponent<CanvasScaler>().matchWidthOrHeight = 1.0f;
            else
                m_UI.GetComponent<CanvasScaler>().matchWidthOrHeight = 0.0f;

            GUIButtonRef guiButtonRef = m_UI.GetComponent<GUIButtonRef>();
            m_closeButtonData = guiButtonRef.GetButtonData("Close");
            m_closeButtonData.Button.onClick.AddListener(close);
            CommonButtonVisual.AddSelectedBorder(m_closeButtonData);

            GUIRef guiRef = m_UI.GetComponent<GUIRef>();
            m_text = guiRef.GetTextGUI("Text");

            m_UI.SetActive(false);
        }

        public void Show()
        {
            m_UI.SetActive(true);

            m_closeButtonData.SelectedGO.SetActive(CommonButtonVisual.ShowSelected());
            CommonButtonVisual.UpdateButtonIcons(m_closeButtonData, Game.Instance.GetGamepadType());

            m_text.text = balance.MenuTutorialText[(int)runData.MenuState];
        }

        public bool TutorialClosed()
        {
            if (CommonButtonVisual.NavigateEnter() || CommonButtonVisual.NavigateGamepadButton(m_closeButtonData))
            {
                close();
                return true;
            }
            return false;
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