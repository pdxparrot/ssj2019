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
        [Min(0)]
        private int _damageAmount = 1;

        public int DamageAmount => _damageAmount;

        [SerializeField]
        [Min(0)]
        private int _blockDamageAmount;

        public int BlockDamageAmount => _blockDamageAmount;

        [SerializeField]
        [Min(0)]
        private int _pushBackScale;

        public int PushBackScale => _pushBackScale;

        [SerializeField]
        private Vector3 _attackVolumeOffset;

        public Vector3 AttackVolumeOffset => _attackVolumeOffset;

        [SerializeField]
        private Vector3 _attackVolumeSize = new Vector3(1.0f, 1.0f, 1.0f);

        public Vector3 AttackVolumeSize => _attackVolumeSize;

        [SerializeField]
        private string _animationName = "default";

        public string AnimationName => _animationName;

        [SerializeField]
        private string _boneName = "root";

        public string BoneName => _boneName;
    }
}
