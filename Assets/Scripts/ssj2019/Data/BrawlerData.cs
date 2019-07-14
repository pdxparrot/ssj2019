using System;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Data
{
    [CreateAssetMenu(fileName="BrawlerData", menuName="pdxpartyparrot/ssj2019/Data/Brawler Data")]
    [Serializable]
    public sealed class BrawlerData : ScriptableObject
    {
        [Space(10)]

#region Animations
        [Header("Animations")]

        [SerializeField]
        private string _idleAnimationName = "Idle";

        public string IdleAnimationName => _idleAnimationName;
#endregion

        [Space(10)]

#region Action Volume Events
        [Header("Action Volume Events")]

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

#region Action Window Events
        [Header("Action Window Events")]

        [SerializeField]
        private string _parryWindowOpenEvent = "parry_windowopen";

        public string ParryWindowOpenEvent => _parryWindowOpenEvent;

        [SerializeField]
        private string _parryWindowCloseEvent = "parry_windowclose";

        public string ParryWindowCloseEvent => _parryWindowCloseEvent;
#endregion

        [Space(10)]

#region Reaction Events
        [Header("Reaction Events")]

        [SerializeField]
        private string _hitImpactEvent = "hit_impact";

        public string HitImpactEvent => _hitImpactEvent;

        [SerializeField]
        private string _hitStunEvent = "stun_no_block_cancel";

        public string HitStunEvent => _hitStunEvent;

        [SerializeField]
        private string _hitImmunityEvent = "immunity";

        public string HitImmunityEvent => _hitImmunityEvent;
#endregion

        [Space(10)]

#region Attack Volumes
        [SerializeField]
        private Vector3 _blockVolumeOffset;

        public Vector3 BlockVolumeOffset => _blockVolumeOffset;

        [SerializeField]
        private Vector3 _blockVolumeSize = new Vector3(1.0f, 1.0f, 1.0f);

        public Vector3 BlockVolumeSize => _blockVolumeSize;
#endregion

        [Space(10)]

        [SerializeField]
        private AttackComboData _attackComboData;

        public AttackComboData AttackComboData => _attackComboData;

        [Space(10)]

#region Stats
        [Header("Stats")]

        [SerializeField]
        [Min(1)]
        private int _maxHealth = 1;

        public int MaxHealth => _maxHealth;
#endregion
    }
}
