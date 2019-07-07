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
using UnityEngine.Assertions;

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

        // triggers when the brawler should combo
        void OnCombo(CharacterBehaviorComponent.CharacterBehaviorAction action);

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

#region Dash Animations
        [SerializeField]
        private EffectTrigger _dashEffectTrigger;
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
        [CanBeNull]
        private ComboData _currentComboMove;

        [CanBeNull]
        private AttackData CurrentAttack => null == _currentComboMove ? null : _currentComboMove.AttackData;
#endregion

        [SerializeField]
        private AttackBehaviorComponent _attackBehaviorComponent;

        [SerializeField]
        private BlockBehaviorComponent _blockBehaviorComponent;

        [SerializeField]
        private DashBehaviorComponent _dashBehaviorComponent;

        [Space(10)]

        [SerializeField]
        [ReadOnly]
        [CanBeNull]
        private IBrawlerBehaviorActions _actionHandler;

        [CanBeNull]
        private Actor Owner => null == _actionHandler ? null : _actionHandler.Owner;

        [CanBeNull]
        private Brawler Brawler => null == _actionHandler ? null : _actionHandler.Brawler;

        public bool CanBlock => _actionHandler.CanBlock;

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
            Assert.IsNotNull(actionHandler.Brawler);

            _actionHandler = actionHandler;

            _attackBehaviorComponent.Brawler = _actionHandler.Brawler;
            _blockBehaviorComponent.Brawler = _actionHandler.Brawler;
            _dashBehaviorComponent.Brawler = _actionHandler.Brawler;
        }

        public void Initialize()
        {
            Assert.IsNotNull(Brawler);
            Assert.IsNotNull(Brawler.BrawlerData);
            Assert.IsNotNull(Brawler.BrawlerData.ComboData);

            Brawler.BrawlerData.ComboData.Validate();
        }

        public void Attack(AttackBehaviorComponent.AttackAction attackAction)
        {
            if(!AdvanceCombo(attackAction)) {
                ComboFail();
                return;
            }

            if(GameManager.Instance.DebugBrawlers) {
                Debug.Log($"Brawler {Owner.Id} starting attack {CurrentAttack.Name}");
            }

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

        public void Dash(DashBehaviorComponent.DashAction dashAction)
        {
            if(!AdvanceCombo(dashAction)) {
                ComboFail();
                return;
            }

            if(GameManager.Instance.DebugBrawlers) {
                Debug.Log($"Brawler {Owner.Id} starting dash");
            }

            Debug.Log("TODO: dash");

            Brawler.CurrentAction = new BrawlerAction(BrawlerAction.ActionType.Dash);
            _dashEffectTrigger.Trigger(() => {
                Brawler.CurrentAction = new BrawlerAction(BrawlerAction.ActionType.Idle);
                _actionHandler.OnIdle();
            });
        }

#region Combos
        private bool AdvanceCombo(CharacterBehaviorComponent.CharacterBehaviorAction action)
        {
            if(null == _currentComboMove) {
                _currentComboMove = Brawler.BrawlerData.ComboData.NextMove(action);
                Assert.IsNotNull(_currentComboMove);
            } else {
                _currentComboMove = _currentComboMove.NextMove(action);
            }

            if(null == _currentComboMove) {
                return false;
            }

            if(GameManager.Instance.DebugBrawlers) {
                Debug.Log($"Brawler {Owner.Id} starting combo {_currentComboMove.Name}");
            }

            return true;
        }

        private void Combo()
        {
            CharacterBehaviorComponent.CharacterBehaviorAction action = _actionHandler.NextAction;
            if(!(action is AttackBehaviorComponent.AttackAction) && !(action is DashBehaviorComponent.DashAction)) {
                ComboFail();
                return;
            }

            _actionHandler.PopNextAction();

            _actionHandler.OnCombo(action);
        }

        private void ComboFail()
        {
            if(GameManager.Instance.DebugBrawlers) {
                Debug.Log($"Brawler {Owner.Id} combo failed / exhausted");
            }

            _currentComboMove = null;
        }
#endregion

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
                    Debug.Log($"TODO: Brawler {Owner.Id} can parry");
                }

                // TODO: somehow we need to handle chip damage
                // but for now we'll just dump all of it

                if(GameManager.Instance.DebugBrawlers) {
                    Debug.Log($"Brawler {Owner.Id} blocked damaged by {source.Id} for {amount}");
                }

                _blockEffectTrigger.Trigger();
                _actionHandler.OnHit(true);
                return false;
            }

            if(GameManager.Instance.DebugBrawlers) {
                Debug.Log($"Brawler {Owner.Id} damaged by {source.Id} for {amount}");
            }

            CancelActions(false);

            Brawler.Health -= amount;
            if(_actionHandler.IsDead) {
                _deathEffectTrigger.Trigger(() => _actionHandler.OnDeathComplete());
                _actionHandler.OnDead();
            } else {
                //Owner.Behavior.Movement.AddImpulse(force * amount);

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

#region Effects
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
#endregion

#region Spawn
        public void OnSpawn()
        {
            _currentComboMove = null;
        }

        public void OnReSpawn()
        {
            _currentComboMove = null;
        }
#endregion

#region Event Handlers
        private void AttackVolumeHitEventHandler(object sender, AttackVolumeEventArgs args)
        {
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
