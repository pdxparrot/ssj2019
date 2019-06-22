using System;

using pdxpartyparrot.Core.Util;
using pdxpartyparrot.ssj2019.Characters;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Data
{
    [CreateAssetMenu(fileName="PlayerCharacterData", menuName="pdxpartyparrot/ssj2019/Data/Player/PlayerCharacter Data")]
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
        private GameObject _characterPortraitPrefab;

        public GameObject CharacterPortraitPrefab => _characterPortraitPrefab;

        [SerializeField]
        private CharacterModel _characterModelPrefab;

        public CharacterModel CharacterModelPrefab => _characterModelPrefab;

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
