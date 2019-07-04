using JetBrains.Annotations;

using pdxpartyparrot.ssj2019.Data;

using TMPro;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Players
{
    public sealed class PlayerIndicator : MonoBehaviour
    {
        [SerializeField]
        [CanBeNull]
        private SpriteRenderer _sprite;

        [SerializeField]
        [CanBeNull]
        private TextMeshPro _text;

        [SerializeField]
        private MeshRenderer _ground;

        public void Initialize(PlayerData.PlayerIndicatorState indicatorState)
        {
            if(null != _sprite && null != indicatorState.PlayerIndicatorSprite) {
                _sprite.sprite = indicatorState.PlayerIndicatorSprite;
                _sprite.color = indicatorState.PlayerColor;
            }

            if(null != _text) {
                _text.text = indicatorState.PlayerIndicatorText;
                _text.color = indicatorState.PlayerColor;
            }

            _ground.material.color = indicatorState.PlayerColor;
        }
    }
}
