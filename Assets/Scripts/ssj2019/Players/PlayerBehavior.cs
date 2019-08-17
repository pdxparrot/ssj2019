using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Core.Data;
using pdxpartyparrot.Core.Effects.EffectTriggerComponents;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Core.World;
using pdxpartyparrot.Game.Actors;
using pdxpartyparrot.Game.Characters.Players;
using pdxpartyparrot.ssj2019.Characters.Brawlers;
using pdxpartyparrot.ssj2019.Data.Players;

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

            _brawlerBehavior.ActionHandler = this;
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
        public void OnHit(bool blocked)
        {
            PlayerManager.Instance.GamePlayerUI.HUD.SetPlayerHealthPercent(GamePlayerOwner.NetworkPlayer.ControllerId, Brawler.HealthPercent);
            GameManager.Instance.PlayerHit(PlayerManager.Instance.PlayerData.HitPoints);
        }

        public void OnComboMove(bool isOpener, ComboMove move)
        {
            // openers don't score
            if(isOpener) {
                return;
            }

            // TODO: only score on non-opener attack hits

            GameManager.Instance.PlayerCombo(move.ComboPoints);
        }

        public void OnDead()
        {
            ClearActionBuffer();

            GameManager.Instance.PlayerDied(GamePlayerOwner);
        }

        public void OnDeathComplete()
        {
        }
#endregion

#region Actions
        public void Jump()
        {
            _brawlerBehavior.Jump();
        }

        public void Attack(Vector3 lastMove)
        {
            _brawlerBehavior.Attack(lastMove, IsGrounded);
        }

        public void StartBlock(Vector3 lastMove)
        {
            _brawlerBehavior.StartBlock(lastMove);
        }

        public void EndBlock()
        {
            _brawlerBehavior.EndBlock();
        }

        public void Dash()
        {
            _brawlerBehavior.Dash();
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
