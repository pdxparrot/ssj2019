﻿using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Core.Effects;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Actors
{
    public sealed class TrainingDummyBehavior : ActorBehavior
    {
        [SerializeField]
        private EffectTrigger _damageEffect;

        public bool OnDamage(Actor source, string type, int amount, Bounds damageVolume, Vector3 force)
        {
            _damageEffect.Trigger();
            return true;
        }
    }
}
