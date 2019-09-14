using System;
using System.Collections.Generic;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Core.Collections;
using pdxpartyparrot.Core.Data;
using pdxpartyparrot.Core.Time;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Core.World;
using pdxpartyparrot.Game.Actors;
using pdxpartyparrot.Game.Characters.NPCs;
using pdxpartyparrot.Game.Interactables;
using pdxpartyparrot.Game.KungFuCircle;
using pdxpartyparrot.ssj2019.Characters.Brawlers;
using pdxpartyparrot.ssj2019.Data.NPCs;
using pdxpartyparrot.ssj2019.Level;
using pdxpartyparrot.ssj2019.Players;

using UnityEngine;
using UnityEngine.Assertions;

namespace pdxpartyparrot.ssj2019.NPCs
{
    // TODO: abstract this more into separate components

    [RequireComponent(typeof(BrawlerBehavior))]
    [RequireComponent(typeof(NPCFidgetBehavior))]
    public sealed class NPCBrawlerBehavior : NPCBehavior, IBrawlerBehaviorActions
    {
        [Serializable]
        private struct Target
        {
            [CanBeNull]
            public Actor Actor { get; private set; }

            [CanBeNull]
            public KungFuGrid TargetGrid { get; private set; }

            public int TargetGridSlot { get; private set; }

            // TODO: if we can ever target non-players this will need to change
            public bool IsValid => null != Actor && null != TargetGrid && TargetGridSlot >= 0;

            public void ReTarget([CanBeNull] Actor actor, int gridWeight)
            {
                if(null != TargetGrid && TargetGridSlot >= 0) {
                    TargetGrid.EmptyGridSlot(TargetGridSlot);
                }
                TargetGrid = null;
                TargetGridSlot = -1;

                Actor = actor;

                Player player = Actor as Player;
                if(null != player) {
                    TargetGrid = player.KungFuGrid;
                    TargetGridSlot = TargetGrid.GetAvailableGridSlot(gridWeight);
                }
            }
        }

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
                if(!NPCBrawler.HasPath) {
                    return Vector3.zero;
                }

                Vector3 nextPosition = NPCBrawler.NextPosition;
                return nextPosition - Movement.Position;
            }
        }

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
        private Target _target;

        [SerializeField]
        [ReadOnly]
        private int _attackSlotIndex;

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
            Gizmos.DrawLine(position, position + new Vector3(NPCBrawler.NPCBrawlerData.MaxTrackDistance * Mathf.Sign(Owner.FacingDirection.x), 0.0f, 0.0f));
        }
#endregion

        public override void Initialize(ActorBehaviorData behaviorData)
        {
            Assert.IsTrue(behaviorData is NPCBrawlerBehaviorData);

            base.Initialize(behaviorData);

            _brawlerBehavior.ActionHandler = this;
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
                NPCOwner.Stop(true, false);

                // have to use the transform here since physics lags behind
                _fidgetBehavior.Origin = Owner.transform.position;
                break;
            case State.Track:
                break;
            case State.Attack:
                NPCBrawler.Stop(true, false);
                break;
            }

            _stateCooldown.Start(NPCBrawler.NPCBrawlerData.StateCooldownSeconds);
        }

        private bool SetTarget(IReadOnlyCollection<Actor> targets)
        {
            foreach(Actor target in targets) {
                _target.ReTarget(target, NPCBrawlerBehaviorData.KungFuGridWeight);
                if(!_target.IsValid) {
                    continue;
                }

                if(NPCManager.Instance.DebugBehavior) {
                    Debug.Log($"NPC {Owner.Id} targeting {target.Id}");
                }
                return true;
            }

            _target.ReTarget(null, NPCBrawlerBehaviorData.KungFuGridWeight);
            return false;
        }

        private void HandleIdle()
        {
            if(NPCManager.Instance.DumbBrawlers) {
                return;
            }

            Actor previousTarget = _target.Actor;

            // if we have something nearby we can attack, attack it
            // TODO: make this list a class member to avoid re-allocation
            List<Player> interactablePlayers = new List<Player>();
            _interactables.GetInteractables(interactablePlayers);
            if(SetTarget(interactablePlayers)) {
                SetState(State.Attack);
                return;
            }

            // if we already had a target, track it
            if(null != previousTarget) {
                _target.ReTarget(previousTarget, NPCBrawlerBehaviorData.KungFuGridWeight);

                SetState(State.Track);
                return;
            }

            // look for something to track
            // TODO: make this list a class member to avoid re-allocation
            List<Actor> targets = new List<Actor>();

            ActorManager.Instance.GetActors<Player>().WithinDistance(Movement.Position, NPCBrawler.NPCBrawlerData.MaxTrackDistance, targets);
            if(SetTarget(targets)) {
                SetState(State.Track);
                return;
            }

            // still idle...
            _fidgetBehavior.Fidget();
        }

        private void HandleTrack()
        {
            if(NPCManager.Instance.DumbBrawlers) {
                OnIdle();
                return;
            }

            if(!EnsureTarget()) {
                return;
            }

            // TODO: from here we're assuming our target is a Player, but what if it isn't?

            // is our target interactable?
            // TODO: make this list a class member to avoid re-allocation
            List<Player> interactablePlayers = new List<Player>();
            _interactables.GetInteractables(interactablePlayers);
            if(interactablePlayers.Contains(_target.Actor as Player)) {
                SetState(State.Attack);
                return;
            }

            Actor previousTarget = _target.Actor;

            // if we have something else nearby we can attack, attack it
            if(SetTarget(interactablePlayers)) {
                SetState(State.Attack);
                return;
            }

            _target.ReTarget(previousTarget, NPCBrawlerBehaviorData.KungFuGridWeight);
    
            if(!NPCBrawler.UpdatePath(_target.TargetGrid.GetAttackSlotLocation(_target.TargetGridSlot))) {
                SetState(State.Idle);
                return;
            }
        }

        private void HandleAttack()
        {
            if(NPCManager.Instance.DumbBrawlers) {
                OnIdle();
                return;
            }

            if(!EnsureTarget()) {
                return;
            }

            // TODO: from here we're assuming our target is a Player, but what if it isn't?

            // is our target interactable?
            // TODO: make this list a class member to avoid re-allocation
            List<Player> interactablePlayers = new List<Player>();
            _interactables.GetInteractables(interactablePlayers);
            if(interactablePlayers.Contains(_target.Actor as Player)) {
                // TODO: pass in last move
                Attack(Vector3.zero);
                return;
            }

            Actor previousTarget = _target.Actor;

            // if we have something else nearby we can attack, attack it
            if(SetTarget(interactablePlayers)) {
                // TODO: pass in last move
                Attack(Vector3.zero);

                return;
            }

            // go back to tracking
            _target.ReTarget(previousTarget, NPCBrawlerBehaviorData.KungFuGridWeight);
            SetState(State.Track);
        }
