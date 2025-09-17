using UnityEngine;
using UnityEngine.UI;
using CommonTools;
using TMPro;
using System;

namespace Cardwheel
{
    public struct JokerBuyPopupGUI
    {
        public GameObject GO;
        public Image ShopCard;
        public Transform DescriptionParent;
        public TextMeshProUGUI Cost;
        public Button BuyButton;
        public Image BuyButtonImage;
        public Button CancelButton;
        public TextMeshProUGUI RarityText;
        public Image Border;
        public Image BorderRarity;
    }

    public struct CardPackBuyPopupGUI
    {
        public GameObject GO;
        public Image ShopCard;
        public TextMeshProUGUI Description;
        public TextMeshProUGUI Cost;
        public Button BuyButton;
        public Image BuyButtonImage;
        public Button CancelButton;
    }

    public struct VoucherBuyPopupGUI
    {
        public GameObject GO;
        public Image ShopCard;
        public TextMeshProUGUI Description;
        public TextMeshProUGUI Cost;
        public Button BuyButton;
        public Image BuyButtonImage;
        public Button CancelButton;
    }

    public struct ShopCardGUI
    {
        public GameObject GO;
        public GameObject JokerGO;
        public TextMeshProUGUI CostText;
        public Image CardImage;
        public GameObject RainbowGO;
        public GameObject MetalGO;
        public GameObject ShinyGO;
    }

    public struct CardPackGUI
    {
        public GameObject GO;
        public GameObject CardPackGO;
        public TextMeshProUGUI CostText;
        public Image CardImage;
    }


    public struct VoucherGUI
    {
        public GameObject GO;
        public GameObject CardPackGO;
        public TextMeshProUGUI CostText;
        public Image CardImage;
    }

    public class ShopVisual : MonoBehaviour
    {
        GameObject m_UI;

        TopBarGUI m_topBarGUI;

        VerticalLayoutGroup m_verticalLayoutGroup;

        ShopCardGUI[] m_jokers;
        VoucherGUI m_voucher;
        CardPackGUI[] m_cardPacks;

        Image m_rerollButtonImage;
        TextMeshProUGUI m_rerollCostText;

        CardsBallsSpinWheelGUI m_cardsBallsSpinWheelGUI;

        JokerBuyPopupGUI m_jokerBuyPopupGUI;
        CardPackBuyPopupGUI m_cardPackBuyPopupGUI;
        VoucherBuyPopupGUI m_voucherBuyPopupGUI;

        GameObject m_descriptionGO;

        float m_hidePopupTimer;
        float m_hidePopupTime = 0.1f;
        Animation m_voucherPopupAnimation;
        Animation m_jokerPopupAnimation;
        Animation m_cardpackPopupAnimation;

