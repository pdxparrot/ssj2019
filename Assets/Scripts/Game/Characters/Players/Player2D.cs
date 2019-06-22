#pragma warning disable 0618    // disable obsolete warning for now

using System;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Core.Camera;
using pdxpartyparrot.Core.Data;
using pdxpartyparrot.Core.World;
using pdxpartyparrot.Game.Camera;
using pdxpartyparrot.Game.Data.Characters;
using pdxpartyparrot.Game.Players;
using pdxpartyparrot.Game.Players.Input;
using pdxpartyparrot.Game.State;

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;
using UnityEngine.Serialization;

namespace pdxpartyparrot.Game.Characters.Players
{
    [RequireComponent(typeof(Game.Players.NetworkPlayer))]
    public abstract class Player2D : Actor2D, IPlayer
    {
        public GameObject GameObject => gameObject;

#region Network
        public override bool IsLocalActor => NetworkPlayer.isLocalPlayer;

        // need this to hand off to the NetworkManager before instantiating
        [SerializeField]
        private Game.Players.NetworkPlayer _networkPlayer;

        public Game.Players.NetworkPlayer NetworkPlayer => _networkPlayer;
#endregion

#region Input / Behavior
        [SerializeField]
        [FormerlySerializedAs("_driver")]
        private PlayerInput _input;

        public PlayerInput PlayerInput => _input;

        [CanBeNull]
        public PlayerBehavior PlayerBehavior => (PlayerBehavior)Behavior;
#endregion

#region Viewer
        [CanBeNull]
        public IPlayerViewer PlayerViewer { get; protected set; }

        [CanBeNull]
        public Viewer Viewer
        {
            get => null == PlayerViewer ? null : PlayerViewer.Viewer;
            set {}
        }
#endregion

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            Assert.IsTrue(Behavior is PlayerBehavior);
        }

        protected override void OnDestroy()
        {
            if(null != Viewer && ViewerManager.HasInstance) {
                ViewerManager.Instance.ReleaseViewer(Viewer);
            }
            PlayerViewer = null;

            base.OnDestroy();
        }
#endregion

        public override void Initialize(Guid id, ActorBehaviorData behaviorData)
        {
            Assert.IsTrue(behaviorData is PlayerBehaviorData);

            base.Initialize(id, behaviorData);

            InitializeLocalPlayer(id);
        }

        protected virtual bool InitializeLocalPlayer(Guid id)
        {
            if(!IsLocalActor) {
                return false;
            }

            Debug.Log($"Initializing local player {id}");

            _input.Initialize();

            NetworkPlayer.NetworkTransform.transformSyncMode = NetworkTransform.TransformSyncMode.SyncRigidbody2D;
            NetworkPlayer.NetworkTransform.syncRotationAxis = NetworkTransform.AxisSyncMode.AxisZ;

            PlayerBehavior.InitializeLocalPlayerBehavior();

            return true;
        }

#region Spawn
        public override bool OnSpawn(SpawnPoint spawnpoint)
        {
            if(!base.OnSpawn(spawnpoint)) {
                return false;
            }

            Debug.Log($"Spawning player (controller={NetworkPlayer.playerControllerId}, isLocalPlayer={IsLocalActor})");

            Initialize(Guid.NewGuid(), GameStateManager.Instance.PlayerManager.PlayerBehaviorData);
            if(!NetworkClient.active) {
                NetworkPlayer.RpcSpawn(Id.ToString());
            }

            return true;
        }

        public override bool OnReSpawn(SpawnPoint spawnpoint)
        {
            if(!base.OnReSpawn(spawnpoint)) {
                return false;
            }

            Debug.Log($"Respawning player (controller={NetworkPlayer.playerControllerId}, isLocalPlayer={IsLocalActor})");

            return true;
        }
#endregion
    }
}
