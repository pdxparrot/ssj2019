using System;

using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Core.World;
using pdxpartyparrot.Game.Characters.Players;
using pdxpartyparrot.ssj2019.Camera;
using pdxpartyparrot.ssj2019.Data;

using UnityEngine;
using UnityEngine.Assertions;

namespace pdxpartyparrot.ssj2019.Players
{
    [RequireComponent(typeof(NetworkPlayer))]
    public sealed class Player : Player3D
    {
        public PlayerBehavior GamePlayerBehavior => (PlayerBehavior)PlayerBehavior;

        [SerializeField]
        [ReadOnly]
        private PlayerCharacterData _playerCharacterData;

        public PlayerCharacterData PlayerCharacterData => _playerCharacterData;

        private GameViewer PlayerGameViewer => (GameViewer)Viewer;

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            Assert.IsTrue(PlayerBehavior is PlayerBehavior);
            Assert.IsTrue(PlayerInput is PlayerInput);
        }
#endregion

        protected override bool InitializeLocalPlayer(Guid id)
        {
            if(!base.InitializeLocalPlayer(id)) {
                return false;
            }

            PlayerViewer = GameManager.Instance.Viewer;

            Billboard billboard = Model.GetComponent<Billboard>();
            if(billboard != null) {
                billboard.Camera = PlayerViewer.Viewer.Camera;
            }

            return true;
        }

#region Spawn
        public override bool OnSpawn(SpawnPoint spawnpoint)
        {
            if(!base.OnSpawn(spawnpoint)) {
                return false;
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
