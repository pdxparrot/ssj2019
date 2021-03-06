using System;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Core.Effects;
using pdxpartyparrot.Core.Effects.EffectTriggerComponents;
using pdxpartyparrot.Core.Time;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.Characters.BehaviorComponents;
using pdxpartyparrot.Game.UI;
using pdxpartyparrot.ssj2019.Data.Brawlers;
using pdxpartyparrot.ssj2019.Players.BehaviorComponents;
using pdxpartyparrot.ssj2019.Volumes;

using Spine;

using UnityEngine;

using DashBehaviorComponent = pdxpartyparrot.ssj2019.Characters.BehaviorComponents.DashBehaviorComponent;

namespace pdxpartyparrot.ssj2019.Characters.Brawlers
{
    // TODO: split this into some more useful core interfaces
    // eg - action handling, state checking, etc
    public interface IBrawlerBehaviorActions
    {
        Brawler Brawler { get; }

        CharacterBehaviorComponent.CharacterBehaviorAction NextAction { get; }

        bool CanApplyAttackForce { get; }

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

        // triggers when the brawler performs a successful combo move
        void OnComboMove(bool isOpener, ComboMove move, BrawlerAction currentAction);

        // triggers when the brawler is dead
        void OnDead();

        // triggers when the brawler death animation completes
        void OnDeathComplete();
    }

    public sealed class BrawlerBehavior : MonoBehaviour
    {
        public Brawler Brawler { get; private set; }

        [Header("Animations")]

#region Attack Effects
        [SerializeField]
        private SpineAnimationEffectTriggerComponent _attackAnimationEffectTriggerComponent;
#endregion

#region Block Effects
        [SerializeField]
        private SpineAnimationEffectTriggerComponent _blockBeginAnimationEffectTriggerComponent;

        [SerializeField]
        private SpineAnimationEffectTriggerComponent _blockEndAnimationEffectTriggerComponent;

        [SerializeField]
        private EffectTrigger _blockEffectTrigger;

        public EffectTrigger BlockEffectTrigger => _blockEffectTrigger;
#endregion

#region Hit Effects
        [SerializeField]
        private EffectTrigger _hitEffectTrigger;

        public EffectTrigger HitEffectTrigger => _hitEffectTrigger;

        [SerializeField]
        private SpineAnimationEffectTriggerComponent _hitAnimationEffectTriggerComponent;

        [SerializeField]
        private AudioEffectTriggerComponent _hitAudioEffectTriggerComponent;
#endregion

#region Knock Down
        [SerializeField]
        private EffectTrigger _knockDownEffectTrigger;

        private ITimer _knockDownTimer;
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
        [ReadOnly]
        private Vector3 _velocityModifier;

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

#region Debug
        [Header("Debug")]

        [SerializeField]
        [CanBeNull]
        private Transform _debugTextTarget;

        [SerializeField]
        [CanBeNull]
        private FloatingTextQueue _debugTextQueue;
#endregion

        public IBrawlerBehaviorActions ActionHandler { get; set; }

        public Actor Owner => Brawler.Actor;

        private bool CanJump => !ActionHandler.IsDead && Brawler.CurrentAction.Cancellable;

        private bool CanAttack => !ActionHandler.IsDead && Brawler.CurrentAction.Cancellable;

        public bool CanBlock => !ActionHandler.IsDead && ActionHandler.IsGrounded && Brawler.CurrentAction.Cancellable;

        private bool CanDash => !ActionHandler.IsDead && Brawler.CurrentAction.Cancellable;

#region Unity Lifecycle
        private void Awake()
        {
            _knockDownTimer = TimeManager.Instance.AddTimer();
            _knockDownTimer.TimesUpEvent += KnockDownTimerTimesUpEventHandler;

            _attackVolume.EnableVolume(false);
            _attackVolume.AttackHitEvent += AttackVolumeHitEventHandler;

            _blockVolume.EnableVolume(false);

            InitializeEffects();
        }

        private void OnDestroy()
        {
            ShutdownEffects();

            if(TimeManager.HasInstance) {
                TimeManager.Instance.RemoveTimer(_knockDownTimer);
            }
            _knockDownTimer = null;
        }
#endregion

        public void Initialize(Brawler brawler)
        {
            Brawler = brawler;

            _jumpBehaviorComponent.JumpBehaviorComponentData = Brawler.BrawlerData.JumpBehaviorComponentData;
            _dashBehaviorComponent.DashBehaviorComponentData = Brawler.BrawlerData.DashBehaviorComponentData;
        }

        public void PhysicsUpdate(float dt)
        {
            if(ActionHandler.CanApplyAttackForce) {
                Owner.Behavior.Movement.Move(_velocityModifier * dt);
            }
        }

#region Actions
        public void Jump()
        {
            if(!CanJump) {
                return;
            }

            CancelActions(false);

            ActionHandler.ActionPerformed(JumpBehaviorComponent.JumpAction.Default);
        }

