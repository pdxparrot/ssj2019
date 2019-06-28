using System;
using System.Collections.Generic;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Data
{
    [CreateAssetMenu(fileName="PlayerData", menuName="pdxpartyparrot/ssj2019/Data/Player/Player Data")]
    [Serializable]
    public sealed class PlayerData : ScriptableObject
    {
        [Serializable]
        public sealed class PlayerIndicatorState
        {
            [SerializeField]
            private Sprite _playerIndicator;

            public Sprite PlayerIndicator => _playerIndicator;

            [SerializeField]
            private Color _playerColor;

            public Color PlayerColor => _playerColor;
        }

        [SerializeField]
        private PlayerIndicatorState[] _playerIndicators;

        public IReadOnlyCollection<PlayerIndicatorState> PlayerIndicators => _playerIndicators;
    }
}
