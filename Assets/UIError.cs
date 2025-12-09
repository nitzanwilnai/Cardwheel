using CommonTools;
using TMPro;
using UnityEngine;

namespace Cardwheel
{
    public class UIError : MonoBehaviour
    {
        public TextMeshProUGUI ErrorMessage;
        GUIButtonData m_closeButtonData;
        void Awake()
        {
            GUIRef guiRef = GetComponent<GUIRef>();
            ErrorMessage = guiRef.GetTextGUI("Error");

            GUIButtonRef guiButtonRef = GetComponent<GUIButtonRef>();
            m_closeButtonData = guiButtonRef.GetButtonData("Close");
            CommonButtonVisual.AddSelectedBorder(m_closeButtonData);
            CommonButtonVisual.UpdateButtonIcons(m_closeButtonData, Game.Instance.GamepadType);
            m_closeButtonData.SelectedGO.SetActive(false);

            m_closeButtonData.Button.onClick.AddListener(close);
        }

        // Update is called once per frame
        void Update()
        {
            if (CommonButtonVisual.NavigateEnter() || CommonButtonVisual.NavigateGamepadButton(m_closeButtonData))
                gameObject.SetActive(false);
        }

        void OnEnable()
        {
            m_closeButtonData.SelectedGO.SetActive(CommonButtonVisual.ShowSelected());

            transform.SetAsLastSibling();
        }

        void close()
        {
            gameObject.SetActive(false);
        }
    }
}