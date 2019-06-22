using System;

using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Core.World;
using pdxpartyparrot.Game.NPCs;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Level
{
    public sealed class LevelHelper : MonoBehaviour
    {
        [SerializeField]
        [ReadOnly]
        private Collider2D _cameraBounds;

        public Collider2D CameraBounds => _cameraBounds;

        [SerializeField]
        private WaveSpawner _waveSpawnerPrefab;

        private WaveSpawner _waveSpawner;

        public WaveSpawner WaveSpawner => _waveSpawner;

#region Unity Lifecycle
        private void Awake()
        {
            GameManager.Instance.RegisterLevelHelper(this);

            GameManager.Instance.GameStartServerEvent += GameStartServerEventHandler;
            GameManager.Instance.GameStartClientEvent += GameStartClientEventHandler;
        }

        private void OnDestroy()
        {
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

            _waveSpawner = Instantiate(_waveSpawnerPrefab);
            _waveSpawner.Initialize();

            _waveSpawner.WaveCompleteEvent += WaveCompleteEventHandler;
        }

        private void ShutdownWaveSpawner()
        {
            if(null == _waveSpawner) {
                return;
            }

            Destroy(_waveSpawner);
            _waveSpawner = null;
        }

#region Event Handlers
        private void GameStartServerEventHandler(object sender, EventArgs args)
        {
            SpawnManager.Instance.Initialize();

            InitializeWaveSpawner();

            // TODO: this should wait until after we have all the players
            if(null != _waveSpawner) {
                _waveSpawner.StartSpawner();
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
    }
}
