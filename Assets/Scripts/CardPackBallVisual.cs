/*
  Cardwheel — Non-Commercial, No-Modification License
  Copyright © 2025 Nitzan Wilnai
  Source Code: https://github.com/nitzanwilnai/Cardwheel

  Permission is granted to view and run this code for non-commercial purposes only.
  Modification, redistribution of altered versions, and commercial use are strictly prohibited.

  See the LICENSE file for full legal terms.
*/

using UnityEngine;
using UnityEngine.UI;
using CommonTools;
using TMPro;
using UnityEngine.Purchasing;
using UnityEngine.InputSystem;

namespace Cardwheel
{
    public struct CardPackCardGUI
    {
        public GameObject GO;
        public Transform DescriptionParent;
        public Image CardImage;
        public GUIButtonData UseButtonData;
        public Image UseButtonImage;
    }

    public class CardPackBallVisual : MonoBehaviour
    {
        public enum MENU_BUTTONS
        {
            BALL_1 = 30,
            BALL_2 = 31,
            BALL_3 = 32,
            BALL_4 = 33,
            BALL_5 = 34,
            BALL_6 = 35,
            CARD_PACK_CARD_1 = 50,
            CARD_PACK_CARD_2 = 51,
            CARD_PACK_CARD_3 = 52,
            CARD_PACK_CARD_4 = 53,
            REROLL = 60,
            ABANDON = 61,
        }
        MENU_BUTTONS m_cardPackButton;

        public AnimationCurve BallChangeAnimCurve;

        GameObject m_UI;

        TopBarGUI m_topBarGUI;

        UIBallMoveData m_uiBallMoveData = new UIBallMoveData();
        UIBallVisualData m_uiBallVisualData = new UIBallVisualData();

        CardPackCardGUI[][] m_cardPackCardGUIs;

        TextMeshProUGUI m_rerollCostText;

        GameObject[] m_descriptionGOs;
        float m_packAnimationTime = 1.5f;
        float m_packAnimationTimer;

        float m_ballChangedTime = 1.5f;
        float m_ballChangedTimer = 0.0f;
        float m_ballAnimTimer = 0.0f;

        GUIButtonData m_abandonButtonData;
        GUIButtonData m_rerollButtonData;

        GameObject[] m_ballStars;

        RunData runData;
        Balance balance;
        Camera mainCamera;

        // Start is called before the first frame update
        public void Init(RunData runData, Balance balance, Camera mainCamera)
        {
            this.runData = runData;
            this.balance = balance;
            this.mainCamera = mainCamera;

            m_UI = AssetManager.Instance.LoadCardPackBallUI();
            m_UI.GetComponent<Canvas>().worldCamera = mainCamera;
            CommonVisual.ChangeCanvasScalerMatching(m_UI);

            GUIRef guiRef = m_UI.GetComponent<GUIRef>();
            GUIButtonRef guiButtonRef = m_UI.GetComponent<GUIButtonRef>();

            CardPackCommonVisual.InitRerollButton(guiRef, guiButtonRef, ref m_rerollButtonData, ref m_rerollCostText);

            Logic.UnSelectAllCardPacksBalls(runData);

            CommonBallVisual.InitBallsMoveData(balance, guiRef, m_uiBallMoveData);
            CommonBallVisual.InitBallsVisualData(balance, guiRef, m_uiBallVisualData);

            m_ballStars = new GameObject[balance.MaxBalls];
            for (int i = 0; i < balance.MaxBalls; i++)
            {
                GameObject go = guiRef.GetGameObject("Star" + (i + 1).ToString());
                m_ballStars[i] = go;
                m_ballStars[i].SetActive(false);
            }


            m_cardPackCardGUIs = new CardPackCardGUI[3][];
            m_descriptionGOs = new GameObject[balance.MaxShopCardPackCards];
            for (int i = 0; i < 3; i++)
            {
                int numCards = 2 + i;
                CardPackCommonVisual.InitCards(guiRef, ref m_cardPackCardGUIs[i], numCards, i);
                for (int j = 0; j < numCards; j++)
                {
                    int localJ = j;
                    m_cardPackCardGUIs[i][j].UseButtonData.Button.onClick.AddListener(() => useCardPackOnBalls(localJ));
                }
            }

            CommonVisual.InitTopBarGUI(guiRef.GetGameObject("TopBar"), ref m_topBarGUI);

            m_abandonButtonData = guiButtonRef.GetButtonData("Abandon");
            m_abandonButtonData.Button.onClick.AddListener(Game.Instance.AbandonCardPack);

            CommonButtonVisual.AddSelectedBorder(m_rerollButtonData);
            CommonButtonVisual.AddSelectedBorder(m_abandonButtonData);

            m_UI.SetActive(false);

        }

