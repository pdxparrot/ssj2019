using JetBrains.Annotations;

using pdxpartyparrot.ssj2019.Characters.Brawlers;
using pdxpartyparrot.ssj2019.Data.Brawlers;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Actors
{
    public sealed class DamageData : Game.Actors.DamageData
    {
        [CanBeNull]
        public IBrawlerBehaviorActions SourceBrawlerActionHandler { get; set; }

        public AttackData AttackData { get; set; }

        public Bounds Bounds { get; set; }

        public Vector3 Direction { get; set; }
    }
}
