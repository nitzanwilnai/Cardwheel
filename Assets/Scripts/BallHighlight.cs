using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallHighlight : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        float parentRotationZ = transform.parent.localRotation.eulerAngles.z;
        transform.localRotation = Quaternion.Euler(0.0f, 0.0f, -parentRotationZ);
    }
}
