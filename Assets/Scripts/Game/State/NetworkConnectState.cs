#pragma warning disable 0618    // disable obsolete warning for now

using System;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Network;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.UI;

using UnityEngine;
using UnityEngine.Networking;

namespace pdxpartyparrot.Game.State
{
    public sealed class NetworkConnectState : SubGameState
    {
        public enum ConnectType
        {
            Local,
            Server,
            Client
        }

        [SerializeField]
        [CanBeNull]
        private NetworkConnectUI _networkConnectUIPrefab;

        [CanBeNull]
        private NetworkConnectUI _networkConnectUI;

        private GameState _gameStatePrefab;

        private Action<GameState> _gameStateInit;

        [SerializeField]
        [ReadOnly]
        private ConnectType _connectType = ConnectType.Local;

        public void Initialize(ConnectType connectType, GameState gameStatePrefab, Action<GameState> gameStateInit=null)
        {
            _connectType = connectType;
            _gameStatePrefab = gameStatePrefab;
            _gameStateInit = gameStateInit;
        }

        public void Cancel()
        {
            Core.Network.NetworkManager.Instance.DiscoverStop();

            switch(_connectType)
            {
            case ConnectType.Local:
                Core.Network.NetworkManager.Instance.StopHost();
                break;
            case ConnectType.Server:
                Core.Network.NetworkManager.Instance.StopServer();
                break;
            case ConnectType.Client:
                Core.Network.NetworkManager.Instance.StopClient();
                break;
            }

            GameStateManager.Instance.TransitionToInitialStateAsync();
        }

        public override void OnEnter()
        {
            base.OnEnter();

            _networkConnectUI = GameStateManager.Instance.GameUIManager.InstantiateUIPrefab(_networkConnectUIPrefab);
            if(null != _networkConnectUI) {
                _networkConnectUI.Initialize(this);
            }
        }

        protected override void DoEnter()
        {
            base.DoEnter();

            switch(_connectType)
            {
            case ConnectType.Local:
                StartSinglePlayer();
                break;
            case ConnectType.Server:
                StartServer();
                break;
            case ConnectType.Client:
                StartClient();
                break;
            }
        }

        protected override void DoExit()
        {
            if(null != _networkConnectUI) {
                Destroy(_networkConnectUI.gameObject);
            }
            _networkConnectUI = null;

            if(Core.Network.NetworkManager.HasInstance) {
                Core.Network.NetworkManager.Instance.Discovery.ReceivedBroadcastEvent -= ReceivedBroadcastEventHandler;
                Core.Network.NetworkManager.Instance.DiscoverStop();

                Core.Network.NetworkManager.Instance.ServerConnectEvent -= ServerConnectEventHandler;
                Core.Network.NetworkManager.Instance.ClientConnectEvent -= ClientConnectEventHandler;
                Core.Network.NetworkManager.Instance.ClientSceneChangedEvent -= ClientSceneChangedEventHandler;
            }

            base.DoExit();
        }

        private void StartSinglePlayer()
        {
            Core.Network.NetworkManager.Instance.ServerConnectEvent += ServerConnectEventHandler;

            GameStateManager.Instance.NetworkClient = Core.Network.NetworkManager.Instance.StartHost();
            if(!NetworkClient.active) {
                SetStatus("Unable to start network host!");
            }
        }

        private void StartServer()
        {
            Core.Network.NetworkManager.Instance.ServerConnectEvent += ServerConnectEventHandler;

            if(!Core.Network.NetworkManager.Instance.StartServer()) {
                SetStatus("Unable to start network server!");
                return;
            }

            if(!Core.Network.NetworkManager.Instance.DiscoverServer()) {
                SetStatus("Unable to start network server discovery!");
                return;
            }

            SetStatus("Waiting for players...");
        }

        private void StartClient()
        {
            Core.Network.NetworkManager.Instance.Discovery.ReceivedBroadcastEvent += ReceivedBroadcastEventHandler;

            if(!Core.Network.NetworkManager.Instance.DiscoverClient()) {
                SetStatus("Unable to start network client discovery!");
                return;
            }

            SetStatus("Searching for server...");
        }

        private void SetStatus(string status)
        {
            Debug.Log(status);
            if(null != _networkConnectUI) {
                _networkConnectUI.SetStatus(status);
            }
        }

#region Event Handlers
        private void ServerConnectEventHandler(object sender, EventArgs args)
        {
            Core.Network.NetworkManager.Instance.DiscoverStop();

            SetStatus($"Client connected, transitioning to game state...");

            Core.Network.NetworkManager.Instance.ServerChangeScene(_gameStatePrefab.CurrentSceneName);

            GameStateManager.Instance.TransitionStateAsync(_gameStatePrefab, _gameStateInit);
        }

        private void ClientConnectEventHandler(object sender, EventArgs args)
        {
            Core.Network.NetworkManager.Instance.DiscoverStop();

            SetStatus("Connected, waiting for server...");

            Core.Network.NetworkManager.Instance.ClientSceneChangedEvent += ClientSceneChangedEventHandler;
        }

        private void ClientSceneChangedEventHandler(object sender, ClientSceneEventArgs args)
        {
            SetStatus("Server ready, transitioning to game state...");

            GameStateManager.Instance.TransitionStateAsync(_gameStatePrefab, _gameStateInit);
        }

        private void ReceivedBroadcastEventHandler(object sender, ReceivedBroadcastEventArgs args)
        {
            Core.Network.NetworkManager.Instance.Discovery.ReceivedBroadcastEvent -= ReceivedBroadcastEventHandler;
            Core.Network.NetworkManager.Instance.DiscoverStop();

            SetStatus($"Found server at {args.EndPoint}, connecting...");

            Core.Network.NetworkManager.Instance.networkAddress = args.EndPoint.Address.ToString();
            Core.Network.NetworkManager.Instance.networkPort = args.EndPoint.Port;

            Core.Network.NetworkManager.Instance.ClientConnectEvent += ClientConnectEventHandler;

            GameStateManager.Instance.NetworkClient = Core.Network.NetworkManager.Instance.StartClient();
            if(null == GameStateManager.Instance.NetworkClient) {
                SetStatus("Unable to start network client!");
                return;
            }
        }
#endregion
    }
}
