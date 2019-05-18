using System;
using System.Collections.Generic;

using pdxpartyparrot.Core.Audio;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.Data.NPCs;

using UnityEngine;

namespace pdxpartyparrot.Game.NPCs
{
    [Serializable]
    internal class SpawnWave
    {
        private readonly SpawnWaveData _spawnWaveData;

        public float Duration => _spawnWaveData.Duration;

        [SerializeField]
        [ReadOnly]
        private /*readonly*/ List<SpawnGroup> _spawnGroups = new List<SpawnGroup>();

        private readonly WaveSpawner _owner;

        public SpawnWave(SpawnWaveData spawnWaveData, WaveSpawner owner)
        {
            _spawnWaveData = spawnWaveData;
            _owner = owner;

            foreach(SpawnGroupData spawnGroup in _spawnWaveData.SpawnGroups) {
                _spawnGroups.Add(new SpawnGroup(spawnGroup, _owner));
            }
        }

        public void Initialize()
        {
            foreach(SpawnGroup spawnGroup in _spawnGroups) {
                spawnGroup.Initialize(Duration);
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
        }
    }
}
