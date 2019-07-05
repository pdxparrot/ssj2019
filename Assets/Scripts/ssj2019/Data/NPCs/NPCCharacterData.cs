using System;

using pdxpartyparrot.Core.Time;
using pdxpartyparrot.ssj2019.Characters;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Data.NPCs
{
    [CreateAssetMenu(fileName="NPCCharacterData", menuName="pdxpartyparrot/ssj2019/Data/NPCs/NPCCharacter Data")]
    [Serializable]
    public sealed class NPCCharacterData : ScriptableObject
    {
        [SerializeField]
        private CharacterModel _characterModelPrefab;

        public CharacterModel CharacterModelPrefab => _characterModelPrefab;

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
        [Tooltip("How long to pause between attacks, on top of any other cooldowns")]
        private float _attackCooldownMs;

        public float AttackCooldownSeconds => _attackCooldownMs * TimeManager.MilliSecondsToSeconds;

        public bool CanTrackDistance(float distance)
        {
            if(MaxTrackDistance <= 0.0f) {
                return true;
            }
            return distance <= MaxTrackDistance;
        }
    }
}
