﻿using System;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Core.World;
using pdxpartyparrot.Game.Actors;
using pdxpartyparrot.Game.Characters.Players;
using pdxpartyparrot.Game.Interactables;
using pdxpartyparrot.ssj2019.Camera;
using pdxpartyparrot.ssj2019.Characters;
using pdxpartyparrot.ssj2019.Data.Players;

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

namespace pdxpartyparrot.ssj2019.Players
{
    [RequireComponent(typeof(NetworkPlayer))]
    [RequireComponent(typeof(Brawler))]
    public sealed class Player : Player3D, IDamagable, IInteractable
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

        [SerializeField]
        [ReadOnly]
        private int _playerNumber = -1;

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

            InputDevice device = GamePlayerInput.GamepadListener.Gamepad;
#if UNITY_EDITOR
            if(null == device) {
                device = Keyboard.current;
            }
#endif

            // TODO: the server needs the character data and model setup
            // so those need to move out of here and into Initialize()
            // and need to not be directly dependent on the input device like they are now

            _playerCharacterData = GameManager.Instance.AcquireCharacter(device, out _playerNumber);
            if(null == _playerCharacterData) {
                // this is "ok", we have a chance to recover when we spawn
                Debug.LogWarning($"Player {Id} failed to get a character");
            } else {
                Debug.Log($"Player {Id} got character {_playerCharacterData.Name}");
            }

            PlayerViewer = GameManager.Instance.Viewer;

            InitializeModel();

            return true;
        }

        private void InitializeModel()
        {
            if(null == Model || null == _playerCharacterData) {
                return;
            }

            Brawler.InitializeModel(Behavior, _playerCharacterData.CharacterModelPrefab, Model, 0);

            PlayerData.PlayerIndicatorState indicatorState = PlayerManager.Instance.GetPlayerIndicatorState(_playerNumber);
            if(null != indicatorState) {
                _playerIndicator.Initialize(indicatorState);
            } else {
                Debug.LogWarning($"Unable to get indicator state for player {_playerNumber}");
            }
        }

#region Spawn
        public override bool OnSpawn(SpawnPoint spawnpoint)
        {
            if(!base.OnSpawn(spawnpoint)) {
                return false;
            }

            if(null == _playerCharacterData) {
                // if something junked up and we didn't get a character, we have a chance to steal one
                _playerCharacterData = GameManager.Instance.AcquireFreeCharacter(out _playerNumber);
                if(null == _playerCharacterData) {
                    // this is bad ya'll
                    Debug.LogError($"No characters available for player {Id}!");
                    return false;
                } 

                Debug.LogWarning($"Player {Id} stole free character {_playerCharacterData.Name}");

                InitializeModel();
            }

            PlayerGameViewer.AddTarget(this);

            Brawler.Initialize(PlayerCharacterData.BrawlerData);

            GameManager.Instance.PlayerSpawned(this);

            return true;
        }

        public override bool OnReSpawn(SpawnPoint spawnpoint)
        {
            if(!base.OnReSpawn(spawnpoint)) {
                return false;
            }

            PlayerGameViewer.AddTarget(this);

            Brawler.Initialize(PlayerCharacterData.BrawlerData);

            GameManager.Instance.PlayerSpawned(this);

            return true;
        }

        public override void OnDeSpawn()
        {
            PlayerGameViewer.RemoveTarget(this);

            base.OnDeSpawn();
        }
#endregion

        public bool Damage(Actor source, string type, int amount, Bounds attackBounds, Vector3 force)
        {
            return GamePlayerBehavior.OnDamage(source, type, amount, attackBounds, force);
        }
    }
}
