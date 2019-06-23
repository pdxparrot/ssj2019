using JetBrains.Annotations;

using pdxpartyparrot.Game.Actors;
using pdxpartyparrot.ssj2019.Data;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Characters
{
    public sealed class AttackVolume : ActionVolume
    {
        [CanBeNull]
        private AttackData _attackData;

#region Unity Lifecycle
        protected override void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            base.OnDrawGizmos();
        }

        private void OnTriggerEnter(Collider other)
        {
            if(!Enabled || null == _attackData) {
                return;
            }

            IDamagable damagable = other.gameObject.GetComponent<IDamagable>();
            if(null == damagable) {
                return;
            }

            damagable.Damage(Owner, _attackData.DamageType, _attackData.DamageAmount);
        }
#endregion

        public void SetAttack([NotNull] AttackData attackData, Vector3 direction)
        {
            _attackData = attackData;

            Vector3 offset = _attackData.AttackVolumeOffset;
            offset.x *= Mathf.Sign(direction.x);

            Offset = offset;
            Size = _attackData.AttackVolumeSize;
        }
    }
}
