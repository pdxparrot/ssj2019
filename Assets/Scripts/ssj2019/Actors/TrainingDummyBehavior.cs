using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Core.Effects;
using pdxpartyparrot.Game.Actors;
using pdxpartyparrot.Game.Effects.EffectTriggerComponents;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Actors
{
    public sealed class TrainingDummyBehavior : ActorBehavior
    {
        [SerializeField]
        private EffectTrigger _damageEffect;

        [SerializeField]
        private FloatingTextEffectTriggerComponent _floatingTextEffectTriggerComponent;

        public bool OnDamage(DamageData damageData)
        {
            _floatingTextEffectTriggerComponent.Text = $"{damageData.type}: {damageData.amount}";

            _damageEffect.Trigger();
            return true;
        }
    }
}
