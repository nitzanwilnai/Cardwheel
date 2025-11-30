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

public class BallHighlight : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        float parentRotationZ = transform.parent.localRotation.eulerAngles.z;
        transform.localRotation = Quaternion.Euler(0.0f, 0.0f, -parentRotationZ);
    }
}