        // Start is called before the first frame update
        public void Init(RunData runData, Balance balance, Camera camera)
        {
            m_UI = AssetManager.Instance.LoadShopUI();
            m_UI.GetComponent<Canvas>().worldCamera = camera;
            GUIRef guiRef = m_UI.GetComponent<GUIRef>();


            m_jokers = new ShopCardGUI[3];
            for (int i = 0; i < m_jokers.Length; i++)
                initShopCardGUI(guiRef.GetGameObject("Joker" + (i + 1).ToString()), ref m_jokers[i]);

            initVoucherGUI(guiRef.GetGameObject("Voucher"), ref m_voucher);

            m_cardPacks = new CardPackGUI[2];
            for (int i = 0; i < m_cardPacks.Length; i++)
                initCardPackGUI(guiRef.GetGameObject("CardPack" + (i + 1).ToString()), ref m_cardPacks[i]);
            for (int i = 0; i < m_cardPacks.Length; i++)
                m_cardPacks[i].GO.SetActive(false);

            m_rerollButtonImage = guiRef.GetImage("Reroll");
            guiRef.GetButton("Reroll").onClick.AddListener(Game.Instance.RerollShop);
            m_rerollCostText = guiRef.GetTextGUI("RerollCost");
            guiRef.GetButton("NextRound").onClick.AddListener(Game.Instance.GoToRoundSelection);
            guiRef.GetButton("Info").onClick.AddListener(Game.Instance.ShowShopInfo);

            m_jokerPopupAnimation = guiRef.GetAnimation("JokerPopup");
            m_cardpackPopupAnimation = guiRef.GetAnimation("CardpackPopup");
            m_voucherPopupAnimation = guiRef.GetAnimation("VoucherPopup");

            CommonVisual.InitTopBarGUI(guiRef.GetGameObject("TopBar"), ref m_topBarGUI);
            CommonVisual.InitCardsBallsSpinWheelGUI(runData, balance, guiRef.GetGameObject("CardsAndBalls"), ref m_cardsBallsSpinWheelGUI);

            m_jokerBuyPopupGUI.GO = guiRef.GetGameObject("JokerBuyPopup");
            GUIRef jokerBuyPopupGUIRef = m_jokerBuyPopupGUI.GO.GetComponent<GUIRef>();
            m_jokerBuyPopupGUI.ShopCard = jokerBuyPopupGUIRef.GetImage("Card");
            m_jokerBuyPopupGUI.DescriptionParent = jokerBuyPopupGUIRef.GetGameObject("Description").transform;
            m_jokerBuyPopupGUI.Cost = jokerBuyPopupGUIRef.GetTextGUI("Cost");
            m_jokerBuyPopupGUI.BuyButton = jokerBuyPopupGUIRef.GetButton("Buy");
            m_jokerBuyPopupGUI.BuyButtonImage = jokerBuyPopupGUIRef.GetImage("Buy");
            m_jokerBuyPopupGUI.RarityText = jokerBuyPopupGUIRef.GetTextGUI("Rarity");
            m_jokerBuyPopupGUI.Border = jokerBuyPopupGUIRef.GetImage("Border");
            m_jokerBuyPopupGUI.BorderRarity = jokerBuyPopupGUIRef.GetImage("BorderRarity");

            jokerBuyPopupGUIRef.GetButton("Cancel").onClick.AddListener(HideJokerBuyPopup);

            m_cardPackBuyPopupGUI.GO = guiRef.GetGameObject("CardPackBuyPopup");
            GUIRef cardPackBuyPopupGUIRef = m_cardPackBuyPopupGUI.GO.GetComponent<GUIRef>();
            m_cardPackBuyPopupGUI.ShopCard = cardPackBuyPopupGUIRef.GetImage("Card");
            m_cardPackBuyPopupGUI.Description = cardPackBuyPopupGUIRef.GetTextGUI("Description");
            m_cardPackBuyPopupGUI.Cost = cardPackBuyPopupGUIRef.GetTextGUI("Cost");
            m_cardPackBuyPopupGUI.BuyButton = cardPackBuyPopupGUIRef.GetButton("Buy");
            m_cardPackBuyPopupGUI.BuyButtonImage = cardPackBuyPopupGUIRef.GetImage("Buy");
            cardPackBuyPopupGUIRef.GetButton("Cancel").onClick.AddListener(HideCardpackBuyPopup);

            m_voucherBuyPopupGUI.GO = guiRef.GetGameObject("VoucherBuyPopup");
            GUIRef voucherBuyPopupGUIRef = m_voucherBuyPopupGUI.GO.GetComponent<GUIRef>();
            m_voucherBuyPopupGUI.ShopCard = voucherBuyPopupGUIRef.GetImage("Card");
            m_voucherBuyPopupGUI.Description = voucherBuyPopupGUIRef.GetTextGUI("Description");
            m_voucherBuyPopupGUI.Cost = voucherBuyPopupGUIRef.GetTextGUI("Cost");
            m_voucherBuyPopupGUI.BuyButton = voucherBuyPopupGUIRef.GetButton("Buy");
            m_voucherBuyPopupGUI.BuyButtonImage = voucherBuyPopupGUIRef.GetImage("Buy");
            voucherBuyPopupGUIRef.GetButton("Cancel").onClick.AddListener(HideVoucherBuyPopup);

            m_verticalLayoutGroup = m_UI.GetComponent<VerticalLayoutGroup>();

            m_UI.SetActive(false);
        }

