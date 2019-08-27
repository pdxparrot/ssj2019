using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Core.Util;

using UnityEngine;
using UnityEngine.Serialization;

namespace pdxpartyparrot.ssj2019.KungFuCircle
{
    public class KungFuGrid : MonoBehaviour
    {
        // TODO: clean these up so they're not public
        public int maxgridslots = 1;
        public int gridcapacity = 10;
        public int attackcapacity = 10;
        public float degreesbetweenslots = 90;
        public float attackslotdistance = .5f;
        public float outerslotdistance = 1;

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

#region Unity Lifecycle
        private void Awake()
        {
            _slotsTaken = new int[maxgridslots];
            _innerGridSlotsDegrees = new float[maxgridslots];

            float currentDegrees = degreesbetweenslots;
            for(int i = 0; i < maxgridslots; ++i) {
                _innerGridSlotsDegrees[i] += currentDegrees;
                currentDegrees += degreesbetweenslots;
            }
        }
#endregion

        public Vector3 GetAttackSlotLocation(int i)
        {
            // Get the vector form from a quaternion ( i had no idea how else to get it in unity) 
            Vector3 newDirection = Quaternion.AngleAxis(_innerGridSlotsDegrees[i], new Vector3(0.0f, 1.0f, 0.0f)) * new Vector3(1.0f, 1.0f, 1.0f);
            newDirection.Normalize();
            return Owner.Behavior.Movement.Position + (newDirection * attackslotdistance);
        }

        public bool HasGridCapacity(int gridWeight)
        {
            if(gridcapacity - gridWeight < 0) {
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
            gridcapacity -= gridWeight;
            _slotsTaken[i] = gridWeight;
        }

        public void EmptyGridSlot(int i)
        {
            gridcapacity += _slotsTaken[i];
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
            return Owner.Behavior.Movement.Position + (toVector * outerslotdistance);
        }

        // TODO: make use of these I guess?
        // these are pretty bad tho because we have no RAII around it...

        public bool AllocateAttack(int attackWeight)
        {
            if(attackcapacity - attackWeight < 0) {
                return false;
            }

            attackcapacity -= attackWeight;
            return true;
        }

        public void ReleaseAttack(int attackWeight)
        {
            attackcapacity += attackWeight;
        }
    }
}
