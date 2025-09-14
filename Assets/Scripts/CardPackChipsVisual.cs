using UnityEngine;
using UnityEngine.UI;
using CommonTools;
using TMPro;

namespace Cardwheel
{
    public class CardPackChipsVisual : MonoBehaviour
    {
        GameObject m_UI;

        TopBarGUI m_topBarGUI;

        CardPackCardGUI[][] m_cardPackCardGUIs;

        Button m_rerollButton;
        TextMeshProUGUI m_rerollCostText;

        GameObject[] m_descriptionGOs;
        float m_packAnimationTime = 1.5f;
        float m_packAnimationTimer;

        float m_slotChangedTime = 1.5f;
        float m_slotChangedTimer = 0.0f;
        float m_slotAnimTimer = 0.0f;

        Button m_abandonButton;

        TextMeshProUGUI[] m_baseChipsText;
        Animation[] m_baseChipsAnimation;

        // Start is called before the first frame update
        public void Init(Balance balance, Camera camera)
        {
            m_UI = AssetManager.Instance.LoadCardPackChipsUI();
            m_UI.GetComponent<Canvas>().worldCamera = camera;
            GUIRef guiRef = m_UI.GetComponent<GUIRef>();

            CardPackCommonVisual.InitRerollButton(guiRef, ref m_rerollButton, ref m_rerollCostText);

            m_cardPackCardGUIs = new CardPackCardGUI[3][];
            m_descriptionGOs = new GameObject[balance.MaxShopCardPackCards];
            for (int i = 0; i < 3; i++)
            {
                int numCards = 2 + i;
                CardPackCommonVisual.InitCards(guiRef, ref m_cardPackCardGUIs[i], numCards, i);
                for (int j = 0; j < numCards; j++)
                {
                    int localJ = j;
                    m_cardPackCardGUIs[i][j].UseButton.onClick.AddListener(() => Game.Instance.UseCardPackOnChips(localJ));
                    m_cardPackCardGUIs[i][j].UseButton.interactable = true;
                }
            }

            CommonVisual.InitTopBarGUI(guiRef.GetGameObject("TopBar"), ref m_topBarGUI);

            m_abandonButton = guiRef.GetButton("Abandon");
            m_abandonButton.onClick.AddListener(Game.Instance.CloseCardPack);

            GUIRef chipsGUIRef = guiRef.GetGameObject("Chips").GetComponent<GUIRef>();
            m_baseChipsText = new TextMeshProUGUI[(int)SLOT_TYPE.LAST];
            for (int i = 0; i < m_baseChipsText.Length; i++)
                m_baseChipsText[i] = chipsGUIRef.GetTextGUI("Chips" + (i + 1));

            m_baseChipsAnimation = new Animation[(int)SLOT_TYPE.LAST];
            for (int i = 0; i < m_baseChipsAnimation.Length; i++)
                m_baseChipsAnimation[i] = chipsGUIRef.GetAnimation("Chips" + (i + 1));

            m_UI.SetActive(false);
        }

        public void Show(RunData runData, Balance balance)
        {
            m_UI.SetActive(true);

            m_packAnimationTimer = 0.0f;

            CommonVisual.ShowTopBarNoSettings(runData, m_topBarGUI, "Card Pack - Slot Chips");

            CardPackCommonVisual.ShowCards(runData, balance, m_cardPackCardGUIs, m_descriptionGOs, balance.CardPackChipsBalance.DescriptionName, balance.CardPackChipsBalance.Weights, balance.CardPackChipsBalance.AffectedSlotType);

            CardPackCommonVisual.ShowRerollButton(runData, balance, m_rerollButton, m_rerollCostText);

            m_abandonButton.gameObject.SetActive(false);
            m_rerollButton.gameObject.SetActive(false);

            for (int i = 0; i < m_cardPackCardGUIs.Length; i++)
                for (int j = 0; j < m_cardPackCardGUIs[i].Length; j++)
                    m_cardPackCardGUIs[i][j].UseButtonImage.color = balance.ButtonColorEnabled;

            for (int i = 0; i < balance.CardPackMaxCards[runData.SelectedShopCardPackIdx]; i++)
            {
                int type = runData.CardPackCardIdxs[i];
                m_descriptionGOs[i].GetComponent<GUIRef>().GetTextGUI("Chips").text = (runData.BaseChips[type] + balance.BaseChips).ToString("N0");
            }

            for (int i = 0; i < m_baseChipsText.Length; i++)
                m_baseChipsText[i].text = "+" + runData.BaseChips[i].ToString("N0");
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
            CardPackCommonVisual.TickCardPackAnimation(runData, balance, dt, ref m_packAnimationTimer, m_packAnimationTime, m_cardPackCardGUIs, m_descriptionGOs, m_abandonButton, m_rerollButton);

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
        }

        public void UseCardPackChips(RunData runData, Balance balance, int cardIdx)
        {
            Logic.UseCardPackChipsCard(runData, balance, cardIdx);

            m_abandonButton.gameObject.SetActive(false);
            m_rerollButton.gameObject.SetActive(false);

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
                m_cardPackCardGUIs[index][i].UseButton.gameObject.SetActive(false);
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