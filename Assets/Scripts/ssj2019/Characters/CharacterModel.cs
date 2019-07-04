using JetBrains.Annotations;

using pdxpartyparrot.Core.Actors;

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

        public void InitializeBehavior(ActorBehavior behavior, int skinIndex)
        {
            if(null != ModelSprite) {
                behavior.SpriteAnimationHelper.AddRenderer(ModelSprite);
            }

            if(null != SpineModel) {
                behavior.SpineAnimationHelper.SkeletonAnimation = SpineModel;

                behavior.SpineSkinHelper.SkeletonAnimation = SpineModel;
               if(skinIndex < 0) {
                    behavior.SpineSkinHelper.SetRandomSkin();
                } else {
                    behavior.SpineSkinHelper.SetSkin(skinIndex);
                }
            }

            behavior.SpriteAnimationHelper.AddRenderer(ShadowSprite);
        }
    }
}
