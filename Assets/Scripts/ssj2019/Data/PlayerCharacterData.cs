using System;

using pdxpartyparrot.Core.Util;

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
        private GameObject _characterSelectIcon;

        public GameObject CharacterSelectIcon => _characterSelectIcon;

        [SerializeField]
        private AttackComboData _attackComboData;

        public AttackComboData AttackComboData => _attackComboData;

        // TODO: override movement speed
    }
}
