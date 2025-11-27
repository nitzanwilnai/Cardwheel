using UnityEngine;
using UnityEngine.UI;
using CommonTools;
using TMPro;

namespace Cardwheel
{
    public class CardPackChipsVisual : MonoBehaviour
    {
        COMMON_CARDPACK_BUTTONS m_cardPackButton;

        GameObject m_UI;

        TopBarGUI m_topBarGUI;

        CardPackCardGUI[][] m_cardPackCardGUIs;

        GUIButtonData m_rerollButtonData;
        TextMeshProUGUI m_rerollCostText;

        GameObject[] m_descriptionGOs;
        float m_packAnimationTime = 1.5f;
        float m_packAnimationTimer;

        float m_slotChangedTime = 1.5f;
        float m_slotChangedTimer = 0.0f;
        float m_slotAnimTimer = 0.0f;

        GUIButtonData m_abandonButtonData;

        TextMeshProUGUI[] m_baseChipsText;
        Animation[] m_baseChipsAnimation;

        RunData runData;
        Balance balance;

        // Start is called before the first frame update
        public void Init(RunData runData, Balance balance, Camera camera)
        {
            this.runData = runData;
            this.balance = balance;

            m_UI = AssetManager.Instance.LoadCardPackChipsUI();
            m_UI.GetComponent<Canvas>().worldCamera = camera;
            CommonVisual.ChangeCanvasScalerMatching(m_UI);

            GUIRef guiRef = m_UI.GetComponent<GUIRef>();
            GUIButtonRef guiButtonRef = m_UI.GetComponent<GUIButtonRef>();

            CardPackCommonVisual.InitRerollButton(guiRef, guiButtonRef, ref m_rerollButtonData, ref m_rerollCostText);

            m_cardPackCardGUIs = new CardPackCardGUI[3][];
            m_descriptionGOs = new GameObject[balance.MaxShopCardPackCards];
            for (int i = 0; i < 3; i++)
            {
                int numCards = 2 + i;
                CardPackCommonVisual.InitCards(guiRef, ref m_cardPackCardGUIs[i], numCards, i);
                for (int j = 0; j < numCards; j++)
                {
                    int localJ = j;
                    m_cardPackCardGUIs[i][j].UseButtonData.Button.onClick.AddListener(() => useCardPackChips(localJ));
                    m_cardPackCardGUIs[i][j].UseButtonData.Button.interactable = true;
                }
            }

            CommonVisual.InitTopBarGUI(guiRef.GetGameObject("TopBar"), ref m_topBarGUI);

            m_abandonButtonData = guiButtonRef.GetButtonData("Abandon");
            m_abandonButtonData.Button.onClick.AddListener(Game.Instance.AbandonCardPack);

            GUIRef chipsGUIRef = guiRef.GetGameObject("Chips").GetComponent<GUIRef>();
            m_baseChipsText = new TextMeshProUGUI[(int)SLOT_TYPE.LAST];
            for (int i = 0; i < m_baseChipsText.Length; i++)
                m_baseChipsText[i] = chipsGUIRef.GetTextGUI("Chips" + (i + 1));

            m_baseChipsAnimation = new Animation[(int)SLOT_TYPE.LAST];
            for (int i = 0; i < m_baseChipsAnimation.Length; i++)
                m_baseChipsAnimation[i] = chipsGUIRef.GetAnimation("Chips" + (i + 1));

            CommonButtonVisual.AddSelectedBorder(m_rerollButtonData);
            CommonButtonVisual.AddSelectedBorder(m_abandonButtonData);

            m_UI.SetActive(false);
        }

        public void Show()
        {
            m_UI.SetActive(true);

            m_packAnimationTimer = 0.0f;

            CommonVisual.ShowTopBarNoSettings(runData, m_topBarGUI, "Card Pack - Slot Chips");

            Logic.GetCardPackCards(runData, balance, balance.CardPackChipsBalance.Weights, balance.CardPackChipsBalance.AffectedSlotType);

            CommonVisual.ShowUpdatedCards(runData, balance, balance.CardPackChipsBalance.DescriptionName, ref m_packAnimationTimer, m_cardPackCardGUIs, m_descriptionGOs, m_rerollButtonData, m_rerollCostText);

            m_abandonButtonData.Button.gameObject.SetActive(false);
            m_rerollButtonData.Button.gameObject.SetActive(false);
            m_abandonButtonData.SelectedGO.SetActive(false);
            m_rerollButtonData.SelectedGO.SetActive(false);

            for (int i = 0; i < m_cardPackCardGUIs.Length; i++)
                for (int j = 0; j < m_cardPackCardGUIs[i].Length; j++)
                    m_cardPackCardGUIs[i][j].UseButtonImage.color = balance.ButtonColorEnabled;

            for (int i = 0; i < m_cardPackCardGUIs.Length; i++)
                for (int j = 0; j < m_cardPackCardGUIs[i].Length; j++)
                    m_cardPackCardGUIs[i][j].UseButtonData.SelectedGO.SetActive(false);

            for (int i = 0; i < balance.CardPackMaxCards[runData.SelectedShopCardPackIdx]; i++)
            {
                int type = runData.CardPackCardIdxs[i];
                m_descriptionGOs[i].GetComponent<GUIRef>().GetTextGUI("Chips").text = (runData.BaseChips[type] + balance.BaseChips).ToString("N0");
            }

            for (int i = 0; i < m_baseChipsText.Length; i++)
                m_baseChipsText[i].text = "+" + runData.BaseChips[i].ToString("N0");

            CardPackCommonVisual.SelectButton(
                runData,
                balance,
                COMMON_CARDPACK_BUTTONS.CARD_PACK_CARD_1,
                ref m_cardPackButton,
                m_cardPackCardGUIs,
                m_abandonButtonData,
                m_rerollButtonData);
        }

