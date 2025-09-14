using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CommonTools;
using TMPro;

namespace Cardwheel
{
    public static class CardPackCommonVisual
    {
        public static void InitCards(GUIRef guiRef, ref CardPackCardGUI[] cardPackCardGUIs, int numCards, int index)
        {
            cardPackCardGUIs = new CardPackCardGUI[numCards];
            for (int i = 0; i < numCards; i++)
            {
                GameObject go = guiRef.GetGameObject("Card" + (index + 2).ToString() + "_" + (i + 1).ToString());
                GUIRef cardGUIRef = go.GetComponent<GUIRef>();
                cardPackCardGUIs[i].GO = go;
                cardPackCardGUIs[i].DescriptionParent = cardGUIRef.GetGameObject("Description").transform;
                cardPackCardGUIs[i].CardImage = cardGUIRef.GetImage("Card");
                cardPackCardGUIs[i].UseButton = cardGUIRef.GetButton("Use");
                cardPackCardGUIs[i].UseButtonImage = cardGUIRef.GetImage("Use");
            }
        }

        public static void ShowCards(RunData runData, Balance balance, CardPackCardGUI[][] cardPackCardGUIs, GameObject[] descriptionGOs, string[] descriptionNames, int[] weights, SLOT_TYPE[] slotTypes)
        {
            int numCards = balance.CardPackMaxCards[runData.SelectedShopCardPackIdx];

            // show cards
            CARD_PACK_TYPE cardPackType = balance.CardPackType[runData.SelectedShopCardPackIdx];
            Logic.GetCardPackCards(runData, balance, weights, slotTypes);
            for (int i = 0; i < cardPackCardGUIs.Length; i++)
                for (int j = 0; j < cardPackCardGUIs[i].Length; j++)
                    cardPackCardGUIs[i][j].GO.SetActive(false);

            int index = numCards - 2;
            for (int i = 0; i < cardPackCardGUIs[index].Length; i++)
            {
                cardPackCardGUIs[index][i].GO.SetActive(true);
                Debug.Log("m_cardPackCardGUIs[" + index + "][" + i + "].GO " + cardPackCardGUIs[index][i].GO.name + " set to active " + cardPackCardGUIs[index][i].GO.activeSelf);
                int cardIdx = runData.CardPackCardIdxs[i];

                if (cardPackType == CARD_PACK_TYPE.BALL)
                    cardPackCardGUIs[index][i].CardImage.sprite = AssetManager.Instance.LoadBallCardSprite();
                else if (cardPackType == CARD_PACK_TYPE.SLOT)
                    cardPackCardGUIs[index][i].CardImage.sprite = AssetManager.Instance.LoadSlotCardSprite();
                else if (cardPackType == CARD_PACK_TYPE.CHIPS)
                    cardPackCardGUIs[index][i].CardImage.sprite = AssetManager.Instance.LoadChipsCardSprite();

                GameObject descriptionGO = AssetManager.Instance.GetDescriptionGO(descriptionNames[cardIdx], cardPackCardGUIs[index][i].DescriptionParent);
                descriptionGOs[i] = descriptionGO;

                descriptionGOs[i].SetActive(false);
                cardPackCardGUIs[index][i].UseButton.gameObject.SetActive(false);
            }
            for (int i = 0; i < descriptionGOs.Length; i++)
            {
                if (descriptionGOs[i] != null)
                {
                    descriptionGOs[i].transform.localPosition = Vector3.zero;
                    descriptionGOs[i].GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
                    descriptionGOs[i].transform.localScale = Vector3.one;
                }
            }
        }

        public static void TickCardPackAnimation(RunData runData, Balance balance, float dt, ref float packAnimationTimer, float packAnimationTime, CardPackCardGUI[][] cardPackCardGUIs, GameObject[] descriptionGOs, Button abandonButton, Button rerollButton)
        {
            if (packAnimationTimer < packAnimationTime)
            {
                packAnimationTimer += dt;
                if (packAnimationTimer >= packAnimationTime)
                {
                    int numCards = balance.CardPackMaxCards[runData.SelectedShopCardPackIdx];
                    int index = numCards - 2;
                    for (int i = 0; i < cardPackCardGUIs[index].Length; i++)
                    {
                        descriptionGOs[i].SetActive(true);
                        cardPackCardGUIs[index][i].UseButton.gameObject.SetActive(true);
                    }

                    for (int i = 0; i < descriptionGOs.Length; i++)
                    {
                        if (descriptionGOs[i] != null)
                        {
                            descriptionGOs[i].transform.localPosition = Vector3.zero;
                            descriptionGOs[i].GetComponent<RectTransform>().anchoredPosition = Vector3.zero;
                            descriptionGOs[i].transform.localScale = Vector3.one;
                        }
                    }

                    abandonButton.gameObject.SetActive(true);
                    rerollButton.gameObject.SetActive(true);
                }
            }
        }

        public static void InitRerollButton(GUIRef guiRef, ref Button rerollButton, ref TextMeshProUGUI rerollCostText)
        {
            rerollButton = guiRef.GetButton("Reroll");
            rerollCostText = guiRef.GetTextGUI("Reroll");
            rerollButton.onClick.AddListener(Game.Instance.RerollCardPack);
        }

        public static void ShowRerollButton(RunData runData, Balance balance, Button rerollButton, TextMeshProUGUI rerollCostText)
        {
            int cost = Logic.GetCardPackRerollCost(runData, balance);
            rerollButton.image.color = Logic.CanBuy(runData, balance, cost) ? balance.RerollColorEnabled : balance.ButtonColorDisabled;
            rerollCostText.text = "$" + cost;
        }
    }
}