#endregion

        private bool EnsureTarget()
        {
            // lost target?
            if(!_target.IsValid) {
                if(NPCManager.Instance.DebugBehavior) {
                    Debug.Log($"NPC {Owner.Id} lost target while attacking");
                }

                OnIdle();
                return false;
            }

            // dead target?
            if(_target.Actor is Player player && player.IsDead) {
                if(NPCManager.Instance.DebugBehavior) {
                    Debug.Log($"NPC {Owner.Id} attack target died");
                }

                OnIdle();
                return false;
            }

            return true;
        }

#region Spawn
        public override void OnSpawn(SpawnPoint spawnpoint)
        {
            base.OnSpawn(spawnpoint);

            // TODO: add a small window of immunity on spawn
            _immune = false;

            _target.ReTarget(null, NPCBrawlerBehaviorData.KungFuGridWeight);
        }

        public override void OnReSpawn(SpawnPoint spawnpoint)
        {
            base.OnReSpawn(spawnpoint);

            // TODO: add a small window of immunity on respawn
            _immune = false;

            _target.ReTarget(null, NPCBrawlerBehaviorData.KungFuGridWeight);
        }

        public override void OnDeSpawn()
        {
            // TODO: this cast could become unsafe real quick
            ((BarLevel)GameManager.Instance.LevelHelper).WaveSpawner.CurrentWave.OnWaveSpawnMemberDone();

            base.OnDeSpawn();
        }
#endregion

#region Brawler Actions
        public override void OnIdle()
        {
            SetState(State.Idle);

            base.OnIdle();
        }

        public void OnHit(bool blocked)
        {
            _attackCooldown.Stop();
            _dashCooldown.Stop();
        }

        public void OnComboMove(bool isOpener, ComboMove move, BrawlerAction currentAction)
        {
        }

        public void OnDead()
        {
            _target.ReTarget(null, NPCBrawlerBehaviorData.KungFuGridWeight);

            ClearActionBuffer();

            GameManager.Instance.NPCBrawlerKilled(NPCBrawler.NPCBrawlerData.Points);
        }

        public void OnDeathComplete()
        {
            NPCOwner.Recycle();
        }
#endregion

#region Actions
        public void Jump()
        {
            _brawlerBehavior.Jump();

            _attackCooldown.Stop();
            _dashCooldown.Stop();
        }

        public void Attack(Vector3 lastMove)
        {
            _brawlerBehavior.Attack(lastMove, IsGrounded);

            if(!_attackCooldown.IsRunning) {
                _attackCooldown.Start(NPCBrawler.NPCBrawlerData.AttackCooldownSeconds);
                _dashCooldown.Stop();
            }
        }
        
        public void StartBlock(Vector3 lastMove)
        {
            _brawlerBehavior.StartBlock(lastMove);

            _attackCooldown.Stop();
            _dashCooldown.Stop();
        }

        public void EndBlock()
        {
            _brawlerBehavior.EndBlock();
        }

        public void Dash()
        {
            _brawlerBehavior.Dash();

            if(!_dashCooldown.IsRunning) {
                _attackCooldown.Stop();
                _dashCooldown.Start(NPCBrawler.NPCBrawlerData.DashCooldownSeconds);
            }
        }
#endregion

#region Events
        public bool OnDamage(DamageData damageData)
        {
            return _brawlerBehavior.Damage(damageData);
        }
#endregion
    }
}
