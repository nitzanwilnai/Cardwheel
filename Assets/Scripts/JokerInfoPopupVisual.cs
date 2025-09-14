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
        Button m_sellButton;

        TextMeshProUGUI m_rarityText;
        Image m_border;
        Image m_borderRarity;

        public void Init(Camera camera)
        {
            m_UI = AssetManager.Instance.LoadJokerInfoPopupUI();
            m_UI.GetComponent<Canvas>().worldCamera = camera;

            GUIRef guiRef = m_UI.GetComponent<GUIRef>();

            m_shopCard = guiRef.GetImage("Card");
            m_descriptionParent = guiRef.GetGameObject("Description").transform;
            m_cost = guiRef.GetTextGUI("Cost");
            m_sellButton = guiRef.GetButton("Sell");
            guiRef.GetButton("Close").onClick.AddListener(Game.Instance.HideJokerInfoPopup);
            guiRef.GetButton("CloseBackground").onClick.AddListener(Game.Instance.HideJokerInfoPopup);

            m_rarityText = guiRef.GetTextGUI("Rarity");
            m_border = guiRef.GetImage("Border");
            m_borderRarity = guiRef.GetImage("BorderRarity");
            Hide();
        }

        public void Show(RunData runData, Balance balance, int jokerIdx)
        {
            ShowCommon(runData, balance, jokerIdx);

            m_sellButton.image.color = balance.ButtonColorEnabled;

            m_sellButton.onClick.AddListener(() => Game.Instance.SellJoker(jokerIdx));
        }

        public void ShowInGame(RunData runData, Balance balance, int jokerIdx)
        {
            m_sellButton.image.color = balance.ButtonColorDisabled;

            ShowCommon(runData, balance, jokerIdx);
        }

        void ShowCommon(RunData runData, Balance balance, int jokerIdx)
        {
            int jokerType = runData.JokerTypes[jokerIdx];
            m_sellButton.onClick.RemoveAllListeners();

            m_cost.text = "$" + runData.JokerSellValues[jokerIdx].ToString();

            m_shopCard.sprite = AssetManager.Instance.LoadJokerSprite(balance.JokerBalance.JokerSpritesNames[jokerType]);

            GameObject descriptionGO = AssetManager.Instance.GetDescriptionGO(balance.JokerBalance.DescriptionName[jokerType], m_descriptionParent);
            descriptionGO.transform.localPosition = Vector3.zero;
            descriptionGO.transform.localScale = Vector3.one;
            m_descriptionGO = descriptionGO;

            RARITY rarity = balance.JokerBalance.Rarity[jokerType];
            m_rarityText.text = rarity.ToString();
            m_border.color = balance.RarityColors[(int)rarity];
            m_borderRarity.color = balance.RarityColors[(int)rarity];

            CommonVisual.ShowJokerDescriptionInHand(runData, balance, m_descriptionGO, jokerIdx);

            m_UI.SetActive(true);
        }

        public void Hide()
        {
            if (m_descriptionGO != null)
                GameObject.Destroy(m_descriptionGO);

            m_UI.SetActive(false);
        }
    }
}