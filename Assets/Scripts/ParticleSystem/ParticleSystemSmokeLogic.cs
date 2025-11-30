/*
  Cardwheel — Non-Commercial, No-Modification License
  Copyright © 2025 Nitzan Wilnai
  Source Code: https://github.com/nitzanwilnai/Cardwheel

  Permission is granted to view and run this code for non-commercial purposes only.
  Modification, redistribution of altered versions, and commercial use are strictly prohibited.

  See the LICENSE file for full legal terms.
*/

using UnityEngine;
using System;
using Cardwheel;

namespace ParticleSystemDOD
{
    public static class ParticleSystemSmokeLogic
    {
        public static void Init(ParticleData gameData, ParticleBalance particleBalance)
        {
            gameData.StartPos = new Vector2[particleBalance.MaxSprites];
            gameData.CurrentPos = new Vector2[particleBalance.MaxSprites];
            gameData.Radius = new Vector2[particleBalance.MaxSprites];
            gameData.TargetPos = new Vector2[particleBalance.MaxSprites];
            gameData.Time = new float[particleBalance.MaxSprites];
            gameData.Velocity = new float[particleBalance.MaxSprites];
            gameData.AliveIndices = new int[particleBalance.MaxSprites];
            gameData.DeadIndices = new int[particleBalance.MaxSprites];
            gameData.ParticleColor = new Color[particleBalance.MaxSprites];
        }

        public static void StartSimulation(
            ParticleData particleData,
            ParticleBalance particleBalance
            )
        {
            particleData.AliveCount = 0;
            particleData.DeadCount = particleBalance.MaxSprites;
            for (int i = 0; i < particleBalance.MaxSprites; i++)
                particleData.DeadIndices[i] = particleBalance.MaxSprites - 1 - i;

            for (int i = 0; i < particleBalance.MaxSprites; i++)
                particleData.Time[i] = 0.0f;
        }

        static void emitCommon(ParticleData particleData, ParticleBalance particleBalance, int numSprites, Span<int> addedSpriteIndices, ref int addedSpriteCount, Color color)
        {
            for (int i = 0; i < numSprites; i++)
            {
                if (particleData.DeadCount > 0)
                {
                    int spriteIndex = particleData.DeadIndices[--particleData.DeadCount];
                    particleData.AliveIndices[particleData.AliveCount++] = spriteIndex;
                    particleData.Time[spriteIndex] = 0.0f;
                    particleData.ParticleColor[spriteIndex] = color;

                    particleData.Velocity[spriteIndex] = UnityEngine.Random.value * (particleBalance.SpeedMax - particleBalance.SpeedMin) + particleBalance.SpeedMin;

                    addedSpriteIndices[addedSpriteCount++] = spriteIndex;
                }
            }
        }

        static Vector2 GetRandomCirclePosition(float radius)
        {
            float angle = UnityEngine.Random.value * 360.0f;
            return new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;
        }

        public static void Emit(ParticleData particleData, ParticleBalance particleBalance, Span<int> addedSpriteIndices, Vector2 position, float magnitude, ref int addedSpriteCount, Color color)
        {
            emitCommon(particleData, particleBalance, Mathf.FloorToInt(magnitude), addedSpriteIndices, ref addedSpriteCount, color);

            for (int i = 0; i < addedSpriteCount; i++)
            {
                int particleIndex = addedSpriteIndices[i];

                particleData.CurrentPos[particleIndex] = particleData.StartPos[particleIndex] = position + GetRandomCirclePosition(0.1f);

                float radius = UnityEngine.Random.value * (particleBalance.TargetRadiusMax - particleBalance.TargetRadiusMin) + particleBalance.TargetRadiusMin;
                particleData.TargetPos[particleIndex] = position + GetRandomCirclePosition(radius);

                particleData.Radius[particleIndex].x = (particleBalance.RadiusMax - particleBalance.RadiusMin) * UnityEngine.Random.value + particleBalance.RadiusMin;
                particleData.Radius[particleIndex].y = (particleBalance.RadiusMax - particleBalance.RadiusMin) * UnityEngine.Random.value + particleBalance.RadiusMin;
            }
        }

        public static void TickParticles(
            ParticleData particleData,
            ParticleBalance particleBalance,
            float dt,
            Span<int> removedSpriteIndices,
            ref int removedSpriteCount)
        {
            // increment particle time
            TickParticleTime(particleData, dt, removedSpriteIndices, ref removedSpriteCount);

            // move particles
            for (int i = 0; i < particleData.AliveCount; i++)
            {
                int particleIndex = particleData.AliveIndices[i];
                float velocityValue = particleBalance.VelocityCurve.Evaluate(particleData.Time[particleIndex]);
                Vector2 diff = particleData.TargetPos[particleIndex] - particleData.StartPos[particleIndex];

                particleData.CurrentPos[particleIndex] = particleData.StartPos[particleIndex] + diff * velocityValue;
            }
        }

        private static void TickParticleTime(ParticleData particleData, float dt, Span<int> removedSpriteIndices, ref int removedSpriteCount)
        {
            int count = 0;
            for (int i = 0; i < particleData.AliveCount; i++)
            {
                int particleIndex = particleData.AliveIndices[i];
                particleData.Time[particleIndex] += dt * particleData.Velocity[particleIndex];
                if (particleData.Time[particleIndex] > 1.0f)
                {
                    particleData.DeadIndices[particleData.DeadCount++] = particleIndex;
                    removedSpriteIndices[removedSpriteCount++] = particleIndex;
                }
                else
                    particleData.AliveIndices[count++] = particleIndex;
            }
            particleData.AliveCount = count;
        }
    }
}