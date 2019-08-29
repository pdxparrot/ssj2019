using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Core.Util;

using UnityEngine;
using UnityEngine.Serialization;

namespace pdxpartyparrot.ssj2019.KungFuCircle
{
    public class KungFuGrid : MonoBehaviour
    {
        [SerializeField]
        [FormerlySerializedAs("maxgridslots")]
        private int _maxGridSlots = 4;

        [SerializeField]
        [FormerlySerializedAs("gridcapacity")]
        private int _gridCapacity = 10;

        [SerializeField]
        [FormerlySerializedAs("attackcapacity")]
        private int _attackCapacity = 10;

        [SerializeField]
        [FormerlySerializedAs("attackslotdistance")]
        private float _attackSlotDistance = 0.5f;

        [SerializeField]
        [FormerlySerializedAs("outerslotdistance")]
        private float _outerSlotDistance = 1.0f;

        [SerializeField]
        [FormerlySerializedAs("owner")]
        private Actor _owner;

        public Actor Owner => _owner;

        [SerializeField]
        [ReadOnly]
        private int[] _slotsTaken;

        [SerializeField]
        [ReadOnly]
        private float[] _innerGridSlotsDegrees;

        [SerializeField]
        [ReadOnly]
        private float _degreesBetweenSlots;

#region Unity Lifecycle
        private void Awake()
        {
            _slotsTaken = new int[_maxGridSlots];
            _innerGridSlotsDegrees = new float[_maxGridSlots];

            _degreesBetweenSlots = 360.0f / _maxGridSlots;

            float currentDegrees = _degreesBetweenSlots;
            for(int i = 0; i < _maxGridSlots; ++i) {
                _innerGridSlotsDegrees[i] += currentDegrees;
                currentDegrees += _degreesBetweenSlots;
            }
        }
#endregion

        public Vector3 GetAttackSlotLocation(int i)
        {
            // Get the vector form from a quaternion ( i had no idea how else to get it in unity) 
            Vector3 newDirection = Quaternion.AngleAxis(_innerGridSlotsDegrees[i], new Vector3(0.0f, 1.0f, 0.0f)) * new Vector3(1.0f, 1.0f, 1.0f);
            newDirection.Normalize();
            return Owner.Behavior.Movement.Position + (newDirection * _attackSlotDistance);
        }

        public bool HasGridCapacity(int gridWeight)
        {
            if(_gridCapacity - gridWeight < 0) {
                return false;
            }

            for(int i = 0; i < _slotsTaken.Length; ++i) {
                if(_slotsTaken[i] <= 0)  {
                    return true;
                }
            }
            return false;
        }

        public void FillGridSlot(int i, int gridWeight)
        {
            _gridCapacity -= gridWeight;
            _slotsTaken[i] = gridWeight;
        }

        public void EmptyGridSlot(int i)
        {
            _gridCapacity += _slotsTaken[i];
            _slotsTaken[i] = 0;
        }

        public bool IsGridSlotAvailable(int i)
        {
            return _slotsTaken[i] <= 0;
        }

        public int GetAvailableGridSlot()
        {
            for(int i = 0; i < _slotsTaken.Length; i++) {
                if(_slotsTaken[i] <= 0) {
                    return i;
                }
            }
            return -1;
        }

        public Vector3 GetOuterSlotLocation(Actor attacker)
        {
            // Get the vector form from a quaternion ( i had no idea how else to get it in unity) 
            Vector3 toVector = attacker.Behavior.Movement.Position - Owner.Behavior.Movement.Position;
            toVector.Normalize();
            return Owner.Behavior.Movement.Position + (toVector * _outerSlotDistance);
        }

        // TODO: make use of these I guess?
        // these are pretty bad tho because we have no RAII around it...

        public bool AllocateAttack(int attackWeight)
        {
            if(_attackCapacity - attackWeight < 0) {
                return false;
            }

            _attackCapacity -= attackWeight;
            return true;
        }

        public void ReleaseAttack(int attackWeight)
        {
            _attackCapacity += attackWeight;
        }
    }
}
