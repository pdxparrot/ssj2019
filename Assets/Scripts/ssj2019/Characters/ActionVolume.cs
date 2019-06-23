using pdxpartyparrot.Core.Actors;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Characters
{
    [RequireComponent(typeof(BoxCollider))]
    public abstract class ActionVolume : MonoBehaviour
    {
        [SerializeField]
        private Actor _owner;

        protected Actor Owner => _owner;

        protected BoxCollider _collider;

        protected bool Enabled { get; private set; }

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

#region Unity Lifecycle
        protected virtual void Awake()
        {
            _collider = GetComponent<BoxCollider>();
            _collider.isTrigger = true;
        }

        protected virtual void OnDrawGizmos()
        {
            if(!Application.isPlaying || !Enabled) {
                return;
            }
            Gizmos.DrawCube(transform.position + _collider.center, _collider.size);
        }
#endregion

        public void EnableVolume(bool enable)
        {
            Enabled = enable;
        }
    }
}
