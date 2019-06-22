﻿using System.Linq;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Core.Collections;
using pdxpartyparrot.Core.Data;
using pdxpartyparrot.Core.Util;
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

        public BrawlerData BrawlerData => GameNPCOwner.NPCCharacterData.BrawlerData;

        public Brawler Brawler => GameNPCOwner.Brawler;

        [SerializeField]
        private Interactables _interactables;

        private bool CanJump => !IsBlocking;

        private bool CanAttack => !IsBlocking;

        private bool CanBlock => IsGrounded;

        public bool IsDead => GameNPCOwner.IsDead;

        [SerializeField]
        [ReadOnly]
        private bool _blocking;

        public bool IsBlocking
        {
            get => _blocking;
            set => _blocking = value;
        }

        [SerializeField]
        [ReadOnly]
        private bool _parry;

        public bool IsParry
        {
            get => _parry;
            set => _parry = value;
        }

        [SerializeField]
        [ReadOnly]
        private bool _immune;

        public bool IsImmune => _immune;

        public override bool CanMove => base.CanMove && !IsBlocking;

        [SerializeField]
        [ReadOnly]
        private NPCState _state = NPCState.Idle;

        [SerializeField]
        [ReadOnly]
        [CanBeNull]
        private Actor _target;

        private BrawlerBehavior _brawlerBehavior;

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            Assert.IsTrue(Owner is NPC);

            _brawlerBehavior = GetComponent<BrawlerBehavior>();
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
                break;
            case NPCState.Track:
                break;
            case NPCState.Attack:
                break;
            }
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
            Player player = ActorManager.Instance.GetActors<Player>().NearestManhattan(Movement.Position) as Player;
            if(null != player) {
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
            GameNPCOwner.Agent.SetDestination(_target.Behavior.Movement.Position);
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

        public void OnAttack()
        {
            _brawlerBehavior.Attack(GameNPCOwner.NPCCharacterData.BrawlerData.AttackComboData.AttackData.ElementAt(0));
        }

        public void OnDead()
        {
            NPC.Recycle();
        }
#endregion

#region Actions
        public void Jump()
        {
            if(!CanJump) {
                return;
            }

            //ClearActionBuffer();

            ActionPerformed(JumpBehaviorComponent.JumpAction.Default);
        }

        // TODO: we might want the entire move buffer
        public void Attack(Vector3 lastMove)
        {
            if(!CanAttack) {
                return;
            }

            BufferAction(new AttackBehaviorComponent.AttackAction{
                Axes = lastMove,
            });
        }
        
        public void Block()
        {
            if(IsBlocking) {
                _brawlerBehavior.ToggleBlock();
                return;
            }

            if(!CanBlock) {
                return;
            }

            ClearActionBuffer();

            _brawlerBehavior.ToggleBlock();
        }
#endregion

#region Events
        public void OnDamage(Actor source, string type, int amount)
        {
            _brawlerBehavior.Damage(source, type, amount);
        }
#endregion
    }
}
