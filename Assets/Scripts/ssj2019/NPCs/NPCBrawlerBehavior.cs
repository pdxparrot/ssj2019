using System.Linq;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Core.Collections;
using pdxpartyparrot.Core.Data;
using pdxpartyparrot.Core.Time;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Core.World;
using pdxpartyparrot.Game.Characters.BehaviorComponents;
using pdxpartyparrot.Game.Characters.NPCs;
using pdxpartyparrot.Game.Interactables;
using pdxpartyparrot.ssj2019.Characters.Brawlers;
using pdxpartyparrot.ssj2019.Data.NPCs;
using pdxpartyparrot.ssj2019.Players;
using pdxpartyparrot.ssj2019.Players.BehaviorComponents;

using UnityEngine;
using UnityEngine.Assertions;

namespace pdxpartyparrot.ssj2019.NPCs
{
    // TODO: abstract this more into separate components

    [RequireComponent(typeof(BrawlerBehavior))]
    [RequireComponent(typeof(NPCFidgetBehavior))]
    public sealed class NPCBrawlerBehavior : NPCBehavior, IBrawlerBehaviorActions
    {
        private enum State
        {
            Idle,
            Track,
            Attack
        }

        public NPCBrawlerBehaviorData NPCBrawlerBehaviorData => (NPCBrawlerBehaviorData)NPCBehaviorData;

        public NPCBrawler NPCBrawler => (NPCBrawler)Owner;

        public Brawler Brawler => NPCBrawler.Brawler;

        [Space(10)]

        [SerializeField]
        private Interactables _interactables;

        public override Vector3 MoveDirection
        {
            get
            {
                Vector3 nextPosition = NPCBrawler.NextPosition;
                return nextPosition - Movement.Position;
            }
        }

        private bool CanJump => !IsDead && Brawler.CurrentAction.Cancellable;

        private bool CanAttack => !IsDead && Brawler.CurrentAction.Cancellable;

        public bool CanBlock => !IsDead && IsGrounded && Brawler.CurrentAction.Cancellable;

        private bool CanDash => !IsDead && Brawler.CurrentAction.Cancellable;

        public bool IsDead => NPCBrawler.IsDead;

        [SerializeField]
        [ReadOnly]
        private bool _immune;

        public bool IsImmune
        {
            get => NPCManager.Instance.NPCsImmune || _immune || NPCBrawler.Brawler.CurrentAction.IsImmune;
            set => _immune = value;
        }

        public override bool CanMove => base.CanMove && !IsDead && !Brawler.CurrentAction.IsStunned;

        [SerializeField]
        [ReadOnly]
        private State _state = State.Idle;

        [SerializeField]
        [ReadOnly]
        [CanBeNull]
        private Actor _target;

        private BrawlerBehavior _brawlerBehavior;

        private NPCFidgetBehavior _fidgetBehavior;

        private ITimer _stateCooldown;

        private ITimer _attackCooldown;

        private ITimer _dashCooldown;

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            Assert.IsTrue(Owner is NPCBrawler);

            _brawlerBehavior = GetComponent<BrawlerBehavior>();

            _fidgetBehavior = GetComponent<NPCFidgetBehavior>();
            _fidgetBehavior.Initialize(NPCOwner);

            _stateCooldown = TimeManager.Instance.AddTimer();
            _attackCooldown = TimeManager.Instance.AddTimer();
            _dashCooldown = TimeManager.Instance.AddTimer();
        }

        protected override void OnDestroy()
        {
            if(TimeManager.HasInstance) {
                TimeManager.Instance.RemoveTimer(_stateCooldown);
                _stateCooldown = null;

                TimeManager.Instance.RemoveTimer(_attackCooldown);
                _attackCooldown = null;

                TimeManager.Instance.RemoveTimer(_dashCooldown);
                _dashCooldown = null;
            }
        }

