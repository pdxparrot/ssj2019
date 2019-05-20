#pragma warning disable 0618    // disable obsolete warning for now

using System;

using pdxpartyparrot.Core.ObjectPool;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.Data;
using pdxpartyparrot.Game.State;
using pdxpartyparrot.Game.UI;

using UnityEngine;
using UnityEngine.Networking;

namespace pdxpartyparrot.Game
{
    public interface IGameManager
    {
        GameData GameData { get; }

        bool IsGameOver { get; }

        void Initialize();

        void Shutdown();
    }

    public abstract class GameManager<T> : SingletonBehavior<T>, IGameManager where T: GameManager<T>
    {
#region Events
        public event EventHandler<EventArgs> GameStartEvent;
        public event EventHandler<EventArgs> GameOverEvent;
#endregion

        [SerializeField]
        private GameData _gameData;

        public GameData GameData => _gameData;

        [SerializeField]
        [ReadOnly]
        private bool _isGameOver;

        public virtual bool IsGameOver
        {
            get => _isGameOver;
            protected  set => _isGameOver = value;
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

            InitializeObjectPools();
        }

        public virtual void Shutdown()
        {
            IsGameOver = false;

            DestroyObjectPools();

            if(NetworkServer.active && null != GameStateManager.Instance.PlayerManager) {
                GameStateManager.Instance.PlayerManager.DespawnPlayers();
            }
        }

        public virtual void InitializeObjectPools()
        {
        }

        public virtual void DestroyObjectPools()
        {
        }
    }
}