        void initVoucherGUI(GameObject go, ref VoucherGUI voucherGUI)
        {
            GUIRef guiRef = go.GetComponent<GUIRef>();
            voucherGUI.GO = go;
            voucherGUI.CostText = guiRef.GetTextGUI("Cost");
            voucherGUI.CardPackGO = guiRef.GetGameObject("Card");

            GUIRef jokerGUIRef = voucherGUI.CardPackGO.GetComponent<GUIRef>();
            voucherGUI.CardImage = jokerGUIRef.GetImage("Joker");
        }

        void initCardPackGUI(GameObject go, ref CardPackGUI cardPackGUI)
        {
            GUIRef guiRef = go.GetComponent<GUIRef>();
            cardPackGUI.GO = go;
            cardPackGUI.CostText = guiRef.GetTextGUI("Cost");
            cardPackGUI.CardPackGO = guiRef.GetGameObject("Card");

            GUIRef jokerGUIRef = cardPackGUI.CardPackGO.GetComponent<GUIRef>();
            cardPackGUI.CardImage = jokerGUIRef.GetImage("Joker");
        }

        void initShopCardGUI(GameObject go, ref ShopCardGUI shopCardGUI)
        {
            GUIRef guiRef = go.GetComponent<GUIRef>();
            shopCardGUI.GO = go;
            shopCardGUI.CostText = guiRef.GetTextGUI("Cost");
            shopCardGUI.JokerGO = guiRef.GetGameObject("Card");

            GUIRef jokerGUIRef = shopCardGUI.JokerGO.GetComponent<GUIRef>();
            shopCardGUI.CardImage = jokerGUIRef.GetImage("Joker");
            shopCardGUI.RainbowGO = jokerGUIRef.GetGameObject("Rainbow");
            shopCardGUI.ShinyGO = jokerGUIRef.GetGameObject("Shiny");
            shopCardGUI.MetalGO = jokerGUIRef.GetGameObject("Metal");

            shopCardGUI.RainbowGO.SetActive(false);
            shopCardGUI.ShinyGO.SetActive(false);
            shopCardGUI.MetalGO.SetActive(false);
            jokerGUIRef.GetGameObject("Debuffed").SetActive(false);
        }

