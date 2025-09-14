using System.Collections;
using System.Collections.Generic;
using Cardwheel;
using Unity.Burst.Intrinsics;
using Unity.VisualScripting;
using UnityEngine;

public class Ball : MonoBehaviour
{
    void OnCollisionEnter2D(Collision2D col)
    {
        char c = col.gameObject.name[0];
        int i = (int)c;

        // Debug.Log(gameObject.name + " collided with " + col.gameObject.name + " c " + c + " i " + i);

        if (c == 'C')
            Game.Instance.BallSpinWheelCollision();
        else if (i >= 48 && i <= 54)
            Game.Instance.BallBallCollision();
    }
}
