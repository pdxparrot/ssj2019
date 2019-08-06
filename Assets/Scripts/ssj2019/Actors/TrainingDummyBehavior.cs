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

        public bool OnDamage(Game.Actors.DamageData damageData)
        {
            DamageData dd = (DamageData)damageData;

            _floatingTextEffectTriggerComponent.Text = $"{dd.AttackData.DamageAmount}";

            _damageEffect.Trigger();
            return true;
        }
    }
}
