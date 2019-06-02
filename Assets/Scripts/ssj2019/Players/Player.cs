using System;

using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.Characters.Players;
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

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            Assert.IsTrue(PlayerBehavior is PlayerBehavior);
            Assert.IsTrue(PlayerDriver is PlayerDriver);
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
    }
}
