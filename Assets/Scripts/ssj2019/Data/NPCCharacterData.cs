﻿using System;

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
        private AttackComboData _attackComboData;

        public AttackComboData AttackComboData => _attackComboData;

        // TODO: override stats
    }
}