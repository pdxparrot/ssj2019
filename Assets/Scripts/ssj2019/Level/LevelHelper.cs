using System;

using pdxpartyparrot.Core.DebugMenu;
using pdxpartyparrot.Core.UI;
using pdxpartyparrot.Core.World;
using pdxpartyparrot.Game.NPCs;

using UnityEngine;
using UnityEngine.AI;

namespace pdxpartyparrot.ssj2019.Level
{
    [RequireComponent(typeof(NavMeshSurface))]
    public sealed class LevelHelper : MonoBehaviour
    {
        [SerializeField]
        private Collider2D _cameraBounds;

        [SerializeField]
        private WaveSpawner _waveSpawnerPrefab;

        public WaveSpawner WaveSpawner { get; private set; }

        private NavMeshSurface _navMeshSurface;

        private DebugMenuNode _debugMenuNode;

#region Unity Lifecycle
        private void Awake()
        {
            _navMeshSurface = GetComponent<NavMeshSurface>();

            GameManager.Instance.RegisterLevelHelper(this);

            GameManager.Instance.GameStartServerEvent += GameStartServerEventHandler;
            GameManager.Instance.GameStartClientEvent += GameStartClientEventHandler;

            InitDebugMenu();
        }

        private void OnDestroy()
        {
            DestroyDebugMenu();

            ShutdownWaveSpawner();

            if(GameManager.HasInstance) {
                GameManager.Instance.GameStartClientEvent -= GameStartClientEventHandler;
                GameManager.Instance.GameStartServerEvent -= GameStartServerEventHandler;

                GameManager.Instance.UnRegisterLevelHelper(this);
            }
        }
#endregion

        private void InitializeWaveSpawner()
        {
            if(null == _waveSpawnerPrefab) {
                return;
            }

            WaveSpawner = Instantiate(_waveSpawnerPrefab);
            WaveSpawner.Initialize();

            WaveSpawner.WaveCompleteEvent += WaveCompleteEventHandler;
        }

        private void ShutdownWaveSpawner()
        {
            if(null == WaveSpawner) {
                return;
            }

            Destroy(WaveSpawner);
            WaveSpawner = null;
        }

        private void SpawnTrainingDummy()
        {
            Debug.Log("Spawning training dummy...");

            Instantiate(GameManager.Instance.GameGameData.TrainingDummyPrefab, transform);
        }

#region Event Handlers
        private void GameStartServerEventHandler(object sender, EventArgs args)
        {
            // TODO: better to do this before we drop the loading screen and spawn stuff
            _navMeshSurface.BuildNavMesh();

            SpawnManager.Instance.Initialize();

            InitializeWaveSpawner();

            // TODO: this should wait until after we have all the players
            if(null != WaveSpawner) {
                WaveSpawner.StartSpawner();
            }
        }

        private void GameStartClientEventHandler(object sender, EventArgs args)
        {
            GameManager.Instance.Viewer.SetBounds(_cameraBounds);
        }

        private void WaveCompleteEventHandler(object sender, SpawnWaveEventArgs args)
        {
            if(args.IsFinalWave) {
                GameManager.Instance.GameOver();
            }
        }
#endregion

        private void InitDebugMenu()
        {
            _debugMenuNode = DebugMenuManager.Instance.AddNode(() => "ssj2019.Level");
            _debugMenuNode.RenderContentsAction = () => {
                if(GUIUtils.LayoutButton("Spawn Training Dummy")) {
                    SpawnTrainingDummy();
                }
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
