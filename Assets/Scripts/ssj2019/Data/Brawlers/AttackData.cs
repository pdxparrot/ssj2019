using System;

using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Data.Brawlers
{
    [CreateAssetMenu(fileName="AttackData", menuName="pdxpartyparrot/ssj2019/Data/Brawlers/Attack Data")]
    [Serializable]
    public sealed class AttackData : ScriptableObject
    {
        [Serializable]
        public class ReorderableList : ReorderableList<AttackData>
        {
        }

        // TODO: when we have a direction, this should expand to include that
        public string Id => "attack";

#region Damage
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
#endregion

        [Space(10)]

#region Push Back
        [SerializeField]
        [Min(0)]
        private int _pushBackScale;

        public int PushBackScale => _pushBackScale;
#endregion

        [Space(10)]

#region Attack Volume
        [SerializeField]
        private Vector3 _attackVolumeOffset;

        public Vector3 AttackVolumeOffset => _attackVolumeOffset;

        [SerializeField]
        private Vector3 _attackVolumeSize = new Vector3(1.0f, 1.0f, 1.0f);

        public Vector3 AttackVolumeSize => _attackVolumeSize;

        [SerializeField]
        private string _boneName = "root";

        public string BoneName => _boneName;
#endregion

        [Space(10)]

#region Animation
        [SerializeField]
        private string _animationName = "default";

        public string AnimationName => _animationName;
#endregion
    }
}
