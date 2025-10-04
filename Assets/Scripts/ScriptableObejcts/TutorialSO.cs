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