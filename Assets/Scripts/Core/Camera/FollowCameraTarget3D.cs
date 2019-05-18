using UnityEngine;

namespace pdxpartyparrot.Core.Camera
{
    public class FollowCameraTarget3D : FollowCameraTarget
    {
        [SerializeField]
        private Collider _collider;

        public Collider Collider => _collider;
    }
}
