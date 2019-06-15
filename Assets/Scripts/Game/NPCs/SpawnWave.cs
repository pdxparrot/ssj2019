using System;
using System.Collections.Generic;

using pdxpartyparrot.Core.Audio;
using pdxpartyparrot.Core.Time;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.Data.NPCs;

using UnityEngine;

namespace pdxpartyparrot.Game.NPCs
{
    [Serializable]
    internal class SpawnWave
    {
        private readonly SpawnWaveData _spawnWaveData;

        [SerializeField]
        [ReadOnly]
        private /*readonly*/ List<SpawnGroup> _spawnGroups = new List<SpawnGroup>();

        private TimedSpawnWaveData TimedSpawnWaveData => (TimedSpawnWaveData)_spawnWaveData;

        private readonly WaveSpawner _owner;

        [SerializeField]
        [ReadOnly]
        private int _spawnedCount;

        private ITimer _waveTimer;

        public SpawnWave(SpawnWaveData spawnWaveData, WaveSpawner owner)
        {
            _spawnWaveData = spawnWaveData;
            _owner = owner;

            foreach(SpawnGroupData spawnGroup in _spawnWaveData.SpawnGroups) {
                _spawnGroups.Add(new SpawnGroup(spawnGroup, _owner, this));
            }
        }

        public void Initialize()
        {
            if(_spawnWaveData is TimedSpawnWaveData) {
                _waveTimer = TimeManager.Instance.AddTimer();
                _waveTimer.TimesUpEvent += WaveTimerTimesUpEventHandler;
            }

            foreach(SpawnGroup spawnGroup in _spawnGroups) {
                spawnGroup.Initialize();
            }
        }

        public void Shutdown()
        {
            foreach(SpawnGroup spawnGroup in _spawnGroups) {
                spawnGroup.Shutdown();
            }
        }

        public void Start()
        {
            if(_spawnWaveData is TimedSpawnWaveData) {
                _waveTimer.Start(TimedSpawnWaveData.Duration);
            }

            _spawnedCount = 0;

            foreach(SpawnGroup spawnGroup in _spawnGroups) {
                spawnGroup.Start();
            }
            AudioManager.Instance.TransitionMusicAsync(_spawnWaveData.WaveMusic, _owner.WaveSpawnData.MusicTransitionSeconds);
        }

        public void Stop()
        {
            foreach(SpawnGroup spawnGroup in _spawnGroups) {
                spawnGroup.Stop();
            }

            _spawnedCount = 0;

            if(TimeManager.HasInstance) {
                TimeManager.Instance.RemoveTimer(_waveTimer);
            }
            _waveTimer = null;
        }

#region Events
        public void OnWaveSpawned(int count)
        {
            _spawnedCount += count;
        }

        public void OnWaveSpawnMemberDone()
        {
            _spawnedCount--;

            if(_spawnedCount <= 0) {
                _owner.Advance();
            }
        }
#endregion

#region Event Handlers
        private void WaveTimerTimesUpEventHandler(object sender, EventArgs args)
        {
            _owner.Advance();
        }
#endregion
    }
}
