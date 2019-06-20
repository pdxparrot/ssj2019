using UnityEngine;

namespace pdxpartyparrot.ssj2019.Characters
{
    [RequireComponent(typeof(Collider))]
    public sealed class BlockVolume : MonoBehaviour
    {
        private Collider _collider;

#region Unity Lifecycle
        private void Awake()
        {
            _collider = GetComponent<Collider>();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(_collider.bounds.center, _collider.bounds.size);
        }
#endregion
    }
}
