using JetBrains.Annotations;

using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Core.Effects;
using pdxpartyparrot.Core.Effects.EffectTriggerComponents;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.Characters.BehaviorComponents;
using pdxpartyparrot.Game.Effects.EffectTriggerComponents;
using pdxpartyparrot.ssj2019.Data.Brawlers;
using pdxpartyparrot.ssj2019.Characters.BehaviorComponents;
using pdxpartyparrot.ssj2019.Players.BehaviorComponents;
using pdxpartyparrot.ssj2019.Volumes;

using Spine;

using UnityEngine;
using UnityEngine.Assertions;

using DashBehaviorComponent = pdxpartyparrot.ssj2019.Characters.BehaviorComponents.DashBehaviorComponent;

namespace pdxpartyparrot.ssj2019.Characters.Brawlers
{
    // TODO: split this into some more useful core interfaces
    // eg - action handling, state checking, etc
    public interface IBrawlerBehaviorActions
    {
        Actor Owner { get; }

        Brawler Brawler { get; }

        CharacterBehaviorComponent.CharacterBehaviorAction NextAction { get; }

        bool IsGrounded { get; }

        bool IsDead { get; }

        bool IsImmune { get; set; }

        void ActionPerformed(CharacterBehaviorComponent.CharacterBehaviorAction action);

        void BufferAction(CharacterBehaviorComponent.CharacterBehaviorAction action);

        void PopNextAction();

        void ClearActionBuffer();

        // tells the brawler to go idle
        void OnIdle();

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
        private SpineAnimationEffectTriggerComponent _attackAnimationEffectTriggerComponent;
#endregion

#region Block Animations
        [SerializeField]
        private SpineAnimationEffectTriggerComponent _blockBeginAnimationEffectTriggerComponent;

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

        [SerializeField]
        private EffectTrigger _comboFailEffectTrigger;

        [Space(10)]

#region Action Volumes
        [Header("Action Volumes")]

        [SerializeField]
        private AttackVolume _attackVolume;

        [SerializeField]
        private BlockVolume _blockVolume;
#endregion

        [Space(10)]

#region Current Combo / Attack
        [SerializeField]
        [ReadOnly]
        [CanBeNull]
        private BrawlerCombo.IComboEntry _currentComboEntry;

        [CanBeNull]
        public AttackData CurrentAttack => null == _currentComboEntry ? null : _currentComboEntry.Move.AttackData;
#endregion

        [Space(10)]

        [SerializeField]
        private JumpBehaviorComponent _jumpBehaviorComponent;

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
        public Actor Owner => null == _actionHandler ? null : _actionHandler.Owner;

        [CanBeNull]
        public Brawler Brawler => null == _actionHandler ? null : _actionHandler.Brawler;

        private bool CanJump => !_actionHandler.IsDead && Brawler.CurrentAction.Cancellable;

        private bool CanAttack => !_actionHandler.IsDead && Brawler.CurrentAction.Cancellable;

        public bool CanBlock => !_actionHandler.IsDead && _actionHandler.IsGrounded && Brawler.CurrentAction.Cancellable;

        private bool CanDash => !_actionHandler.IsDead && Brawler.CurrentAction.Cancellable;

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
        }

        public void Initialize()
        {
            Assert.IsNotNull(Brawler);
            Assert.IsNotNull(Brawler.BrawlerData);

            _jumpBehaviorComponent.JumpBehaviorComponentData = Brawler.BrawlerData.JumpBehaviorComponentData;
            _dashBehaviorComponent.DashBehaviorComponentData = Brawler.BrawlerData.DashBehaviorComponentData;
        }

#region Actions
        public void Jump()
        {
            if(!CanJump) {
                return;
            }

            CancelActions(false);

            _actionHandler.ActionPerformed(JumpBehaviorComponent.JumpAction.Default);
        }

        // TODO: we might want the entire move buffer
        public void Attack(Vector3 lastMove, bool isGrounded)
        {
            if(!CanAttack) {
                return;
            }

            if(Brawler.CurrentAction.CanQueue) {
                _actionHandler.BufferAction(new AttackBehaviorComponent.AttackAction(this) {
                    Axes = lastMove,
                    IsGrounded = isGrounded,
                });
            } else {
                _actionHandler.ActionPerformed(new AttackBehaviorComponent.AttackAction(this) {
                    Axes = lastMove,
                    IsGrounded = isGrounded,
                });
            }
        }

        // TODO: does this really need the move input?
        public void StartBlock(Vector3 lastMove)
        {
            _actionHandler.ActionPerformed(new BlockBehaviorComponent.BlockAction{
                Axes = lastMove,
                Cancel = false,
            });
        }

        public void EndBlock()
        {
            _actionHandler.ActionPerformed(new BlockBehaviorComponent.BlockAction{
                Cancel = true,
            });
        }

        public void Dash()
        {
            if(!CanDash || !_dashBehaviorComponent.CanDash) {
                return;
            }

            if(Brawler.CurrentAction.CanQueue) {
                _actionHandler.BufferAction(Game.Characters.BehaviorComponents.DashBehaviorComponent.DashAction.Default);
            } else {
                _actionHandler.ActionPerformed(Game.Characters.BehaviorComponents.DashBehaviorComponent.DashAction.Default);
            }
        }

        public void Idle()
        {
            Brawler.CurrentAction = new BrawlerAction(BrawlerAction.ActionType.Idle);

            _actionHandler.OnIdle();
        }
#endregion

#region Combos
        public bool AdvanceCombo(CharacterBehaviorComponent.CharacterBehaviorAction action)
        {
            if(null == _currentComboEntry) {
                _currentComboEntry = Brawler.BrawlerCombo.RootComboEntry.NextEntry(action, true);
                if(null == _currentComboEntry) {
                    Debug.LogWarning($"Unable to find combo opener for action {action}");
                }
            } else {
                _currentComboEntry = _currentComboEntry.NextEntry(action, false);
            }

            if(null == _currentComboEntry) {
                return false;
            }

            if(GameManager.Instance.DebugBrawlers) {
                Debug.Log($"Brawler {Owner.Id} advancing combo to {_currentComboEntry.Move.Id}");
            }

            return true;
        }

        private void Combo()
        {
            CharacterBehaviorComponent.CharacterBehaviorAction action = _actionHandler.NextAction;
            if(!(action is AttackBehaviorComponent.AttackAction) && !(action is Game.Characters.BehaviorComponents.DashBehaviorComponent.DashAction)) {
                ComboFail();
                return;
            }

            _actionHandler.PopNextAction();
            _actionHandler.ActionPerformed(action);
        }

        public void ComboFail()
        {
            if(GameManager.Instance.DebugBrawlers) {
                Debug.Log($"Brawler {Owner.Id} combo failed / exhausted");
            }

            _currentComboEntry = null;

            _comboFailEffectTrigger.Trigger();
        }
#endregion

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

                _actionHandler.ClearActionBuffer();
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
                Owner.Behavior.Movement.AddImpulse(force * amount);

                _hitEffectTrigger.Trigger(() => {
                    Idle();
                });

                _actionHandler.ClearActionBuffer();
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
            Idle();
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
            _currentComboEntry = null;
        }

        public void OnReSpawn()
        {
            _currentComboEntry = null;
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
