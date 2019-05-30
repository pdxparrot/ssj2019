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
        private SkeletonAnimation _animation;

        public void Pause(bool pause)
        {
            _animation.timeScale = pause ? 0.0f : 1.0f;
        }

        public void ResetAnimation()
        {
            _animation.ClearState();
        }

        public TrackEntry SetAnimation(string animationName, bool loop)
        {
            return SetAnimation(0, animationName, loop);
        }

        public TrackEntry SetAnimation(int track, string animationName, bool loop)
        {
            return _animation.AnimationState.SetAnimation(track, animationName, loop);
        }

        public void SetFacing(Vector3 direction)
        {
            if(Mathf.Abs(direction.x) < MathUtil.Epsilon) {
                return;
            }

            // TODO: if the skeleton is scaled, does this unscale it?
            // if so, we might have to take the Abs() first
            _animation.Skeleton.ScaleX = direction.x < 0 ? -1.0f : 1.0f;
        }
    }
}
#endif
