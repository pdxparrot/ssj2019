using System;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Core.Data;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Core.World;
using pdxpartyparrot.Game.Actors;
using pdxpartyparrot.Game.Characters.Players;
using pdxpartyparrot.Game.Interactables;
using pdxpartyparrot.Game.UI;
using pdxpartyparrot.ssj2019.Camera;
using pdxpartyparrot.ssj2019.Characters.Brawlers;
using pdxpartyparrot.ssj2019.Data.Players;

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

namespace pdxpartyparrot.ssj2019.Players
{
    [RequireComponent(typeof(NetworkPlayer))]
    [RequireComponent(typeof(Brawler))]
    public sealed class Player : Player25D, IDamagable, IInteractable
    {
        public PlayerInput GamePlayerInput => (PlayerInput)PlayerInput;

        public PlayerBehavior GamePlayerBehavior => (PlayerBehavior)PlayerBehavior;

        [SerializeField]
        [ReadOnly]
        [CanBeNull]
        private PlayerCharacterData _playerCharacterData;

        public PlayerCharacterData PlayerCharacterData => _playerCharacterData;

        [SerializeField]
        private PlayerIndicator _playerIndicator;

        private GameViewer PlayerGameViewer => (GameViewer)Viewer;

        public bool IsDead => Brawler.Health < 1;

        public bool CanInteract => !IsDead;

        public Brawler Brawler { get; private set; }

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            Assert.IsTrue(PlayerInput is PlayerInput);
            Assert.IsTrue(PlayerBehavior is PlayerBehavior);

            Brawler = GetComponent<Brawler>();
        }
#endregion

        protected override bool InitializeLocalPlayer(Guid id)
        {
            if(!base.InitializeLocalPlayer(id)) {
                return false;
            }

            // TODO: the server needs the character data and model setup
            // so those need to move out of here and into Initialize()
            // and need to not be directly dependent on the input device like they are now

            _playerCharacterData = GameManager.Instance.AcquireCharacter(NetworkPlayer.ControllerId, out InputDevice device);
            if(null == _playerCharacterData) {
                Debug.LogError($"Player {Id} failed to get a character ({NetworkPlayer.ControllerId})");
                return false;
            }

            if(null == device || device is Gamepad) {
                GamePlayerInput.SetGamepad((Gamepad)device);
            }

            PlayerManager.Instance.GamePlayerUI.HUD.ShowCharacterPanel(NetworkPlayer.ControllerId, _playerCharacterData);

            Brawler.Initialize(this, _playerCharacterData.BrawlerData);

            PlayerViewer = GameManager.Instance.Viewer;

            InitializeModel();

            return true;
        }

        private void InitializeModel()
        {
            if(null == Model || null == _playerCharacterData) {
                return;
            }

            Brawler.InitializeModel(Behavior, _playerCharacterData.BrawlerModelPrefab, Model, 0);

            PlayerData.PlayerIndicatorState indicatorState = PlayerManager.Instance.GetPlayerIndicatorState(NetworkPlayer.ControllerId);
            if(null != indicatorState) {
                _playerIndicator.Initialize(indicatorState);
            } else {
                Debug.LogWarning($"Unable to get indicator state for player {NetworkPlayer.ControllerId}");
            }
        }

#region Spawn
        public override bool OnSpawn(SpawnPoint spawnpoint)
        {
            if(!base.OnSpawn(spawnpoint)) {
                return false;
            }

            PlayerGameViewer.AddTarget(this);

            Brawler.OnSpawn();

            GameManager.Instance.PlayerSpawned(this);

            return true;
        }

        public override bool OnReSpawn(SpawnPoint spawnpoint)
        {
            if(!base.OnReSpawn(spawnpoint)) {
                return false;
            }

            PlayerGameViewer.AddTarget(this);

            Brawler.OnReSpawn();

            GameManager.Instance.PlayerSpawned(this);

            return true;
        }

        public override void OnDeSpawn()
        {
            PlayerGameViewer.RemoveTarget(this);

            base.OnDeSpawn();
        }
#endregion

        public bool Damage(DamageData damageData)
        {
            return GamePlayerBehavior.OnDamage(damageData);
        }
    }
}