        public void Show(RunData runData, Balance balance)
        {
            m_UI.SetActive(true);

            CommonVisual.ShowTopBar(runData, m_topBarGUI, "Shop");

            for (int i = 0; i < balance.MaxShopJokers; i++)
                m_jokers[i].GO.SetActive(false);

            // show jokers
            for (int i = 0; i < runData.ShopJokerCount; i++)
            {
                int jokerType = runData.ShopJokerIdxs[i];
                if (jokerType > -1)
                {
                    m_jokers[i].GO.SetActive(true);

                    m_jokers[i].CardImage.sprite = AssetManager.Instance.LoadJokerSprite(balance.JokerBalance.JokerSpritesNames[jokerType]);

                    int localI = i;
                    m_jokers[i].JokerGO.GetComponent<Button>().onClick.RemoveAllListeners();
                    m_jokers[i].JokerGO.GetComponent<Button>().onClick.AddListener(() => Game.Instance.ShowJokerBuyPopup(localI));

                    m_jokers[i].CostText.text = "$" + Logic.GetJokerShopCost(runData, balance, jokerType).ToString();
                }
            }

            // show card packs
            for (int i = 0; i < balance.MaxShopCardPacks; i++)
            {
                int cardPackIdx = runData.ShopCardPackIdxs[i];
                if (cardPackIdx > -1)
                {
                    CARD_PACK_TYPE cardPackType = balance.CardPackType[cardPackIdx];
                    m_cardPacks[i].GO.SetActive(cardPackType > CARD_PACK_TYPE.NONE);
                    if (cardPackType == CARD_PACK_TYPE.BALL)
                        m_cardPacks[i].CardImage.sprite = AssetManager.Instance.LoadBallCardPackSprite();
                    else if (cardPackType == CARD_PACK_TYPE.SLOT)
                        m_cardPacks[i].CardImage.sprite = AssetManager.Instance.LoadSlotCardPackSprite();
                    else if (cardPackType == CARD_PACK_TYPE.CHIPS)
                        m_cardPacks[i].CardImage.sprite = AssetManager.Instance.LoadChipsCardPackSprite();

                    int localI = i;
                    m_cardPacks[i].CardPackGO.GetComponent<Button>().onClick.RemoveAllListeners();
                    m_cardPacks[i].CardPackGO.GetComponent<Button>().onClick.AddListener(() => Game.Instance.ShowCardBuyPopup(localI));

                    m_cardPacks[i].CostText.text = "$" + Logic.GetCardPackShopCost(runData, balance, cardPackIdx);
                }
            }

            // show voucher
            m_voucher.CardImage.sprite = AssetManager.Instance.LoadVoucherSprite(balance.VoucherBalance.SpriteName[Logic.GetVoucherForRound(runData)]);
            m_voucher.CardPackGO.GetComponent<Button>().onClick.RemoveAllListeners();
            m_voucher.CardPackGO.GetComponent<Button>().onClick.AddListener(Game.Instance.ShowVoucherBuyPopup);
            m_voucher.CostText.text = "$" + Logic.GetVoucherCost(runData, balance);
            m_voucher.GO.SetActive(!runData.VoucherPurchased);

            CommonVisual.ShowJokersBallsAndSpinWheel(runData, balance, m_cardsBallsSpinWheelGUI, runData.SlotType);

            HideBuyPopupCommon();

            Canvas.ForceUpdateCanvases();
            if (m_verticalLayoutGroup != null)
            {
                m_verticalLayoutGroup.enabled = false;
                m_verticalLayoutGroup.enabled = true;
            }

            Span<int> jokerIdxs = new int[balance.MaxJokersInHand];
            int jokerCount = 0;

            if (Logic.CheckForSortSlotsJoker(runData, balance, jokerIdxs, ref jokerCount))
            {
                SortSlots(runData, balance);
                for (int jIdx = 0; jIdx < jokerCount; jIdx++)
                    CommonVisual.JokerGUIs[jokerIdxs[jIdx]].Animation.Play("ScoreGrow");
            }

            UpdateRerollButton(runData, balance);
        }

        public void UpdateRerollButton(RunData runData, Balance balance)
        {
            int cost = Logic.GetShopRerollCost(runData, balance);
            m_rerollCostText.text = "$" + cost.ToString("N0");
            m_rerollButtonImage.color = Logic.CanBuy(runData, balance, cost) ? balance.RerollColorEnabled : balance.ButtonColorDisabled;

        }

        public void Hide()
        {
            m_UI.SetActive(false);
            CommonVisual.HideJokers();
        }

        public void ShowJokerBuyPopup(RunData runData, Balance balance, int shopJokerIdx)
        {
            int jokerType = runData.ShopJokerIdxs[shopJokerIdx];

            if (jokerType > -1)
            {
                Debug.Log("ShopVisual.ShowBuyPopup(shopJokerIdx " + shopJokerIdx + ")");
                m_jokerBuyPopupGUI.BuyButton.onClick.RemoveAllListeners();
                m_jokerBuyPopupGUI.BuyButton.onClick.AddListener(() => Game.Instance.BuyShopJoker(shopJokerIdx));
                m_jokerBuyPopupGUI.BuyButton.interactable = Logic.RoomForJokerInHand(runData) && Logic.CanBuy(runData, balance, Logic.GetJokerShopCost(runData, balance, jokerType));
                m_jokerBuyPopupGUI.BuyButtonImage.color = (Logic.RoomForJokerInHand(runData) && Logic.CanBuy(runData, balance, Logic.GetJokerShopCost(runData, balance, jokerType))) ? balance.ButtonColorEnabled : balance.ButtonColorDisabled;
                m_jokerBuyPopupGUI.Cost.text = "$" + Logic.GetJokerShopCost(runData, balance, jokerType).ToString();
                RARITY rarity = balance.JokerBalance.Rarity[jokerType];
                m_jokerBuyPopupGUI.RarityText.text = rarity.ToString();
                m_jokerBuyPopupGUI.Border.color = balance.RarityColors[(int)rarity];
                m_jokerBuyPopupGUI.BorderRarity.color = balance.RarityColors[(int)rarity];

                m_jokerBuyPopupGUI.ShopCard.sprite = AssetManager.Instance.LoadJokerSprite(balance.JokerBalance.JokerSpritesNames[jokerType]);

                GameObject descriptionGO = AssetManager.Instance.GetDescriptionGO(balance.JokerBalance.DescriptionName[jokerType], m_jokerBuyPopupGUI.DescriptionParent);
                descriptionGO.transform.localPosition = Vector3.zero;
                descriptionGO.transform.localScale = Vector3.one;
                m_descriptionGO = descriptionGO;

                CommonVisual.ShowJokerDescriptionShop(runData, balance, m_descriptionGO, jokerType);

                m_jokerBuyPopupGUI.GO.SetActive(true);
            }
        }

