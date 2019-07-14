using System.Collections;
using System.Collections.Generic;

using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Core.Collections;
using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.Core.KungFuCircle
{
    public sealed class StageManager : SingletonBehavior<StageManager>
    {
        [SerializeField]
        [ReadOnly]
        private int GridCount;

        private Dictionary<Actor, KungFuGrid> _KungFuGrids = new Dictionary<Actor, KungFuGrid>();

        // This will change in the future but for testing.
        private Dictionary<Actor, int> filledgridslots = new Dictionary<Actor, int>();

        private void Start() {
            GridCount = 0;
        }

        public void Register(KungFuGrid grid) {
            GridCount++;
            Actor NewActor = grid.GetComponentInParent<Actor>();
            if (NewActor != null)
                _KungFuGrids.Add(NewActor, grid);

        }

        public void Unregister(KungFuGrid grid) {
            GridCount--;
            Actor NewActor = grid.GetComponentInParent<Actor>();
            if (NewActor != null)
                _KungFuGrids.Remove(NewActor);
        }

        public Vector3 RequestAttackSlotLocation(Actor target, Actor Attacker) {

            KungFuGrid actorgrid = _KungFuGrids[target];
            // check to see if we have already provided a slot for this NPC
            if (filledgridslots.ContainsKey(Attacker))
            {
                return actorgrid.GetAttackSlotLocation(filledgridslots[Attacker]);
            }

            // check the attack weight for the target
            if (actorgrid.CanBeAttacked())
            {
                int gridslotindex = actorgrid.GetAvailableGridSlot();
                filledgridslots.Add(Attacker, gridslotindex);
                actorgrid.FillGridSlot(gridslotindex);
                return actorgrid.GetAttackSlotLocation(gridslotindex);
            }

            return actorgrid.GetOuterSlotLocation(Attacker);
        }
    }
}
