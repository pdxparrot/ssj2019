using System;

using JetBrains.Annotations;

using pdxpartyparrot.Game.Actors;
using pdxpartyparrot.Game.Interactables;
using pdxpartyparrot.ssj2019.Data;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Characters
{
    public sealed class AttackVolume : ActionVolume
    {
#region Events
        public event EventHandler<AttackVolumeEventArgs> AttackHitEvent;
#endregion

        [CanBeNull]
        private AttackData _attackData;

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            Interactables.InteractableAddedEvent += InteractableAddedEventHandler;
        }

        protected override void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            base.OnDrawGizmos();
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

        public override void EnableVolume(bool enable)
        {
            base.EnableVolume(enable);

            if(!Enabled || null == _attackData) {
                return;
            }

            foreach(IInteractable interactable in Interactables) {
                AttackInteractable(interactable);
            }
        }

#region Event Handlers
        private void InteractableAddedEventHandler(object sender, InteractableEventArgs args)
        {
            if(!Enabled || null == _attackData) {
                return;
            }

            AttackInteractable(args.Interactable);
        }
#endregion

        private void AttackInteractable(IInteractable interactable)
        {
            IDamagable damagable = interactable.gameObject.GetComponent<IDamagable>();
            if(null == damagable) {
                return;
            }

            damagable.Damage(Owner, _attackData.DamageType, _attackData.DamageAmount);

            AttackHitEvent?.Invoke(this, new AttackVolumeEventArgs{
                HitTarget = damagable,
            });
        }
    }
}
