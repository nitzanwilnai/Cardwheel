/*
  Cardwheel — Non-Commercial, No-Modification License
  Copyright © 2025 Nitzan Wilnai
  Source Code: https://github.com/nitzanwilnai/Cardwheel

  Permission is granted to view and run this code for non-commercial purposes only.
  Modification, redistribution of altered versions, and commercial use are strictly prohibited.

  See the LICENSE file for full legal terms.
*/

using UnityEngine;

namespace Cardwheel
{
    [CreateAssetMenu(fileName = "GameInfoSO", menuName = "Cardwheel/GameInfoSO", order = 1)]
    public class GameInfoSO : ScriptableObject
    {
        [Header("Bundles")]
        public string CommonBundle;
        public string CommonBundleUIPath;

        [Header("Board Position")]
        public Vector3 Position;
        public float CameraSize;
        public Vector3 Scale;
        public float Gravity;

        [Header("Keyboard Gamepad")]
        public bool KeyboardGamepadSupport = false;
    }
}