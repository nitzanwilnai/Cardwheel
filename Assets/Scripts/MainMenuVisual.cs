using System;
using CommonTools;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Cardwheel
{
    public class MainMenuVisual : MonoBehaviour
    {
        GameObject m_UI;
        TextMeshProUGUI m_title;

        float m_goToWheelSelectTimer = 0.0f;
        float m_goToWheelSelectionTime = 0.1f;
        float m_continueGametTimer = 0.0f;
        float m_continueGameTime = 0.1f;
        Animation m_animation;

        public Transform Joker1Parent;
        public Transform Joker2Parent;

        public int NumJokers;

        GameObject[] m_jokersGO;
        Vector2[] m_jokerPos;
        float[] m_jokerSpeed;
        float[] m_jokerAngle;
        float[] m_jokerRotationSpeed;
        int[] m_shuffledJokerIdxs;
        int m_jokerIdx;

        GameObject m_continueButton;

        public void Init(Camera camera, Balance balance)
        {
            m_UI = AssetManager.Instance.LoadMainMenuUI();
            m_UI.GetComponent<Canvas>().worldCamera = camera;

            GUIRef guiRef = m_UI.GetComponent<GUIRef>();
            guiRef.GetButton("Play").onClick.AddListener(Game.Instance.AnimateGoToWheelSelection);
            guiRef.GetButton("Continue").onClick.AddListener(Game.Instance.AnimateContinueRun);
            m_continueButton = guiRef.GetButton("Continue").gameObject;

            m_animation = guiRef.GetAnimation("Animation");

            Joker1Parent = guiRef.GetGameObject("Jokers1").transform;
            Joker2Parent = guiRef.GetGameObject("Jokers2").transform;

            m_jokersGO = new GameObject[balance.JokerBalance.NumJokers];

            m_jokerPos = new Vector2[balance.JokerBalance.NumJokers];
            m_jokerSpeed = new float[balance.JokerBalance.NumJokers];
            m_jokerAngle = new float[balance.JokerBalance.NumJokers];
            m_jokerRotationSpeed = new float[balance.JokerBalance.NumJokers];
            m_shuffledJokerIdxs = new int[balance.JokerBalance.NumJokers];

            // pick NumJokers random jokers
            for (int i = 0; i < balance.JokerBalance.NumJokers; i++)
                m_shuffledJokerIdxs[i] = i;
            uint seed = (uint)(UInt16.MaxValue * UnityEngine.Random.value);
            Logic.ShuffleIntArray(ref seed, m_shuffledJokerIdxs);

            for (int i = 0; i < balance.JokerBalance.NumJokers; i++)
            {
                GameObject jokerGO = new GameObject("Joker" + (i + 1));
                m_jokersGO[i] = jokerGO;

                Image image = jokerGO.AddComponent<Image>();
                image.sprite = AssetManager.Instance.LoadJokerSprite(balance.JokerBalance.JokerSpritesNames[i]);

                RectTransform trans = jokerGO.GetComponent<RectTransform>();
                trans.transform.SetParent(UnityEngine.Random.value < 0.5f ? Joker1Parent : Joker2Parent); // setting parent
                trans.localScale = new Vector3(0.5f, 0.5f, 1.0f);

                trans.sizeDelta = new Vector2(256, 448); // custom size

                m_jokerPos[i] = new Vector3(-1080.0f, 0.0f);
                m_jokerSpeed[i] = 0.0f;

            }

            for (int i = 0; i < NumJokers; i++)
                moveNewJoker();

            m_title = guiRef.GetTextGUI("Title");
            Hide();
        }

        void stopJoker(int jkrIdx)
        {
            m_jokerPos[jkrIdx] = new Vector3(-1080.0f, 0.0f);
            m_jokerSpeed[jkrIdx] = 0.0f;
        }

        void moveNewJoker()
        {
            int jkrIdx = m_shuffledJokerIdxs[m_jokerIdx];
            m_jokerIdx = (m_jokerIdx + 1) % m_shuffledJokerIdxs.Length;
            float posX = -1080.0f - 2060.0f * UnityEngine.Random.value;
            float posY = 720.0f * UnityEngine.Random.value - 360.0f;
            m_jokerPos[jkrIdx] = new Vector2(posX, posY);

            m_jokerSpeed[jkrIdx] = UnityEngine.Random.value * 100.0f + 500.0f;
            m_jokerAngle[jkrIdx] = UnityEngine.Random.value * 360.0f;
            m_jokerRotationSpeed[jkrIdx] = UnityEngine.Random.value * 50.0f + 50.0f;
        }

        public void Show(Balance balance)
        {
            m_title.text = CommonVisual.ColorText(balance, "Cardwheel");

            MENU_STATE menuState = RunDataIO.LoadMenuStateOnly();
            m_continueButton.SetActive(menuState >= MENU_STATE.IN_GAME && menuState < MENU_STATE.GAME_OVER);

            m_UI.SetActive(true);
        }

        public void Tick(Balance balance, float dt)
        {
            if (CommonVisual.AnimateCloseTick(ref m_goToWheelSelectTimer, dt))
                Game.Instance.GoToWheelSelection();
            if (CommonVisual.AnimateCloseTick(ref m_continueGametTimer, dt))
                Game.Instance.ContinueRun();

            for (int i = 0; i < balance.JokerBalance.NumJokers; i++)
            {
                m_jokerPos[i].x += dt * m_jokerSpeed[i];
                if (m_jokerPos[i].x > 1080.0f)
                {
                    stopJoker(i);
                    moveNewJoker();
                }
                m_jokersGO[i].transform.localPosition = m_jokerPos[i];

                m_jokerAngle[i] += m_jokerRotationSpeed[i] * dt;
                if (m_jokerAngle[i] > 360.0f)
                    m_jokerAngle[i] -= 360.0f;
                m_jokersGO[i].transform.localRotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, m_jokerAngle[i]));

            }
        }

        public void Hide()
        {
            m_UI.SetActive(false);
        }

        public void AnimateGoToWheelSelection()
        {
            CommonVisual.AnimateClose(ref m_goToWheelSelectTimer, m_goToWheelSelectionTime, m_animation, "Main Menu Close");
        }

        public void AnimateContinueGame()
        {
            CommonVisual.AnimateClose(ref m_continueGametTimer, m_continueGameTime, m_animation, "Main Menu Close");
        }
    }
}
