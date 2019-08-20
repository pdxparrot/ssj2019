using System;

using pdxpartyparrot.Core.Util;
using pdxpartyparrot.ssj2019.Characters.Brawlers;
using pdxpartyparrot.ssj2019.Data.Brawlers;
using pdxpartyparrot.ssj2019.UI;

using UnityEngine;
using UnityEngine.Serialization;

namespace pdxpartyparrot.ssj2019.Data.Players
{
    [CreateAssetMenu(fileName="PlayerCharacterData", menuName="pdxpartyparrot/ssj2019/Data/Players/PlayerCharacter Data")]
    [Serializable]
    public sealed class PlayerCharacterData : ScriptableObject
    {
        [Serializable]
        public class ReorderableList : ReorderableList<PlayerCharacterData>
        {
        }

        [SerializeField]
        private string _name;

        public string Name => _name;

        [SerializeField]
        private CharacterPortrait _characterPortraitPrefab;

        public CharacterPortrait CharacterPortraitPrefab => _characterPortraitPrefab;

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
    }
}
