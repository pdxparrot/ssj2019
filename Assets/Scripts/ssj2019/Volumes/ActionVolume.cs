using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.Interactables;

using Spine.Unity;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Volumes
{
    [RequireComponent(typeof(Interactables))]
    [RequireComponent(typeof(BoxCollider))]
    [RequireComponent(typeof(BoneFollower))]
    public abstract class ActionVolume : MonoBehaviour, IInteractable
    {
        [SerializeField]
        private Actor _owner;

        protected Actor Owner => _owner;

        protected BoxCollider _collider;

        [SerializeField]
        [ReadOnly]
        private bool _enabled;

        protected bool IsEnabled
        {
            get => _enabled;
            private set => _enabled = value;
        }

        public bool CanInteract => IsEnabled;

        public Vector3 Offset
        {
            get => _collider.center;
            protected set => _collider.center = value;
        }

        public Vector3 Size
        {
            get => _collider.size;
            protected set => _collider.size = value;
        }

        protected Interactables Interactables { get; private set; }

        protected BoneFollower BoneFollower { get; private set; }

#region Unity Lifecycle
        protected virtual void Awake()
        {
            _collider = GetComponent<BoxCollider>();
            _collider.isTrigger = true;

            Interactables = GetComponent<Interactables>();
            BoneFollower = GetComponent<BoneFollower>();
        }

        protected virtual void OnDrawGizmos()
        {
            if(!Application.isPlaying || !IsEnabled) {
                return;
            }
            Gizmos.DrawCube(transform.position + _collider.center, _collider.size);
        }
#endregion

        public void Initialize(SkeletonRenderer skeletonRenderer)
        {
            BoneFollower.SkeletonRenderer = skeletonRenderer;
        }

        public virtual void EnableVolume(bool enable)
        {
            IsEnabled = enable;
        }

        public bool Intersects(Bounds attackBounds)
        {
            return IsEnabled && _collider.bounds.Intersects(attackBounds);
        }
    }
}
