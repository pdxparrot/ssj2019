#if USE_SPINE
using pdxpartyparrot.Core.Math;

using Spine;
using Spine.Unity;

using UnityEngine;

namespace pdxpartyparrot.Core.Animation
{
    public class SpineAnimationHelper : MonoBehaviour
    {
        [SerializeField]
        private SkeletonAnimation _skeletonAnimation;

        public SkeletonAnimation SkeletonAnimation
        {
            get => _skeletonAnimation;
            set => _skeletonAnimation = value;
        }

        public void Pause(bool pause)
        {
            // null check this just in case we pause
            // before the skeleton link is valid
            if(null != SkeletonAnimation) {
                SkeletonAnimation.timeScale = pause ? 0.0f : 1.0f;
            }
        }

        public void ResetAnimation()
        {
            SkeletonAnimation.ClearState();
        }

        public TrackEntry SetAnimation(string animationName, bool loop)
        {
            return SetAnimation(0, animationName, loop);
        }

        public TrackEntry SetAnimation(int track, string animationName, bool loop)
        {
            return SkeletonAnimation.AnimationState.SetAnimation(track, animationName, loop);
        }

        public void SetFacing(Vector3 direction)
        {
            if(Mathf.Abs(direction.x) < MathUtil.Epsilon) {
                return;
            }

            // TODO: if the skeleton is scaled, does this unscale it?
            // if so, we might have to take the Abs() first
            SkeletonAnimation.Skeleton.ScaleX = Mathf.Sign(direction.x);
        }
    }
}
#endif
