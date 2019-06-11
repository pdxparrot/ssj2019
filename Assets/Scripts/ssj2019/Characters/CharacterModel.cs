using UnityEngine;

namespace pdxpartyparrot.ssj2019.Characters
{
    public sealed class CharacterModel : MonoBehaviour
    {
        [SerializeField]
        private SpriteRenderer _modelSprite;

        public SpriteRenderer ModelSprite => _modelSprite;

        [SerializeField]
        private SpriteRenderer _shadowSprite;

        public SpriteRenderer ShadowSprite => _shadowSprite;
    }
}
