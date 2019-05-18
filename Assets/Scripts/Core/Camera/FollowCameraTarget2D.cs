using UnityEngine;

namespace pdxpartyparrot.Core.Camera
{
    public class FollowCameraTarget2D : FollowCameraTarget
    {
        [SerializeField]
        private Collider2D _collider;

        public Collider2D Collider => _collider;
    }
}
