using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cardwheel
{
    public enum LIGHT_ANIM_STATE { CYCLE, ZOOM_IN, ZOOM_OUT, ZOOM_IN2, ZOOM_OUT2, ALTERNATE };

    [Serializable]
    public struct SpinWheelLightsStateData
    {
        public LIGHT_ANIM_STATE AnimState;
        public float Time;
        public int Count;
    }

    public class SpinWheelLights : MonoBehaviour
    {
        public SpinWheelLight[] SpinWheelLight;

        LIGHT_ANIM_STATE m_animState = LIGHT_ANIM_STATE.CYCLE;

        float m_lightTimer;
        float m_lightTime;

        int m_animCount;

        public SpinWheelLightsStateData[] SpinWheelLightsStateData;
        public int AnimIndex;

        public void Init()
        {
            for (int i = 0; i < SpinWheelLight.Length; i++)
                SpinWheelLight[i].Init(i);
        }

        public void StartAnimation()
        {
            AnimIndex = 0;

            SetAnimationState();
        }

        public void SetAnimationState()
        {
            m_animState = SpinWheelLightsStateData[AnimIndex].AnimState;

            m_lightTimer = MusicManager.Instance.TimeSinceLastBeat();
            m_lightTime = SpinWheelLightsStateData[AnimIndex].Time;

            for (int i = 0; i < SpinWheelLight.Length; i++)
                SpinWheelLight[i].SetAnimationState(SpinWheelLightsStateData[AnimIndex]);
        }

        public void Tick(float dt)
        {
            m_lightTimer += dt;
            if (m_lightTimer >= m_lightTime)
            {
                m_lightTimer -= m_lightTime;

                m_animCount++;

                for (int i = 0; i < SpinWheelLight.Length; i++)
                    SpinWheelLight[i].Tick();

                if (m_animCount > SpinWheelLightsStateData[AnimIndex].Count)
                {
                    AnimIndex = (AnimIndex + 1) % SpinWheelLightsStateData.Length;
                    m_animCount = 0;
                    SetAnimationState();
                }

            }

        }
    }
}