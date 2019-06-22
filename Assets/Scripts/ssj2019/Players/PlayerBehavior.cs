using System.Linq;

using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Core.Data;
using pdxpartyparrot.Core.Effects.EffectTriggerComponents;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.Characters.BehaviorComponents;
using pdxpartyparrot.Game.Characters.Players;
using pdxpartyparrot.ssj2019.Characters;
using pdxpartyparrot.ssj2019.Data;
using pdxpartyparrot.ssj2019.Players.BehaviorComponents;

using UnityEngine;
using UnityEngine.Assertions;

namespace pdxpartyparrot.ssj2019.Players
{
    [RequireComponent(typeof(BrawlerBehavior))]
    public sealed class PlayerBehavior : Game.Characters.Players.PlayerBehavior, IBrawlerBehaviorActions
    {
        public PlayerBehaviorData GamePlayerBehaviorData => (PlayerBehaviorData)PlayerBehaviorData;

        public Player GamePlayerOwner => (Player)Owner;

        public BrawlerData BrawlerData => GamePlayerOwner.PlayerCharacterData.BrawlerData;

        public Brawler Brawler => GamePlayerOwner.Brawler;

        private bool CanJump => !IsBlocking;

        private bool CanAttack => !IsBlocking;

        private bool CanBlock => IsGrounded;

        public bool IsDead => GamePlayerOwner.IsDead;

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

        public bool IsImmune => PlayerManager.Instance.PlayersImmune || _immune;

        public override bool CanMove => base.CanMove && !IsBlocking;

        private BrawlerBehavior _brawlerBehavior;

#region Unity Lifecycle
        protected override void Awake()
        {
            Assert.IsTrue(Owner is Player);

            base.Awake();

            _brawlerBehavior = GetComponent<BrawlerBehavior>();
        }
#endregion

        public override void Initialize(ActorBehaviorData behaviorData)
        {
            Assert.IsTrue(behaviorData is PlayerBehaviorData);

            base.Initialize(behaviorData);

            _brawlerBehavior.Initialize(this);
        }

        public override void InitializeLocalPlayerBehavior()
        {
            InitializeEffects();
        }

        private void InitializeEffects()
        {
            RumbleEffectTriggerComponent rumbleEffect = _spawnEffect.GetEffectTriggerComponent<RumbleEffectTriggerComponent>();
            rumbleEffect.GamepadListener = GamePlayerOwner.GamePlayerInput.GamepadListener;

            rumbleEffect = _respawnEffect.GetEffectTriggerComponent<RumbleEffectTriggerComponent>();
            rumbleEffect.GamepadListener = GamePlayerOwner.GamePlayerInput.GamepadListener;

            rumbleEffect = _brawlerBehavior.HitEffectTrigger.GetEffectTriggerComponent<RumbleEffectTriggerComponent>();
            rumbleEffect.GamepadListener = GamePlayerOwner.GamePlayerInput.GamepadListener;

            rumbleEffect = _brawlerBehavior.BlockEffectTrigger.GetEffectTriggerComponent<RumbleEffectTriggerComponent>();
            rumbleEffect.GamepadListener = GamePlayerOwner.GamePlayerInput.GamepadListener;

            rumbleEffect = _brawlerBehavior.DeathEffectTrigger.GetEffectTriggerComponent<RumbleEffectTriggerComponent>();
            rumbleEffect.GamepadListener = GamePlayerOwner.GamePlayerInput.GamepadListener;
        }

#region Brawler Actions
        public void OnIdle()
        {
            SpineAnimationHelper.SetAnimation(GamePlayerOwner.PlayerCharacterData.BrawlerData.IdleAnimationName, false);
        }

        public void OnAttack()
        {
            _brawlerBehavior.Attack(GamePlayerOwner.PlayerCharacterData.BrawlerData.AttackComboData.AttackData.ElementAt(0));
        }

        public void OnDead()
        {
            GameManager.Instance.PlayerDied(GamePlayerOwner);
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
