using System;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Data
{
    [CreateAssetMenu(fileName="PlayerBehaviorData", menuName="pdxpartyparrot/ssj2019/Data/Player/PlayerBehavior Data")]
    [Serializable]
    public sealed class PlayerBehaviorData : Game.Data.Characters.PlayerBehaviorData
    {
#region Animations
        [Header("Animations")]

        [SerializeField]
        private string _idleAnimationName = "Idle";

        public string IdleAnimationName => _idleAnimationName;
#endregion

        [Space(10)]

#region Action Volumes
        [SerializeField]
        private string _attackVolumeSpawnEvent = "attack_spawnvolume";

        public string AttackVolumeSpawnEvent => _attackVolumeSpawnEvent;

        [SerializeField]
        private string _attackVolumeDeSpawnEvent = "attack_despawnvolume";

        public string AttackVolumeDeSpawnEvent => _attackVolumeDeSpawnEvent;

        [SerializeField]
        private string _blockVolumeSpawnEvent = "block_spawnvolume";

        public string BlockVolumeSpawnEvent => _blockVolumeSpawnEvent;

        [SerializeField]
        private string _blockVolumeDeSpawnEvent = "block_release_despawnvolume";

        public string BlockVolumeDeSpawnEvent => _blockVolumeDeSpawnEvent;
#endregion

        [Space(10)]

#region Action Windows
        [SerializeField]
        private string _parryWindowOpenEvent = "parry_windowopen";

        public string ParryWindowOpenEvent => _parryWindowOpenEvent;

        [SerializeField]
        private string _parryWindowCloseEvent = "parry_windowclose";

        public string ParryWindowCloseEvent => _parryWindowCloseEvent;
#endregion

        [Space(10)]

        [SerializeField]
        private string _hitImpactEvent = "hit_impact";

        public string HitImpactEvent => _hitImpactEvent;
    }
}
