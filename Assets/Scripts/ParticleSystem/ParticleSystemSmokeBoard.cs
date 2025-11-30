/*
  Cardwheel — Non-Commercial, No-Modification License
  Copyright © 2025 Nitzan Wilnai
  Source Code: https://github.com/nitzanwilnai/Cardwheel

  Permission is granted to view and run this code for non-commercial purposes only.
  Modification, redistribution of altered versions, and commercial use are strictly prohibited.

  See the LICENSE file for full legal terms.
*/

using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;

namespace ParticleSystemDOD
{

    public class ParticleData
    {
        public Vector2[] StartPos;
        public Vector2[] CurrentPos;
        public Vector2[] TargetPos;
        public Color[] ParticleColor;
        public float[] Velocity;
        public float[] Time;
        public Vector2[] Radius;

        public int[] AliveIndices;
        public int AliveCount;
        public int[] DeadIndices;
        public int DeadCount;
    }

    [Serializable]
    public class ParticleBalance
    {
        public int MaxSprites;
        public int NumSprites;
        public AnimationCurve VelocityCurve;
        public AnimationCurve AlphaCurve;
        public AnimationCurve ScaleCurve;
        public float SpeedMin;
        public float SpeedMax;
        public float TargetRadiusMin;
        public float TargetRadiusMax;
        public float RadiusMin;
        public float RadiusMax;
    }

    public class ParticleSystemSmokeBoard : MonoBehaviour
    {
        public GameObject SpritePrefab;

        GameObject[] m_particlePool;
        SpriteRenderer[] m_particleSR;

        GameObject[] m_trailPool;
        SpriteRenderer[] m_trailSR;

        ParticleData particleData = new ParticleData();
        public ParticleBalance particleBalance;

        Transform m_particleParent;

        public void Init(Transform particleParent)
        {
            m_particleParent = particleParent;

            m_particlePool = new GameObject[particleBalance.MaxSprites];
            m_particleSR = new SpriteRenderer[particleBalance.MaxSprites];
            for (int i = 0; i < particleBalance.MaxSprites; i++)
            {
                m_particlePool[i] = Instantiate(SpritePrefab, particleParent);
                m_particlePool[i].name = "Particle " + i;
                m_particlePool[i].transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                m_particleSR[i] = m_particlePool[i].GetComponentInChildren<SpriteRenderer>();
            }

            ParticleSystemSmokeLogic.Init(particleData, particleBalance);
        }

        void Start()
        {
            ParticleSystemSmokeLogic.StartSimulation(particleData, particleBalance);

            for (int i = 0; i < particleBalance.MaxSprites; i++)
                m_particlePool[i].SetActive(false);

            m_particleParent.localScale = new Vector3(1.0f / m_particleParent.parent.localScale.x, 1.0f / m_particleParent.parent.localScale.y, 1.0f);
        }

        void Update()
        {
            float dt = Time.deltaTime;
            TickParticles(dt);
        }

        private void TickParticles(float dt)
        {
            Span<int> removedSpriteIndices = stackalloc int[particleBalance.MaxSprites];
            int removedSpriteCount = 0;

            ParticleSystemSmokeLogic.TickParticles(
                particleData,
                particleBalance,
                dt,
                removedSpriteIndices,
                ref removedSpriteCount);

            for (int i = 0; i < removedSpriteCount; i++)
            {
                int particleIndex = removedSpriteIndices[i];
                m_particlePool[particleIndex].SetActive(false);
            }

            // m_particleParent.rotation = Quaternion.identity;

            for (int i = 0; i < particleData.AliveCount; i++)
            {
                int particleIndex = particleData.AliveIndices[i];
                updateParticleVisualLocal(particleIndex);
            }
        }

        public void Emit(Color color, Vector2 position, float magnitude)
        {
            Vector2 localPosition = m_particleParent.InverseTransformPoint(position);

            Span<int> addedSpriteIndices = stackalloc int[particleBalance.NumSprites];
            int addedSpriteCount = 0;

            ParticleSystemSmokeLogic.Emit(particleData, particleBalance, addedSpriteIndices, localPosition, magnitude, ref addedSpriteCount, color);

            for (int i = 0; i < addedSpriteCount; i++)
            {
                int particleIndex = addedSpriteIndices[i];
                updateParticleVisualLocal(particleIndex);
                m_particlePool[particleIndex].SetActive(true);
            }
        }

        private void updateParticleVisualLocal(int spriteIndex)
        {
            m_particlePool[spriteIndex].transform.localPosition = new Vector3(particleData.CurrentPos[spriteIndex].x, particleData.CurrentPos[spriteIndex].y, -30.0f);

            // m_particlePool[spriteIndex].transform.localPosition = new Vector3(0.0f, 1.0f, -10.0f);

            float scale = particleBalance.ScaleCurve.Evaluate(particleData.Time[spriteIndex]);
            // scale /= m_particleParent.parent.lossyScale.x;
            m_particlePool[spriteIndex].transform.localScale = new Vector3(scale * particleData.Radius[spriteIndex].x, scale * particleData.Radius[spriteIndex].y, 1.0f);

            // updateParticleRotation(spriteIndex);

            updateParticleColor(spriteIndex);
        }

        private void updateParticleColor(int spriteIndex)
        {
            float alphaValue = particleBalance.AlphaCurve.Evaluate(particleData.Time[spriteIndex]);
            particleData.ParticleColor[spriteIndex].a = alphaValue;
            m_particleSR[spriteIndex].color = particleData.ParticleColor[spriteIndex];
        }
    }
}