using System;

using pdxpartyparrot.Core.Time;
using pdxpartyparrot.ssj2019.Characters.Brawlers;
using pdxpartyparrot.ssj2019.Data.Brawlers;

using UnityEngine;
using UnityEngine.Serialization;

namespace pdxpartyparrot.ssj2019.Data.NPCs
{
    [CreateAssetMenu(fileName="NPCBrawlerData", menuName="pdxpartyparrot/ssj2019/Data/NPCs/NPCBrawler Data")]
    [Serializable]
    public sealed class NPCBrawlerData : ScriptableObject
    {
        [SerializeField]
        [FormerlySerializedAs("_characterModelPrefab")]
        private BrawlerModel _brawlerModelPrefab;

        public BrawlerModel BrawlerModelPrefab => _brawlerModelPrefab;

        [SerializeField]
        [Tooltip("Set to -1 for a random skin")]
        private int _skinIndex;

        public int SkinIndex => _skinIndex;

        [SerializeField]
        private BrawlerData _brawlerData;

        public BrawlerData BrawlerData => _brawlerData;

        [SerializeField]
        [Min(0.0f)]
        private float _maxTrackDistance = 1.0f;

        public float MaxTrackDistance => _maxTrackDistance;

        [SerializeField]
        [Tooltip("How long to pause when changing states")]
        private float _stateCooldownMs;

        public float StateCooldownSeconds => _stateCooldownMs * TimeManager.MilliSecondsToSeconds;

        [SerializeField]
        [Tooltip("How long to pause after attacking, on top of any other cooldowns")]
        private float _attackCooldownMs;

        public float AttackCooldownSeconds => _attackCooldownMs * TimeManager.MilliSecondsToSeconds;

        [SerializeField]
        [Tooltip("How long to pause after dashing, on top of any other cooldowns")]
        private float _dashCooldownMs;

        public float DashCooldownSeconds => _dashCooldownMs * TimeManager.MilliSecondsToSeconds;

        [SerializeField]
        private int _points = 100;

        public int Points => _points;

        public bool CanTrackDistance(float distance)
        {
            if(MaxTrackDistance <= 0.0f) {
                return true;
            }
            return distance <= MaxTrackDistance;
        }
    }
}
