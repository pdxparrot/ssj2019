#pragma warning disable 0618    // disable obsolete warning for now

using System;

using pdxpartyparrot.Core.Network;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.Characters.Players;
using pdxpartyparrot.Game.State;

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Networking;

namespace pdxpartyparrot.Game.Players
{
    [RequireComponent(typeof(NetworkAnimator))]
    public abstract class NetworkPlayer : NetworkActor
    {
        public IPlayer Player => (IPlayer)Actor;

        [SerializeField]
        [ReadOnly]
        private short _controllerId;

        public short ControllerId => _controllerId;

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            Assert.IsTrue(Actor is IPlayer);
        }
#endregion

        public void Initialize(short controllerId)
        {
            _controllerId = controllerId;
        }

// TODO: we could make better use of NetworkBehaviour callbacks in here (and in other NetworkBehaviour types)

#region Callbacks
        [ClientRpc]
        public virtual void RpcSpawn(string id)
        {
            Debug.Log($"Network player {id} spawn");

            Actor.Initialize(Guid.Parse(id), GameStateManager.Instance.PlayerManager.PlayerBehaviorData);
        }
#endregion
    }
}
