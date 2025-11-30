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
    [CreateAssetMenu(fileName = "TutorialSO", menuName = "Cardwheel/TutorialSO", order = 1)]
    public class TutorialSO : ScriptableObject
    {
        public MENU_STATE MenuState;
        [TextArea] public string text;
    }
}