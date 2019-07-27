using System;

using pdxpartyparrot.Core.DebugMenu;
using pdxpartyparrot.Core.Effects;
using pdxpartyparrot.Core.Effects.EffectTriggerComponents;
using pdxpartyparrot.Core.UI;
using pdxpartyparrot.Core.World;
using pdxpartyparrot.Game.NPCs;

using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace pdxpartyparrot.ssj2019.Level
{
    [RequireComponent(typeof(NavMeshSurface))]
    public sealed class LevelHelper : MonoBehaviour
    {
        [SerializeField]
        private Image _fullScreenImage;

        [SerializeField]
        private Collider2D _cameraBounds;

        [SerializeField]
        private WaveSpawner _waveSpawnerPrefab;

        public WaveSpawner WaveSpawner { get; private set; }

        [SerializeField]
        private string _nextLevel;

        [Space(10)]

        [SerializeField]
        private EffectTrigger _levelEnterEffect;

        [SerializeField]
        private EffectTrigger _levelExitEffect;

        [Space(10)]

        [SerializeField]
        private FadeEffectTriggerComponent _enterFadeEffectTrigger;

        [SerializeField]
        private FadeEffectTriggerComponent _exitFadeEffectTrigger;

        private NavMeshSurface _navMeshSurface;

        private DebugMenuNode _debugMenuNode;

#region Unity Lifecycle
        private void Awake()
        {
            _navMeshSurface = GetComponent<NavMeshSurface>();

            GameManager.Instance.RegisterLevelHelper(this);

            GameManager.Instance.GameStartServerEvent += GameStartServerEventHandler;
            GameManager.Instance.GameStartClientEvent += GameStartClientEventHandler;

            GameManager.Instance.GameReadyEvent += GameReadyEventHandler;

            

            if(null != _enterFadeEffectTrigger) {
                _enterFadeEffectTrigger.Image = _fullScreenImage;
            }

            if(null != _exitFadeEffectTrigger) {
                _exitFadeEffectTrigger.Image = _fullScreenImage;
            }

            InitDebugMenu();
        }

        private void OnDestroy()
        {
            DestroyDebugMenu();

            ShutdownWaveSpawner();

            if(GameManager.HasInstance) {
                GameManager.Instance.GameReadyEvent -= GameReadyEventHandler;

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

        private void TransitionLevel()
        {
            // load the next level if we have one
            if(!string.IsNullOrWhiteSpace(_nextLevel)) {
                GameManager.Instance.TransitionScene(_nextLevel, null);
            } else {
                GameManager.Instance.GameOver();
            }
        }

#region Event Handlers
        private void GameStartServerEventHandler(object sender, EventArgs args)
        {
            // TODO: better to do this before we drop the loading screen and spawn stuff
            _navMeshSurface.BuildNavMesh();

            SpawnManager.Instance.Initialize();

            InitializeWaveSpawner();
        }

        private void GameStartClientEventHandler(object sender, EventArgs args)
        {
            GameManager.Instance.Viewer.SetBounds(_cameraBounds);

            // TODO: we really should communicate our ready state to the server
            // and then have it communicate back to us when everybody is ready
            if(null != _levelEnterEffect) {
                _levelEnterEffect.Trigger(GameManager.Instance.GameReady);
            } else {
                GameManager.Instance.GameReady();
            }
        }

        private void GameReadyEventHandler(object sender, EventArgs args)
        {
            // TODO: this should wait until after all of the players are ready
            if(null != WaveSpawner) {
                WaveSpawner.StartSpawner();
            }
        }

        private void WaveCompleteEventHandler(object sender, SpawnWaveEventArgs args)
        {
            if(!args.IsFinalWave) {
                return;
            }

            if(null != _levelExitEffect) {
                _levelExitEffect.Trigger(TransitionLevel);
            } else {
                TransitionLevel();
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
