using System;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Data
{
    [CreateAssetMenu(fileName="PlayerInputData", menuName="pdxpartyparrot/ssj2019/Data/Player/PlayerInput Data")]
    [Serializable]
    public sealed class PlayerInputData : Game.Data.PlayerInputData
    {
        [SerializeField]
        [Tooltip("Clear the input queue after this many milliseconds without input")]
        private int _inputQueueTimeout = 500;

        public int InputQueueTimeout => _inputQueueTimeout;
    }
}
