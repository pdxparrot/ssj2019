using System.Linq;

using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Core.Data;
using pdxpartyparrot.Core.Effects.EffectTriggerComponents;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Core.World;
using pdxpartyparrot.Game.Characters.BehaviorComponents;
using pdxpartyparrot.Game.Characters.Players;
using pdxpartyparrot.ssj2019.Characters.Brawlers;
using pdxpartyparrot.ssj2019.Data.Brawlers;
using pdxpartyparrot.ssj2019.Data.Players;
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

        private bool CanJump => !IsDead && Brawler.CurrentAction.Cancellable;

        private bool CanAttack => !IsDead && Brawler.CurrentAction.Cancellable;

        public bool CanBlock => !IsDead && IsGrounded && Brawler.CurrentAction.Cancellable;

        public bool IsDead => GamePlayerOwner.IsDead;

        [Space(10)]

        [SerializeField]
        [ReadOnly]
        private bool _immune;

        public bool IsImmune
        {
            get => PlayerManager.Instance.PlayersImmune || _immune || GamePlayerOwner.Brawler.CurrentAction.IsImmune;
            set => _immune = value;
        }

        public override bool CanMove => base.CanMove && !IsDead && !Brawler.CurrentAction.IsStunned;

        [SerializeField]
        [ReadOnly]
        private int _currentComboIndex;

        public AttackData CurrentAttack => GamePlayerOwner.PlayerCharacterData.BrawlerData.AttackComboData.AttackData.ElementAt(_currentComboIndex);

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

        public override void OnReSpawn(SpawnPoint spawnpoint)
        {
            base.OnReSpawn(spawnpoint);

            // TODO: add a small window of immunity on spawn
            _immune = false;
        }
#endregion

#region Brawler Actions
        public void OnIdle()
        {
            _idleEffect.Trigger();
        }

        public void OnAttack(AttackBehaviorComponent.AttackAction action)
        {
            ActionPerformed(action);
        }

        public bool OnAdvanceCombo()
        {
            if(_currentComboIndex >= GamePlayerOwner.PlayerCharacterData.BrawlerData.AttackComboData.AttackData.Count - 1) {
                return false;
            }

            _currentComboIndex++;
            return true;
        }

        public void OnComboFail()
        {
            _currentComboIndex = 0;
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

            _brawlerBehavior.CancelActions(false);

            ActionPerformed(JumpBehaviorComponent.JumpAction.Default);
        }

        // TODO: we might want the entire move buffer
        public void Attack(Vector3 lastMove)
        {
            if(!CanAttack) {
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
