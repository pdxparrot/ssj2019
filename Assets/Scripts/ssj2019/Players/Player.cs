using System;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Core.World;
using pdxpartyparrot.Game.Characters.Players;
using pdxpartyparrot.ssj2019.Camera;
using pdxpartyparrot.ssj2019.Characters;
using pdxpartyparrot.ssj2019.Data;

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

namespace pdxpartyparrot.ssj2019.Players
{
    [RequireComponent(typeof(NetworkPlayer))]
    public sealed class Player : Player3D
    {
        public PlayerInput GamePlayerInput => (PlayerInput)PlayerInput;

        public PlayerBehavior GamePlayerBehavior => (PlayerBehavior)PlayerBehavior;

        [SerializeField]
        [ReadOnly]
        [CanBeNull]
        private PlayerCharacterData _playerCharacterData;

        private GameViewer PlayerGameViewer => (GameViewer)Viewer;

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            Assert.IsTrue(PlayerInput is PlayerInput);
            Assert.IsTrue(PlayerBehavior is PlayerBehavior);
            Assert.IsTrue(PlayerInput is PlayerInput);
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

            _playerCharacterData = GameManager.Instance.AcquireCharacter(device);
            if(null == _playerCharacterData) {
                // this is "ok", we have a chance to recover when we spawn
                Debug.LogWarning($"Player {Id} failed to get a character");
            } else {
                Debug.Log($"Player {Id} got character {_playerCharacterData.Name}");
            }

            InitializeModel();

            PlayerViewer = GameManager.Instance.Viewer;

            Billboard billboard = Model.GetComponent<Billboard>();
            if(billboard != null) {
                billboard.Camera = PlayerViewer.Viewer.Camera;
            }

            return true;
        }

        private void InitializeModel()
        {
            if(null == Model || null == _playerCharacterData) {
                return;
            }

            CharacterModel model = Instantiate(_playerCharacterData.CharacterModelPrefab, Model.transform);

            Behavior.SpriteAnimationHelper.AddRenderer(model.ModelSprite);
            Behavior.SpriteAnimationHelper.AddRenderer(model.ShadowSprite);
        }

#region Spawn
        public override bool OnSpawn(SpawnPoint spawnpoint)
        {
            if(!base.OnSpawn(spawnpoint)) {
                return false;
            }

            if(null == _playerCharacterData) {
                // if something junked up and we didn't get a character, we have a chance to steal one
                // TODO: we have no model tho, ugh :(
                _playerCharacterData = GameManager.Instance.AcquireFreeCharacter();
                if(null == _playerCharacterData) {
                    // this is bad ya'll
                    Debug.LogError($"No characters available for player {Id}!");
                    return false;
                } 
                Debug.LogWarning($"Player {Id} stole free character {_playerCharacterData.Name}");
            }

            PlayerGameViewer.AddTarget(this);

            return true;
        }

        public override bool OnReSpawn(SpawnPoint spawnpoint)
        {
            if(!base.OnReSpawn(spawnpoint)) {
                return false;
            }

            PlayerGameViewer.AddTarget(this);

            return true;
        }

        public override void OnDeSpawn()
        {
            PlayerGameViewer.RemoveTarget(this);

            base.OnDeSpawn();
        }
#endregion
    }
}
