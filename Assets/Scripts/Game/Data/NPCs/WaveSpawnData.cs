using System;
using System.Collections.Generic;

using UnityEngine;

namespace pdxpartyparrot.Game.Data.NPCs
{
    // TODO: this is really hard to work with when duplicating data

    [CreateAssetMenu(fileName="WaveSpawnData", menuName="pdxpartyparrot/Game/Data/Wave Spawn Data")]
    [Serializable]
    public class WaveSpawnData : ScriptableObject
    {
        [SerializeField]
        private SpawnWaveData.ReorderableList _waves = new SpawnWaveData.ReorderableList();

        [SerializeField]
        private float _musicTransitionSeconds = 1.0f;

        public float MusicTransitionSeconds => _musicTransitionSeconds;

        public IReadOnlyCollection<SpawnWaveData> Waves => _waves.Items;
    }
}
