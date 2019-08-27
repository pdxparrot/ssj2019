/*
        public Vector3 RequestAttackSlotLocation(Actor target, Actor attacker)
        {
            KungFuGrid actorgrid = _kungFuGrids[target];

            // check to see if we have already provided a slot for this NPC
            if(_filledgridslotsindex.ContainsKey(attacker))  {
                return actorgrid.GetAttackSlotLocation(_filledgridslotsindex[attacker]);
            }

            if(_filledgridslotactors.ContainsKey(actorgrid) && !_filledgridslotactors[actorgrid].Contains(attacker)) {
                Vector3 newattackerdistance = target.Behavior.Movement.Position - attacker.Behavior.Movement.Position;
                for (int i = 0; i < _filledgridslotactors[actorgrid].Count; i++)
                {
                    Actor currentattacker = _filledgridslotactors[actorgrid][i];
                    Vector3 attackerdistance = currentattacker.Behavior.Movement.Position - target.Behavior.Movement.Position;

                    if (newattackerdistance.magnitude < attackerdistance.magnitude) {
                        _filledgridslotactors[actorgrid].Add(attacker);
                        _filledgridslotactors[actorgrid].Remove(currentattacker);

                        _filledgridslotsindex.Add(attacker, _filledgridslotsindex[currentattacker]);
                        _filledgridslotsindex.Remove(currentattacker);

                        return actorgrid.GetAttackSlotLocation(_filledgridslotsindex[attacker]);
                    }
                }
            }
            
            // check the grid weight for the target
            // hard coded 5 for now, this will change to be data driven,
            // after i talk with you shane if you read this, 
            // need to find out the best way to pass this data.
            if (actorgrid.HasGridCapacity(5)) {
                int gridslotindex = actorgrid.GetAvailableGridSlot();
                _filledgridslotsindex.Add(attacker, gridslotindex);

                if(!_filledgridslotactors.ContainsKey(actorgrid)) {
                    _filledgridslotactors.Add(actorgrid, new List<Actor>());
                }

                _filledgridslotactors[actorgrid].Add(attacker);

                // hard coded 5 for now, this will change to be data driven,
                // after i talk with you shane if you read this, 
                // need to find out the best way to pass this data.
                actorgrid.FillGridSlot(gridslotindex, 5);
                return actorgrid.GetAttackSlotLocation(gridslotindex);
            }

            return actorgrid.GetOuterSlotLocation(attacker);
        }
*/
