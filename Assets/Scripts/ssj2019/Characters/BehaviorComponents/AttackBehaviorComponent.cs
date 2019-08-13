using pdxpartyparrot.Core.Effects;
using pdxpartyparrot.Core.Effects.EffectTriggerComponents;
using pdxpartyparrot.Core.Math;
using pdxpartyparrot.Game.Effects.EffectTriggerComponents;
using pdxpartyparrot.ssj2019.Characters.Brawlers;
using pdxpartyparrot.ssj2019.Data.Brawlers;
using pdxpartyparrot.ssj2019.Volumes;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Players.BehaviorComponents
{
    public sealed class AttackBehaviorComponent : GameCharacterBehaviorComponent
    {
#region Actions
        public class AttackAction : CharacterBehaviorAction
        {
            public Vector3 Axes { get; set; }

            public bool IsGrounded { get; set; }

            public AttackData.Direction Direction
            {
                get
                {
                    if(Axes.sqrMagnitude < MathUtil.Epsilon) {
                        return AttackData.Direction.None;
                    }

                    AttackData.Direction direction = AttackData.DirectionFromAxes(Axes);

                    // flip the forward / backward if we're turned around
                    switch(direction)
                    {
                    case AttackData.Direction.Forward when _owner.Owner.FacingDirection.x < 0.0f:
                        return AttackData.Direction.Backward;
                    case AttackData.Direction.Backward when _owner.Owner.FacingDirection.x < 0.0f:
                        return AttackData.Direction.Forward;
                    default:
                        return direction;
                    }
                }
            }

            private BrawlerBehavior _owner;

            public AttackAction(BrawlerBehavior owner)
            {
                _owner = owner;
            }

            public override string ToString()
            {
                return $"AttackAction(Axes: {Axes})";
            }
        }
#endregion

        [SerializeField]
        private BrawlerBehavior _brawlerBehavior;

        [SerializeField]
        private AttackVolume _attackVolume;

        [SerializeField]
        private EffectTrigger _attackEffectTrigger;

        [SerializeField]
        private SpineAnimationEffectTriggerComponent _attackAnimationEffectTriggerComponent;

        [SerializeField]
        private AudioEffectTriggerComponent _attackAudioEffectTriggerComponent;

        [SerializeField]
        private FloatingTextEffectTriggerComponent _attackFloatingTextEffectTriggerComponent;

        public override bool OnPerformed(CharacterBehaviorAction action)
        {
            if(!(action is AttackAction attackAction)) {
                return false;
            }

            if(!_brawlerBehavior.AdvanceCombo(attackAction)) {
                _brawlerBehavior.ComboFail();
                return true;
            }

            if(GameManager.Instance.DebugBrawlers) {
                Debug.Log($"Brawler {Behavior.Owner.Id} starting attack {_brawlerBehavior.CurrentAttack.Name}");
            }

            // TODO: calling Initialize() here is dumb, but we can't do it in our own Initialize()
            // because the models haven't been initialized yet (and that NEEDS to get changed cuz this is dumb)
            _attackVolume.Initialize(_brawlerBehavior.Brawler.Model.SpineModel);
            _attackVolume.SetAttack(_brawlerBehavior.CurrentAttack, Behavior.Owner.FacingDirection);

            _attackAnimationEffectTriggerComponent.SpineAnimationName = _brawlerBehavior.CurrentAttack.AnimationName;
            _attackAudioEffectTriggerComponent.AudioClip = _brawlerBehavior.CurrentAttack.AttackAudioClip;
            _attackFloatingTextEffectTriggerComponent.Text = $"{_brawlerBehavior.CurrentAttack.Name}";

            _brawlerBehavior.Brawler.CurrentAction = new BrawlerAction(BrawlerAction.ActionType.Attack);
            _attackEffectTrigger.Trigger(() => {
                _brawlerBehavior.Idle();
            });

            return true;
        }
    }
}
