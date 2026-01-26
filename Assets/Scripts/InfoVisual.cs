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

namespace Cardwheel
{
    public class InfoVisual : MonoBehaviour
    {
        public enum INFO_MENU_BUTTONS
        {
            CLOSE,
            WHEEL = 11,
            BALLS = 12,
            JOKER_1 = 20,
            JOKER_2 = 21,
            JOKER_3 = 22,
            JOKER_4 = 23,
            JOKER_5 = 24,
        };

        public struct JokerPopupGUI
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

        public struct RoundInfoGUI
        {
            public TextMeshProUGUI Title;
            public TextMeshProUGUI Description;
            public GameObject Cover;
        }


        GameObject m_UI;

        TextMeshProUGUI[] m_baseChipsText;
        TextMeshProUGUI m_mostFrequent;
        TextMeshProUGUI m_leastFrequest;

        ScoringSlot[] m_scoringSlots;
        SpinCircle m_spinCircle;

        CardsBallsSpinWheelGUI m_cardsBallsSpinWheelGUI;

        UIBallVisualData m_uiBallVisualData = new UIBallVisualData();

        float m_closeTimer = 0.0f;
        float m_closeTime = 0.1f;

        Animation m_animation;

        RoundInfoGUI[] m_shopRoundInfoGUI;

        GUIButtonData m_closeButtonData;

        RunData runData;
        Balance balance;

        public void Init(RunData runData, Balance balance, Camera camera)
        {
            this.runData = runData;
            this.balance = balance;

            m_UI = AssetManager.Instance.LoadInfoUI();
            m_UI.GetComponent<Canvas>().worldCamera = camera;
            CommonVisual.ChangeCanvasScalerMatching(m_UI);

            GUIRef guiRef = m_UI.GetComponent<GUIRef>();

            m_shopRoundInfoGUI = new RoundInfoGUI[3];
            for (int i = 0; i < 3; i++)
            {
                initRoundInfo(guiRef.GetGameObject("Round" + (i + 1)).GetComponent<GUIRef>(), ref m_shopRoundInfoGUI[i]);
                m_shopRoundInfoGUI[i].Cover.SetActive(false);
            }

            m_animation = guiRef.GetAnimation("Animation");

            m_baseChipsText = new TextMeshProUGUI[(int)SLOT_TYPE.LAST];
            CommonChipsVisual.InitChipsInfo(guiRef, m_baseChipsText, ref m_mostFrequent, ref m_leastFrequest);

            GUIButtonRef guiButtonRef = m_UI.GetComponent<GUIButtonRef>();
            m_closeButtonData = guiButtonRef.GetButtonData("Close");
            m_closeButtonData.Button.onClick.AddListener(animateClose);
            CommonButtonVisual.AddSelectedBorder(m_closeButtonData);

            CommonVisual.InitCardsBallsSpinWheelGUI(balance, guiRef.GetGameObject("CardsBallsSpinwheel"), ref m_cardsBallsSpinWheelGUI);

            CommonBallVisual.InitBallsVisualData(balance, guiRef.GetGameObject("Balls").GetComponent<GUIRef>(), m_uiBallVisualData);

            m_UI.SetActive(false);
        }

        void selectButton(INFO_MENU_BUTTONS selectedButton)
        {
            hideAllButtonSelections();
        }

        void hideAllButtonSelections()
        {
            m_cardsBallsSpinWheelGUI.BallsButtonData.SelectedGO.SetActive(false);
            m_cardsBallsSpinWheelGUI.SpinwheelButtonData.SelectedGO.SetActive(false);
            CommonVisual.UnselectAllJokers();
        }

        void initRoundInfo(GUIRef guiRef, ref RoundInfoGUI shopRoundInfoGUI)
        {
            shopRoundInfoGUI.Title = guiRef.GetTextGUI("Round");
            shopRoundInfoGUI.Description = guiRef.GetTextGUI("Description");
            shopRoundInfoGUI.Cover = guiRef.GetGameObject("Cover");
        }

        public void Show()
        {
            m_UI.SetActive(true);

            CommonChipsVisual.Show(runData, balance, m_baseChipsText, m_mostFrequent, m_leastFrequest);

            int smallRound = runData.Round % 3;
            int bigRound = runData.Round / 3;
            for (int i = 0; i < 3; i++)
            {
                m_shopRoundInfoGUI[i].Cover.SetActive(i != smallRound);
                m_shopRoundInfoGUI[i].Title.text = CommonVisual.GetRoundString(bigRound, i);

                CommonVisual.ShowRoundShopInfo(runData, balance, bigRound, i, m_shopRoundInfoGUI[i].Description);
            }

            m_closeButtonData.SelectedGO.SetActive(CommonButtonVisual.ShowSelected());
            CommonButtonVisual.UpdateButtonIcons(m_closeButtonData, Game.Instance.GetGamepadType());

            CommonVisual.ShowBallsAndSpinWheel(runData, balance, m_cardsBallsSpinWheelGUI, runData.SlotType);

            // show jokers
            CommonVisual.ShowJokersCommon(runData, balance, m_cardsBallsSpinWheelGUI.JokerParent);
            for (int jkrIdx = 0; jkrIdx < runData.JokerCount; jkrIdx++)
            {
                int localJokerIdx = jkrIdx;
                CommonVisual.JokerGUIs[jkrIdx].Button.onClick.AddListener(() => showJokerInfoPopup(localJokerIdx));
            }

            CommonBallVisual.ShowBalls(runData.BallTypes, balance, m_uiBallVisualData);
        }

        public void Tick(float dt)
        {
            if (CommonVisual.AnimateCloseTick(ref m_closeTimer, dt))
                Game.Instance.SetMenuState(runData.PrevMenuState);

            if (CommonButtonVisual.NavigateEnter() || CommonButtonVisual.NavigateGamepadButton(m_closeButtonData))
                animateClose();

            CommonSlotsVisual.TickSpinWheelUI(runData, balance.UISpinWheelSpeed, dt, m_cardsBallsSpinWheelGUI);
        }

        public void Hide()
        {
            m_UI.SetActive(false);
        }

        void animateClose()
        {
            SoundManager.Instance.PlaySFXButtonOK();

            CommonVisual.AnimateClose(ref m_closeTimer, m_closeTime, m_animation, "In Game Info Close");
        }

        void showJokerInfoPopup(int jkrIdx)
        {

        }
    }
}