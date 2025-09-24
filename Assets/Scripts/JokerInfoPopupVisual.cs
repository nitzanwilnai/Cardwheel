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

        SettingsData settingsData;
        RunData runData;

        int m_jokerIndex;

        public void Init(RunData runData, Camera camera, SettingsData settingsData)
        {
            this.runData = runData;
            this.settingsData = settingsData;

            m_UI = AssetManager.Instance.LoadJokerInfoPopupUI();
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

        public void Show(RunData runData, Balance balance, int jokerIdx, int availableInputs)
        {
            m_jokerIndex = jokerIdx;
            ShowCommon(runData, balance);

            m_sellButtonData.Button.image.color = balance.ButtonColorEnabled;

            m_sellButtonData.Button.onClick.AddListener(() => sellJoker());

            m_sellButtonData.SelectedGO.SetActive(false);
            m_closeButtonData.SelectedGO.SetActive(CommonButtonVisual.NavigateEnter(availableInputs) || CommonButtonVisual.NavigateGamepadButton(m_closeButtonData, availableInputs));
        }

        public void ShowInGame(RunData runData, Balance balance, int jokerIdx, int availableInputs)
        {
            m_jokerIndex = jokerIdx;
            m_sellButtonData.Button.image.color = balance.ButtonColorDisabled;

            m_sellButtonData.SelectedGO.SetActive(false);
            m_closeButtonData.SelectedGO.SetActive(CommonButtonVisual.NavigateEnter(availableInputs) || CommonButtonVisual.NavigateGamepadButton(m_closeButtonData, availableInputs));

            ShowCommon(runData, balance);
        }

        void ShowCommon(RunData runData, Balance balance)
        {
            int jokerType = runData.JokerTypes[m_jokerIndex];
            m_sellButtonData.Button.onClick.RemoveAllListeners();

            m_cost.text = "$" + runData.JokerSellValues[m_jokerIndex].ToString();

            m_shopCard.sprite = AssetManager.Instance.LoadJokerSprite(balance.JokerBalance.JokerSpritesNames[jokerType]);

            GameObject descriptionGO = AssetManager.Instance.GetDescriptionGO(balance.JokerBalance.DescriptionName[jokerType], m_descriptionParent);
            descriptionGO.transform.localPosition = Vector3.zero;
            descriptionGO.transform.localScale = Vector3.one;
            m_descriptionGO = descriptionGO;

            RARITY rarity = balance.JokerBalance.Rarity[jokerType];
            m_rarityText.text = rarity.ToString();
            m_border.color = balance.RarityColors[(int)rarity];
            m_borderRarity.color = balance.RarityColors[(int)rarity];

            CommonVisual.ShowJokerDescriptionInHand(runData, balance, m_descriptionGO, m_jokerIndex);

            m_UI.SetActive(true);
        }

        public void Hide()
        {
            if (m_descriptionGO != null)
                GameObject.Destroy(m_descriptionGO);

            m_UI.SetActive(false);
        }

        public void Tick(int availableInputs)
        {
            if (CommonButtonVisual.NavigateEnter(availableInputs) || CommonButtonVisual.NavigateGamepadButton(m_closeButtonData, availableInputs))
            {
                hideJokerInfoPopup();
                return;
            }

            if (CommonButtonVisual.NavigateEnter(availableInputs) || CommonButtonVisual.NavigateGamepadButton(m_sellButtonData, availableInputs))
            {
                sellJoker();
                return;
            }

        }

        void sellJoker()
        {
            SoundManager.Instance.PlaySFXMoney(settingsData);

            Logic.SellJoker(runData, m_jokerIndex);

            hideJokerInfoPopup();
        }

        void hideJokerInfoPopup()
        {
            SoundManager.Instance.PlaySFXButtonCancel(settingsData);

            Game.Instance.SetMenuState(runData.PrevMenuState);
        }
    }
}