using pdxpartyparrot.Core.Actors;

using UnityEngine;

namespace pdxpartyparrot.Game.Actors
{
    public interface IDamagable
    {
        bool Damage(Actor source, string type, int amount, Bounds damageVolume);
    }
}
