﻿using System;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.Actors;
using pdxpartyparrot.Game.Interactables;
using pdxpartyparrot.ssj2019.Data.Brawlers;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Volumes
{
    [RequireComponent(typeof(Interactables))]
    public sealed class AttackVolume : ActionVolume
    {
#region Events
        public event EventHandler<AttackVolumeEventArgs> AttackHitEvent;
#endregion

        [SerializeField]
        [ReadOnly]
        [CanBeNull]
        private AttackData _attackData;

        [SerializeField]
        [ReadOnly]
        private Vector3 _direction;

        private Interactables Interactables { get; set; }

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            Interactables = GetComponent<Interactables>();
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
            _direction = direction;

            Vector3 offset = _attackData.AttackVolumeOffset;
            offset.x *= Mathf.Sign(direction.x);

            Offset = offset;
            Size = _attackData.AttackVolumeSize;

            BoneFollower.SetBone(attackData.BoneName);
        }

        public override void EnableVolume(bool enable)
        {
            base.EnableVolume(enable);

            if(!IsEnabled || null == _attackData) {
                return;
            }

            foreach(IInteractable interactable in Interactables) {
                AttackInteractable(interactable);
            }
        }

#region Event Handlers
        private void InteractableAddedEventHandler(object sender, InteractableEventArgs args)
        {
            if(!IsEnabled || null == _attackData) {
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

            Actors.DamageData damageData = new Actors.DamageData{
                Source = Owner,
                AttackData = _attackData,
                Bounds = _collider.bounds,
                Direction = _direction,
            };

            if(damagable.Damage(damageData)) {
                AttackHitEvent?.Invoke(this, new AttackVolumeEventArgs{
                    HitTarget = damagable,
                });
            }
        }
    }
}
