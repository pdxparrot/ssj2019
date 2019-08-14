#pragma warning disable 0618    // disable obsolete warning for now

using System;

using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.Data;
using pdxpartyparrot.Game.State;

using UnityEngine;
using UnityEngine.Networking;

namespace pdxpartyparrot.Game
{
    public interface IGameManager
    {
#region Events
        event EventHandler<EventArgs> GameStartServerEvent;
        event EventHandler<EventArgs> GameStartClientEvent;

        event EventHandler<EventArgs> GameReadyEvent;
        event EventHandler<EventArgs> GameUnReadyEvent;
        event EventHandler<EventArgs> GameOverEvent;
#endregion

        GameData GameData { get; }

        bool IsGameReady { get; }

        bool IsGameOver { get; }

        void Initialize();

        void Shutdown();
    }

    public abstract class GameManager<T> : SingletonBehavior<T>, IGameManager where T: GameManager<T>
    {
#region Events
        public event EventHandler<EventArgs> GameStartServerEvent;
        public event EventHandler<EventArgs> GameStartClientEvent;

        public event EventHandler<EventArgs> GameReadyEvent;
        public event EventHandler<EventArgs> GameUnReadyEvent;
        public event EventHandler<EventArgs> GameOverEvent;
#endregion

        [SerializeField]
        private GameData _gameData;

        public GameData GameData => _gameData;

        [SerializeField]
        [ReadOnly]
        private bool _isGameReady;

        public virtual bool IsGameReady
        {
            get => _isGameReady;
            private set => _isGameReady = value;
        }

        [SerializeField]
        [ReadOnly]
        private bool _isGameOver;

        public virtual bool IsGameOver
        {
            get => _isGameOver;
            private set => _isGameOver = value;
        }

        // this state is not reset between games
        // so the main menu can make decisions
        // based on whether a game has been played or not
        [SerializeField]
        [ReadOnly]
        private bool _hasGameStarted;

        public bool HasGameStarted
        {
            get => _hasGameStarted;
            private set => _hasGameStarted = value;
        }

#region Unity Lifecycle
        protected virtual void Awake()
        {
            GameStateManager.Instance.RegisterGameManager(this);
        }

        protected override void OnDestroy()
        {
            if(GameStateManager.HasInstance) {
                GameStateManager.Instance.UnregisterGameManager();
            }

            base.OnDestroy();
        }
#endregion

        public virtual void Initialize()
        {
            IsGameOver = false;
            IsGameReady = false;

            InitializeObjectPools();
        }

        public virtual void Shutdown()
        {
            IsGameOver = false;
            IsGameReady = false;

            DestroyObjectPools();

            if(NetworkServer.active && null != GameStateManager.Instance.PlayerManager) {
                GameStateManager.Instance.PlayerManager.DespawnPlayers();
            }
        }

        protected virtual void InitializeObjectPools()
        {
        }

        protected virtual void DestroyObjectPools()
        {
        }

        public virtual void StartGameServer()
        {
            Debug.Log("Start Game (Server)");

            GameStartServerEvent?.Invoke(this, EventArgs.Empty);
        }

        public virtual void StartGameClient()
        {
            Debug.Log("Start Game (Client)");

            GameStartClientEvent?.Invoke(this, EventArgs.Empty);
        }

        public virtual void GameReady()
        {
            Debug.Log("Game Ready");

            IsGameReady = true;
            HasGameStarted = true;

            GameReadyEvent?.Invoke(this, EventArgs.Empty);
        }

        public virtual void GameUnReady()
        {
            Debug.Log("Game UnReady");

            IsGameReady = false;

            GameUnReadyEvent?.Invoke(this, EventArgs.Empty);
        }

        public virtual void GameOver()
        {
            Debug.Log("Game Over");

            IsGameOver = true;

            GameOverEvent?.Invoke(this, EventArgs.Empty);
        }

        public virtual void TransitionScene(string nextScene, Action onComplete)
        {
            GameStateManager.Instance.CurrentState.ChangeSceneAsync(nextScene, onComplete);
        }
    }
}
