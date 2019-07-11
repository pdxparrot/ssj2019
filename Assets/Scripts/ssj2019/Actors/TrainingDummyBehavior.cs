using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Core.Effects;
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

        public bool OnDamage(Actor source, string type, int amount, Bounds damageVolume, Vector3 force)
        {
            _floatingTextEffectTriggerComponent.Text = $"{amount}";

            _damageEffect.Trigger();
            return true;
        }
    }
}
