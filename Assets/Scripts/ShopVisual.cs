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
        public GUIButtonData BuyButtonData;
        public GUIButtonData CancelButtonData;
        public Image BuyButtonImage;
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
        public GUIButtonData BuyButtonData;
        public GUIButtonData CancelButtonData;
        public Image BuyButtonImage;
    }

    public struct VoucherBuyPopupGUI
    {
        public GameObject GO;
        public Image ShopCard;
        public TextMeshProUGUI Description;
        public TextMeshProUGUI Cost;
        public GUIButtonData BuyButtonData;
        public GUIButtonData CancelButtonData;
        public Image BuyButtonImage;
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
        public GUIButtonData JokerButtonData;
    }

    public struct CardPackGUI
    {
        public GameObject GO;
        public GameObject CardPackGO;
        public TextMeshProUGUI CostText;
        public Image CardImage;
        public GUIButtonData CardPackButtonData;
    }


    public struct VoucherGUI
    {
        public GameObject GO;
        public GameObject CardPackGO;
        public TextMeshProUGUI CostText;
        public Image CardImage;
        public GUIButtonData VoucherButtonData;
    }

    public class ShopVisual : MonoBehaviour
    {
        public enum MENU_BUTTONS
        {
            INFO,
            NEXT_ROUND,
            REROLL,
            SHOP_JOKER_1,
            SHOP_JOKER_2,
            SHOP_JOKER_3,
            SHOP_CARDPACK_1,
            SHOP_CARDPACK_2,
            SHOP_VOUCHER,
            SETTINGS = 10,
            WHEEL = 11,
            BALLS = 12,
            JOKER_1 = 20,
            JOKER_2 = 21,
            JOKER_3 = 22,
            JOKER_4 = 23,
            JOKER_5 = 24,
        };
        MENU_BUTTONS m_selectedButton;

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

        GUIButtonData m_infoButtonData;
        GUIButtonData m_nextRoundButtonData;
        GUIButtonData m_rerollButtonData;

        RunData runData;
        Balance balance;

        // Start is called before the first frame update
        public void Init(RunData runData, Balance balance, Camera camera)
        {
            this.runData = runData;
            this.balance = balance;

            m_UI = AssetManager.Instance.LoadShopUI();
            m_UI.GetComponent<Canvas>().worldCamera = camera;
            CommonVisual.ChangeCanvasScalerMatching(m_UI);

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
            m_rerollCostText = guiRef.GetTextGUI("RerollCost");

            //TODO chnage to guirefbutton
            GUIButtonRef guiButtonRef = m_UI.GetComponent<GUIButtonRef>();
            m_rerollButtonData = guiButtonRef.GetButtonData("Reroll");
            m_nextRoundButtonData = guiButtonRef.GetButtonData("NextRound");
            m_infoButtonData = guiButtonRef.GetButtonData("Info");
            m_rerollButtonData.Button.onClick.AddListener(Game.Instance.RerollShop);
            m_nextRoundButtonData.Button.onClick.AddListener(goToRoundSelection);
            m_infoButtonData.Button.onClick.AddListener(showShopInfo);
            CommonButtonVisual.AddSelectedBorder(m_rerollButtonData);
            CommonButtonVisual.AddSelectedBorder(m_nextRoundButtonData);
            CommonButtonVisual.AddSelectedBorder(m_infoButtonData);

            m_jokerPopupAnimation = guiRef.GetAnimation("JokerPopup");
            m_cardpackPopupAnimation = guiRef.GetAnimation("CardpackPopup");
            m_voucherPopupAnimation = guiRef.GetAnimation("VoucherPopup");

            CommonVisual.InitTopBarGUI(guiRef.GetGameObject("TopBar"), ref m_topBarGUI);
            CommonVisual.InitCardsBallsSpinWheelGUI(balance, guiRef.GetGameObject("CardsAndBalls"), ref m_cardsBallsSpinWheelGUI);

            m_jokerBuyPopupGUI.GO = guiRef.GetGameObject("JokerBuyPopup");
            GUIRef jokerBuyPopupGUIRef = m_jokerBuyPopupGUI.GO.GetComponent<GUIRef>();
            m_jokerBuyPopupGUI.ShopCard = jokerBuyPopupGUIRef.GetImage("Card");
            m_jokerBuyPopupGUI.DescriptionParent = jokerBuyPopupGUIRef.GetGameObject("Description").transform;
            m_jokerBuyPopupGUI.Cost = jokerBuyPopupGUIRef.GetTextGUI("Cost");
            m_jokerBuyPopupGUI.BuyButtonImage = jokerBuyPopupGUIRef.GetImage("Buy");
            m_jokerBuyPopupGUI.RarityText = jokerBuyPopupGUIRef.GetTextGUI("Rarity");
            m_jokerBuyPopupGUI.Border = jokerBuyPopupGUIRef.GetImage("Border");
            m_jokerBuyPopupGUI.BorderRarity = jokerBuyPopupGUIRef.GetImage("BorderRarity");
            GUIButtonRef jokerBuyPopupButtonRef = m_jokerBuyPopupGUI.GO.GetComponent<GUIButtonRef>();
            m_jokerBuyPopupGUI.BuyButtonData = jokerBuyPopupButtonRef.GetButtonData("Buy");
            m_jokerBuyPopupGUI.CancelButtonData = jokerBuyPopupButtonRef.GetButtonData("Cancel");
            CommonButtonVisual.AddSelectedBorder(m_jokerBuyPopupGUI.BuyButtonData);
            CommonButtonVisual.AddSelectedBorder(m_jokerBuyPopupGUI.CancelButtonData);
            m_jokerBuyPopupGUI.CancelButtonData.Button.onClick.AddListener(HideJokerBuyPopup);

            m_cardPackBuyPopupGUI.GO = guiRef.GetGameObject("CardPackBuyPopup");
            GUIRef cardPackBuyPopupGUIRef = m_cardPackBuyPopupGUI.GO.GetComponent<GUIRef>();
            m_cardPackBuyPopupGUI.ShopCard = cardPackBuyPopupGUIRef.GetImage("Card");
            m_cardPackBuyPopupGUI.Description = cardPackBuyPopupGUIRef.GetTextGUI("Description");
            m_cardPackBuyPopupGUI.Cost = cardPackBuyPopupGUIRef.GetTextGUI("Cost");
            m_cardPackBuyPopupGUI.BuyButtonImage = cardPackBuyPopupGUIRef.GetImage("Buy");
            GUIButtonRef cardPackBuyPopupButtonRef = m_cardPackBuyPopupGUI.GO.GetComponent<GUIButtonRef>();
            m_cardPackBuyPopupGUI.BuyButtonData = cardPackBuyPopupButtonRef.GetButtonData("Buy");
            m_cardPackBuyPopupGUI.CancelButtonData = cardPackBuyPopupButtonRef.GetButtonData("Cancel");
            CommonButtonVisual.AddSelectedBorder(m_cardPackBuyPopupGUI.BuyButtonData);
            CommonButtonVisual.AddSelectedBorder(m_cardPackBuyPopupGUI.CancelButtonData);
            m_cardPackBuyPopupGUI.CancelButtonData.Button.onClick.AddListener(HideCardpackBuyPopup);

            m_voucherBuyPopupGUI.GO = guiRef.GetGameObject("VoucherBuyPopup");
            GUIRef voucherBuyPopupGUIRef = m_voucherBuyPopupGUI.GO.GetComponent<GUIRef>();
            m_voucherBuyPopupGUI.ShopCard = voucherBuyPopupGUIRef.GetImage("Card");
            m_voucherBuyPopupGUI.Description = voucherBuyPopupGUIRef.GetTextGUI("Description");
            m_voucherBuyPopupGUI.Cost = voucherBuyPopupGUIRef.GetTextGUI("Cost");
            m_voucherBuyPopupGUI.BuyButtonImage = voucherBuyPopupGUIRef.GetImage("Buy");
            GUIButtonRef voucherBuyPopupButtonRef = m_voucherBuyPopupGUI.GO.GetComponent<GUIButtonRef>();
            m_voucherBuyPopupGUI.BuyButtonData = voucherBuyPopupButtonRef.GetButtonData("Buy");
            m_voucherBuyPopupGUI.CancelButtonData = voucherBuyPopupButtonRef.GetButtonData("Cancel");
            CommonButtonVisual.AddSelectedBorder(m_voucherBuyPopupGUI.BuyButtonData);
            CommonButtonVisual.AddSelectedBorder(m_voucherBuyPopupGUI.CancelButtonData);
            m_voucherBuyPopupGUI.CancelButtonData.Button.onClick.AddListener(HideVoucherBuyPopup);

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

            GUIButtonRef guiButtonRef = go.GetComponent<GUIButtonRef>();
            voucherGUI.VoucherButtonData = guiButtonRef.GetButtonData("Voucher");
        }

        void initCardPackGUI(GameObject go, ref CardPackGUI cardPackGUI)
        {
            GUIRef guiRef = go.GetComponent<GUIRef>();
            cardPackGUI.GO = go;
            cardPackGUI.CostText = guiRef.GetTextGUI("Cost");
            cardPackGUI.CardPackGO = guiRef.GetGameObject("Card");

            GUIRef jokerGUIRef = cardPackGUI.CardPackGO.GetComponent<GUIRef>();
            cardPackGUI.CardImage = jokerGUIRef.GetImage("Joker");

            GUIButtonRef guiButtonRef = cardPackGUI.CardPackGO.GetComponent<GUIButtonRef>();
            cardPackGUI.CardPackButtonData = guiButtonRef.GetButtonData("CardPack");
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

            GUIButtonRef guiButtonRef = shopCardGUI.JokerGO.GetComponent<GUIButtonRef>();
            shopCardGUI.JokerButtonData = guiButtonRef.GetButtonData("Joker");
        }

        public void Show(GAMEPAD_TYPE gamepadType, int availableInputs)
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
                    m_jokers[i].JokerButtonData.Button.onClick.RemoveAllListeners();
                    m_jokers[i].JokerButtonData.Button.onClick.AddListener(() => showJokerBuyPopup(localI, gamepadType));
                    m_jokers[i].JokerButtonData.SelectedGO.SetActive(false);

                    m_jokers[i].CostText.text = "◇" + Logic.GetJokerShopCost(runData, balance, jokerType).ToString();
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
                    m_cardPacks[i].CardPackButtonData.Button.onClick.RemoveAllListeners();
                    m_cardPacks[i].CardPackButtonData.Button.onClick.AddListener(() => showCardBuyPopup(localI, gamepadType));
                    m_cardPacks[i].CardPackButtonData.SelectedGO.SetActive(false);

                    m_cardPacks[i].CostText.text = "◇" + Logic.GetCardPackShopCost(runData, balance, cardPackIdx);
                }
            }

            // show voucher
            m_voucher.CardImage.sprite = AssetManager.Instance.LoadVoucherSprite(balance.VoucherBalance.SpriteName[Logic.GetVoucherForRound(runData)]);
            m_voucher.VoucherButtonData.Button.onClick.RemoveAllListeners();
            m_voucher.VoucherButtonData.Button.onClick.AddListener(() => showVoucherBuyPopup(gamepadType));
            m_voucher.VoucherButtonData.SelectedGO.SetActive(false);
            m_voucher.CostText.text = "◇" + Logic.GetVoucherCost(runData, balance);
            m_voucher.GO.SetActive(!runData.VoucherPurchased);

            CommonVisual.ShowJokersBallsAndSpinWheel(runData, balance, m_cardsBallsSpinWheelGUI, runData.SlotType);

            HideBuyPopupCommon();

            Canvas.ForceUpdateCanvases();
            if (m_verticalLayoutGroup != null)
            {
                m_verticalLayoutGroup.enabled = false;
                m_verticalLayoutGroup.enabled = true;
            }

            Span<int> jokerIdxs = stackalloc int[balance.MaxJokersInHand];
            int jokerCount = 0;

            if (Logic.CheckForSortSlotsJoker(runData, balance, jokerIdxs, ref jokerCount))
            {
                SortSlots();
                for (int jIdx = 0; jIdx < jokerCount; jIdx++)
                    CommonVisual.JokerGUIs[jokerIdxs[jIdx]].Animation.Play("ScoreGrow");
            }

            UpdateRerollButton();

            CommonButtonVisual.UpdateButtonIcons(m_topBarGUI.SettingsButtonData, gamepadType);

            selectButton(MENU_BUTTONS.NEXT_ROUND, availableInputs);
        }

        void selectButton(MENU_BUTTONS selectedButton, int availableInputs)
        {
            hideAllButtonSelections();

            m_selectedButton = selectedButton;
            if (Logic.IsBitSet(availableInputs, (byte)INPUT_TYPES.GAMEPAD) || Logic.IsBitSet(availableInputs, (byte)INPUT_TYPES.KEYBOARD))
            {
                m_nextRoundButtonData.SelectedGO.SetActive(m_selectedButton == MENU_BUTTONS.NEXT_ROUND);
                m_rerollButtonData.SelectedGO.SetActive(m_selectedButton == MENU_BUTTONS.REROLL);
                m_infoButtonData.SelectedGO.SetActive(m_selectedButton == MENU_BUTTONS.INFO);

                m_jokers[0].JokerButtonData.SelectedGO.SetActive(m_selectedButton == MENU_BUTTONS.SHOP_JOKER_1);
                m_jokers[1].JokerButtonData.SelectedGO.SetActive(m_selectedButton == MENU_BUTTONS.SHOP_JOKER_2);
                m_jokers[2].JokerButtonData.SelectedGO.SetActive(m_selectedButton == MENU_BUTTONS.SHOP_JOKER_3);

                m_cardPacks[0].CardPackButtonData.SelectedGO.SetActive(m_selectedButton == MENU_BUTTONS.SHOP_CARDPACK_1);
                m_cardPacks[1].CardPackButtonData.SelectedGO.SetActive(m_selectedButton == MENU_BUTTONS.SHOP_CARDPACK_2);

                m_voucher.VoucherButtonData.SelectedGO.SetActive(m_selectedButton == MENU_BUTTONS.SHOP_VOUCHER);

                CommonButtonVisual.CommonSelectButton(m_topBarGUI, m_cardsBallsSpinWheelGUI, (COMMON_BUTTONS)m_selectedButton);
            }
        }

        void hideAllButtonSelections()
        {
            CommonButtonVisual.HideAllButtonSelections(m_topBarGUI, m_cardsBallsSpinWheelGUI);
        }

        public void UpdateRerollButton()
        {
            int cost = Logic.GetShopRerollCost(runData, balance);
            m_rerollCostText.text = "◇" + cost.ToString("N0");
            m_rerollButtonImage.color = Logic.CanBuy(runData, balance, cost) ? balance.RerollColorEnabled : balance.ButtonColorDisabled;

        }

        public void Hide()
        {
            m_UI.SetActive(false);
            CommonVisual.HideJokers();
        }

        void showJokerBuyPopup(int shopJokerIdx, GAMEPAD_TYPE gamepadType)
        {
            SoundManager.Instance.PlaySFXButtonOK();

            int jokerType = runData.ShopJokerIdxs[shopJokerIdx];

            if (jokerType > -1)
            {
                Debug.Log("ShopVisual.ShowBuyPopup(shopJokerIdx " + shopJokerIdx + ")");
                m_jokerBuyPopupGUI.BuyButtonData.Button.onClick.RemoveAllListeners();
                m_jokerBuyPopupGUI.BuyButtonData.Button.onClick.AddListener(() => Game.Instance.BuyShopJoker(shopJokerIdx));
                m_jokerBuyPopupGUI.BuyButtonData.Button.interactable = Logic.RoomForJokerInHand(runData) && Logic.CanBuy(runData, balance, Logic.GetJokerShopCost(runData, balance, jokerType));
                m_jokerBuyPopupGUI.BuyButtonImage.color = (Logic.RoomForJokerInHand(runData) && Logic.CanBuy(runData, balance, Logic.GetJokerShopCost(runData, balance, jokerType))) ? balance.ButtonColorEnabled : balance.ButtonColorDisabled;
                m_jokerBuyPopupGUI.Cost.text = "◇" + Logic.GetJokerShopCost(runData, balance, jokerType).ToString();
                RARITY rarity = balance.JokerBalance.Rarity[jokerType];
                m_jokerBuyPopupGUI.RarityText.text = rarity.ToString();
                m_jokerBuyPopupGUI.Border.color = balance.RarityColors[(int)rarity];
                m_jokerBuyPopupGUI.BorderRarity.color = balance.RarityColors[(int)rarity];

                m_jokerBuyPopupGUI.ShopCard.sprite = AssetManager.Instance.LoadJokerSprite(balance.JokerBalance.JokerSpritesNames[jokerType]);

                GameObject descriptionGO = AssetManager.Instance.GetDescriptionGO(balance.JokerBalance.DescriptionName[jokerType], m_jokerBuyPopupGUI.DescriptionParent);
                descriptionGO.transform.localPosition = Vector3.zero;
                descriptionGO.transform.localScale = Vector3.one;
                m_descriptionGO = descriptionGO;

                CommonVisual.ShowJokerDescriptionCommon(runData, balance, m_descriptionGO, jokerType, -1);

                m_jokerBuyPopupGUI.GO.SetActive(true);

                CommonButtonVisual.UpdateButtonIcons(m_jokerBuyPopupGUI.BuyButtonData, gamepadType);
                CommonButtonVisual.UpdateButtonIcons(m_jokerBuyPopupGUI.CancelButtonData, gamepadType);

                m_jokerBuyPopupGUI.BuyButtonData.SelectedGO.SetActive(gamepadType != GAMEPAD_TYPE.NONE);
                m_jokerBuyPopupGUI.CancelButtonData.SelectedGO.SetActive(false);
            }
        }

        void showCardBuyPopup(int shopPackIdx, GAMEPAD_TYPE gamepadType)
        {
            SoundManager.Instance.PlaySFXButtonOK();

            int cardPackIdx = runData.ShopCardPackIdxs[shopPackIdx];

            m_cardPackBuyPopupGUI.BuyButtonData.Button.onClick.RemoveAllListeners();
            m_cardPackBuyPopupGUI.BuyButtonData.Button.onClick.AddListener(() => Game.Instance.BuyShopCardPack(shopPackIdx));
            m_cardPackBuyPopupGUI.BuyButtonData.Button.interactable = Logic.CanBuy(runData, balance, Logic.GetCardPackShopCost(runData, balance, cardPackIdx));
            m_cardPackBuyPopupGUI.BuyButtonImage.color = Logic.CanBuy(runData, balance, Logic.GetCardPackShopCost(runData, balance, cardPackIdx)) ? balance.ButtonColorEnabled : balance.ButtonColorDisabled;
            m_cardPackBuyPopupGUI.Cost.text = "◇" + Logic.GetCardPackShopCost(runData, balance, cardPackIdx);

            if (balance.CardPackType[cardPackIdx] == CARD_PACK_TYPE.BALL)
                m_cardPackBuyPopupGUI.ShopCard.sprite = AssetManager.Instance.LoadBallCardPackSprite();
            else if (balance.CardPackType[cardPackIdx] == CARD_PACK_TYPE.SLOT)
                m_cardPackBuyPopupGUI.ShopCard.sprite = AssetManager.Instance.LoadSlotCardPackSprite();
            else if (balance.CardPackType[cardPackIdx] == CARD_PACK_TYPE.CHIPS)
                m_cardPackBuyPopupGUI.ShopCard.sprite = AssetManager.Instance.LoadChipsCardPackSprite();

            string typeString = balance.CardPackType[cardPackIdx] == CARD_PACK_TYPE.BALL ? "Ball" : "Slot";
            m_cardPackBuyPopupGUI.Description.text = "Pick " + balance.CardPackPickCards[cardPackIdx] + " of " + balance.CardPackMaxCards[cardPackIdx] + " " + typeString + " Upgrades";

            m_cardPackBuyPopupGUI.GO.SetActive(true);

            CommonButtonVisual.UpdateButtonIcons(m_jokerBuyPopupGUI.BuyButtonData, gamepadType);
            CommonButtonVisual.UpdateButtonIcons(m_jokerBuyPopupGUI.CancelButtonData, gamepadType);

            m_cardPackBuyPopupGUI.BuyButtonData.SelectedGO.SetActive(gamepadType != GAMEPAD_TYPE.NONE);
            m_cardPackBuyPopupGUI.CancelButtonData.SelectedGO.SetActive(false);
        }

        void showVoucherBuyPopup(GAMEPAD_TYPE gamepadType)
        {
            SoundManager.Instance.PlaySFXButtonOK();

            m_voucherBuyPopupGUI.BuyButtonData.Button.onClick.RemoveAllListeners();
            m_voucherBuyPopupGUI.BuyButtonData.Button.onClick.AddListener(Game.Instance.BuyVoucher);

            m_voucherBuyPopupGUI.BuyButtonData.Button.interactable = Logic.CanBuy(runData, balance, Logic.GetVoucherCost(runData, balance));
            m_voucherBuyPopupGUI.BuyButtonImage.color = Logic.CanBuy(runData, balance, Logic.GetVoucherCost(runData, balance)) ? balance.ButtonColorEnabled : balance.ButtonColorDisabled;
            m_voucherBuyPopupGUI.Cost.text = "◇" + Logic.GetVoucherCost(runData, balance);

            m_voucherBuyPopupGUI.ShopCard.sprite = AssetManager.Instance.LoadVoucherSprite(balance.VoucherBalance.SpriteName[Logic.GetVoucherForRound(runData)]);

            m_voucherBuyPopupGUI.Description.text = balance.VoucherBalance.Description[Logic.GetVoucherForRound(runData)];

            m_voucherBuyPopupGUI.GO.SetActive(true);

            CommonButtonVisual.UpdateButtonIcons(m_jokerBuyPopupGUI.BuyButtonData, gamepadType);
            CommonButtonVisual.UpdateButtonIcons(m_jokerBuyPopupGUI.CancelButtonData, gamepadType);

            m_voucherBuyPopupGUI.BuyButtonData.SelectedGO.SetActive(gamepadType != GAMEPAD_TYPE.NONE);
            m_voucherBuyPopupGUI.CancelButtonData.SelectedGO.SetActive(false);
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

        public void Tick(float dt, int availableInputs, GAMEPAD_TYPE gamepadType)
        {
            CommonSlotsVisual.TickSpinWheelUI(runData, balance.UISpinWheelSpeed, dt, m_cardsBallsSpinWheelGUI);
            CommonSlotsVisual.TickSortingPopup(dt, m_cardsBallsSpinWheelGUI);

            if (m_hidePopupTimer > 0.0f)
            {
                m_hidePopupTimer -= dt;
                if (m_hidePopupTimer <= 0.0f)
                    HideBuyPopupCommon();
            }

            handleInput(availableInputs, gamepadType);
        }

        void handleInput(int availableInputs, GAMEPAD_TYPE gamepadType)
        {
            if (CommonButtonVisual.CommonHandleInput(m_topBarGUI, m_cardsBallsSpinWheelGUI, availableInputs, (COMMON_BUTTONS)m_selectedButton))
                return;

            int newSelectedButton = CommonButtonVisual.CommonNavigation(runData, availableInputs, (COMMON_BUTTONS)m_selectedButton);
            if (newSelectedButton > -1)
            {
                selectButton((MENU_BUTTONS)newSelectedButton, availableInputs);
                return;
            }

            if (CommonButtonVisual.NavigateGamepadButton(m_infoButtonData, availableInputs) ||
            m_selectedButton == MENU_BUTTONS.INFO && CommonButtonVisual.NavigateEnter(availableInputs))
            {
                showShopInfo();
                return;
            }
            if (CommonButtonVisual.NavigateGamepadButton(m_rerollButtonData, availableInputs) ||
            m_selectedButton == MENU_BUTTONS.REROLL && CommonButtonVisual.NavigateEnter(availableInputs))
            {
                Game.Instance.RerollShop();
                return;
            }
            if (CommonButtonVisual.NavigateGamepadButton(m_nextRoundButtonData, availableInputs) ||
            m_selectedButton == MENU_BUTTONS.NEXT_ROUND && CommonButtonVisual.NavigateEnter(availableInputs))
            {
                goToRoundSelection();
                return;
            }

            if (m_selectedButton >= MENU_BUTTONS.SHOP_JOKER_1 && m_selectedButton <= MENU_BUTTONS.SHOP_JOKER_3 && CommonButtonVisual.NavigateEnter(availableInputs))
            {
                showJokerBuyPopup(m_selectedButton - MENU_BUTTONS.SHOP_JOKER_1, gamepadType);
            }

            // navigate gamepad / enter jokers/cardpacks/voucher

            // navigate up left column
            if (m_selectedButton == MENU_BUTTONS.INFO && CommonButtonVisual.NavigateUp(availableInputs))
            {
                selectButton(MENU_BUTTONS.REROLL, availableInputs);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.REROLL && CommonButtonVisual.NavigateUp(availableInputs))
            {
                selectButton(MENU_BUTTONS.SETTINGS, availableInputs);
                return;
            }

            // navigate down left column
            if (m_selectedButton == MENU_BUTTONS.SETTINGS && CommonButtonVisual.NavigateDown(availableInputs))
            {
                selectButton(MENU_BUTTONS.REROLL, availableInputs);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.REROLL && CommonButtonVisual.NavigateDown(availableInputs))
            {
                selectButton(MENU_BUTTONS.INFO, availableInputs);
                return;
            }

            // navigate up right column
            if (m_selectedButton == MENU_BUTTONS.NEXT_ROUND && CommonButtonVisual.NavigateUp(availableInputs))
            {
                selectButton(MENU_BUTTONS.WHEEL, availableInputs);
                return;
            }

            // navigate up right column
            if (m_selectedButton == MENU_BUTTONS.WHEEL && CommonButtonVisual.NavigateDown(availableInputs))
            {
                selectButton(MENU_BUTTONS.NEXT_ROUND, availableInputs);
                return;
            }

            // navigate left bottom row
            if (m_selectedButton == MENU_BUTTONS.NEXT_ROUND && CommonButtonVisual.NavigateLeft(availableInputs))
            {
                if (runData.ShopCardPackIdxs[1] > -1)
                    selectButton(MENU_BUTTONS.SHOP_CARDPACK_2, availableInputs);
                else if (runData.ShopCardPackIdxs[0] > -1)
                    selectButton(MENU_BUTTONS.SHOP_CARDPACK_1, availableInputs);
                else if (!runData.VoucherPurchased)
                    selectButton(MENU_BUTTONS.SHOP_VOUCHER, availableInputs);
                else
                    selectButton(MENU_BUTTONS.INFO, availableInputs);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.SHOP_CARDPACK_2 && CommonButtonVisual.NavigateLeft(availableInputs))
            {
                if (runData.ShopCardPackIdxs[0] > -1)
                    selectButton(MENU_BUTTONS.SHOP_CARDPACK_1, availableInputs);
                else if (!runData.VoucherPurchased)
                    selectButton(MENU_BUTTONS.SHOP_VOUCHER, availableInputs);
                else
                    selectButton(MENU_BUTTONS.INFO, availableInputs);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.SHOP_CARDPACK_1 && CommonButtonVisual.NavigateLeft(availableInputs))
            {
                if (!runData.VoucherPurchased)
                    selectButton(MENU_BUTTONS.SHOP_VOUCHER, availableInputs);
                else
                    selectButton(MENU_BUTTONS.INFO, availableInputs);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.SHOP_VOUCHER && CommonButtonVisual.NavigateLeft(availableInputs))
            {
                selectButton(MENU_BUTTONS.INFO, availableInputs);
                return;
            }

            // navigate right bottom row
            if ((m_selectedButton == MENU_BUTTONS.INFO || m_selectedButton == MENU_BUTTONS.REROLL) && CommonButtonVisual.NavigateRight(availableInputs))
            {
                if (!runData.VoucherPurchased)
                    selectButton(MENU_BUTTONS.SHOP_VOUCHER, availableInputs);
                else if (runData.ShopCardPackIdxs[0] > -1)
                    selectButton(MENU_BUTTONS.SHOP_CARDPACK_1, availableInputs);
                else if (runData.ShopCardPackIdxs[1] > -1)
                    selectButton(MENU_BUTTONS.SHOP_CARDPACK_2, availableInputs);
                else
                    selectButton(MENU_BUTTONS.NEXT_ROUND, availableInputs);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.SHOP_VOUCHER && CommonButtonVisual.NavigateRight(availableInputs))
            {
                if (runData.ShopCardPackIdxs[0] > -1)
                    selectButton(MENU_BUTTONS.SHOP_CARDPACK_1, availableInputs);
                else if (runData.ShopCardPackIdxs[1] > -1)
                    selectButton(MENU_BUTTONS.SHOP_CARDPACK_2, availableInputs);
                else
                    selectButton(MENU_BUTTONS.NEXT_ROUND, availableInputs);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.SHOP_CARDPACK_1 && CommonButtonVisual.NavigateRight(availableInputs))
            {
                if (runData.ShopCardPackIdxs[1] > -1)
                    selectButton(MENU_BUTTONS.SHOP_CARDPACK_2, availableInputs);
                else
                    selectButton(MENU_BUTTONS.NEXT_ROUND, availableInputs);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.SHOP_CARDPACK_2 && CommonButtonVisual.NavigateRight(availableInputs))
            {
                selectButton(MENU_BUTTONS.NEXT_ROUND, availableInputs);
                return;
            }

            // navigate left top row
            if (((m_selectedButton >= MENU_BUTTONS.JOKER_1 && m_selectedButton <= MENU_BUTTONS.JOKER_5) || m_selectedButton == MENU_BUTTONS.BALLS || m_selectedButton == MENU_BUTTONS.WHEEL) &&
            CommonButtonVisual.NavigateLeft(availableInputs))
            {
                if (runData.ShopJokerCount > 2 && runData.ShopJokerIdxs[2] > -1)
                    selectButton(MENU_BUTTONS.SHOP_JOKER_3, availableInputs);
                else if (runData.ShopJokerIdxs[1] > -1)
                    selectButton(MENU_BUTTONS.SHOP_JOKER_2, availableInputs);
                else if (runData.ShopJokerIdxs[0] > -1)
                    selectButton(MENU_BUTTONS.SHOP_JOKER_1, availableInputs);
                else
                    selectButton(MENU_BUTTONS.SETTINGS, availableInputs);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.SHOP_JOKER_3 && CommonButtonVisual.NavigateLeft(availableInputs))
            {
                if (runData.ShopJokerIdxs[1] > -1)
                    selectButton(MENU_BUTTONS.SHOP_JOKER_2, availableInputs);
                else if (runData.ShopJokerIdxs[0] > -1)
                    selectButton(MENU_BUTTONS.SHOP_JOKER_1, availableInputs);
                else
                    selectButton(MENU_BUTTONS.SETTINGS, availableInputs);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.SHOP_JOKER_2 && CommonButtonVisual.NavigateLeft(availableInputs))
            {
                if (runData.ShopJokerIdxs[0] > -1)
                    selectButton(MENU_BUTTONS.SHOP_JOKER_1, availableInputs);
                else
                    selectButton(MENU_BUTTONS.SETTINGS, availableInputs);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.SHOP_JOKER_1 && CommonButtonVisual.NavigateLeft(availableInputs))
            {
                selectButton(MENU_BUTTONS.SETTINGS, availableInputs);
                return;
            }

            // navigate right top row
            if (m_selectedButton == MENU_BUTTONS.SETTINGS && CommonButtonVisual.NavigateRight(availableInputs))
            {
                if (runData.ShopJokerIdxs[0] > -1)
                    selectButton(MENU_BUTTONS.SHOP_JOKER_1, availableInputs);
                else if (runData.ShopJokerIdxs[1] > -1)
                    selectButton(MENU_BUTTONS.SHOP_JOKER_2, availableInputs);
                else if (runData.ShopJokerCount > 2 && runData.ShopJokerIdxs[2] > -1)
                    selectButton(MENU_BUTTONS.SHOP_JOKER_3, availableInputs);
                else if (runData.JokerCount > 0)
                    selectButton(MENU_BUTTONS.JOKER_1, availableInputs);
                else
                    selectButton(MENU_BUTTONS.BALLS, availableInputs);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.SHOP_JOKER_1 && CommonButtonVisual.NavigateRight(availableInputs))
            {
                if (runData.ShopJokerIdxs[1] > -1)
                    selectButton(MENU_BUTTONS.SHOP_JOKER_2, availableInputs);
                else if (runData.ShopJokerCount > 2 && runData.ShopJokerIdxs[2] > -1)
                    selectButton(MENU_BUTTONS.SHOP_JOKER_3, availableInputs);
                else if (runData.JokerCount > 0)
                    selectButton(MENU_BUTTONS.JOKER_1, availableInputs);
                else
                    selectButton(MENU_BUTTONS.BALLS, availableInputs);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.SHOP_JOKER_2 && CommonButtonVisual.NavigateRight(availableInputs))
            {
                if (runData.ShopJokerCount > 2 && runData.ShopJokerIdxs[2] > -1)
                    selectButton(MENU_BUTTONS.SHOP_JOKER_3, availableInputs);
                else if (runData.JokerCount > 0)
                    selectButton(MENU_BUTTONS.JOKER_1, availableInputs);
                else
                    selectButton(MENU_BUTTONS.BALLS, availableInputs);
                return;
            }

            if (m_selectedButton == MENU_BUTTONS.SHOP_JOKER_3 && CommonButtonVisual.NavigateRight(availableInputs))
            {
                if (runData.JokerCount > 0)
                    selectButton(MENU_BUTTONS.JOKER_1, availableInputs);
                else
                    selectButton(MENU_BUTTONS.BALLS, availableInputs);
                return;
            }

            // navigate down from shop jokers
            int availableJokerCount = 0;
            for (int i = 0; i < runData.ShopJokerCount; i++)
                if (runData.ShopJokerIdxs[i] > -1)
                    availableJokerCount++;

            // MENU_BUTTONS leftJoker = MENU_BUTTONS.SHOP_JOKER_1;
            // if (runData.ShopJokerIdxs[0] == -1 && runData.ShopJokerIdxs[1] > -1)
            //     leftJoker = MENU_BUTTONS.SHOP_JOKER_2;
            // else if (runData.ShopJokerIdxs[0] == -1 && runData.ShopJokerIdxs[2] > -1)
            //     leftJoker = MENU_BUTTONS.SHOP_JOKER_3;

            // if 3 jokers
            if (availableJokerCount > 2)
            {
                // if only 2 jokers, left joker should go to voucher first, then cardpack 1, then card pack 2
                if (m_selectedButton == MENU_BUTTONS.SHOP_JOKER_1 && CommonButtonVisual.NavigateDown(availableInputs))
                {
                    if (!runData.VoucherPurchased)
                        selectButton(MENU_BUTTONS.SHOP_VOUCHER, availableInputs);
                    else if (runData.ShopCardPackIdxs[0] > -1)
                        selectButton(MENU_BUTTONS.SHOP_CARDPACK_1, availableInputs);
                    else if (runData.ShopCardPackIdxs[1] > -1)
                        selectButton(MENU_BUTTONS.SHOP_CARDPACK_2, availableInputs);
                    return;
                }

                // if more than 2 jokers, center joker should go to cardpack 1, then cardpack 2, then voucher
                if (m_selectedButton == MENU_BUTTONS.SHOP_JOKER_2 && CommonButtonVisual.NavigateDown(availableInputs))
                {
                    if (runData.ShopCardPackIdxs[0] > -1)
                        selectButton(MENU_BUTTONS.SHOP_CARDPACK_1, availableInputs);
                    else if (runData.ShopCardPackIdxs[1] > -1)
                        selectButton(MENU_BUTTONS.SHOP_CARDPACK_2, availableInputs);
                    else if (!runData.VoucherPurchased)
                        selectButton(MENU_BUTTONS.SHOP_VOUCHER, availableInputs);
                    return;
                }

                // if more than 2 jokers, right joker should go to cardpack 2, then cardpack 1, then voucher
                if (m_selectedButton == MENU_BUTTONS.SHOP_JOKER_3 && CommonButtonVisual.NavigateDown(availableInputs))
                {
                    if (runData.ShopCardPackIdxs[1] > -1)
                        selectButton(MENU_BUTTONS.SHOP_CARDPACK_2, availableInputs);
                    else if (runData.ShopCardPackIdxs[0] > -1)
                        selectButton(MENU_BUTTONS.SHOP_CARDPACK_1, availableInputs);
                    else if (!runData.VoucherPurchased)
                        selectButton(MENU_BUTTONS.SHOP_VOUCHER, availableInputs);
                    return;
                }
            }

            if (availableJokerCount == 2)
            {
                MENU_BUTTONS leftJoker = runData.ShopJokerIdxs[0] > -1 ? MENU_BUTTONS.SHOP_JOKER_1 : MENU_BUTTONS.SHOP_JOKER_2;
                MENU_BUTTONS rightJoker = runData.ShopJokerIdxs[1] > -1 ? MENU_BUTTONS.SHOP_JOKER_2 : MENU_BUTTONS.SHOP_JOKER_3;

                // if more than 2 jokers, left most joker should go to voucher, then cardpack 1, then card pack 2
                if (m_selectedButton == leftJoker && CommonButtonVisual.NavigateDown(availableInputs))
                {
                    if (!runData.VoucherPurchased)
                        selectButton(MENU_BUTTONS.SHOP_VOUCHER, availableInputs);
                    else if (runData.ShopCardPackIdxs[0] > -1)
                        selectButton(MENU_BUTTONS.SHOP_CARDPACK_1, availableInputs);
                    else if (runData.ShopCardPackIdxs[1] > -1)
                        selectButton(MENU_BUTTONS.SHOP_CARDPACK_2, availableInputs);
                    return;
                }

                if (m_selectedButton == rightJoker && CommonButtonVisual.NavigateDown(availableInputs))
                {
                    if (runData.ShopCardPackIdxs[1] > -1)
                        selectButton(MENU_BUTTONS.SHOP_CARDPACK_2, availableInputs);
                    else if (runData.ShopCardPackIdxs[0] > -1)
                        selectButton(MENU_BUTTONS.SHOP_CARDPACK_1, availableInputs);
                    else if (!runData.VoucherPurchased)
                        selectButton(MENU_BUTTONS.SHOP_VOUCHER, availableInputs);
                    return;
                }
            }

            if (availableJokerCount == 1 && m_selectedButton >= MENU_BUTTONS.SHOP_JOKER_1 && m_selectedButton <= MENU_BUTTONS.SHOP_JOKER_3 && CommonButtonVisual.NavigateDown(availableInputs))
            {
                if (runData.ShopCardPackIdxs[0] > -1)
                    selectButton(MENU_BUTTONS.SHOP_CARDPACK_1, availableInputs);
                else if (runData.ShopCardPackIdxs[1] > -1)
                    selectButton(MENU_BUTTONS.SHOP_CARDPACK_2, availableInputs);
                else if (!runData.VoucherPurchased)
                    selectButton(MENU_BUTTONS.SHOP_VOUCHER, availableInputs);
                return;

            }

            // navigate up from voucher/cardpacks
            if (m_selectedButton == MENU_BUTTONS.SHOP_VOUCHER && CommonButtonVisual.NavigateUp(availableInputs))
            {
                if (runData.ShopJokerIdxs[0] > -1)
                    selectButton(MENU_BUTTONS.SHOP_JOKER_1, availableInputs);
                else if (runData.ShopJokerIdxs[1] > -1)
                    selectButton(MENU_BUTTONS.SHOP_JOKER_2, availableInputs);
                else if (runData.ShopJokerCount == 3 && runData.ShopJokerIdxs[2] > -1)
                    selectButton(MENU_BUTTONS.SHOP_JOKER_3, availableInputs);
            }

            if (m_selectedButton == MENU_BUTTONS.SHOP_CARDPACK_1 && CommonButtonVisual.NavigateUp(availableInputs))
            {
                if (runData.ShopJokerIdxs[1] > -1)
                    selectButton(MENU_BUTTONS.SHOP_JOKER_2, availableInputs);
                else if (runData.ShopJokerIdxs[0] > -1)
                    selectButton(MENU_BUTTONS.SHOP_JOKER_1, availableInputs);
                else if (runData.ShopJokerCount == 3 && runData.ShopJokerIdxs[2] > -1)
                    selectButton(MENU_BUTTONS.SHOP_JOKER_3, availableInputs);
            }

            if (m_selectedButton == MENU_BUTTONS.SHOP_CARDPACK_2 && CommonButtonVisual.NavigateUp(availableInputs))
            {
                if (runData.ShopJokerCount == 3 && runData.ShopJokerIdxs[2] > -1)
                    selectButton(MENU_BUTTONS.SHOP_JOKER_3, availableInputs);
                else if (runData.ShopJokerIdxs[1] > -1)
                    selectButton(MENU_BUTTONS.SHOP_JOKER_2, availableInputs);
                else if (runData.ShopJokerIdxs[0] > -1)
                    selectButton(MENU_BUTTONS.SHOP_JOKER_1, availableInputs);
            }

        }

        public void BuyShopJoker(int shopJokerIdx)
        {
            Debug.Log("ShopVisual.BuyShopJoker(shopJokerIdx " + shopJokerIdx + ")");

            int jokerType = runData.ShopJokerIdxs[shopJokerIdx];

            Logic.BuyShopJoker(runData, balance, shopJokerIdx);
            m_jokers[shopJokerIdx].GO.SetActive(false);

            CommonVisual.ShowJokers(runData, balance, m_cardsBallsSpinWheelGUI.JokerParent);
            CommonVisual.UpdateTopBarMoney(runData, m_topBarGUI);

            UpdateRerollButton();

            if (balance.JokerBalance.SortSlots[jokerType])
                SortSlots();

            HideJokerBuyPopup();
        }

        public void BuyShopCardPack(int shopPackIdx)
        {
            Debug.Log("ShopVisual.BuyShopCardPack(shopPackIdx " + shopPackIdx + ")");

            Logic.BuyCardPack(runData, balance, shopPackIdx);
            m_cardPacks[shopPackIdx].GO.SetActive(false);

            CommonVisual.UpdateTopBarMoney(runData, m_topBarGUI);

            HideCardpackBuyPopup();
        }

        public void BuyVoucher(GAMEPAD_TYPE gamepadType, int availableInputs)
        {
            Logic.BuyVoucher(runData, balance);

            HideVoucherBuyPopup();

            Show(gamepadType, availableInputs);
        }

        public void SortSlots()
        {
            CommonSlotsVisual.TrySortSlots(runData, balance, m_cardsBallsSpinWheelGUI);
        }

        public void RerollShop(GAMEPAD_TYPE gamepadType, int avaiableInputs)
        {
            SoundManager.Instance.PlaySFXMoney();

            if (Logic.TryRerollShop(runData, balance))
                Show(gamepadType, avaiableInputs);

            RunDataIO.SaveRun(runData, balance);
        }

        void showShopInfo()
        {
            SoundManager.Instance.PlaySFXButtonOK();

            Game.Instance.SetMenuState(MENU_STATE.SHOP_INFO);
        }

        void goToRoundSelection()
        {
            SoundManager.Instance.PlaySFXButtonOK();

            Game.Instance.SetMenuState(MENU_STATE.ROUND_SELECTION);

        }
    }

}