using System.Linq;

using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Core.Data;
using pdxpartyparrot.Core.Effects.EffectTriggerComponents;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Core.World;
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

        public Brawler Brawler => GamePlayerOwner.Brawler;

        private bool CanJump => !IsDead && !Brawler.IsBlocking && !Brawler.IsStunned && Brawler.CanCancel;

        private bool CanAttack => !IsDead && !Brawler.IsBlocking && !Brawler.IsStunned && Brawler.CanCancel;

        public bool CanBlock => !IsDead && IsGrounded && !Brawler.IsBlocking && !Brawler.IsStunned && Brawler.CanCancel;

        public bool IsDead => GamePlayerOwner.IsDead;

        [SerializeField]
        [ReadOnly]
        private bool _immune;

        public bool IsImmune
        {
            get => PlayerManager.Instance.PlayersImmune || _immune;
            set => _immune = value;
        }

        public override bool CanMove => base.CanMove && !Brawler.IsBlocking && !Brawler.IsStunned && Brawler.CanCancel && !IsDead;

        // TODO: this depends on which piece of a combo we're in and other factors
        public AttackData CurrentAttack => GamePlayerOwner.PlayerCharacterData.BrawlerData.AttackComboData.AttackData.ElementAt(0);

        [SerializeField]
        private AttackBehaviorComponent _attackBehaviorComponent;

        [SerializeField]
        private BlockBehaviorComponent _blockBehaviorComponent;

        private BrawlerBehavior _brawlerBehavior;

#region Unity Lifecycle
        protected override void Awake()
        {
            Assert.IsTrue(Owner is Player);

            base.Awake();

            _brawlerBehavior = GetComponent<BrawlerBehavior>();

            _attackBehaviorComponent.Brawler = GamePlayerOwner.Brawler;
            _blockBehaviorComponent.Brawler = GamePlayerOwner.Brawler;
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

#region Spawn
        public override void OnSpawn(SpawnPoint spawnpoint)
        {
            base.OnSpawn(spawnpoint);

            // TODO: add a small window of immunity on spawn
            _immune = false;
        }
#endregion

#region Brawler Actions
        public void OnIdle()
        {
            SpineAnimationHelper.SetAnimation(GamePlayerOwner.PlayerCharacterData.BrawlerData.IdleAnimationName, false);
        }

        public void OnAttack(AttackBehaviorComponent.AttackAction action)
        {
            ActionPerformed(action);
        }

        public void OnHit(bool blocked)
        {
            ClearActionBuffer();
        }

        public void OnDead()
        {
            ClearActionBuffer();

            GameManager.Instance.PlayerDied(GamePlayerOwner);
        }

        public void OnDeathComplete()
        {
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
        
        public void Block(Vector3 lastMove)
        {
            ActionPerformed(new BlockBehaviorComponent.BlockAction{
                Axes = lastMove,
            });
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