        private void OnDrawGizmos()
        {
            if(!Application.isPlaying) {
                return;
            }

            Gizmos.color = Color.red;

            // TODO: probably should be a sphere
            Vector3 position = transform.position;
            position.y += Owner.Height * 0.5f;
            Gizmos.DrawLine(position, position + new Vector3(NPCBrawler.NPCBrawlerData.MaxTrackDistance * Mathf.Sign(FacingDirection.x), 0.0f, 0.0f));
        }
#endregion

        public override void Initialize(ActorBehaviorData behaviorData)
        {
            Assert.IsTrue(behaviorData is NPCBrawlerBehaviorData);

            base.Initialize(behaviorData);

            _brawlerBehavior.Initialize(this);
        }

        public override void Think(float dt)
        {
            if(IsDead || _stateCooldown.IsRunning) {
                return;
            }

            switch(_state)
            {
            case State.Idle:
                HandleIdle();
                break;
            case State.Track:
                HandleTrack();
                break;
            case State.Attack:
                HandleAttack();
                break;
            }
        }

#region NPC State
        private void SetState(State state)
        {
            if(NPCManager.Instance.DebugBehavior) {
                Debug.Log($"NPCBrawler {Owner.Id} set state {state}");
            }

            _state = state;
            switch(_state)
            {
            case State.Idle:
                NPCOwner.ResetPath();

                // have to use the transform here since physics lags behind
                _fidgetBehavior.Origin = Owner.transform.position;

                _idleEffect.Trigger();
                break;
            case State.Track:
                break;
            case State.Attack:
                NPCBrawler.Stop(true);
                break;
            }

            _stateCooldown.Start(NPCBrawler.NPCBrawlerData.StateCooldownSeconds);
        }

        private void SetTarget(Actor target)
        {
            if(NPCManager.Instance.DebugBehavior) {
                Debug.Log($"NPC {Owner.Id} targeting {target.Id}");
            }

            _target = target;
        }

        private void HandleIdle()
        {
            // if we have something we can attack, attack it
            var interactablePlayers = _interactables.GetInteractables<Player>();
            if(interactablePlayers.Count > 0) {
                SetTarget(interactablePlayers.ElementAt(0) as Player);

                SetState(State.Attack);
                return;
            }

            // if we already have a target, track it
            if(null != _target) {
                SetState(State.Track);
                return;
            }

            // look for something to track
            Player player = ActorManager.Instance.GetActors<Player>().NearestManhattan(Movement.Position, out float distance) as Player;
            if(null != player && NPCBrawler.NPCBrawlerData.CanTrackDistance(distance)) {
                SetTarget(player);

                SetState(State.Track);
                return;
            }

            _fidgetBehavior.Fidget();
        }

        private void HandleTrack()
        {
            if(!EnsureTarget()) {
                return;
            }

            // TODO: from here we're assuming our target is a Player, but what if it isn't?

            var interactablePlayers = _interactables.GetInteractables<Player>();

            // is our target interactable?
            if(interactablePlayers.Contains(_target as Player)) {
                SetState(State.Attack);
                return;
            }

            // if we have something else we can attack, attack it
            if(interactablePlayers.Count > 0) {
                SetTarget(interactablePlayers.ElementAt(0) as Player);

                SetState(State.Attack);
                return;
            }

            // can't attack our target, so follow it
            NPCBrawler.UpdatePath(_target.Behavior.Movement.Position);
        }

        private void HandleAttack()
        {
            if(!EnsureTarget()) {
                return;
            }

            // TODO: from here we're assuming our target is a Player, but what if it isn't?

            var interactablePlayers = _interactables.GetInteractables<Player>();

            // is our target interactable?
            if(interactablePlayers.Contains(_target as Player)) {
                // TODO: pass in last move
                Attack(Vector3.zero);
                return;
            }

            // if we have something else we can attack, attack it
            if(interactablePlayers.Count > 0) {
                SetTarget(interactablePlayers.ElementAt(0) as Player);

                // TODO: pass in last move
                Attack(Vector3.zero);
                return;
            }

            // go back to tracking
            SetState(State.Track);
        }

