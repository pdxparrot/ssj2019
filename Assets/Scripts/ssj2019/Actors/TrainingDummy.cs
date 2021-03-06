﻿using pdxpartyparrot.Core.Actors;

using pdxpartyparrot.Game.Actors;
using pdxpartyparrot.Game.Interactables;

using UnityEngine.Assertions;

namespace pdxpartyparrot.ssj2019.Actors
{
    public sealed class TrainingDummy : Actor3D, IDamagable, IInteractable
    {
        private TrainingDummyBehavior TrainingDummyBehavior => (TrainingDummyBehavior)Behavior;

        public override bool IsLocalActor => false;

        public bool CanInteract => true;

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            Assert.IsTrue(Behavior is TrainingDummyBehavior);
        }
#endregion

        public bool Damage(Game.Actors.DamageData damageData)
        {
            return TrainingDummyBehavior.OnDamage(damageData);
        }
    }
}
