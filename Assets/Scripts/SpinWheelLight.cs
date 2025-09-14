using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cardwheel
{
    public class SpinWheelLight : MonoBehaviour
    {
        public GameObject[] Lights;
        int m_numLights;
        int m_lightIndex;

        int m_index;

        int m_animCount;

        LIGHT_ANIM_STATE m_animState;

        public void Awake()
        {
            m_numLights = Lights.Length;
            m_lightIndex = 0;

            for (int i = 0; i < m_numLights; i++)
                Lights[i].SetActive(false);
        }

        public void Init(int index)
        {
            m_index = index;
        }

        public void SetAnimationState(SpinWheelLightsStateData SpinWheelLightsStateData)
        {
            m_animState = SpinWheelLightsStateData.AnimState;
            m_lightIndex = 0;

            for (int i = 0; i < m_numLights; i++)
                Lights[i].SetActive(false);
        }

        public void SetAnimationState(LIGHT_ANIM_STATE newAnimState)
        {
            m_animState = newAnimState;


            m_lightIndex = 0;

            for (int i = 0; i < m_numLights; i++)
                Lights[i].SetActive(false);
        }

        public void Tick()
        {
            m_animCount++;

            if (m_animState == LIGHT_ANIM_STATE.CYCLE)
            {
                m_lightIndex = (m_lightIndex + 1) % 24;
                for (int i = 0; i < m_numLights; i++)
                {
                    if (m_lightIndex == m_index)
                        Lights[i].SetActive(true);
                }
            }
            if (m_animState == LIGHT_ANIM_STATE.ZOOM_IN)
            {
                if (m_lightIndex < m_numLights)
                    Lights[m_lightIndex].SetActive(true);

                m_lightIndex++;
                if (m_lightIndex > m_numLights)
                {
                    m_lightIndex = 0;
                    for (int i = 0; i < m_numLights; i++)
                        Lights[i].SetActive(false);
                }
            }
            if (m_animState == LIGHT_ANIM_STATE.ZOOM_OUT)
            {
                if (m_lightIndex < m_numLights)
                    Lights[(m_numLights - 1) - m_lightIndex].SetActive(true);

                m_lightIndex++;
                if (m_lightIndex > m_numLights)
                {
                    m_lightIndex = 0;
                    for (int i = 0; i < m_numLights; i++)
                        Lights[i].SetActive(false);
                }
            }
            if (m_animState == LIGHT_ANIM_STATE.ZOOM_IN2)
            {
                if (m_lightIndex < m_numLights)
                    Lights[m_lightIndex].SetActive(true);
                else if (m_lightIndex - m_numLights < m_numLights)
                    Lights[m_lightIndex - m_numLights].SetActive(false);

                m_lightIndex++;
                if (m_lightIndex > m_numLights * 2)
                {
                    m_lightIndex = 0;
                    for (int i = 0; i < m_numLights; i++)
                        Lights[i].SetActive(false);
                }
            }
            if (m_animState == LIGHT_ANIM_STATE.ZOOM_OUT2)
            {
                if (m_lightIndex < m_numLights)
                    Lights[(m_numLights - 1) - m_lightIndex].SetActive(true);
                else if (m_lightIndex - m_numLights < m_numLights)
                    Lights[(m_numLights - 1) - (m_lightIndex - m_numLights)].SetActive(false);

                m_lightIndex++;
                if (m_lightIndex > m_numLights * 2)
                {
                    m_lightIndex = 0;
                    for (int i = 0; i < m_numLights; i++)
                        Lights[i].SetActive(false);
                }
            }
            else if (m_animState == LIGHT_ANIM_STATE.ALTERNATE)
            {
                m_lightIndex = (m_lightIndex + 1) % 2;
                for (int i = 0; i < m_numLights; i++)
                {
                    Lights[i].SetActive(m_lightIndex == m_index % 2);
                }
            }
        }
    }
}