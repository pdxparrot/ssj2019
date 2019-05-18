using System;
using System.Collections.Generic;

using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.Game.Data.NPCs
{
    [Serializable]
    public class SpawnWaveData
    {
        [Serializable]
        public class ReorderableList : ReorderableList<SpawnWaveData>
        {
        }

        [SerializeField]
        [Tooltip("The duration of the wave")]
        private float _duration;

        public float Duration => _duration;

        [SerializeField]
        private SpawnGroupData.ReorderableList _spawnGroups = new SpawnGroupData.ReorderableList();

        public IReadOnlyCollection<SpawnGroupData> SpawnGroups => _spawnGroups.Items;

        [SerializeField]
        private AudioClip _waveMusic;

        public AudioClip WaveMusic => _waveMusic;
    }
}
