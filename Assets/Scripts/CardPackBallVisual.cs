using UnityEngine;
using UnityEngine.UI;
using CommonTools;
using TMPro;
using UnityEngine.Purchasing;

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
            ABANDON,
            REROLL,
            CARDPACK_1 = 50,
            CARDPACK_2 = 51,
            CARDPACk_3 = 52,
            CARDPACK_4 = 53,
        }
        MENU_BUTTONS m_selectedButton;

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

        // Start is called before the first frame update
        public void Init(RunData runData, Balance balance, Camera camera)
        {
            this.runData = runData;
            this.balance = balance;

            m_UI = AssetManager.Instance.LoadCardPackBallUI();
            m_UI.GetComponent<Canvas>().worldCamera = camera;
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
            // CommonVisual.InitCardsAndBallsGUI(balance, guiRef.GetGameObject("CardsAndBalls"), ref m_cardsAndBallsGUI);

            m_abandonButtonData = guiButtonRef.GetButtonData("Abandon");
            m_abandonButtonData.Button.onClick.AddListener(Game.Instance.CloseCardPack);

            m_UI.SetActive(false);
        }

        void setButton(MENU_BUTTONS selectedButton)
        {
            m_selectedButton = selectedButton;



            for (int i = 0; i < 3; i++)
            {
                int numCards = 2 + i;
                for (int j = 0; j < numCards; j++)
                    m_cardPackCardGUIs[i][j].UseButtonData.SelectedGO.SetActive(m_selectedButton - MENU_BUTTONS.CARDPACK_1 == j);
            }
        }

        public void Show(RunData runData, Balance balance)
        {
            m_UI.SetActive(true);

            m_packAnimationTimer = 0.0f;

            Logic.UnSelectAllCardPacksBalls(runData);

            CommonVisual.ShowTopBarNoSettings(runData, m_topBarGUI, "Card Pack - Balls");

            CommonBallVisual.PositionBalls(balance, m_uiBallMoveData);
            CommonBallVisual.ShowBalls(runData.BallTypes, balance, m_uiBallVisualData);

            for (int i = 0; i < m_ballStars.Length; i++)
                m_ballStars[i].SetActive(false);

            CardPackCommonVisual.ShowCards(runData, balance, m_cardPackCardGUIs, m_descriptionGOs, balance.CardPackBallBalance.DescriptionName, balance.CardPackBallBalance.Weights, balance.CardPackBallBalance.AffectedSlotType);

            CardPackCommonVisual.ShowRerollButton(runData, balance, m_rerollButtonData.Button, m_rerollCostText);

            CheckUseButtonForCards(runData, balance);

            m_abandonButtonData.Button.gameObject.SetActive(false);
            m_rerollButtonData.Button.gameObject.SetActive(false);
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

        public void Tick(RunData runData, Balance balance, Camera camera, float dt, int availableInputs)
        {
            CommonBallVisual.TickMoveBalls(dt, m_uiBallMoveData);

            CommonBallVisual.HanleInputTouchMove(runData, m_uiBallMoveData, camera, true, availableInputs);

            // Debug.Log("m_ballIdx " + m_ballIdx + " m_ballIdx + 1" + (m_ballIdx + 1));
            CommonBallVisual.TickCheckSwapBalls(runData, m_uiBallMoveData, m_uiBallVisualData, true);

            CheckUseButtonForCards(runData, balance);

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
                m_cardPackCardGUIs[index][cardIdx].UseButtonData.Button.interactable = (numBallsSelected == numBallsRequired);
                // Debug.Log("m_cardPackCardGUIs[" + index + "][" + cardIdx + "].UseButton.interactable " + m_cardPackCardGUIs[index][cardIdx].UseButton.interactable);
                m_cardPackCardGUIs[index][cardIdx].UseButtonImage.color = (numBallsSelected == numBallsRequired) ? balance.ButtonColorEnabled : balance.ButtonColorDisabled;
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