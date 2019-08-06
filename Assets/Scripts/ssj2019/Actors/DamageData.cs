using pdxpartyparrot.ssj2019.Data.Brawlers;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Actors
{
    public sealed class DamageData : Game.Actors.DamageData
    {
        public AttackData AttackData { get; set; }

        public Bounds Bounds { get; set; }

        public Vector3 Direction { get; set; }
    }
}
