using System;
using System.Collections.Generic;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Time;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.Data.NPCs;

using UnityEngine;

namespace pdxpartyparrot.Game.NPCs
{
    public class WaveSpawner : MonoBehaviour
    {
#region Events
        public event EventHandler<SpawnWaveEventArgs> WaveStartEvent;
        public event EventHandler<SpawnWaveEventArgs> WaveCompleteEvent;
#endregion

        [SerializeField]
        private WaveSpawnData _waveSpawnData;

        public WaveSpawnData WaveSpawnData => _waveSpawnData;

        [SerializeField]
        [ReadOnly]
        private int _currentWaveIndex;

        public int CurrentWaveIndex => _currentWaveIndex;

        private bool HasCurrentWave => _currentWaveIndex >= 0 && _currentWaveIndex < _spawnWaves.Count;

        [CanBeNull]
        private SpawnWave CurrentWave => HasCurrentWave ? _spawnWaves[_currentWaveIndex] : null;

        private ITimer _waveTimer;

        private readonly List<SpawnWave> _spawnWaves = new List<SpawnWave>();

#region Unity Lifecycle
        private void Awake()
        {
            foreach(SpawnWaveData spawnWave in _waveSpawnData.Waves) {
                _spawnWaves.Add(new SpawnWave(spawnWave, this));
            }
        }

        private void OnDestroy()
        {
            Shutdown();

            _spawnWaves.Clear();
        }
#endregion

        public void Initialize()
        {
            _currentWaveIndex = -1;

            foreach(SpawnWave spawnWave in _spawnWaves) {
                spawnWave.Initialize();
            }
        }

        public void Shutdown()
        {
            StopSpawner();

            foreach(SpawnWave spawnWave in _spawnWaves) {
                spawnWave.Shutdown();
            }
        }

        public void StartSpawner()
        {
            _waveTimer = TimeManager.Instance.AddTimer();
            _waveTimer.TimesUpEvent += WaveTimerTimesUpEventHandler;

            Run();
        }

        public void StopSpawner()
        {
            if(HasCurrentWave) {
                CurrentWave.Stop();
            }

            if(TimeManager.HasInstance) {
                TimeManager.Instance.RemoveTimer(_waveTimer);
            }
            _waveTimer = null;
        }

        private void Run()
        {
            if(_currentWaveIndex >= _spawnWaves.Count) {
                return;
            }

            // stop the current wave timers
            if(_currentWaveIndex >= 0) {
                CurrentWave.Stop();
                WaveCompleteEvent?.Invoke(this, new SpawnWaveEventArgs(_currentWaveIndex, _currentWaveIndex >= _spawnWaves.Count - 1));
            }

            // advance the wave
            _currentWaveIndex++;
            if(_currentWaveIndex >= _spawnWaves.Count) {
                return;
            }

            // start the next wave of timers
            CurrentWave.Start();
            _waveTimer.Start(CurrentWave.Duration);

            WaveStartEvent?.Invoke(this, new SpawnWaveEventArgs(_currentWaveIndex, _currentWaveIndex >= _spawnWaves.Count - 1));
        }

#region Event Handlers
        private void WaveTimerTimesUpEventHandler(object sender, EventArgs args)
        {
            Run();
        }
#endregion
    }
}
