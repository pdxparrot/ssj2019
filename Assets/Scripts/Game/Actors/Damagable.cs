using pdxpartyparrot.Core.Actors;

using UnityEngine;

namespace pdxpartyparrot.Game.Actors
{
    public struct DamageData
    {
        public Actor source;
        public string type;

        public bool blockable;

        public int amount;
        public int chipAmount;

        public Bounds bounds;
        public Vector3 force;
    }

    public interface IDamagable
    {
        // returns true if the actor took any damage
        bool Damage(DamageData damageData);
    }
}