        public void Show()
        {
            m_UI.SetActive(true);

            m_packAnimationTimer = 0.0f;

            Logic.UnSelectAllCardPacksBalls(runData);

            CommonVisual.ShowTopBarNoSettings(runData, m_topBarGUI, "Card Pack - Balls");

            CommonBallVisual.PositionBalls(balance, m_uiBallMoveData);
            CommonBallVisual.ShowBalls(runData.BallTypes, balance, m_uiBallVisualData);

            for (int i = 0; i < m_ballStars.Length; i++)
                m_ballStars[i].SetActive(false);

            Logic.GetCardPackCards(runData, balance, balance.CardPackBallBalance.Weights, balance.CardPackBallBalance.AffectedSlotType);

            CommonVisual.ShowUpdatedCards(runData, balance, balance.CardPackBallBalance.DescriptionName, ref m_packAnimationTimer, m_cardPackCardGUIs, m_descriptionGOs, m_rerollButtonData, m_rerollCostText);

            setUseButtonForCards();

            m_abandonButtonData.Button.gameObject.SetActive(false);
            m_rerollButtonData.Button.gameObject.SetActive(false);
            m_abandonButtonData.SelectedGO.SetActive(false);
            m_rerollButtonData.SelectedGO.SetActive(false);

            selectButton(MENU_BUTTONS.BALL_1);
        }

        void selectButton(MENU_BUTTONS selectedButton)
        {
            COMMON_CARDPACK_BUTTONS cardPackButon = (COMMON_CARDPACK_BUTTONS)selectedButton;
            CardPackCommonVisual.SelectButton(
                runData,
                balance,
                cardPackButon,
                ref cardPackButon,
                m_cardPackCardGUIs,
                m_abandonButtonData,
                m_rerollButtonData);

            m_cardPackButton = (MENU_BUTTONS)cardPackButon;

            for (int i = 0; i < m_uiBallMoveData.BallSelectedGO.Length; i++)
                m_uiBallMoveData.BallSelectedGO[i].SetActive(CommonButtonVisual.ShowSelected() && m_cardPackButton == MENU_BUTTONS.BALL_1 + i);
        }


        public void Hide()
        {
            m_UI.SetActive(false);

            for (int i = 0; i < m_descriptionGOs.Length; i++)
                if (m_descriptionGOs[i] != null)
                    GameObject.Destroy(m_descriptionGOs[i]);

            CommonBallVisual.HideBalls(balance, m_uiBallMoveData);
        }

        public void Tick(float dt)
        {
            CommonBallVisual.TickMoveBalls(dt, m_uiBallMoveData);

            // Debug.Log("m_ballIdx " + m_ballIdx + " m_ballIdx + 1" + (m_ballIdx + 1));
            CommonBallVisual.TickCheckSwapBalls(runData, m_uiBallMoveData, m_uiBallVisualData, true);

            setUseButtonForCards();

            CardPackCommonVisual.TickCardPackAnimation(runData, balance, dt, ref m_packAnimationTimer, m_packAnimationTime, m_cardPackCardGUIs, m_descriptionGOs, m_abandonButtonData.Button, m_rerollButtonData.Button);

            if (m_ballChangedTimer > 0.0f)
            {
                m_ballAnimTimer += dt;
                float value = m_ballAnimTimer;
                if (value > 1.0f)
                    value = 1.0f;
                float scale = BallChangeAnimCurve.Evaluate(value);
                for (int ballIdx = 0; ballIdx < runData.CardPackBallSelected.Length; ballIdx++)
                    if (runData.CardPackBallSelected[ballIdx])
                        m_uiBallVisualData.BallImage[ballIdx].transform.localScale = new Vector3(scale, scale, 1.0f);

                m_ballChangedTimer -= dt;
                if (m_ballChangedTimer <= 0.0f)
                {
                    Hide();
                    Game.Instance.SetMenuState(runData.PrevMenuState);
                }
            }

            handleInput();
        }

        public void HandleTouchInput()
        {
            CommonBallVisual.HanleInputTouchMove(runData, m_uiBallMoveData, mainCamera, true);
        }

        void handleInput()
        {
            if (Gamepad.current != null || Keyboard.current != null)
            {
                MENU_BUTTONS newSelectedButton = (MENU_BUTTONS)CommonBallVisual.HandleInputGamepadKeyboard(runData, m_uiBallMoveData, m_uiBallVisualData, (COMMON_BUTTONS)m_cardPackButton, true);
                selectButton(newSelectedButton);
            }

            COMMON_CARDPACK_BUTTONS currentButton = (COMMON_CARDPACK_BUTTONS)m_cardPackButton;
            if (CardPackCommonVisual.HandleEnter(m_abandonButtonData, m_rerollButtonData, currentButton))
            {
                if (currentButton == COMMON_CARDPACK_BUTTONS.REROLL)
                    selectButton(MENU_BUTTONS.REROLL);

                return;
            }

            if (m_cardPackButton >= MENU_BUTTONS.CARD_PACK_CARD_1 && m_cardPackButton <= MENU_BUTTONS.CARD_PACK_CARD_4 && CommonButtonVisual.NavigateEnter())
            {
                // use card
                int cardIdx = m_cardPackButton - MENU_BUTTONS.CARD_PACK_CARD_1;
                if (checkUseButtonForCard(cardIdx))
                    useCardPackOnBalls(cardIdx);
                return;
            }

            // navigation
            COMMON_CARDPACK_BUTTONS newCardPackButton = CardPackCommonVisual.HandleNavigation((COMMON_CARDPACK_BUTTONS)m_cardPackButton, balance.CardPackMaxCards[runData.SelectedShopCardPackIdx]);
            if ((MENU_BUTTONS)newCardPackButton != m_cardPackButton)
            {
                selectButton((MENU_BUTTONS)newCardPackButton);
                return;
            }

            if (m_cardPackButton >= MENU_BUTTONS.BALL_1 && m_cardPackButton <= MENU_BUTTONS.BALL_6 && CommonButtonVisual.NavigateDown())
            {
                selectButton(MENU_BUTTONS.CARD_PACK_CARD_1);
                return;
            }

            if (m_cardPackButton >= MENU_BUTTONS.CARD_PACK_CARD_1 && m_cardPackButton <= MENU_BUTTONS.CARD_PACK_CARD_4 && CommonButtonVisual.NavigateUp())
            {
                selectButton(MENU_BUTTONS.BALL_1);
                return;
            }


        }

