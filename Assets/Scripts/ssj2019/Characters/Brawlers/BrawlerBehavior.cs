using System.Linq;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Core.Effects;
using pdxpartyparrot.Core.Effects.EffectTriggerComponents;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.Characters.BehaviorComponents;
using pdxpartyparrot.ssj2019.Data.Brawlers;
using pdxpartyparrot.ssj2019.Players.BehaviorComponents;
using pdxpartyparrot.ssj2019.Volumes;

using Spine;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Characters.Brawlers
{
    public interface IBrawlerBehaviorActions
    {
        Actor Owner { get; }

        Brawler Brawler { get; }

        CharacterBehaviorComponent.CharacterBehaviorAction NextAction { get; }

        bool IsDead { get; }

        bool IsImmune { get; set; }

        bool CanBlock { get; }

        Vector3 FacingDirection { get; }

        void PopNextAction();

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

#region Death Animations
        [SerializeField]
        private EffectTrigger _deathEffectTrigger;

        public EffectTrigger DeathEffectTrigger => _deathEffectTrigger;
#endregion

        [Space(10)]

#region Action Volumes
        [Header("Action Volumes")]

        [SerializeField]
        private AttackVolume _attackVolume;

        [SerializeField]
        private BlockVolume _blockVolume;
#endregion

        [Space(10)]

#region Attacks
        [SerializeField]
        [ReadOnly]
        private int _currentComboIndex;

        [SerializeField]
        [ReadOnly]
        private int _currentAttackIndex;

        [CanBeNull]
        public AttackData CurrentAttack => null == Brawler ? null : Brawler.BrawlerData.AttackComboData.ElementAt(_currentComboIndex).AttackData.ElementAt(_currentAttackIndex);
#endregion

        [SerializeField]
        private AttackBehaviorComponent _attackBehaviorComponent;

        [SerializeField]
        private BlockBehaviorComponent _blockBehaviorComponent;

        [Space(10)]

        [SerializeField]
        [ReadOnly]
        [CanBeNull]
        private IBrawlerBehaviorActions _actionHandler;

        [CanBeNull]
        private Brawler Brawler => null == _actionHandler ? null : _actionHandler.Brawler;

        public bool CanBlock => _actionHandler.CanBlock;

        [SerializeField]
        [ReadOnly]
        private bool _lastAttackHit;

#region Unity Lifecycle
        private void Awake()
        {
            _attackVolume.EnableVolume(false);
            _attackVolume.AttackHitEvent += AttackVolumeHitEventHandler;

            _blockVolume.EnableVolume(false);

            InitializeEffects();
        }

        private void OnDestroy()
        {
            ShutdownEffects();
        }
#endregion

        public void Initialize([NotNull] IBrawlerBehaviorActions actionHandler)
        {
            _actionHandler = actionHandler;

            _attackBehaviorComponent.Brawler = _actionHandler.Brawler;
            _blockBehaviorComponent.Brawler = _actionHandler.Brawler;
        }

        public void Attack()
        {
            // TODO: calling Initialize() here is dumb, but we can't do it in our own Initialize()
            // because the models haven't been initialized yet (and that NEEDS to get changed cuz this is dumb)
            _attackVolume.Initialize(Brawler.Model.SpineModel);
            _attackVolume.SetAttack(CurrentAttack, _actionHandler.FacingDirection);

            _attackAnimationEffectTriggerComponent.SpineAnimationName = CurrentAttack.AnimationName;

            Brawler.CurrentAction = new BrawlerAction(BrawlerAction.ActionType.Attack);
            _attackEffectTrigger.Trigger(() => {
                Brawler.CurrentAction = new BrawlerAction(BrawlerAction.ActionType.Idle);
                _actionHandler.OnIdle();
            });
        }

        private void Combo()
        {
            if(!(_actionHandler.NextAction is AttackBehaviorComponent.AttackAction attackAction)) {
                return;
            }

            if(AdvanceCombo()) {
                _actionHandler.PopNextAction();
                _actionHandler.OnAttack(attackAction);
            } else {
                ComboFail();

                CancelActions(true);
            }
        }

        private bool AdvanceCombo()
        {
            if(_currentAttackIndex >= Brawler.BrawlerData.AttackComboData.ElementAt(_currentComboIndex).AttackData.Count - 1) {
                return false;
            }

            _currentAttackIndex++;
            return true;
        }

        private void ComboFail()
        {
            _currentComboIndex = 0;
            _currentAttackIndex = 0;
        }

        public void ToggleBlock()
        {
            if(Brawler.CurrentAction.IsBlocking) {
                _blockEndEffectTrigger.Trigger(() => {
                    Brawler.CurrentAction = new BrawlerAction(BrawlerAction.ActionType.Idle);
                    _actionHandler.OnIdle();
                });
                return;
            }

            CancelActions(false);

            // TODO: calling Initialize() here is dumb, but we can't do it in our own Initialize()
            // because the models haven't been initialized yet (and that NEEDS to get changed cuz this is dumb)
            _blockVolume.Initialize(Brawler.Model.SpineModel);
            _blockVolume.SetBlock(Brawler.BrawlerData.BlockVolumeOffset, Brawler.BrawlerData.BlockVolumeSize, _actionHandler.FacingDirection, Brawler.BrawlerData.BlockBoneName);

            Brawler.CurrentAction = new BrawlerAction(BrawlerAction.ActionType.Block);
            _blockBeginEffectTrigger.Trigger();
        }

        public bool Damage(Actor source, string type, int amount, Bounds attackBounds, Vector3 force)
        {
            if(_actionHandler.IsDead || _actionHandler.IsImmune) {
                return false;
            }

            // did we block the damage?
            if(Brawler.CurrentAction.IsBlocking && _blockVolume.Intersects(attackBounds)) {
                if(BrawlerAction.ActionType.Parry == Brawler.CurrentAction.Type) {
                    Debug.Log($"TODO: Brawler {_actionHandler.Owner.Id} can parry");
                }

                // TODO: somehow we need to handle chip damage
                // but for now we'll just dump all of it

                Debug.Log($"Brawler {_actionHandler.Owner.Id} blocked damaged by {source.Id} for {amount}");

                _blockEffectTrigger.Trigger();
                _actionHandler.OnHit(true);
                return false;
            }

            Debug.Log($"Brawler {_actionHandler.Owner.Id} damaged by {source.Id} for {amount}");

            CancelActions(false);

            Brawler.Health -= amount;
            if(_actionHandler.IsDead) {
                _deathEffectTrigger.Trigger(() => _actionHandler.OnDeathComplete());
                _actionHandler.OnDead();
            } else {
                //_actionHandler.Owner.Behavior.Movement.AddImpulse(force * amount);

                _hitEffectTrigger.Trigger(() => {
                    Brawler.CurrentAction = new BrawlerAction(BrawlerAction.ActionType.Idle);
                    _actionHandler.OnIdle();
                });
                _actionHandler.OnHit(false);
            }

            return true;
        }

        public void CancelActions(bool force)
        {
            if(!force && !Brawler.CurrentAction.Cancellable) {
                return;
            }

            // if we're not inside a cancel window,
            // we need to make sure we clean up after
            // any animations that might be doing stuff

            // cancel blocks
            _blockVolume.EnableVolume(false);

            // cancel attacks
            _attackVolume.EnableVolume(false);

            // idle out
            Brawler.CurrentAction = new BrawlerAction(BrawlerAction.ActionType.Idle);
            _actionHandler.OnIdle();

            _actionHandler.OnCancelActions();
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

#region Event Handlers
        private void AttackVolumeHitEventHandler(object sender, AttackVolumeEventArgs args)
        {
            _lastAttackHit = true;
        }

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
            BrawlerAction action = Brawler.CurrentAction;

            if(Brawler.BrawlerData.AttackVolumeSpawnEvent == evt.Data.Name) {
                _lastAttackHit = false;

               _attackVolume.EnableVolume(true);
            } else if(Brawler.BrawlerData.AttackVolumeDeSpawnEvent == evt.Data.Name) {
                _attackVolume.EnableVolume(false);

                Combo();
            } else {
                Debug.LogWarning($"Unhandled attack event: {evt.Data.Name}");
            }

            Brawler.CurrentAction = action;
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
            BrawlerAction action = Brawler.CurrentAction;

            if(Brawler.BrawlerData.BlockVolumeSpawnEvent == evt.Data.Name) {
                _blockVolume.EnableVolume(true);
            } else if(Brawler.BrawlerData.ParryWindowOpenEvent == evt.Data.Name) {
                action.Type = BrawlerAction.ActionType.Parry;
            } else if(Brawler.BrawlerData.ParryWindowCloseEvent == evt.Data.Name) {
                action.Type = BrawlerAction.ActionType.Block;
            } else {
                Debug.LogWarning($"Unhandled block begin event: {evt.Data.Name}");
            }

            Brawler.CurrentAction = action;
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
            BrawlerAction action = Brawler.CurrentAction;

            if(Brawler.BrawlerData.BlockVolumeDeSpawnEvent == evt.Data.Name) {
                _blockVolume.EnableVolume(false);
            } else {
                Debug.LogWarning($"Unhandled block end event: {evt.Data.Name}");
            }

            Brawler.CurrentAction = action;
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
            BrawlerAction action = Brawler.CurrentAction;

            if(Brawler.BrawlerData.HitImpactEvent == evt.Data.Name) {
                // TODO: what do we do with this?
            } else if(Brawler.BrawlerData.HitStunEvent == evt.Data.Name) {
                action.IsStunned = true;
            } else if(Brawler.BrawlerData.HitImmunityEvent == evt.Data.Name) {
                action.IsImmune = true;
            } else {
                Debug.LogWarning($"Unhandled hit end event: {evt.Data.Name}");
            }

            Brawler.CurrentAction = action;
        }
#endregion
    }
}
