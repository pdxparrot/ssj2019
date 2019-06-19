using System;
using System.Collections.Generic;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Data
{
    [CreateAssetMenu(fileName="NPCBehaviorData", menuName="pdxpartyparrot/ssj2019/Data/NPCs/NPCBehavior Data")]
    [Serializable]
    public sealed class NPCBehaviorData : Game.Data.Characters.NPCBehaviorData
    {
        [SerializeField]
        private string _idleAnimationName = "Idle";

        public string IdleAnimationName => _idleAnimationName;

        [Space(10)]

        [Header("Characters")]

        [SerializeField]
        private NPCCharacterData[] _characterOptions;

        public IReadOnlyCollection<NPCCharacterData> CharacterOptions => _characterOptions;
    }
}
