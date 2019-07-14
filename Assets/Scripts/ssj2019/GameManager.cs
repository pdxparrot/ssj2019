using System.Collections.Generic;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Camera;
using pdxpartyparrot.Core.Collections;
using pdxpartyparrot.Core.DebugMenu;
using pdxpartyparrot.Core.ObjectPool;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game;
using pdxpartyparrot.Game.State;
using pdxpartyparrot.Game.UI;
using pdxpartyparrot.ssj2019.Camera;
using pdxpartyparrot.ssj2019.Data;
using pdxpartyparrot.ssj2019.Data.Players;
using pdxpartyparrot.ssj2019.Level;
using pdxpartyparrot.ssj2019.Players;

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

namespace pdxpartyparrot.ssj2019
{
    public sealed class GameManager : GameManager<GameManager>
    {
        private class PlayerEntry
        {
            public PlayerCharacterData PlayerCharacterData { get; set; }

            public int PlayerNumber { get;  set;}
        }

#region Debug
        [SerializeField]
        private bool _debugBrawlers;

        public bool DebugBrawlers => _debugBrawlers;
#endregion

        [SerializeField]
        private MainGameState _mainGameStatePrefab;

        public MainGameState MainGameStatePrefab => _mainGameStatePrefab;

        public GameData GameGameData => (GameData)GameData;

        // only valid on the client
        public GameViewer Viewer { get; private set; }

        [SerializeField]
        [ReadOnly]
        private LevelHelper _levelHelper;

        public LevelHelper LevelHelper => _levelHelper;

        private readonly Dictionary<InputDevice, PlayerEntry> _characters = new Dictionary<InputDevice, PlayerEntry>();

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

        public override void Shutdown()
        {
            _characters.Clear();

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

        public void RegisterLevelHelper(LevelHelper levelHelper)
        {
            Assert.IsNull(_levelHelper);
            _levelHelper = levelHelper;
        }

        public void UnRegisterLevelHelper(LevelHelper levelHelper)
        {
            Assert.IsTrue(levelHelper == _levelHelper);
            _levelHelper = null;
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
        public void AddCharacter(InputDevice device, int playerNumber, PlayerCharacterData playerCharacterData)
        {
            _characters[device] = new PlayerEntry(){
                PlayerCharacterData = playerCharacterData,
                PlayerNumber = playerNumber,
            };
        }

        //[Client]
        [CanBeNull]
        public PlayerCharacterData AcquireCharacter(InputDevice device, out int playerNumber)
        {
            playerNumber = -1;

            if(device == null) {
                return null;
            }

            PlayerEntry playerEntry = _characters.GetOrDefault(device);
            if(null == playerEntry) {
                return null;
            }

            _characters.Remove(device);

            playerNumber = playerEntry.PlayerNumber;
            return playerEntry.PlayerCharacterData;
        }

        //[Client]
        [CanBeNull]
        public PlayerCharacterData AcquireFreeCharacter(out int playerNumber)
        {
            foreach(var kvp in _characters) {
                _characters.Remove(kvp.Key);

                playerNumber = kvp.Value.PlayerNumber;
                return kvp.Value.PlayerCharacterData;
            }

            playerNumber = -1;
            return null;
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
