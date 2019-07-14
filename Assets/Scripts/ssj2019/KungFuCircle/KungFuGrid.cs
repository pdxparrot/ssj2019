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
        int gridcapacity;
        int attackcapacity;

        public int maxgridslots = 1;
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

        public void FillGridSlot(int i) {
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

        public bool CanBeAttacked() {

            for (int i = 0; i < maxgridslots; i++)
            {
                if (!slotstaken[i])
                {
                    return true;
                }
            }
            return false;
        }

        private void OnDestroy() {
            if (StageManager.HasInstance)
                StageManager.Instance.Unregister(this);
        }
    }
}
