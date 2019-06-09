using System;
using System.Collections.Generic;
using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Data
{
    [CreateAssetMenu(fileName="AttackComboData", menuName="pdxpartyparrot/ssj2019/Data/Characters/AttackCombo Data")]
    [Serializable]
    public sealed class AttackComboData : ScriptableObject
    {
        [SerializeField]
        [ReorderableList]
        private AttackData.ReorderableList _attackData = new AttackData.ReorderableList();

        public IReadOnlyCollection<AttackData> AttackData => _attackData.Items;
    }
}
