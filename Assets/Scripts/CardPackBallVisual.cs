using UnityEngine;
using UnityEngine.UI;
using CommonTools;
using TMPro;
using System;
using System.Linq;
using Unity.VisualScripting;

namespace Cardwheel
{
    public struct CardPackCardGUI
    {
        public GameObject GO;
        public Transform DescriptionParent;
        public Image CardImage;
        public Button UseButton;
        public Image UseButtonImage;
    }

    public class CardPackBallVisual : MonoBehaviour
    {
        public AnimationCurve BallChangeAnimCurve;
        public AnimationCurve StarAnimCurve;

        GameObject m_UI;

        TopBarGUI m_topBarGUI;

        UIBallMoveData m_uiBallMoveData = new UIBallMoveData();
        UIBallVisualData m_uiBallVisualData = new UIBallVisualData();

        CardPackCardGUI[][] m_cardPackCardGUIs;

        Button m_rerollButton;
        TextMeshProUGUI m_rerollCostText;

        GameObject[] m_descriptionGOs;
        float m_packAnimationTime = 1.5f;
        float m_packAnimationTimer;

        float m_ballChangedTime = 1.5f;
        float m_ballChangedTimer = 0.0f;
        float m_ballAnimTimer = 0.0f;

        Button m_abandonButton;

        GameObject[] m_ballStars;

        // Start is called before the first frame update
        public void Init(RunData runData, Balance balance, Camera camera)
        {
            m_UI = AssetManager.Instance.LoadCardPackBallUI();
            m_UI.GetComponent<Canvas>().worldCamera = camera;
            GUIRef guiRef = m_UI.GetComponent<GUIRef>();

            CardPackCommonVisual.InitRerollButton(guiRef, ref m_rerollButton, ref m_rerollCostText);

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
                    m_cardPackCardGUIs[i][j].UseButton.onClick.AddListener(() => Game.Instance.UseCardPackOnBalls(localJ));
                }
            }

            CommonVisual.InitTopBarGUI(guiRef.GetGameObject("TopBar"), ref m_topBarGUI);
            // CommonVisual.InitCardsAndBallsGUI(balance, guiRef.GetGameObject("CardsAndBalls"), ref m_cardsAndBallsGUI);

            m_abandonButton = guiRef.GetButton("Abandon");
            m_abandonButton.onClick.AddListener(Game.Instance.CloseCardPack);

            m_UI.SetActive(false);
        }

        public void Show(RunData runData, Balance balance)
        {
            m_UI.SetActive(true);

            m_packAnimationTimer = 0.0f;

            Logic.UnSelectAllCardPacksBalls(runData);

            CommonVisual.ShowTopBarNoSettings(runData, m_topBarGUI, "Card Pack - Balls");

            CommonBallVisual.PositionBalls(runData, balance, m_uiBallMoveData);
            CommonBallVisual.ShowBalls(runData.BallTypes, balance, m_uiBallVisualData);

            for (int i = 0; i < m_ballStars.Length; i++)
                m_ballStars[i].SetActive(false);

            CardPackCommonVisual.ShowCards(runData, balance, m_cardPackCardGUIs, m_descriptionGOs, balance.CardPackBallBalance.DescriptionName, balance.CardPackBallBalance.Weights, balance.CardPackBallBalance.AffectedSlotType);

            CardPackCommonVisual.ShowRerollButton(runData, balance, m_rerollButton, m_rerollCostText);

            CheckUseButtonForCards(runData, balance);

            m_abandonButton.gameObject.SetActive(false);
            m_rerollButton.gameObject.SetActive(false);
        }

        public void Hide(Balance balance)
        {
            m_UI.SetActive(false);
            CommonVisual.HideJokers();

            for (int i = 0; i < m_descriptionGOs.Length; i++)
                if (m_descriptionGOs[i] != null)
                    GameObject.Destroy(m_descriptionGOs[i]);

            CommonBallVisual.HideBalls(balance, m_uiBallMoveData);
        }

        public void Tick(RunData runData, Balance balance, Camera camera, float dt)
        {
            CommonBallVisual.TickMoveBalls(dt, m_uiBallMoveData);

            CommonBallVisual.HanleInput(runData, m_uiBallMoveData, camera, true);

            // Debug.Log("m_ballIdx " + m_ballIdx + " m_ballIdx + 1" + (m_ballIdx + 1));
            CommonBallVisual.TickCheckSwapBalls(runData, m_uiBallMoveData, m_uiBallVisualData, true);

            CheckUseButtonForCards(runData, balance);

            CardPackCommonVisual.TickCardPackAnimation(runData, balance, dt, ref m_packAnimationTimer, m_packAnimationTime, m_cardPackCardGUIs, m_descriptionGOs, m_abandonButton, m_rerollButton);

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
                    Hide(balance);
                    Game.Instance.SetMenuState(runData.PrevMenuState);
                }
            }
        }

        private void CheckUseButtonForCards(RunData runData, Balance balance)
        {
            int numBallsSelected = 0;
            for (int i = 0; i < runData.CardPackBallSelected.Length; i++)
                if (runData.CardPackBallSelected[i])
                    numBallsSelected++;

            int numCards = balance.CardPackMaxCards[runData.SelectedShopCardPackIdx];
            int index = numCards - 2;
            for (int cardIdx = 0; cardIdx < numCards; cardIdx++)
            {
                int cardType = runData.CardPackCardIdxs[cardIdx];
                int numBallsRequired = balance.CardPackBallBalance.NumBalls[cardType];
                // Debug.Log("cardIdx " + cardIdx + " cardType " + cardType + " numBallsSelected " + numBallsSelected + " numBallsRequired " + numBallsRequired);
                m_cardPackCardGUIs[index][cardIdx].UseButton.interactable = (numBallsSelected == numBallsRequired);
                Debug.Log("m_cardPackCardGUIs[" + index + "][" + cardIdx + "].UseButton.interactable " + m_cardPackCardGUIs[index][cardIdx].UseButton.interactable);
                m_cardPackCardGUIs[index][cardIdx].UseButtonImage.color = (numBallsSelected == numBallsRequired) ? balance.ButtonColorEnabled : balance.ButtonColorDisabled;
            }
        }

        public void UseCardPackOnBalls(RunData runData, Balance balance, int cardIdx)
        {
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

            m_abandonButton.gameObject.SetActive(false);
            m_rerollButton.gameObject.SetActive(false);

            int numCards = balance.CardPackMaxCards[runData.SelectedShopCardPackIdx];
            int index = numCards - 2;
            for (int i = 0; i < m_cardPackCardGUIs[index].Length; i++)
            {
                if (i != cardIdx)
                    m_cardPackCardGUIs[index][i].GO.SetActive(false);
                m_cardPackCardGUIs[index][i].UseButton.gameObject.SetActive(false);
                m_cardPackCardGUIs[index][i].UseButtonImage.color = balance.ButtonColorDisabled;
            }

            m_ballChangedTimer = m_ballChangedTime;
            m_ballAnimTimer = 0.0f;
        }

        public void Reroll(RunData runData, Balance balance)
        {
            if (Logic.TryRerollCardPack(runData, balance))
            {
                Hide(balance);
                Show(runData, balance);
            }
        }
    }
}