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
        public Vector3 Scale;
    }
}