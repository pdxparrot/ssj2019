using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Core.Effects;
using pdxpartyparrot.Core.Effects.EffectTriggerComponents;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.Characters.BehaviorComponents;
using pdxpartyparrot.ssj2019.Data;
using pdxpartyparrot.ssj2019.Players.BehaviorComponents;

using Spine;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Characters
{
    public interface IBrawlerBehaviorActions
    {
        Actor Owner { get; }

        Brawler Brawler { get; }

        CharacterBehaviorComponent.CharacterBehaviorAction LastAction { get; }

        bool IsDead { get; }

        bool IsImmune { get; set; }

        bool CanBlock { get; }

        Vector3 FacingDirection { get; }

        AttackData CurrentAttack { get; }

        // tells the brawler to go idle
        void OnIdle();

        // triggers when the brawler should attack
        void OnAttack(AttackBehaviorComponent.AttackAction action);

        // triggers when the brawler is hit
        void OnHit(bool blocked);

        // triggers when the brawler is dead
        void OnDead();

        // triggers when the brawler death animation completes
        void OnDeathComplete();

        void OnCancelActions();
    }

    public sealed class BrawlerBehavior : MonoBehaviour
    {
        [Header("Animations")]

#region Attack Animations
        [SerializeField]
        private EffectTrigger _attackEffectTrigger;

        [SerializeField]
        private SpineAnimationEffectTriggerComponent _attackAnimationEffectTriggerComponent;
#endregion

#region Block Animations
        [SerializeField]
        private EffectTrigger _blockBeginEffectTrigger;

        [SerializeField]
        private SpineAnimationEffectTriggerComponent _blockBeginAnimationEffectTriggerComponent;

        [SerializeField]
        private EffectTrigger _blockEndEffectTrigger;

        [SerializeField]
        private SpineAnimationEffectTriggerComponent _blockEndAnimationEffectTriggerComponent;

        [SerializeField]
        private EffectTrigger _blockEffectTrigger;

        public EffectTrigger BlockEffectTrigger => _blockEffectTrigger;
#endregion

#region Hit Animations
        [SerializeField]
        private EffectTrigger _hitEffectTrigger;

        public EffectTrigger HitEffectTrigger => _hitEffectTrigger;

        [SerializeField]
        private SpineAnimationEffectTriggerComponent _hitAnimationEffectTriggerComponent;
#endregion

        [SerializeField]
        private EffectTrigger _deathEffectTrigger;

        public EffectTrigger DeathEffectTrigger => _deathEffectTrigger;

        [Space(10)]

#region Action Volumes
        [Header("Action Volumes")]

        [SerializeField]
        private AttackVolume _attackVolume;

        [SerializeField]
        private BlockVolume _blockVolume;
#endregion

        [Space(10)]

        [SerializeField]
        [ReadOnly]
        private IBrawlerBehaviorActions _actionHandler;

        public bool CanBlock => _actionHandler.CanBlock;

#region Unity Lifecycle
        private void Awake()
        {
            EnableAttackVolume(false);
            EnableBlockVolume(false);

            InitializeEffects();
        }

        private void OnDestroy()
        {
            ShutdownEffects();
        }

        private void Update()
        {
            // pump the action buffer
            if(_actionHandler.LastAction is AttackBehaviorComponent.AttackAction attackAction && !_attackEffectTrigger.IsRunning) {
                _actionHandler.OnAttack(attackAction);
            }
        }
#endregion

        public void Initialize(IBrawlerBehaviorActions actionHandler)
        {
            _actionHandler = actionHandler;
        }

        public void Attack()
        {
            _attackVolume.SetAttack(_actionHandler.CurrentAttack, _actionHandler.FacingDirection);

            _attackEffectTrigger.Trigger(() => _actionHandler.OnIdle());
        }

        public void ToggleBlock()
        {
            if(_actionHandler.Brawler.IsBlocking) {
                _actionHandler.Brawler.IsBlocking = false;
                _blockEndEffectTrigger.Trigger(() => _actionHandler.OnIdle());
                return;
            }

            CancelActions();

            _blockVolume.SetBlock(_actionHandler.Brawler.BrawlerData.BlockVolumeOffset, _actionHandler.Brawler.BrawlerData.BlockVolumeSize, _actionHandler.FacingDirection);

            _actionHandler.Brawler.IsBlocking = true;
            _blockBeginEffectTrigger.Trigger();
        }

        public void Damage(Actor source, string type, int amount)
        {
            if(_actionHandler.IsDead || _actionHandler.IsImmune) {
                return;
            }

            CancelActions();

            // TODO: parry

            // TODO: somehow we need to handle chip damage
            // but for now we'll just dump all of it
            // TODO: also blocking is all about the block volume
            /*if(_actionHandler.IsBlocking) {
                Debug.Log($"Brawler {_actionHandler.Owner.Id} blocked damaged by {source.Id} for {amount}");
                _blockEffectTrigger.Trigger();
                _actionHandler.OnHit(true);
                return;
            }*/

            Debug.Log($"Brawler {_actionHandler.Owner.Id} damaged by {source.Id} for {amount}");

            _actionHandler.Brawler.Health -= amount;
            if(_actionHandler.IsDead) {
                _deathEffectTrigger.Trigger(() => _actionHandler.OnDeathComplete());
                _actionHandler.OnDead();
            } else {
                _hitEffectTrigger.Trigger(() => _actionHandler.OnIdle());
                _actionHandler.OnHit(false);
            }
        }

        private void CancelActions()
        {
            if(!_actionHandler.Brawler.CanCancel) {
                return;
            }

            // if we're not inside a cancel window,
            // we need to make sure we clean up after
            // any animations that might be doing stuff

            // cancel blocks
            EnableBlockVolume(false);
            _actionHandler.Brawler.IsParry = false;
            _actionHandler.Brawler.IsBlocking = false;

            // cancel attacks
            EnableAttackVolume(false);

            _actionHandler.OnCancelActions();

            // idle out
            _actionHandler.OnIdle();
        }

        private void InitializeEffects()
        {
            _attackAnimationEffectTriggerComponent.StartEvent += AttackAnimationStartHandler;
            _attackAnimationEffectTriggerComponent.CompleteEvent += AttackAnimationCompleteHandler;

            _blockBeginAnimationEffectTriggerComponent.StartEvent += BlockBeginAnimationStartHandler;
            _blockBeginAnimationEffectTriggerComponent.CompleteEvent += BlockBeginAnimationCompleteHandler;

            _blockEndAnimationEffectTriggerComponent.StartEvent += BlockEndAnimationStartHandler;
            _blockEndAnimationEffectTriggerComponent.CompleteEvent += BlockEndAnimationCompleteHandler;

            _hitAnimationEffectTriggerComponent.StartEvent += HitAnimationStartHandler;
            _hitAnimationEffectTriggerComponent.CompleteEvent += HitAnimationCompleteHandler;
        }

        private void ShutdownEffects()
        {
            _attackAnimationEffectTriggerComponent.StartEvent -= AttackAnimationStartHandler;
            _attackAnimationEffectTriggerComponent.CompleteEvent -= AttackAnimationCompleteHandler;

            _blockBeginAnimationEffectTriggerComponent.StartEvent -= BlockBeginAnimationStartHandler;
            _blockBeginAnimationEffectTriggerComponent.CompleteEvent -= BlockBeginAnimationCompleteHandler;

            _blockEndAnimationEffectTriggerComponent.StartEvent -= BlockEndAnimationStartHandler;
            _blockEndAnimationEffectTriggerComponent.CompleteEvent -= BlockEndAnimationCompleteHandler;

            _hitAnimationEffectTriggerComponent.StartEvent -= HitAnimationStartHandler;
            _hitAnimationEffectTriggerComponent.CompleteEvent -= HitAnimationCompleteHandler;
        }

        private void EnableAttackVolume(bool enable)
        {
            _attackVolume.EnableVolume(enable);
        }

        private void EnableBlockVolume(bool enable)
        {
            _blockVolume.EnableVolume(enable);
        }

#region Event Handlers
        private void AttackAnimationStartHandler(object sender, SpineAnimationEffectTriggerComponent.EventArgs args)
        {
            args.TrackEntry.Event += AttackAnimationEvent;
        }

        private void AttackAnimationCompleteHandler(object sender, SpineAnimationEffectTriggerComponent.EventArgs args)
        {
            args.TrackEntry.Event -= AttackAnimationEvent;
        }

        private void AttackAnimationEvent(TrackEntry trackEntry, Spine.Event evt)
        {
            if(_actionHandler.Brawler.BrawlerData.AttackVolumeSpawnEvent == evt.Data.Name) {
                EnableAttackVolume(true);
            } else if(_actionHandler.Brawler.BrawlerData.AttackVolumeDeSpawnEvent == evt.Data.Name) {
                EnableAttackVolume(false);
            } else {
                Debug.LogWarning($"Unhandled attack event: {evt.Data.Name}");
            }
        }

        private void BlockBeginAnimationStartHandler(object sender, SpineAnimationEffectTriggerComponent.EventArgs args)
        {
            args.TrackEntry.Event += BlockBeginAnimationEvent;
        }

        private void BlockBeginAnimationCompleteHandler(object sender, SpineAnimationEffectTriggerComponent.EventArgs args)
        {
            args.TrackEntry.Event -= BlockBeginAnimationEvent;
        }

        private void BlockBeginAnimationEvent(TrackEntry trackEntry, Spine.Event evt)
        {
            if(_actionHandler.Brawler.BrawlerData.BlockVolumeSpawnEvent == evt.Data.Name) {
                EnableBlockVolume(true);
            } else if(_actionHandler.Brawler.BrawlerData.ParryWindowOpenEvent == evt.Data.Name) {
                _actionHandler.Brawler.IsParry = true;
            } else if(_actionHandler.Brawler.BrawlerData.ParryWindowCloseEvent == evt.Data.Name) {
                _actionHandler.Brawler.IsParry = false;
            } else {
                Debug.Log($"Unhandled block begin event: {evt.Data.Name}");
            }
        }

        private void BlockEndAnimationStartHandler(object sender, SpineAnimationEffectTriggerComponent.EventArgs args)
        {
            args.TrackEntry.Event += BlockEndAnimationEvent;
        }

        private void BlockEndAnimationCompleteHandler(object sender, SpineAnimationEffectTriggerComponent.EventArgs args)
        {
            args.TrackEntry.Event -= BlockEndAnimationEvent;
        }

        private void BlockEndAnimationEvent(TrackEntry trackEntry, Spine.Event evt)
        {
            if(_actionHandler.Brawler.BrawlerData.BlockVolumeDeSpawnEvent == evt.Data.Name) {
                EnableBlockVolume(false);
            } else {
                Debug.Log($"Unhandled block end event: {evt.Data.Name}");
            }
        }

        private void HitAnimationStartHandler(object sender, SpineAnimationEffectTriggerComponent.EventArgs args)
        {
            args.TrackEntry.Event += HitAnimationEvent;
        }

        private void HitAnimationCompleteHandler(object sender, SpineAnimationEffectTriggerComponent.EventArgs args)
        {
            args.TrackEntry.Event -= HitAnimationEvent;
        }

        private void HitAnimationEvent(TrackEntry trackEntry, Spine.Event evt)
        {
            if(_actionHandler.Brawler.BrawlerData.HitImpactEvent == evt.Data.Name) {
                // TODO: damage
            } else {
                Debug.Log($"Unhandled hit end event: {evt.Data.Name}");
            }
        }
#endregion
    }
}
