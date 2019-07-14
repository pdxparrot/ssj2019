using JetBrains.Annotations;

using Spine.Unity;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Characters
{
    public sealed class CharacterModel : MonoBehaviour
    {
        [SerializeField]
        [CanBeNull]
        private SpriteRenderer _modelSprite;

        public SpriteRenderer ModelSprite => _modelSprite;

        [SerializeField]
        [CanBeNull]
        private SkeletonAnimation _spineModel;

        public SkeletonAnimation SpineModel => _spineModel;

        [SerializeField]
        private SpriteRenderer _shadowSprite;

        public SpriteRenderer ShadowSprite => _shadowSprite;
    }
}
