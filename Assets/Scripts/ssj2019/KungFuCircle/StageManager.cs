using System.Collections;
using System.Collections.Generic;

using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Core.Collections;
using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.KungFuCircle
{
    public sealed class StageManager : SingletonBehavior<StageManager>
    {
        [SerializeField]
        [ReadOnly]
        private int GridCount;

        private Dictionary<Actor, KungFuGrid> _KungFuGrids = new Dictionary<Actor, KungFuGrid>();

        // This will change in the future but for testing.
        private Dictionary<Actor, int> filledgridslotsindex = new Dictionary<Actor, int>();
        private Dictionary<KungFuGrid, List<Actor>> filledgridslotactors = new Dictionary<KungFuGrid, List<Actor>>();

        private void Start() {
            GridCount = 0;
        }

        public void Register(KungFuGrid grid) {
            GridCount++;
            Actor NewActor = grid.Owner;
            if (NewActor != null)
                _KungFuGrids.Add(NewActor, grid);

        }

        public void Unregister(KungFuGrid grid) {
            GridCount--;
            Actor NewActor = grid.Owner;
            if (NewActor != null)
                _KungFuGrids.Remove(NewActor);
        }

        public Vector3 RequestAttackSlotLocation(Actor target, Actor Attacker) {

            KungFuGrid actorgrid = _KungFuGrids[target];
            // check to see if we have already provided a slot for this NPC
            if (filledgridslotsindex.ContainsKey(Attacker))
            {
                return actorgrid.GetAttackSlotLocation(filledgridslotsindex[Attacker]);
            }

            if (filledgridslotactors.ContainsKey(actorgrid) && !filledgridslotactors[actorgrid].Contains(Attacker))
            {
                Vector3 newattackerdistance = target.Behavior.Movement.Position - Attacker.Behavior.Movement.Position;
                for (int i = 0; i < filledgridslotactors[actorgrid].Count; i++)
                {
                    Actor currentattacker = filledgridslotactors[actorgrid][i];
                    Vector3 attackerdistance = currentattacker.Behavior.Movement.Position - target.Behavior.Movement.Position;

                    if (newattackerdistance.magnitude < attackerdistance.magnitude)
                    {
                        filledgridslotactors[actorgrid].Add(Attacker);
                        filledgridslotactors[actorgrid].Remove(currentattacker);

                        filledgridslotsindex.Add(Attacker, filledgridslotsindex[currentattacker]);
                        filledgridslotsindex.Remove(currentattacker);

                        return actorgrid.GetAttackSlotLocation(filledgridslotsindex[Attacker]);
                    }
                }
            }
            
            // check the grid weight for the target
            // hard coded 5 for now, this will change to be data driven,
            // after i talk with you shane if you read this, 
            // need to find out the best way to pass this data.
            if (actorgrid.HasGridCapacity(5))
            {
                int gridslotindex = actorgrid.GetAvailableGridSlot();
                filledgridslotsindex.Add(Attacker, gridslotindex);
                if (!filledgridslotactors.ContainsKey(actorgrid))
                    filledgridslotactors.Add(actorgrid, new List<Actor>());

                filledgridslotactors[actorgrid].Add(Attacker);
                // hard coded 5 for now, this will change to be data driven,
                // after i talk with you shane if you read this, 
                // need to find out the best way to pass this data.
                actorgrid.FillGridSlot(gridslotindex, 5);
                return actorgrid.GetAttackSlotLocation(gridslotindex);
            }

            return actorgrid.GetOuterSlotLocation(Attacker);
        }

        public bool CanAttackTarget(Actor target, Actor Attacker, int attackweight) {
            KungFuGrid actorgrid = _KungFuGrids[target];

            // check to see if we have already provided a slot for this NPC
            // if the NPC does not have a grid slot it cannot attack
            if (!filledgridslotsindex.ContainsKey(Attacker)) {
                return false;
            }

            return actorgrid.CanBeAttacked(attackweight);
        }

        public void RegisterAttack(Actor target, int attackweight) {
            KungFuGrid actorgrid = _KungFuGrids[target];
            actorgrid.RegisterAttack(attackweight);
        }

        public void ReleaseKungFuGridSlot(Actor target, Actor Attacker) {
            if(null == target) {
                Debug.LogError("Could not release null actor!");
            }

            KungFuGrid actorgrid = _KungFuGrids[target];
            // check to see if we have already provided a slot for this NPC
            if (filledgridslotsindex.ContainsKey(Attacker))
            {
                actorgrid.EmptyGridSlot(filledgridslotsindex[Attacker]);
                filledgridslotsindex.Remove(Attacker);
                filledgridslotactors[actorgrid].Remove(Attacker);
            }
        }
    }
}