        // TODO: we might want the entire move buffer
        public void Attack(Vector3 lastMove, bool isGrounded)
        {
            if(!CanAttack) {
                return;
            }

            if(Brawler.CurrentAction.CanQueue) {
                ActionHandler.BufferAction(new AttackBehaviorComponent.AttackAction(this) {
                    Axes = lastMove,
                    IsGrounded = isGrounded,
                });
            } else {
                ActionHandler.ActionPerformed(new AttackBehaviorComponent.AttackAction(this) {
                    Axes = lastMove,
                    IsGrounded = isGrounded,
                });
            }
        }

        // TODO: does this really need the move input?
        public void StartBlock(Vector3 lastMove)
        {
            ActionHandler.ActionPerformed(new BlockBehaviorComponent.BlockAction{
                Axes = lastMove,
                Cancel = false,
            });
        }

        public void EndBlock()
        {
            ActionHandler.ActionPerformed(new BlockBehaviorComponent.BlockAction{
                Cancel = true,
            });
        }

        public void Dash()
        {
            if(!CanDash || !_dashBehaviorComponent.CanDash) {
                return;
            }

            if(Brawler.CurrentAction.CanQueue) {
                ActionHandler.BufferAction(Game.Characters.BehaviorComponents.DashBehaviorComponent.DashAction.Default);
            } else {
                ActionHandler.ActionPerformed(Game.Characters.BehaviorComponents.DashBehaviorComponent.DashAction.Default);
            }
        }

        public void Idle()
        {
            Brawler.CurrentAction = new BrawlerAction(BrawlerAction.ActionType.Idle);

            _currentComboEntry = null;
            _velocityModifier = Vector3.zero;

            ActionHandler.OnIdle();
        }
#endregion

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

#region Combos
        public bool AdvanceCombo(CharacterBehaviorComponent.CharacterBehaviorAction action)
        {
            bool isOpener = null == _currentComboEntry;

            if(isOpener) {
                _currentComboEntry = Brawler.BrawlerCombo.RootComboEntry.NextEntry(action, null, Brawler.CurrentAction);
                if(null == _currentComboEntry) {
                    Debug.LogWarning($"Unable to find combo opener for action {action}");
                }
            } else {
                _currentComboEntry = _currentComboEntry.NextEntry(action, _currentComboEntry, Brawler.CurrentAction);
            }

            if(null == _currentComboEntry) {
                _currentComboEntry = FudgeFailedCombo(action);
                if(null == _currentComboEntry) {
                    return false;
                }
            }

            ActionHandler.OnComboMove(isOpener, _currentComboEntry.Move, Brawler.CurrentAction);

            if(GameManager.Instance.DebugBrawlers) {
                DisplayDebugText($"Advance combo: {_currentComboEntry.Move.Id}", Color.green);
            }

            return true;
        }

        private BrawlerCombo.IComboEntry FudgeFailedCombo(CharacterBehaviorComponent.CharacterBehaviorAction action)
        {
            // if we fail a combo into a dash, just do the dash as a new opener
            // or, if we fail a combo out of a dash, try and find something out of the root instead as a new opener
            if(action is DashBehaviorComponent.DashAction || BrawlerAction.ActionType.Dash == Brawler.CurrentAction.Type) {
                return Brawler.BrawlerCombo.RootComboEntry.NextEntry(action, null, Brawler.CurrentAction);
            }

            // any other fudging we might want to do?

            return null;
        }

        private void Combo()
        {
            if(GameManager.Instance.DebugBrawlers) {
                DisplayDebugText($"Attempting combo: {_currentComboEntry.Move.Id}", Color.yellow);
            }

            CharacterBehaviorComponent.CharacterBehaviorAction action = ActionHandler.NextAction;
            if(!(action is AttackBehaviorComponent.AttackAction) && !(action is Game.Characters.BehaviorComponents.DashBehaviorComponent.DashAction)) {
                ComboFail();
                return;
            }

            ActionHandler.PopNextAction();
            ActionHandler.ActionPerformed(action);
        }

