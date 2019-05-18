using JetBrains.Annotations;

using UnityEngine.Assertions;
using UnityEngine.Profiling;

namespace pdxpartyparrot.Core.Camera
{
    public class FollowCamera2D : FollowCamera
    {
        [CanBeNull]
        public FollowCameraTarget2D Target2D => (FollowCameraTarget2D)Target;

        public override void SetTarget(FollowCameraTarget target)
        {
            Assert.IsTrue(Target is FollowCameraTarget2D);

            base.SetTarget(target);
        }

        protected override void HandleInput(float dt)
        {
            if(null == Target) {
                return;
            }

            Profiler.BeginSample("FollowCamera2D.HandleInput");
            try {
                Zoom(dt);
            } finally {
                Profiler.EndSample();
            }
        }

        private void Zoom(float dt)
        {
            if(!EnableZoom) {
                return;
            }

            // TODO
        }

        protected override void FollowTarget(float dt)
        {
            if(null == Target) {
                return;
            }

            Profiler.BeginSample("FollowCamera3D.FollowTarget");
            try {
                // TODO
            } finally {
                Profiler.EndSample();
            }
        }
    }
}
