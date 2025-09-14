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

        float m_closeTimer = 0.0f;
        float m_closeTime = 0.1f;

        Animation m_animation;

        ShopRoundInfoGUI[] m_shopRoundInfoGUI;

        public void Init(Camera camera, Balance balance)
        {

            m_UI = AssetManager.Instance.LoadShopInfoUI();
            m_UI.GetComponent<Canvas>().worldCamera = camera;
            GUIRef guiRef = m_UI.GetComponent<GUIRef>();

            m_shopRoundInfoGUI = new ShopRoundInfoGUI[3];
            for (int i = 0; i < 3; i++)
            {
                initRoundInfo(guiRef.GetGameObject("Round" + (i + 1)).GetComponent<GUIRef>(), ref m_shopRoundInfoGUI[i]);
                m_shopRoundInfoGUI[i].Cover.SetActive(false);
            }

            guiRef.GetButton("Close").onClick.AddListener(Game.Instance.CloseShopInfo);
            m_animation = guiRef.GetAnimation("Animation");

            m_baseChipsText = new TextMeshProUGUI[(int)SLOT_TYPE.LAST];
            CommonChipsVisual.InitChipsInfo(guiRef, m_baseChipsText);

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

            CommonChipsVisual.Show(runData, m_baseChipsText);

            int smallRound = runData.Round % 3;
            int bigRound = runData.Round / 3;
            for (int i = 0; i < 3; i++)
            {
                m_shopRoundInfoGUI[i].Cover.SetActive(i != smallRound);
                m_shopRoundInfoGUI[i].Title.text = CommonVisual.GetRoundString(bigRound, i);

                CommonVisual.ShowRoundShopInfo(runData, balance, bigRound, i, m_shopRoundInfoGUI[i].Description);
            }
        }

        public void Tick(RunData runData, float dt)
        {
            if (CommonVisual.AnimateCloseTick(ref m_closeTimer, dt))
                Game.Instance.SetMenuState(runData.PrevMenuState);

        }

        public void Hide()
        {
            m_UI.SetActive(false);
        }

        public void AnimateClose()
        {
            CommonVisual.AnimateClose(ref m_closeTimer, m_closeTime, m_animation, "In Game Info Close");
        }
    }
}