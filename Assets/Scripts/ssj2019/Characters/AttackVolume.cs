using JetBrains.Annotations;

using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Game.Actors;
using pdxpartyparrot.ssj2019.Data;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Characters
{
    [RequireComponent(typeof(Collider))]
    public sealed class AttackVolume : MonoBehaviour
    {
        [SerializeField]
        private Actor _owner;

        private Collider _collider;

        [CanBeNull]
        public AttackData AttackData { get; set; }

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
            if(null == AttackData) {
                return;
            }

            IDamagable damagable = other.gameObject.GetComponent<IDamagable>();
            if(null == damagable) {
                return;
            }

            damagable.Damage(_owner, AttackData.DamageType, AttackData.DamageAmount);
        }
#endregion
    }
}
