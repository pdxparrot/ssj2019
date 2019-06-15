using System;

using pdxpartyparrot.Game.NPCs;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Level
{
    public sealed class LevelHelper : MonoBehaviour
    {
        [SerializeField]
        private Collider2D _cameraBounds;

        public Collider2D CameraBounds => _cameraBounds;

        [SerializeField]
        private WaveSpawner _waveSpawnerPrefab;

        private WaveSpawner _waveSpawner;

#region Unity Lifecycle
        private void Awake()
        {
            GameManager.Instance.GameStartServerEvent += GameStartServerEventHandler;
            GameManager.Instance.GameStartClientEvent += GameStartClientEventHandler;
        }

        private void OnDestroy()
        {
            if(GameManager.HasInstance) {
                GameManager.Instance.GameStartClientEvent -= GameStartClientEventHandler;
                GameManager.Instance.GameStartServerEvent -= GameStartServerEventHandler;
            }

            if(null != _waveSpawner) {
                Destroy(_waveSpawner);
            }
            _waveSpawner = null;
        }
#endregion

        private void InitializeWaveSpawner()
        {
            if(null == _waveSpawnerPrefab) {
                return;
            }

            _waveSpawner = Instantiate(_waveSpawnerPrefab);
        }

#region Event Handlers
        private void GameStartServerEventHandler(object sender, EventArgs args)
        {
            InitializeWaveSpawner();
        }

        private void GameStartClientEventHandler(object sender, EventArgs args)
        {
            GameManager.Instance.Viewer.SetBounds(_cameraBounds);
        }
#endregion
    }
}
