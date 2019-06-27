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
using pdxpartyparrot.ssj2019.Characters;
using pdxpartyparrot.ssj2019.Data;
using pdxpartyparrot.ssj2019.Players;
using pdxpartyparrot.ssj2019.Players.BehaviorComponents;

using UnityEngine;
using UnityEngine.Assertions;

namespace pdxpartyparrot.ssj2019.NPCs
{
    [RequireComponent(typeof(BrawlerBehavior))]
    public sealed class NPCBehavior : Game.Characters.NPCs.NPCBehavior, IBrawlerBehaviorActions
    {
        private enum NPCState
        {
            Idle,
            Track,
            Attack
        }

        public NPCBehaviorData GameNPCBehaviorData => (NPCBehaviorData)NPCBehaviorData;

        public NPC GameNPCOwner => (NPC)Owner;

        public Brawler Brawler => GameNPCOwner.Brawler;

        [SerializeField]
        private Interactables _interactables;

        public override Vector3 MoveDirection
        {
            get
            {
                Vector3 nextPosition = GameNPCOwner.NextPosition;
                return nextPosition - Movement.Position;
            }
        }

        private bool CanJump => !IsDead && Brawler.CurrentAction.Cancellable;

        private bool CanAttack => !IsDead && Brawler.CurrentAction.Cancellable;

        public bool CanBlock => !IsDead && IsGrounded && Brawler.CurrentAction.Cancellable;

        public bool IsDead => GameNPCOwner.IsDead;

        [SerializeField]
        [ReadOnly]
        private bool _immune;

        public bool IsImmune
        {
            get => NPCManager.Instance.NPCsImmune || _immune || GameNPCOwner.Brawler.CurrentAction.IsImmune;
            set => _immune = value;
        }

        public override bool CanMove => base.CanMove && !IsDead && !Brawler.CurrentAction.IsStunned;

        // TODO: this depends on which piece of a combo we're in and other factors
        public AttackData CurrentAttack => GameNPCOwner.NPCCharacterData.BrawlerData.AttackComboData.AttackData.ElementAt(0);

        [SerializeField]
        [ReadOnly]
        private NPCState _state = NPCState.Idle;

        [SerializeField]
        [ReadOnly]
        [CanBeNull]
        private Actor _target;

        [SerializeField]
        private AttackBehaviorComponent _attackBehaviorComponent;

        [SerializeField]
        private BlockBehaviorComponent _blockBehaviorComponent;

        private BrawlerBehavior _brawlerBehavior;

        private ITimer _stateCooldown;

        private ITimer _attackCooldown;

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            Assert.IsTrue(Owner is NPC);

            _brawlerBehavior = GetComponent<BrawlerBehavior>();

            _attackBehaviorComponent.Brawler = GameNPCOwner.Brawler;
            _blockBehaviorComponent.Brawler = GameNPCOwner.Brawler;

            _stateCooldown = TimeManager.Instance.AddTimer();
            _attackCooldown = TimeManager.Instance.AddTimer();
        }

        protected override void OnDestroy()
        {
            if(TimeManager.HasInstance) {
                TimeManager.Instance.RemoveTimer(_stateCooldown);
                _stateCooldown = null;

                TimeManager.Instance.RemoveTimer(_attackCooldown);
                _attackCooldown = null;
            }
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;

            // TODO: probably should be a sphere
            Vector3 position = transform.position;
            position.y += Owner.Height * 0.5f;
            Gizmos.DrawLine(position, position + new Vector3(GameNPCOwner.NPCCharacterData.MaxTrackDistance * Mathf.Sign(FacingDirection.x), 0.0f, 0.0f));
        }
#endregion

        public override void Initialize(ActorBehaviorData behaviorData)
        {
            Assert.IsTrue(behaviorData is NPCBehaviorData);

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
            case NPCState.Idle:
                HandleIdle();
                break;
            case NPCState.Track:
                HandleTrack();
                break;
            case NPCState.Attack:
                HandleAttack();
                break;
            }
        }

#region NPC State
        private void SetState(NPCState state)
        {
            if(NPCManager.Instance.DebugBehavior) {
                Debug.Log($"NPC {Owner.Id} set state {state}");
            }

            _state = state;
            switch(_state)
            {
            case NPCState.Idle:
                SpineAnimationHelper.SetAnimation(GameNPCOwner.NPCCharacterData.BrawlerData.IdleAnimationName, false);
                GameNPCOwner.ResetPath();
                break;
            case NPCState.Track:
                break;
            case NPCState.Attack:
                GameNPCOwner.Stop(true);
                break;
            }

            _stateCooldown.Start(GameNPCOwner.NPCCharacterData.StateCooldownSeconds);
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

                SetState(NPCState.Attack);
                return;
            }

            // if we already have a target, track it
            if(null != _target) {
                SetState(NPCState.Track);
                return;
            }

            // look for something to track
            Player player = ActorManager.Instance.GetActors<Player>().NearestManhattan(Movement.Position, out float distance) as Player;
            if(null != player && GameNPCOwner.NPCCharacterData.CanTrackDistance(distance)) {
                SetTarget(player);

                SetState(NPCState.Track);
                return;
            }
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
                SetState(NPCState.Attack);
                return;
            }

            // if we have something else we can attack, attack it
            if(interactablePlayers.Count > 0) {
                SetTarget(interactablePlayers.ElementAt(0) as Player);

                SetState(NPCState.Attack);
                return;
            }

            // can't attack our target, so follow it
            GameNPCOwner.UpdatePath(_target.Behavior.Movement.Position);
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
            SetState(NPCState.Track);
        }

        private bool EnsureTarget()
        {
            // lost target?
            if(null == _target) {
                if(NPCManager.Instance.DebugBehavior) {
                    Debug.Log($"NPC {Owner.Id} lost target while attacking");
                }

                SetState(NPCState.Idle);
                return false;
            }

            // dead target?
            if(_target is Player player && player.IsDead) {
                if(NPCManager.Instance.DebugBehavior) {
                    Debug.Log($"NPC {Owner.Id} attack target died");
                }

                SetState(NPCState.Idle);
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
            SetState(NPCState.Idle);
        }

        public void OnAttack(AttackBehaviorComponent.AttackAction action)
        {
            ActionPerformed(action);

            _attackCooldown.Start(GameNPCOwner.NPCCharacterData.AttackCooldownSeconds);
        }

        public void OnHit(bool blocked)
        {
            ClearActionBuffer();

            _attackCooldown.Stop();
        }

        public void OnDead()
        {
            ClearActionBuffer();
        }

        public void OnDeathComplete()
        {
            NPC.Recycle();
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

            _brawlerBehavior.CancelActions();

            ActionPerformed(JumpBehaviorComponent.JumpAction.Default);

            _attackCooldown.Stop();
        }

        // TODO: we might want the entire move buffer
        public void Attack(Vector3 lastMove)
        {
            if(!CanAttack || _attackCooldown.IsRunning) {
                return;
            }

            if(BrawlerAction.ActionType.Attack == Brawler.CurrentAction.Type) {
                BufferAction(new AttackBehaviorComponent.AttackAction{
                    Axes = lastMove,
                });
            } else {
                ActionPerformed(new AttackBehaviorComponent.AttackAction{
                    Axes = lastMove,
                });
            }
        }
        
        public void Block(Vector3 lastMove)
        {
            ActionPerformed(new BlockBehaviorComponent.BlockAction{
                Axes = lastMove,
            });

            _attackCooldown.Stop();
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