        public void ComboFail()
        {
            if(GameManager.Instance.DebugBrawlers) {
                DisplayDebugText("Combo failed / exhausted", Color.red);
            }

            _currentComboEntry = null;
            _velocityModifier = Vector3.zero;

            _comboFailEffectTrigger.Trigger();
        }
#endregion

#region Damage
        public bool Damage(Game.Actors.DamageData damageData)
        {
            if(ActionHandler.IsDead) {
                return false;
            }

            Actors.DamageData dd = (Actors.DamageData)damageData;

            // did we block the damage?
            if(!dd.AttackData.Unblockable && Brawler.CurrentAction.IsBlocking && _blockVolume.Intersects(dd.Bounds)) {
                if(BrawlerAction.ActionType.Parry == Brawler.CurrentAction.Type) {
                    Debug.Log($"TODO: Brawler {Owner.Id} can parry");
                }

                if(GameManager.Instance.DebugBrawlers) {
                    DisplayDebugText($"Blocked damage for {dd.AttackData.DamageAmount} (took {dd.AttackData.BlockDamageAmount})", Color.blue);
                }

                if(null != dd.SourceBrawlerActionHandler) {
                    BrawlerAction currentAction = Brawler.CurrentAction;
                    currentAction.WasBlocked = true;
                    Brawler.CurrentAction = currentAction;
                }

                if(DoDamage(dd, true)) {
                    return true;
                }

                //_blockAudioEffectTriggerComponent.AudioClip = dd.AttackData.BlockAudioCip;
                _blockEffectTrigger.Trigger();

                ActionHandler.ClearActionBuffer();
                ActionHandler.OnHit(true);

                return false;
            }

            if(GameManager.Instance.DebugBrawlers) {
                DisplayDebugText($"Damage: {dd.AttackData.DamageAmount}", Color.red);
            }

            CancelActions(false);

            DoDamage(dd, false);

            return true;
        }

        // returns true if the hit kills the brawler
        private bool DoDamage(Actors.DamageData damageData, bool blocked)
        {
            if(!ActionHandler.IsImmune) {
                Brawler.Health -= damageData.AttackData.DamageAmount;

                // TODO: we shouldn't actually die until we're done being combo'd to death
                if(ActionHandler.IsDead) {
                    Brawler.Health = 0;

                    _deathEffectTrigger.Trigger(() => ActionHandler.OnDeathComplete());

                    ActionHandler.OnHit(false);
                    ActionHandler.OnDead();

                    return true;
                }
            }

            if(blocked) {
                return false;
            }

            // push the source
            if(null != damageData.SourceBrawlerActionHandler) {
                damageData.SourceBrawlerActionHandler.Brawler.BrawlerBehavior._velocityModifier = damageData.Direction * damageData.AttackData.MoveFoward;
            }

            // push the recipient
            _velocityModifier = damageData.Direction * damageData.AttackData.PushBackAmount;

            // vertical push
            if(damageData.AttackData.KnockUpForce > 0.0f) {
                Owner.Behavior.Movement.PrepareJump();
            }
            Owner.Behavior.Movement.Velocity += damageData.AttackData.KnockDownForce * Vector3.down + damageData.AttackData.KnockUpForce * Vector3.up;

            _hitAudioEffectTriggerComponent.AudioClip = damageData.AttackData.ImpactAudioCip;
            if(damageData.AttackData.KnockDown) {
                Brawler.CurrentAction = new BrawlerAction(BrawlerAction.ActionType.KnockedDown);

                // TODO: make the time configurable
                _knockDownEffectTrigger.Trigger();
                _knockDownTimer.Start(1.0f);
            } else {
                Brawler.CurrentAction = new BrawlerAction(BrawlerAction.ActionType.Stunned);

                _hitEffectTrigger.Trigger(() => {
                    Idle();
                });
            }

            ActionHandler.ClearActionBuffer();
            ActionHandler.OnHit(false);

            return false;
        }
#endregion

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

        public void OnDeSpawn()
        {
            _attackVolume.Shutdown();
            _blockVolume.Shutdown();
        }
#endregion

#region Event Handlers
        private void KnockDownTimerTimesUpEventHandler(object sender, EventArgs args)
        {
            Idle();
        }

        private void AttackVolumeHitEventHandler(object sender, AttackVolumeEventArgs args)
        {
            BrawlerAction currentAction = Brawler.CurrentAction;
            currentAction.DidHit = true;
            Brawler.CurrentAction = currentAction;
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
            if(Brawler.BrawlerData.AttackVolumeSpawnEvent == evt.Data.Name) {
               _attackVolume.EnableVolume(true);
            } else if(Brawler.BrawlerData.AttackVolumeDeSpawnEvent == evt.Data.Name) {
                _attackVolume.EnableVolume(false);
            //} else if(Brawler.BrawlerData.ComboWindowEvent == evt.Data.Name) {
                Combo();
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
            if(Brawler.BrawlerData.BlockVolumeDeSpawnEvent == evt.Data.Name) {
                _blockVolume.EnableVolume(false);
            } else {
                Debug.LogWarning($"Unhandled block end event: {evt.Data.Name}");
            }
        }

        // TODO: this is temporary until we have an actual dash animation
        public bool DashAnimationCompleteHandler()
        {
            Combo();

            return null != _currentComboEntry;
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

#region Debug
        public void DisplayDebugText(string text, Color color)
        {
            Debug.Log($"Brawler debug: {text}");
            if(null != _debugTextQueue) {
                _debugTextQueue.QueueFloatingText(text, color, () => null == _debugTextTarget ? transform.position : _debugTextTarget.position);
            }
        }
#endregion
    }
}
