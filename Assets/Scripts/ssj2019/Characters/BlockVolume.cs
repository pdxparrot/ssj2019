using pdxpartyparrot.Core.Actors;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Characters
{
    [RequireComponent(typeof(Collider))]
    public sealed class BlockVolume : MonoBehaviour
    {
        [SerializeField]
        private Actor _owner;

        private Collider _collider;

#region Unity Lifecycle
        private void Awake()
        {
            _collider = GetComponent<Collider>();
            _collider.isTrigger = true;
        }

        private void OnDrawGizmos()
        {
            if(!Application.isPlaying) {
                return;
            }

            Gizmos.color = Color.blue;
            Gizmos.DrawCube(_collider.bounds.center, _collider.bounds.size);
        }
#endregion
    }
}
