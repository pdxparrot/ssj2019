using System;

using pdxpartyparrot.Game.Actors;

namespace pdxpartyparrot.ssj2019.Volumes
{
    public sealed class AttackVolumeEventArgs : EventArgs
    {
        public IDamagable HitTarget { get; set; }
    }
}
