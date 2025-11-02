using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CommonTools;
using TMPro;

namespace Cardwheel
{
    public enum COMMON_CARDPACK_BUTTONS
    {
        CARD_PACK_CARD_1 = 50,
        CARD_PACK_CARD_2 = 51,
        CARD_PACK_CARD_3 = 52,
        CARD_PACK_CARD_4 = 53,
        REROLL = 60,
        ABANDON = 61,
    }

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
                cardPackCardGUIs[i].UseButtonImage = cardGUIRef.GetImage("Use");

                GUIButtonRef cardGUIButtonRef = go.GetComponent<GUIButtonRef>();
                cardPackCardGUIs[i].UseButtonData = cardGUIButtonRef.GetButtonData("Use");
                CommonButtonVisual.AddSelectedBorder(cardPackCardGUIs[i].UseButtonData);
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
                cardPackCardGUIs[index][i].UseButtonData.Button.gameObject.SetActive(false);
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
                        cardPackCardGUIs[index][i].UseButtonData.Button.gameObject.SetActive(true);
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

        public static void InitRerollButton(GUIRef guiRef, GUIButtonRef guiButtonRef, ref GUIButtonData rerollButtonData, ref TextMeshProUGUI rerollCostText)
        {
            rerollButtonData = guiButtonRef.GetButtonData("Reroll");
            rerollCostText = guiRef.GetTextGUI("Reroll");
            rerollButtonData.Button.onClick.AddListener(Game.Instance.RerollCardPack);
        }

        public static void ShowRerollButton(RunData runData, Balance balance, Button rerollButton, TextMeshProUGUI rerollCostText)
        {
            int cost = Logic.GetCardPackRerollCost(runData, balance);
            rerollButton.image.color = Logic.CanBuy(runData, balance, cost) ? balance.RerollColorEnabled : balance.ButtonColorDisabled;
            rerollCostText.text = "◇" + cost;
        }

        public static void SelectButton(RunData runData, Balance balance, CardPackCardGUI[][] cardPackCardGUIs, COMMON_CARDPACK_BUTTONS cardPackButton)
        {
            int numCards = balance.CardPackMaxCards[runData.SelectedShopCardPackIdx];
            int index = numCards - 2;
            for (int i = 0; i < numCards; i++)
                cardPackCardGUIs[index][i].UseButtonData.SelectedGO.SetActive(cardPackButton == COMMON_CARDPACK_BUTTONS.CARD_PACK_CARD_1 + i);

        }

        public static bool HandleEnter(GUIButtonData abandonButtonData, GUIButtonData rerollButtonData, COMMON_CARDPACK_BUTTONS cardPackButton)
        {
            if (CommonButtonVisual.NavigateGamepadButton(abandonButtonData, Game.Instance.GetTickAvailableInputs()) ||
            cardPackButton == COMMON_CARDPACK_BUTTONS.ABANDON && CommonButtonVisual.NavigateEnter(Game.Instance.GetTickAvailableInputs()))
            {
                Game.Instance.CloseCardPack();
                return true;
            }

            if (CommonButtonVisual.NavigateGamepadButton(rerollButtonData, Game.Instance.GetTickAvailableInputs()) ||
            cardPackButton == COMMON_CARDPACK_BUTTONS.REROLL && CommonButtonVisual.NavigateEnter(Game.Instance.GetTickAvailableInputs()))
            {
                Game.Instance.RerollCardPack();
                return true;
            }

            return false;
        }

        public static COMMON_CARDPACK_BUTTONS HandleNavigation(COMMON_CARDPACK_BUTTONS m_cardPackButton, int maxCards)
        {
            // navigation
            if (m_cardPackButton >= COMMON_CARDPACK_BUTTONS.CARD_PACK_CARD_1 && m_cardPackButton < COMMON_CARDPACK_BUTTONS.CARD_PACK_CARD_4 && CommonButtonVisual.NavigateRight(Game.Instance.GetTickAvailableInputs()))
            {
                return (m_cardPackButton - COMMON_CARDPACK_BUTTONS.CARD_PACK_CARD_1) < maxCards - 1 ? m_cardPackButton + 1 : m_cardPackButton;
            }

            if (m_cardPackButton > COMMON_CARDPACK_BUTTONS.CARD_PACK_CARD_1 && m_cardPackButton <= COMMON_CARDPACK_BUTTONS.CARD_PACK_CARD_4 && CommonButtonVisual.NavigateLeft(Game.Instance.GetTickAvailableInputs()))
                return m_cardPackButton - 1;

            if (m_cardPackButton == COMMON_CARDPACK_BUTTONS.CARD_PACK_CARD_1 && CommonButtonVisual.NavigateLeft(Game.Instance.GetTickAvailableInputs()))
                return COMMON_CARDPACK_BUTTONS.ABANDON;

            if (m_cardPackButton == COMMON_CARDPACK_BUTTONS.ABANDON && CommonButtonVisual.NavigateRight(Game.Instance.GetTickAvailableInputs()))
                return COMMON_CARDPACK_BUTTONS.CARD_PACK_CARD_1;

            if (m_cardPackButton == COMMON_CARDPACK_BUTTONS.ABANDON && CommonButtonVisual.NavigateUp(Game.Instance.GetTickAvailableInputs()))
                return COMMON_CARDPACK_BUTTONS.REROLL;

            if (m_cardPackButton == COMMON_CARDPACK_BUTTONS.REROLL && CommonButtonVisual.NavigateDown(Game.Instance.GetTickAvailableInputs()))
                return COMMON_CARDPACK_BUTTONS.ABANDON;

            if (m_cardPackButton == COMMON_CARDPACK_BUTTONS.REROLL && CommonButtonVisual.NavigateRight(Game.Instance.GetTickAvailableInputs()))
                return COMMON_CARDPACK_BUTTONS.CARD_PACK_CARD_1;

            return m_cardPackButton;
        }

        public static void SelectButton(
            RunData runData,
            Balance balance,
            COMMON_CARDPACK_BUTTONS newSelectedButton,
            ref COMMON_CARDPACK_BUTTONS m_cardPackButton,
            CardPackCardGUI[][] m_cardPackCardGUIs,
            GUIButtonData m_abandonButtonData,
            GUIButtonData m_rerollButtonData)
        {
            m_cardPackButton = newSelectedButton;

            if (Logic.IsBitSet(Game.Instance.GetAvailableInputs(), (byte)INPUT_TYPES.GAMEPAD) || Logic.IsBitSet(Game.Instance.GetAvailableInputs(), (byte)INPUT_TYPES.KEYBOARD))
            {
                m_abandonButtonData.SelectedGO.SetActive(m_cardPackButton == COMMON_CARDPACK_BUTTONS.ABANDON);
                m_rerollButtonData.SelectedGO.SetActive(m_cardPackButton == COMMON_CARDPACK_BUTTONS.REROLL);

                CardPackCommonVisual.SelectButton(runData, balance, m_cardPackCardGUIs, (COMMON_CARDPACK_BUTTONS)m_cardPackButton);
            }
        }
    }
}