using System.Collections;
using System.Collections.Generic;

using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Core.KungFuCircle;
using pdxpartyparrot.Core.Collections;
using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.Core.KungFuCircle
{
    public class KungFuGrid : MonoBehaviour
    {
        int gridcapacity;
        int attackcapacity;

        public int maxgridslots = 1;
        public float degreesbetweenslots = 90;
        public float attackslotdistance = .5f;
        public float outerslotdistance = 1;

        private Transform ownertransform;

        bool[] slotstaken;
        float[] innergridslotsdegrees;

        // Start is called before the first frame update
        void Start()
        {
            StageManager.Instance.Register(this);
            ownertransform = GetComponentInParent<Transform>();
            slotstaken = new bool[maxgridslots];
            innergridslotsdegrees = new float[maxgridslots];

            float currentdegrees = degreesbetweenslots;
            for (int i = 0; i < maxgridslots; i++) {
                innergridslotsdegrees[i] += currentdegrees;
                currentdegrees += degreesbetweenslots;
            }
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        public Vector3 GetAttackSlotLocation(int i) {
            ownertransform = GetComponentInParent<Transform>();
            // Get the vector form from a quaternion ( i had no idea how else to get it in unity) 
            Vector3 NewDirection = Quaternion.AngleAxis(innergridslotsdegrees[i], new Vector3(0, 1, 0)) * new Vector3(1, 1, 1);
            NewDirection.Normalize();
            Vector3 NewLocation = (ownertransform.position + (NewDirection * attackslotdistance));
            return NewLocation;
        }

        public void FillGridSlot(int i)
        {
            slotstaken[i] = true;
        }

        public bool IsGridSlotAvailable(int i)
        {
            return slotstaken[i];
        }

        public int GetAvailableGridSlot()
        {
            for (int i = 0; i < maxgridslots; i++)
            {
                if (!slotstaken[i])
                {
                    return i;
                }
            }
            return -1;
        }

        public Vector3 GetOuterSlotLocation(Actor Attacker)
        {
            ownertransform = GetComponentInParent<Transform>();
            // Get the vector form from a quaternion ( i had no idea how else to get it in unity) 
            Vector3 ToVector = Attacker.transform.position - ownertransform.position;
            ToVector.Normalize();
            Vector3 NewLocation = (ownertransform.position + (ToVector * outerslotdistance));
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

        private void OnDestroy()
        {
            if (StageManager.HasInstance)
                StageManager.Instance.Unregister(this);
        }
    }
}
