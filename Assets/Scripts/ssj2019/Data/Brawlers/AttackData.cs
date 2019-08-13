using System;

using pdxpartyparrot.Core.Math;
using pdxpartyparrot.ssj2019.Players.BehaviorComponents;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Data.Brawlers
{
    [CreateAssetMenu(fileName="AttackData", menuName="pdxpartyparrot/ssj2019/Data/Brawlers/Attack Data")]
    [Serializable]
    public sealed class AttackData : ScriptableObject, IEquatable<AttackData>, IEquatable<AttackBehaviorComponent.AttackAction>
    {
        public enum Direction
        {
            None,
            Up,
            Down,
            Forward,
            Backward
        }

        public static Direction DirectionFromAxes(Vector3 axes)
        {
            return (Direction)(MathUtil.Vector3ToQuadrant(axes) + 1);
        }

        [SerializeField]
        private string _name;

        public string Name => _name;

        [Space(10)]
        
        [SerializeField]
        private Direction _attackDirection = Direction.None;

        public Direction AttackDirection => _attackDirection;

        [SerializeField]
        private bool _airAttack;

        public bool AirAttack => _airAttack;

        [SerializeField]
        private bool _unblockable;

        public bool Unblockable => _unblockable;

        [Space(10)]

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

#region Attack Forces
        [SerializeField]
        [Min(0)]
        private float _moveForward;

        public float MoveFoward => _moveForward;

        [SerializeField]
        [Min(0)]
        private float _pushBackAmount;

        public float PushBackAmount => _pushBackAmount;

        [SerializeField]
        [Min(0)]
        private float _knockUpForce;

        public float KnockUpForce => _knockUpForce;

        [SerializeField]
        [Min(0)]
        private float _knockDownForce;

        public float KnockDownForce => _knockDownForce;
#endregion

        [Space(10)]

#region Special
        [Header("Special")]

        [SerializeField]
        private bool _knockDown;

        public bool KnockDown => _knockDown;
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

#region Effects
        [SerializeField]
        private string _animationName = "default";

        public string AnimationName => _animationName;

        [SerializeField]
        private AudioClip _attackAudioClip;

        public AudioClip AttackAudioClip => _attackAudioClip;

        [SerializeField]
        private AudioClip _impactAudioClip;

        public AudioClip ImpactAudioCip => _impactAudioClip;
#endregion

        public bool Equals(AttackData other)
        {
            return null != other && AttackDirection == other.AttackDirection && AirAttack == other.AirAttack;
        }

        public bool Equals(AttackBehaviorComponent.AttackAction other)
        {
            return null != other && AirAttack != other.IsGrounded && AttackDirection == other.Direction;
        }
    }
}
