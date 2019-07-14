using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Serialization;

namespace pdxpartyparrot.ssj2019.Data.NPCs
{
    [CreateAssetMenu(fileName="NPCBrawlerBehaviorData", menuName="pdxpartyparrot/ssj2019/Data/NPCs/NPCBrawlerBehavior Data")]
    [Serializable]
    public sealed class NPCBrawlerBehaviorData : Game.Data.Characters.NPCBehaviorData
    {
        [Space(10)]

        [Header("Brawler Characters")]

        [SerializeField]
        [FormerlySerializedAs("_characterOptions")]
        private NPCBrawlerData[] _brawlerCharacterOptions;

        public IReadOnlyCollection<NPCBrawlerData> BrawlerCharacterOptions => _brawlerCharacterOptions;
    }
}
