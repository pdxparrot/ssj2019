using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Game.Actors;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Characters
{
    [RequireComponent(typeof(Collider))]
    public sealed class AttackVolume : MonoBehaviour
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

            Gizmos.color = Color.red;
            Gizmos.DrawCube(_collider.bounds.center, _collider.bounds.size);
        }

        private void OnTriggerEnter(Collider other)
        {
            IDamagable damagable = other.gameObject.GetComponent<IDamagable>();
            if(null == damagable) {
                return;
            }

            damagable.Damage(_owner);
        }
#endregion
    }
}
