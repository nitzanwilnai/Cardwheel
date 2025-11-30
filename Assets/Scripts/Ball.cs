/*
  Cardwheel — Non-Commercial, No-Modification License
  Copyright © 2025 Nitzan Wilnai
  Source Code: https://github.com/nitzanwilnai/Cardwheel

  Permission is granted to view and run this code for non-commercial purposes only.
  Modification, redistribution of altered versions, and commercial use are strictly prohibited.

  See the LICENSE file for full legal terms.
*/

using Cardwheel;
using ParticleSystemDOD;
using UnityEngine;

namespace Cardwheel
{
    public class Ball : MonoBehaviour
    {
        enum COLLISION_TYPE { OTHER, BALL_TO_BALL, BALL_TO_SPIN_WHEEL };

        public int MyIndex;
        public GameObject SpriteGO;

        public AnimationCurve WobbleAnimCurveX;
        public AnimationCurve WobbleAnimCurveY;
        // float m_animTimer = 0.0f;
        // float m_animScale = 1.0f;
        // float m_animVelocity = 1.0f;

        public ParticleSystemSmokeBoard ParticleSystemSmoke;

        public void Init(Transform particleParent)
        {
            // ParticleSystemSmoke.Init(particleParent);
        }

        void OnEnable()
        {
            SpriteGO.transform.localScale = Vector3.one;
        }

        void OnCollisionEnter2D(Collision2D col)
        {
            char c = col.gameObject.name[0];
            int i = (int)c;

            // float animScaleValue = 0.0f;
            COLLISION_TYPE collisionType = COLLISION_TYPE.OTHER;
            if (c == 'C')
            {
                collisionType = COLLISION_TYPE.BALL_TO_SPIN_WHEEL;
                // animScaleValue = 1 / 20.0f;
            }
            else if (i >= 48 && i <= 54)
            {
                collisionType = COLLISION_TYPE.BALL_TO_BALL;
                // animScaleValue = 1 / 50.0f;
            }


            // float collisionScale = col.relativeVelocity.magnitude;
            // if (collisionScale < 1.0f)
            //     collisionScale = 1.0f;
            // else
            //     collisionScale = 1.0f + collisionScale * animScaleValue;

            Debug.Log(gameObject.name + " collided with " + col.gameObject.name + " c " + c + " i " + i + " col.relativeVelocity.magnitude " + col.relativeVelocity.magnitude);

            if (collisionType != COLLISION_TYPE.OTHER)
            {
                // if (m_animTimer <= 0.0f || m_animScale < collisionScale)
                // {
                //     m_animTimer = 1.0f;
                //     m_animScale = collisionScale;
                //     m_animVelocity = Random.value * 0.2f + 0.9f;
                // }

                Vector2 collisionPoint = col.contactCount > 0 ? col.contacts[0].point : transform.position;
                float magnitude = col.relativeVelocity.magnitude;

                if (collisionType == COLLISION_TYPE.BALL_TO_SPIN_WHEEL)
                    Game.Instance.BallSpinWheelCollision(collisionPoint, magnitude);
                else if (collisionType == COLLISION_TYPE.BALL_TO_BALL)
                    Game.Instance.BallBallCollision(collisionPoint, magnitude);
            }
        }

        void Update()
        {
            // if (m_animTimer > 0.0f)
            // {
            //     // m_animScale = 5.0f;
            //     float valueX = (1.0f - WobbleAnimCurveX.Evaluate(1.0f - m_animTimer)) * m_animScale + 1.0f;
            //     float valueY = (1.0f - WobbleAnimCurveY.Evaluate(1.0f - m_animTimer)) * m_animScale + 1.0f;
            //     m_animTimer -= Time.deltaTime * m_animVelocity;
            //     if (m_animTimer <= 0.0f)
            //         valueX = valueY = 1.0f;

            //     // Debug.Log("Ball " + gameObject.name + " m_animTimer " + m_animTimer + " m_animScale " + m_animScale + " valueX " + valueX + " valueY " + valueY);

            //     SpriteGO.transform.localScale = new Vector3(valueX, valueY, 1.0f);
            // }
        }
    }
}