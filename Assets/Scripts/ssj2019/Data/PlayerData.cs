using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Serialization;

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
            [FormerlySerializedAs("_playerIndicator")]
            private Sprite _playerIndicatorSprite;

            public Sprite PlayerIndicatorSprite => _playerIndicatorSprite;

            [SerializeField]
            private string _playerIndicatorText;

            public string PlayerIndicatorText => _playerIndicatorText;

            [SerializeField]
            private Color _playerColor;

            public Color PlayerColor => _playerColor;
        }

        [SerializeField]
        private PlayerIndicatorState[] _playerIndicators;

        public IReadOnlyCollection<PlayerIndicatorState> PlayerIndicators => _playerIndicators;
    }
}
