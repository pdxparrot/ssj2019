using System;

using pdxpartyparrot.ssj2019.Characters;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Data
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

        public bool CanTrackDistance(float distance)
        {
            if(MaxTrackDistance <= 0.0f) {
                return true;
            }
            return distance <= MaxTrackDistance;
        }
    }
}
