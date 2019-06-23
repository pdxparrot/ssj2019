﻿using pdxpartyparrot.Core.Actors;
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

        BrawlerData BrawlerData { get; }

        Brawler Brawler { get; }

        CharacterBehaviorComponent.CharacterBehaviorAction LastAction { get; }

        bool IsDead { get; }

        bool IsImmune { get; set; }

        bool CanBlock { get; }

        // tells the brawler to go idle
        void OnIdle();

        // triggers when the brawler should attack
        void OnAttack();

        // triggers when the brawler is hit
        void OnHit(bool blocked);

        // triggers when the brawler is dead
        void OnDead();

        // triggers when the brawler death animation completes
        void OnDeathComplete();
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
            _attackVolume.gameObject.SetActive(false);
            _blockVolume.gameObject.SetActive(false);

            InitializeEffects();
        }

        private void OnDestroy()
        {
            ShutdownEffects();
        }

        private void Update()
        {
            // process actions here rather than Think() so that they're instantaneous
            if(_actionHandler.LastAction is AttackBehaviorComponent.AttackAction && !_attackEffectTrigger.IsRunning) {
                _actionHandler.OnAttack();
            }
        }
#endregion

        public void Initialize(IBrawlerBehaviorActions actionHandler)
        {
            _actionHandler = actionHandler;
        }

        public void Attack(AttackData attackData)
        {
            _attackVolume.AttackData = attackData;
            _attackEffectTrigger.Trigger(() => _actionHandler.OnIdle());
        }

        public void ToggleBlock()
        {
            if(_actionHandler.Brawler.IsBlocking) {
                _actionHandler.Brawler.IsBlocking = false;
                _blockEndEffectTrigger.Trigger(() => _actionHandler.OnIdle());
                return;
            }

            _actionHandler.Brawler.IsBlocking = true;
            _blockBeginEffectTrigger.Trigger();
        }

        public void Damage(Actor source, string type, int amount)
        {
            if(_actionHandler.IsDead) {
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
            _attackVolume.gameObject.SetActive(enable);
        }

        private void EnableBlockVolume(bool enable)
        {
            _blockVolume.gameObject.SetActive(enable);
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
            if(_actionHandler.BrawlerData.AttackVolumeSpawnEvent == evt.Data.Name) {
                EnableAttackVolume(true);
            } else if(_actionHandler.BrawlerData.AttackVolumeDeSpawnEvent == evt.Data.Name) {
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
            if(_actionHandler.BrawlerData.BlockVolumeSpawnEvent == evt.Data.Name) {
                EnableBlockVolume(true);
            } else if(_actionHandler.BrawlerData.ParryWindowOpenEvent == evt.Data.Name) {
                _actionHandler.Brawler.IsParry = true;
            } else if(_actionHandler.BrawlerData.ParryWindowCloseEvent == evt.Data.Name) {
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
            if(_actionHandler.BrawlerData.BlockVolumeDeSpawnEvent == evt.Data.Name) {
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
            if(_actionHandler.BrawlerData.HitImpactEvent == evt.Data.Name) {
                // TODO: damage
            } else {
                Debug.Log($"Unhandled hit end event: {evt.Data.Name}");
            }
        }
#endregion
    }
}
