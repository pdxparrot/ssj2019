using System;

using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Data
{
    [CreateAssetMenu(fileName="AttackData", menuName="pdxpartyparrot/ssj2019/Data/Characters/Attack Data")]
    [Serializable]
    public sealed class AttackData : ScriptableObject
    {
        [Serializable]
        public class ReorderableList : ReorderableList<AttackData>
        {
        }

        [SerializeField]
        private string _damageType;

        public string DamageType => _damageType;

        [SerializeField]
        private int _damageAmount = 1;

        public int DamageAmount => _damageAmount;
    }
}