        private bool checkUseButtonForCard(int cardIdx)
        {
            int numBallsSelected = 0;
            for (int i = 0; i < runData.CardPackBallSelected.Length; i++)
                if (runData.CardPackBallSelected[i])
                    numBallsSelected++;

            int cardType = runData.CardPackCardIdxs[cardIdx];
            int numBallsRequired = balance.CardPackBallBalance.NumBalls[cardType];

            return numBallsSelected == numBallsRequired;
        }

        private void setUseButtonForCards()
        {

            int numCards = balance.CardPackMaxCards[runData.SelectedShopCardPackIdx];
            int index = numCards - 2;
            for (int cardIdx = 0; cardIdx < numCards; cardIdx++)
            {
                bool okToUse = checkUseButtonForCard(cardIdx);
                // Debug.Log("cardIdx " + cardIdx + " cardType " + cardType + " numBallsSelected " + numBallsSelected + " numBallsRequired " + numBallsRequired);
                m_cardPackCardGUIs[index][cardIdx].UseButtonData.Button.interactable = okToUse;
                // Debug.Log("m_cardPackCardGUIs[" + index + "][" + cardIdx + "].UseButton.interactable " + m_cardPackCardGUIs[index][cardIdx].UseButton.interactable);
                m_cardPackCardGUIs[index][cardIdx].UseButtonImage.color = okToUse ? balance.ButtonColorEnabled : balance.ButtonColorDisabled;
            }
        }

        void useCardPackOnBalls(int cardIdx)
        {
            SoundManager.Instance.PlaySFXButtonOK();

            Logic.UseCardPackBallCard(runData, balance, cardIdx);

            for (int ballIdx = 0; ballIdx < runData.CardPackBallSelected.Length; ballIdx++)
                if (runData.CardPackBallSelected[ballIdx])
                {
                    // maybe trigger some animation
                    int ballType = runData.BallTypes[ballIdx];
                    m_uiBallVisualData.BallImage[ballIdx].sprite = AssetManager.Instance.LoadBallSprite(balance.BallBalance.BallSprite[ballType]);
                    m_ballStars[ballIdx].SetActive(true);
                    m_ballStars[ballIdx].transform.localPosition = m_uiBallVisualData.BallImage[ballIdx].transform.localPosition;

                    m_uiBallVisualData.BallDescription[ballIdx].text = balance.BallBalance.BallDescription[ballType];
                }

            m_abandonButtonData.Button.gameObject.SetActive(false);
            m_rerollButtonData.Button.gameObject.SetActive(false);

            int numCards = balance.CardPackMaxCards[runData.SelectedShopCardPackIdx];
            int index = numCards - 2;
            for (int i = 0; i < m_cardPackCardGUIs[index].Length; i++)
            {
                if (i != cardIdx)
                    m_cardPackCardGUIs[index][i].GO.SetActive(false);
                m_cardPackCardGUIs[index][i].UseButtonData.Button.gameObject.SetActive(false);
                m_cardPackCardGUIs[index][i].UseButtonImage.color = balance.ButtonColorDisabled;
            }

            m_ballChangedTimer = m_ballChangedTime;
            m_ballAnimTimer = 0.0f;
        }

        public void Reroll()
        {
            if (Logic.TryRerollCardPack(runData, balance, balance.CardPackBallBalance.Weights, balance.CardPackBallBalance.AffectedSlotType))
            {
                m_UI.SetActive(false);

                for (int i = 0; i < m_descriptionGOs.Length; i++)
                    if (m_descriptionGOs[i] != null)
                        GameObject.Destroy(m_descriptionGOs[i]);

                m_UI.SetActive(true);
                CommonVisual.ShowUpdatedCards(runData, balance, balance.CardPackBallBalance.DescriptionName, ref m_packAnimationTimer, m_cardPackCardGUIs, m_descriptionGOs, m_rerollButtonData, m_rerollCostText);
            }
        }
    }
}