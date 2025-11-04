using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CommonTools;
using TMPro;

namespace Cardwheel
{
    public class JokerInfoPopupVisual : MonoBehaviour
    {
        public enum MENU_BUTTONS { NONE, SELL, CLOSE };
        MENU_BUTTONS m_selectedButton = MENU_BUTTONS.NONE;

        GameObject m_UI;

        GameObject m_descriptionGO;

        Image m_shopCard;
        Transform m_descriptionParent;
        TextMeshProUGUI m_cost;

        GUIButtonData m_sellButtonData;
        GUIButtonData m_closeButtonData;

        TextMeshProUGUI m_rarityText;
        Image m_border;
        Image m_borderRarity;

        RunData runData;

        int m_jokerIndex;

        public void Init(RunData runData, Camera camera)
        {
            this.runData = runData;

            m_UI = AssetManager.Instance.LoadJokerInfoPopupUI();
            CommonVisual.ChangeCanvasScalerMatching(m_UI);

            m_UI.GetComponent<Canvas>().worldCamera = camera;

            GUIRef guiRef = m_UI.GetComponent<GUIRef>();

            m_shopCard = guiRef.GetImage("Card");
            m_descriptionParent = guiRef.GetGameObject("Description").transform;
            m_cost = guiRef.GetTextGUI("Cost");

            m_rarityText = guiRef.GetTextGUI("Rarity");
            m_border = guiRef.GetImage("Border");
            m_borderRarity = guiRef.GetImage("BorderRarity");

            GUIButtonRef guiButtonRef = m_UI.GetComponent<GUIButtonRef>();
            m_sellButtonData = guiButtonRef.GetButtonData("Sell");
            m_closeButtonData = guiButtonRef.GetButtonData("Close");

            m_closeButtonData.Button.onClick.AddListener(hideJokerInfoPopup);
            guiButtonRef.GetButtonData("CloseBackground").Button.onClick.AddListener(hideJokerInfoPopup);

            CommonButtonVisual.AddSelectedBorder(m_sellButtonData);
            CommonButtonVisual.AddSelectedBorder(m_closeButtonData);
            m_sellButtonData.SelectedGO.SetActive(false);
            m_closeButtonData.SelectedGO.SetActive(false);

            Hide();
        }

        public void Show(RunData runData, Balance balance, int jokerIdx)
        {
            m_jokerIndex = jokerIdx;
            ShowCommon(runData, balance);

            m_sellButtonData.Button.interactable = true;
            m_sellButtonData.Button.image.color = balance.ButtonColorEnabled;
            m_sellButtonData.Button.onClick.AddListener(() => sellJoker());

        }

        public void ShowInGame(RunData runData, Balance balance, int jokerIdx)
        {
            m_jokerIndex = jokerIdx;
            m_sellButtonData.Button.interactable = false;
            m_sellButtonData.Button.image.color = balance.ButtonColorDisabled;

            ShowCommon(runData, balance);
        }

        void ShowCommon(RunData runData, Balance balance)
        {
            int jokerType = runData.JokerTypes[m_jokerIndex];
            m_sellButtonData.Button.onClick.RemoveAllListeners();

            m_cost.text = "◇" + runData.JokerSellValues[m_jokerIndex].ToString();

            m_shopCard.sprite = AssetManager.Instance.LoadJokerSprite(balance.JokerBalance.JokerSpritesNames[jokerType]);

            GameObject descriptionGO = AssetManager.Instance.GetDescriptionGO(balance.JokerBalance.DescriptionName[jokerType], m_descriptionParent);
            descriptionGO.transform.localPosition = Vector3.zero;
            descriptionGO.transform.localScale = Vector3.one;
            m_descriptionGO = descriptionGO;

            RARITY rarity = balance.JokerBalance.Rarity[jokerType];
            m_rarityText.text = rarity.ToString();
            m_border.color = balance.RarityColors[(int)rarity];
            m_borderRarity.color = balance.RarityColors[(int)rarity];

            CommonVisual.ShowJokerDescriptionCommon(runData, balance, m_descriptionGO, jokerType, m_jokerIndex);

            m_selectedButton = MENU_BUTTONS.CLOSE;
            m_closeButtonData.SelectedGO.SetActive(CommonButtonVisual.ShowSelected());

            m_UI.SetActive(true);
        }

        public void Hide()
        {
            if (m_descriptionGO != null)
                GameObject.Destroy(m_descriptionGO);

            m_UI.SetActive(false);
        }

        public void Tick()
        {
            handleInput();
        }

        void handleInput()
        {
            if ((m_selectedButton == MENU_BUTTONS.CLOSE && CommonButtonVisual.NavigateEnter(Game.Instance.GetAvailableInputs())) || CommonButtonVisual.NavigateGamepadButton(m_closeButtonData, Game.Instance.GetAvailableInputs()))
            {
                hideJokerInfoPopup();
                return;
            }

            if (m_sellButtonData.Button.interactable)
                if ((m_selectedButton == MENU_BUTTONS.SELL && CommonButtonVisual.NavigateEnter(Game.Instance.GetAvailableInputs())) || CommonButtonVisual.NavigateGamepadButton(m_sellButtonData, Game.Instance.GetAvailableInputs()))
                {
                    sellJoker();
                    return;
                }

            if (m_selectedButton == MENU_BUTTONS.CLOSE && CommonButtonVisual.NavigateUp(Game.Instance.GetAvailableInputs()))
            {
                m_selectedButton = MENU_BUTTONS.SELL;
                m_sellButtonData.SelectedGO.SetActive(true);
                m_closeButtonData.SelectedGO.SetActive(false);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.SELL && CommonButtonVisual.NavigateDown(Game.Instance.GetAvailableInputs()))
            {
                m_selectedButton = MENU_BUTTONS.CLOSE;
                m_sellButtonData.SelectedGO.SetActive(false);
                m_closeButtonData.SelectedGO.SetActive(true);
                return;
            }
        }

        void sellJoker()
        {
            SoundManager.Instance.PlaySFXMoney();

            Logic.SellJoker(runData, m_jokerIndex);

            hideJokerInfoPopup();
        }

        void hideJokerInfoPopup()
        {
            SoundManager.Instance.PlaySFXButtonCancel();

            Game.Instance.SetMenuState(runData.PrevMenuState);
        }
    }
}