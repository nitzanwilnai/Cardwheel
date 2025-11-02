using UnityEngine;
using UnityEngine.UI;
using CommonTools;
using TMPro;

namespace Cardwheel
{
    public class CardPackSlotVisual : MonoBehaviour
    {
        COMMON_CARDPACK_BUTTONS m_cardPackButton;

        public AnimationCurve SlotScaleCurve;

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

        ScoringSlot[] m_scoringSlots;

        SpinCircle m_spinCircle;

        RunData runData;
        Balance balance;

        // Start is called before the first frame update
        public void Init(RunData runData, Balance balance, Camera camera)
        {
            this.runData = runData;
            this.balance = balance;

            m_UI = AssetManager.Instance.LoadCardPackSlotUI();
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
                    m_cardPackCardGUIs[i][j].UseButtonData.Button.onClick.AddListener(() => useCardPackOnSlots(localJ));
                    m_cardPackCardGUIs[i][j].UseButtonData.Button.interactable = true;
                }
            }

            CommonVisual.InitTopBarGUI(guiRef.GetGameObject("TopBar"), ref m_topBarGUI);

            m_abandonButtonData = guiButtonRef.GetButtonData("Abandon");
            m_abandonButtonData.Button.onClick.AddListener(Game.Instance.CloseCardPack);

            SpinWheelRef spinWheelRef = guiRef.GetGameObject("SpinWheel").GetComponent<SpinWheelRef>();
            spinWheelRef.SortingPopup.SetActive(false);
            m_spinCircle = spinWheelRef.SpinCircle;
            m_scoringSlots = new ScoringSlot[spinWheelRef.SlotGO.Length];
            for (int i = 0; i < spinWheelRef.SlotGO.Length; i++)
            {
                m_scoringSlots[i] = spinWheelRef.SlotGO[i].GetComponentInChildren<ScoringSlot>();
                m_scoringSlots[i].Index = i;
            }

            CommonButtonVisual.AddSelectedBorder(m_rerollButtonData);
            CommonButtonVisual.AddSelectedBorder(m_abandonButtonData);

            m_UI.SetActive(false);
        }

        public void Show(RunData runData, Balance balance)
        {
            m_UI.SetActive(true);

            m_packAnimationTimer = 0.0f;

            Logic.ResetSlots(runData, balance);

            CommonVisual.ShowTopBarNoSettings(runData, m_topBarGUI, "Card Pack - Slots");

            // show slots
            CommonSlotsVisual.ShowSpinWheelUI(runData, balance, m_scoringSlots, runData.SlotType);

            CardPackCommonVisual.ShowCards(runData, balance, m_cardPackCardGUIs, m_descriptionGOs, balance.CardPackSlotBalance.DescriptionName, balance.CardPackSlotBalance.Weights, balance.CardPackSlotBalance.AffectedSlotType);

            CardPackCommonVisual.ShowRerollButton(runData, balance, m_rerollButtonData.Button, m_rerollCostText);

            m_abandonButtonData.Button.gameObject.SetActive(false);
            m_rerollButtonData.Button.gameObject.SetActive(false);
            m_abandonButtonData.SelectedGO.SetActive(false);
            m_rerollButtonData.SelectedGO.SetActive(false);

            for (int i = 0; i < m_cardPackCardGUIs.Length; i++)
                for (int j = 0; j < m_cardPackCardGUIs[i].Length; j++)
                    m_cardPackCardGUIs[i][j].UseButtonImage.color = balance.ButtonColorEnabled;

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

        public void Tick(RunData runData, Balance balance, float dt)
        {
            CardPackCommonVisual.TickCardPackAnimation(runData, balance, dt, ref m_packAnimationTimer, m_packAnimationTime, m_cardPackCardGUIs, m_descriptionGOs, m_abandonButtonData.Button, m_rerollButtonData.Button);

            runData.SpinWheelAngle += balance.UISpinWheelSpeed * dt;
            m_spinCircle.Angle = runData.SpinWheelAngle;

            if (m_slotChangedTimer > 0.0f)
            {
                m_slotAnimTimer += dt;
                float value = m_slotAnimTimer;

                CommonSlotsVisual.TickHighlightChangedSlots(value, SlotScaleCurve, m_scoringSlots, runData.SlotType, runData.SlotColors);

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

            if (m_cardPackButton >= COMMON_CARDPACK_BUTTONS.CARD_PACK_CARD_1 && m_cardPackButton <= COMMON_CARDPACK_BUTTONS.CARD_PACK_CARD_4 && CommonButtonVisual.NavigateEnter(Game.Instance.GetTickAvailableInputs()))
            {
                // use card
                useCardPackOnSlots(m_cardPackButton - COMMON_CARDPACK_BUTTONS.CARD_PACK_CARD_1);
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

        void useCardPackOnSlots(int cardIdx)
        {
            SoundManager.Instance.PlaySFXButtonOK();

            Logic.UseCardPackSlotCard(runData, balance, cardIdx, CommonSlotsVisual.AffectedSlotsIdxs, ref CommonSlotsVisual.AffectedSlotsCount);

            m_abandonButtonData.Button.gameObject.SetActive(false);
            m_rerollButtonData.Button.gameObject.SetActive(false);

            m_slotChangedTimer = m_slotChangedTime;
            m_slotAnimTimer = 0.0f;

            CommonSlotsVisual.ShowSpinWheelUI(runData, balance, m_scoringSlots, runData.SlotTypeInGame);

            // for (int i = 0; i < m_cardPackCardGUIs.Length; i++)
            //     for (int j = 0; j < m_cardPackCardGUIs[i].Length; j++)
            //         m_cardPackCardGUIs[i][j].UseButtonImage.color = balance.ButtonColorDisabled;

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

        public void Reroll(RunData runData, Balance balance)
        {
            if (Logic.TryRerollCardPack(runData, balance))
            {
                Hide();
                Show(runData, balance);
            }
        }

    }
}