/*
  Cardwheel — Non-Commercial, No-Modification License
  Copyright © 2025 Nitzan Wilnai
  Source Code: https://github.com/nitzanwilnai/Cardwheel

  Permission is granted to view and run this code for non-commercial purposes only.
  Modification, redistribution of altered versions, and commercial use are strictly prohibited.

  See the LICENSE file for full legal terms.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CommonTools;
using TMPro;

namespace Cardwheel
{
    public struct ShopRoundInfoGUI
    {
        public TextMeshProUGUI Title;
        public TextMeshProUGUI Description;
        public GameObject Cover;
    }

    public class ShopInfoVisual : MonoBehaviour
    {

        GameObject m_UI;

        TextMeshProUGUI[] m_baseChipsText;
        TextMeshProUGUI m_mostFrequent;
        TextMeshProUGUI m_leastFrequest;

        float m_closeTimer = 0.0f;
        float m_closeTime = 0.1f;

        Animation m_animation;

        ShopRoundInfoGUI[] m_shopRoundInfoGUI;

        GUIButtonData m_closeButtonData;

        public void Init(Camera camera)
        {
            m_UI = AssetManager.Instance.LoadShopInfoUI();
            m_UI.GetComponent<Canvas>().worldCamera = camera;
            CommonVisual.ChangeCanvasScalerMatching(m_UI);

            GUIRef guiRef = m_UI.GetComponent<GUIRef>();

            m_shopRoundInfoGUI = new ShopRoundInfoGUI[3];
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

            m_UI.SetActive(false);
        }

        void initRoundInfo(GUIRef guiRef, ref ShopRoundInfoGUI shopRoundInfoGUI)
        {
            shopRoundInfoGUI.Title = guiRef.GetTextGUI("Round");
            shopRoundInfoGUI.Description = guiRef.GetTextGUI("Description");
            shopRoundInfoGUI.Cover = guiRef.GetGameObject("Cover");
        }

        public void Show(RunData runData, Balance balance)
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
        }

        public void Tick(RunData runData, float dt)
        {
            if (CommonVisual.AnimateCloseTick(ref m_closeTimer, dt))
                Game.Instance.SetMenuState(runData.PrevMenuState);

            if (CommonButtonVisual.NavigateEnter() || CommonButtonVisual.NavigateGamepadButton(m_closeButtonData))
                animateClose();
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
    }
}