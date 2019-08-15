using System;

using pdxpartyparrot.Game.Data.NPCs;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Data.NPCs
{
    [CreateAssetMenu(fileName="GroupedSpawnWaveData", menuName="pdxpartyparrot/ssj2019/Data/Wave Spawner/GroupedSpawnWave Data")]
    [Serializable]
    public class GroupedSpawnWaveData : SpawnWaveData
    {
        [SerializeField]
        private AudioClip _waveMusic;

        public AudioClip WaveMusic => _waveMusic;
        
        [SerializeField]
        private float _musicTransitionSeconds = 1.0f;

        public float MusicTransitionSeconds => _musicTransitionSeconds;
    }
}