        public void ShowCardBuyPopup(RunData runData, Balance balance, int shopPackIdx)
        {
            int cardPackIdx = runData.ShopCardPackIdxs[shopPackIdx];

            m_cardPackBuyPopupGUI.BuyButton.onClick.RemoveAllListeners();
            m_cardPackBuyPopupGUI.BuyButton.onClick.AddListener(() => Game.Instance.BuyShopCardPack(shopPackIdx));
            m_cardPackBuyPopupGUI.BuyButton.interactable = Logic.CanBuy(runData, balance, Logic.GetCardPackShopCost(runData, balance, cardPackIdx));
            m_cardPackBuyPopupGUI.BuyButtonImage.color = Logic.CanBuy(runData, balance, Logic.GetCardPackShopCost(runData, balance, cardPackIdx)) ? balance.ButtonColorEnabled : balance.ButtonColorDisabled;
            m_cardPackBuyPopupGUI.Cost.text = "$" + Logic.GetCardPackShopCost(runData, balance, cardPackIdx);

            if (balance.CardPackType[cardPackIdx] == CARD_PACK_TYPE.BALL)
                m_cardPackBuyPopupGUI.ShopCard.sprite = AssetManager.Instance.LoadBallCardPackSprite();
            else if (balance.CardPackType[cardPackIdx] == CARD_PACK_TYPE.SLOT)
                m_cardPackBuyPopupGUI.ShopCard.sprite = AssetManager.Instance.LoadSlotCardPackSprite();
            else if (balance.CardPackType[cardPackIdx] == CARD_PACK_TYPE.CHIPS)
                m_cardPackBuyPopupGUI.ShopCard.sprite = AssetManager.Instance.LoadChipsCardPackSprite();

            string typeString = balance.CardPackType[cardPackIdx] == CARD_PACK_TYPE.BALL ? "Ball" : "Slot";
            m_cardPackBuyPopupGUI.Description.text = "Pick " + balance.CardPackPickCards[cardPackIdx] + " of " + balance.CardPackMaxCards[cardPackIdx] + " " + typeString + " Upgrades";

            m_cardPackBuyPopupGUI.GO.SetActive(true);
        }

        public void ShowVoucherBuyPopup(RunData runData, Balance balance)
        {
            m_voucherBuyPopupGUI.BuyButton.onClick.RemoveAllListeners();
            m_voucherBuyPopupGUI.BuyButton.onClick.AddListener(Game.Instance.BuyVoucher);

            m_voucherBuyPopupGUI.BuyButton.interactable = Logic.CanBuy(runData, balance, Logic.GetVoucherCost(runData, balance));
            m_voucherBuyPopupGUI.BuyButtonImage.color = Logic.CanBuy(runData, balance, Logic.GetVoucherCost(runData, balance)) ? balance.ButtonColorEnabled : balance.ButtonColorDisabled;
            m_voucherBuyPopupGUI.Cost.text = "$" + Logic.GetVoucherCost(runData, balance);

            m_voucherBuyPopupGUI.ShopCard.sprite = AssetManager.Instance.LoadVoucherSprite(balance.VoucherBalance.SpriteName[Logic.GetVoucherForRound(runData)]);

            m_voucherBuyPopupGUI.Description.text = balance.VoucherBalance.Description[Logic.GetVoucherForRound(runData)];

            m_voucherBuyPopupGUI.GO.SetActive(true);
        }

