using System.Collections;
using System.Collections.Generic;

using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Core.Collections;
using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.KungFuCircle
{
    public class KungFuGrid : MonoBehaviour
    {

        public int maxgridslots = 1;
        public int gridcapacity = 10;
        public int attackcapacity = 10;
        public float degreesbetweenslots = 90;
        public float attackslotdistance = .5f;
        public float outerslotdistance = 1;

        [SerializeField]
        private Actor owner;

        public Actor Owner => owner;

        bool[] slotstaken;
        float[] innergridslotsdegrees;

        // Start is called before the first frame update
        void Start() {
            StageManager.Instance.Register(this);
            slotstaken = new bool[maxgridslots];
            innergridslotsdegrees = new float[maxgridslots];

            float currentdegrees = degreesbetweenslots;
            for (int i = 0; i < maxgridslots; i++) {
                innergridslotsdegrees[i] += currentdegrees;
                currentdegrees += degreesbetweenslots;
            }
        }

        public Vector3 GetAttackSlotLocation(int i) {
            // Get the vector form from a quaternion ( i had no idea how else to get it in unity) 
            Vector3 NewDirection = Quaternion.AngleAxis(innergridslotsdegrees[i], new Vector3(0, 1, 0)) * new Vector3(1, 1, 1);
            NewDirection.Normalize();
            Vector3 NewLocation = (Owner.Behavior.Movement.Position + (NewDirection * attackslotdistance));
            return NewLocation;
        }

        public void FillGridSlot(int i, int gridweight) {
            gridcapacity -= gridweight;
            slotstaken[i] = true;
        }

        public void EmptyGridSlot(int i) {
            slotstaken[i] = false;
        }

        public bool IsGridSlotAvailable(int i)
        {
            return slotstaken[i];
        }


        public int GetAvailableGridSlot() {
            for (int i = 0; i < maxgridslots; i++)
            {
                if (!slotstaken[i])
                {
                    return i;
                }
            }
            return -1;
        }

        public Vector3 GetOuterSlotLocation(Actor Attacker) {
            // Get the vector form from a quaternion ( i had no idea how else to get it in unity) 
            Vector3 ToVector = Attacker.Behavior.Movement.Position - Owner.Behavior.Movement.Position;
            ToVector.Normalize();
            Vector3 NewLocation = (Owner.Behavior.Movement.Position + (ToVector * outerslotdistance));
            return NewLocation;
        }

        public bool HasGridCapacity(int gridweight) {
            if ((gridcapacity - gridweight) < 0) {
                return false;
            }

            for (int i = 0; i < maxgridslots; i++)
            {
                if (!slotstaken[i])
                {
                    return true;
                }
            }
            return false;
        }

        public bool CanBeAttacked(int attackweight) {
            if ((attackcapacity - attackweight) < 0)
            {
                return false;
            }
            return true;
        }

        public void RegisterAttack(int attackweight) {
            attackcapacity -= attackweight;
        }

        private void OnDestroy() {
            if (StageManager.HasInstance)
                StageManager.Instance.Unregister(this);
        }
    }
}