        public void Hide()
        {
            m_UI.SetActive(false);

            for (int i = 0; i < m_descriptionGOs.Length; i++)
                if (m_descriptionGOs[i] != null)
                    GameObject.Destroy(m_descriptionGOs[i]);
        }

        public void Tick(float dt)
        {
            CardPackCommonVisual.TickCardPackAnimation(runData, balance, dt, ref m_packAnimationTimer, m_packAnimationTime, m_cardPackCardGUIs, m_descriptionGOs, m_abandonButtonData.Button, m_rerollButtonData.Button);

            runData.SpinWheelAngle += balance.UISpinWheelSpeed * dt;

            if (m_slotChangedTimer > 0.0f)
            {
                m_slotAnimTimer += dt;
                float value = m_slotAnimTimer;

                m_slotChangedTimer -= dt;
                if (m_slotChangedTimer <= 0.0f)
                {
                    Hide();
                    Game.Instance.SetMenuState(runData.PrevMenuState);
                }
            }

            handleInput();
        }

        void handleInput()
        {
            COMMON_CARDPACK_BUTTONS currentButton = m_cardPackButton;
            if (CardPackCommonVisual.HandleEnter(m_abandonButtonData, m_rerollButtonData, currentButton))
            {
                if (currentButton == COMMON_CARDPACK_BUTTONS.REROLL)
                    CardPackCommonVisual.SelectButton(
                        runData,
                        balance,
                        currentButton,
                        ref m_cardPackButton,
                        m_cardPackCardGUIs,
                        m_abandonButtonData,
                        m_rerollButtonData);

                return;
            }

            if (m_cardPackButton >= COMMON_CARDPACK_BUTTONS.CARD_PACK_CARD_1 && m_cardPackButton <= COMMON_CARDPACK_BUTTONS.CARD_PACK_CARD_4 && CommonButtonVisual.NavigateEnter())
            {
                // use card
                useCardPackChips(m_cardPackButton - COMMON_CARDPACK_BUTTONS.CARD_PACK_CARD_1);
                return;
            }

            COMMON_CARDPACK_BUTTONS newCardPackButton = CardPackCommonVisual.HandleNavigation(m_cardPackButton, balance.CardPackMaxCards[runData.SelectedShopCardPackIdx]);
            if (newCardPackButton != m_cardPackButton)
            {
                CardPackCommonVisual.SelectButton(
                    runData,
                    balance,
                    newCardPackButton,
                    ref m_cardPackButton,
                    m_cardPackCardGUIs,
                    m_abandonButtonData,
                    m_rerollButtonData);
                return;
            }
        }

        void useCardPackChips(int cardIdx)
        {
            SoundManager.Instance.PlaySFXButtonOK();

            Logic.UseCardPackChipsCard(runData, balance, cardIdx);

            m_abandonButtonData.Button.gameObject.SetActive(false);
            m_rerollButtonData.Button.gameObject.SetActive(false);

            m_slotChangedTimer = m_slotChangedTime;
            m_slotAnimTimer = 0.0f;

            // need to play the grow animation
            int slotType = runData.CardPackCardIdxs[cardIdx];
            m_baseChipsAnimation[slotType].Play("ScoreGrow");
            m_baseChipsText[slotType].text = "+" + runData.BaseChips[slotType].ToString("N0");

            int numCards = balance.CardPackMaxCards[runData.SelectedShopCardPackIdx];
            int index = numCards - 2;
            for (int i = 0; i < m_cardPackCardGUIs[index].Length; i++)
            {
                if (i != cardIdx)
                    m_cardPackCardGUIs[index][i].GO.SetActive(false);
                m_cardPackCardGUIs[index][i].UseButtonData.Button.gameObject.SetActive(false);
                m_cardPackCardGUIs[index][i].UseButtonImage.color = balance.ButtonColorDisabled;
            }

        }

        public void Reroll()
        {
            if (Logic.TryRerollCardPack(runData, balance, balance.CardPackChipsBalance.Weights, balance.CardPackChipsBalance.AffectedSlotType))
            {
                Hide();
                m_UI.SetActive(true);
                CommonVisual.ShowUpdatedCards(runData, balance, balance.CardPackChipsBalance.DescriptionName, ref m_packAnimationTimer, m_cardPackCardGUIs, m_descriptionGOs, m_rerollButtonData, m_rerollCostText);

                for (int i = 0; i < balance.CardPackMaxCards[runData.SelectedShopCardPackIdx]; i++)
                {
                    int type = runData.CardPackCardIdxs[i];
                    m_descriptionGOs[i].GetComponent<GUIRef>().GetTextGUI("Chips").text = (runData.BaseChips[type] + balance.BaseChips).ToString("N0");
                }
            }
        }

    }
}