        public void HideJokerBuyPopup()
        {
            m_hidePopupTimer = m_hidePopupTime;
            m_jokerPopupAnimation.Play("Joker Buy Popup Close");
        }

        public void HideVoucherBuyPopup()
        {
            m_hidePopupTimer = m_hidePopupTime;
            m_voucherPopupAnimation.Play("Voucher Buy Popup Close");
        }

        public void HideCardpackBuyPopup()
        {
            m_hidePopupTimer = m_hidePopupTime;
            m_cardpackPopupAnimation.Play("Cardpack Buy Popup Close");
        }

        public void HideBuyPopupCommon()
        {
            m_jokerBuyPopupGUI.GO.SetActive(false);
            m_cardPackBuyPopupGUI.GO.SetActive(false);
            m_voucherBuyPopupGUI.GO.SetActive(false);

            if (m_descriptionGO != null)
                GameObject.Destroy(m_descriptionGO);
        }

        public void Tick(RunData runData, Balance balance, float dt)
        {
            CommonSlotsVisual.TickSpinWheelUI(runData, balance.UISpinWheelSpeed, dt, m_cardsBallsSpinWheelGUI);
            CommonSlotsVisual.TickSortingPopup(dt, m_cardsBallsSpinWheelGUI);

            if (m_hidePopupTimer > 0.0f)
            {
                m_hidePopupTimer -= dt;
                if (m_hidePopupTimer <= 0.0f)
                    HideBuyPopupCommon();
            }
        }

        public void BuyShopJoker(RunData runData, Balance balance, int shopJokerIdx)
        {
            Debug.Log("ShopVisual.BuyShopJoker(shopJokerIdx " + shopJokerIdx + ")");

            int jokerType = runData.ShopJokerIdxs[shopJokerIdx];

            Logic.BuyShopJoker(runData, balance, shopJokerIdx);
            m_jokers[shopJokerIdx].GO.SetActive(false);

            CommonVisual.ShowJokers(runData, balance, m_cardsBallsSpinWheelGUI.JokerParent);
            CommonVisual.UpdateTopBarMoney(runData, m_topBarGUI);

            UpdateRerollButton(runData, balance);

            if (balance.JokerBalance.SortSlots[jokerType])
                SortSlots(runData, balance);

            HideJokerBuyPopup();
        }

        public void BuyShopCardPack(RunData runData, Balance balance, int shopPackIdx)
        {
            Debug.Log("ShopVisual.BuyShopCardPack(shopPackIdx " + shopPackIdx + ")");

            Logic.BuyCardPack(runData, balance, shopPackIdx);
            m_cardPacks[shopPackIdx].GO.SetActive(false);

            CommonVisual.UpdateTopBarMoney(runData, m_topBarGUI);

            HideCardpackBuyPopup();
        }

        public void BuyVoucher(RunData runData, Balance balance)
        {
            Logic.BuyVoucher(runData, balance);

            HideVoucherBuyPopup();

            Show(runData, balance);
        }

        public void UpdateTopUI(RunData runData, Balance balance)
        {
            CommonVisual.UpdateTopBarMoney(runData, m_topBarGUI);
            CommonVisual.ShowJokers(runData, balance, m_cardsBallsSpinWheelGUI.JokerParent);
        }

        public void SortSlots(RunData runData, Balance balance)
        {
            CommonSlotsVisual.TrySortSlots(runData, balance, m_cardsBallsSpinWheelGUI);
        }

        public void RerollShop(RunData runData, Balance balance)
        {

            if (Logic.TryRerollShop(runData, balance))
                Show(runData, balance);
        }
    }

}