        private bool EnsureTarget()
        {
            // lost target?
            if(null == _target) {
                if(NPCManager.Instance.DebugBehavior) {
                    Debug.Log($"NPC {Owner.Id} lost target while attacking");
                }

                SetState(State.Idle);
                return false;
            }

            // dead target?
            if(_target is Player player && player.IsDead) {
                if(NPCManager.Instance.DebugBehavior) {
                    Debug.Log($"NPC {Owner.Id} attack target died");
                }

                SetState(State.Idle);
                return false;
            }

            return true;
        }
#endregion

#region Spawn
        public override void OnSpawn(SpawnPoint spawnpoint)
        {
            base.OnSpawn(spawnpoint);

            // TODO: add a small window of immunity on spawn
            _immune = false;

            SetState(State.Idle);
        }

        public override void OnReSpawn(SpawnPoint spawnpoint)
        {
            base.OnReSpawn(spawnpoint);

            // TODO: add a small window of immunity on respawn
            _immune = false;

            SetState(State.Idle);
        }

        public override void OnDeSpawn()
        {
            GameManager.Instance.LevelHelper.WaveSpawner.CurrentWave.OnWaveSpawnMemberDone();

            base.OnDeSpawn();
        }
#endregion

#region Brawler Actions
        public void OnIdle()
        {
            SetState(State.Idle);
        }

        public void OnCombo(CharacterBehaviorComponent.CharacterBehaviorAction action)
        {
            ActionPerformed(action);
        }

        public void OnHit(bool blocked)
        {
            ClearActionBuffer();

            _attackCooldown.Stop();
            _dashCooldown.Stop();
        }

        public void OnDead()
        {
            ClearActionBuffer();
        }

        public void OnDeathComplete()
        {
            NPCOwner.Recycle();
        }

        public void OnCancelActions()
        {
            ClearActionBuffer();
        }
#endregion

#region Actions
        public void Jump()
        {
            if(!CanJump) {
                return;
            }

            _brawlerBehavior.CancelActions(false);

            ActionPerformed(JumpBehaviorComponent.JumpAction.Default);

            _attackCooldown.Stop();
            _dashCooldown.Stop();
        }

        // TODO: we might want the entire move buffer
        public void Attack(Vector3 lastMove)
        {
            if(!CanAttack) {
                return;
            }

            if(Brawler.CurrentAction.CanQueue) {
                BufferAction(new AttackBehaviorComponent.AttackAction{
                    Axes = lastMove,
                });
            } else if(!_attackCooldown.IsRunning) {
                ActionPerformed(new AttackBehaviorComponent.AttackAction{
                    Axes = lastMove,
                });

                _attackCooldown.Start(NPCBrawler.NPCBrawlerData.AttackCooldownSeconds);
                _dashCooldown.Stop();
            }
        }
        
        public void Block(Vector3 lastMove)
        {
            ActionPerformed(new BlockBehaviorComponent.BlockAction{
                Axes = lastMove,
            });

            _attackCooldown.Stop();
            _dashCooldown.Stop();
        }

        public void Dash(Vector3 lastMove)
        {
            if(!CanDash) {
                return;
            }

            if(Brawler.CurrentAction.CanQueue) {
                BufferAction(new DashBehaviorComponent.DashAction{
                    Axes = lastMove,
                });
            } else if(!_dashCooldown.IsRunning) {
                ActionPerformed(new DashBehaviorComponent.DashAction{
                    Axes = lastMove,
                });

                _attackCooldown.Stop();
                _dashCooldown.Start(NPCBrawler.NPCBrawlerData.DashCooldownSeconds);
            }
        }
#endregion

#region Events
        public bool OnDamage(Actor source, string type, int amount, Bounds attackBounds, Vector3 force)
        {
            return _brawlerBehavior.Damage(source, type, amount, attackBounds, force);
        }
#endregion
    }
}
