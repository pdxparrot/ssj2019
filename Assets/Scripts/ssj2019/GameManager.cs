using System;
using System.Collections.Generic;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Camera;
using pdxpartyparrot.Core.Collections;
using pdxpartyparrot.Core.DebugMenu;
using pdxpartyparrot.Core.ObjectPool;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game;
using pdxpartyparrot.Game.UI;
using pdxpartyparrot.ssj2019.Camera;
using pdxpartyparrot.ssj2019.Data;
using pdxpartyparrot.ssj2019.Data.Players;
using pdxpartyparrot.ssj2019.Players;

using UnityEngine;
using UnityEngine.InputSystem;

namespace pdxpartyparrot.ssj2019
{
    public sealed class GameManager : GameManager<GameManager>
    {
        private class PlayerEntry
        {
            public PlayerCharacterData PlayerCharacterData { get; set; }

            public InputDevice Device { get; set; }
        }

#region Debug
        [SerializeField]
        private bool _debugBrawlers;

        public bool DebugBrawlers => _debugBrawlers;
#endregion

        public GameData GameGameData => (GameData)GameData;

        // only valid on the client
        public GameViewer Viewer { get; private set; }

        [SerializeField]
        [ReadOnly]
        private int _score;

        public int Score => _score;

        [SerializeField]
        [ReadOnly]
        private bool _transitionToHighScores;

        public bool TransitionToHighScores
        {
            get => _transitionToHighScores;
            set => _transitionToHighScores = value;
        }

        private readonly Dictionary<short, PlayerEntry> _playerCharacters = new Dictionary<short, PlayerEntry>();

        public IReadOnlyCollection<short> PlayerCharacterControllers => _playerCharacters.Keys;

        private readonly HashSet<Player> _activePlayers = new HashSet<Player>();

        private DebugMenuNode _debugMenuNode;

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            InitDebugMenu();
        }

        protected override void OnDestroy()
        {
            DestroyDebugMenu();

            base.OnDestroy();
        }
#endregion

        public override void Initialize()
        {
            base.Initialize();

            _score = 0;
            _transitionToHighScores = false;
        }

        public override void Shutdown()
        {
            _activePlayers.Clear();
            _playerCharacters.Clear();

            base.Shutdown();
        }

#region Object Pools
        protected override void InitializeObjectPools()
        {
            PooledObject pooledObject = GameGameData.FloatingTextPrefab.GetComponent<PooledObject>();
            ObjectPoolManager.Instance.InitializePoolAsync(GameUIManager.Instance.DefaultFloatingTextPoolName, pooledObject, GameGameData.FloatingTextPoolSize);
        }

        protected override void DestroyObjectPools()
        {
            if(ObjectPoolManager.HasInstance) {
                ObjectPoolManager.Instance.DestroyPool(GameUIManager.Instance.DefaultFloatingTextPoolName);
            }
        }
#endregion

        public override void TransitionScene(string nextScene, Action onComplete)
        {
            base.TransitionScene(nextScene, () => {
                // TODO: this is gross and seems wrong
                StartGameServer();
                StartGameClient();

                onComplete?.Invoke();
            });
        }

        //[Client]
        public void InitViewer()
        {
            Viewer = ViewerManager.Instance.AcquireViewer<GameViewer>(gameObject);
            if(null == Viewer) {
                Debug.LogWarning("Unable to acquire game viewer!");
                return;
            }
            Viewer.Initialize(GameGameData);
        }

        //[Client]
        public void AddPlayerCharacter(short playerControllerId, InputDevice device, PlayerCharacterData playerCharacterData)
        {
            _playerCharacters[playerControllerId] = new PlayerEntry{
                PlayerCharacterData = playerCharacterData,
                Device = device,
            };
        }

        //[Client]
        [CanBeNull]
        public PlayerCharacterData AcquireCharacter(short playerControllerId, out InputDevice device)
        {
            device = null;

            PlayerEntry playerEntry = _playerCharacters.GetOrDefault(playerControllerId);
            if(null == playerEntry) {
                return null;
            }

            _playerCharacters.Remove(playerControllerId);

            device = playerEntry.Device;
            return playerEntry.PlayerCharacterData;
        }

#region Events
        public void PlayerSpawned(Player player)
        {
            _activePlayers.Add(player);
        }

        public void PlayerDied(Player player)
        {
            _activePlayers.Remove(player);

            if(_activePlayers.Count < 1) {
                GameOver();
            }
        }
#endregion

#region Score
        public void NPCBrawlerKilled(int points)
        {
            _score += points;
            PlayerManager.Instance.GamePlayerUI.HUD.SetScore(_score);
        }

        public void PlayerCombo(int points)
        {
            _score += points;
            PlayerManager.Instance.GamePlayerUI.HUD.SetScore(_score);
        }

        public void PlayerHit(int points)
        {
            _score = Mathf.Max(_score - points, 0);
            PlayerManager.Instance.GamePlayerUI.HUD.SetScore(_score);
        }
#endregion

        private void InitDebugMenu()
        {
            _debugMenuNode = DebugMenuManager.Instance.AddNode(() => "ssj2019.GameManager");
            _debugMenuNode.RenderContentsAction = () => {
                _debugBrawlers = GUILayout.Toggle(_debugBrawlers, "Debug Brawlers");
            };
        }

        private void DestroyDebugMenu()
        {
            if(DebugMenuManager.HasInstance) {
                DebugMenuManager.Instance.RemoveNode(_debugMenuNode);
            }
            _debugMenuNode = null;
        }
    }
}
