using pdxpartyparrot.Core.Actors;

using UnityEngine;

namespace pdxpartyparrot.Game.Actors
{
    public interface IDamagable
    {
        // returns true if the actor took any damage
        bool Damage(Actor source, string type, int amount, Bounds damageVolume, Vector3 force);
    }
}
