using System;
using System.Collections.Generic;

using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.Game.Data.NPCs
{
    [Serializable]
    public abstract class SpawnWaveData : ScriptableObject
    {
        [Serializable]
        public class ReorderableList : ReorderableList<SpawnWaveData>
        {
        }

        [SerializeField]
        [ReorderableList]
        private SpawnGroupData.ReorderableList _spawnGroups = new SpawnGroupData.ReorderableList();

        public IReadOnlyCollection<SpawnGroupData> SpawnGroups => _spawnGroups.Items;
    }
}
