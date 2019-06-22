using System.Linq;

using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Core.Data;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.Characters.BehaviorComponents;
using pdxpartyparrot.Game.Characters.NPCs;
using pdxpartyparrot.ssj2019.Characters;
using pdxpartyparrot.ssj2019.Data;
using pdxpartyparrot.ssj2019.Players.BehaviorComponents;

using UnityEngine;
using UnityEngine.Assertions;

namespace pdxpartyparrot.ssj2019.NPCs
{
    [RequireComponent(typeof(BrawlerBehavior))]
    public sealed class NPCBehavior : Game.Characters.NPCs.NPCBehavior, IBrawlerBehaviorActions
    {
        public NPCBehaviorData GameNPCBehaviorData => (NPCBehaviorData)NPCBehaviorData;

        public NPC GameNPCOwner => (NPC)Owner;

        public BrawlerData BrawlerData => GameNPCOwner.NPCCharacterData.BrawlerData;

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
            // TODO: here we think but also queue movement and actions
        }

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
            SpineAnimationHelper.SetAnimation(GameNPCOwner.NPCCharacterData.BrawlerData.IdleAnimationName, false);
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
            if(GameNPCOwner.IsDead) {
                return;
            }

            Debug.Log($"NPC {Owner.Id} damaged by {source.Id}");

            GameNPCOwner.Brawler.Health -= amount;
            _brawlerBehavior.Damage();
        }
#endregion
    }
}
