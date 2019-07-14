using System;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Util;
using pdxpartyparrot.ssj2019.Data.Brawlers;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Characters.Brawlers
{
    [Serializable]
    public sealed class ComboMove
    {
        [Serializable]
        public class ReorderableList : ReorderableList<ComboMove>
        {
        }

        public enum ComboMoveType
        {
            Attack,
            Dash
        }

        [SerializeField]
        private ComboMoveType _type = ComboMoveType.Attack;

        public ComboMoveType Type => _type;

        public string Id
        {
            get
            {
                if(ComboMoveType.Attack == _type) {
                    return $"{Type}_{_attackData.Id}";
                }
                return $"{Type}";
            }
        }

        [SerializeField]
        [CanBeNull]
        private AttackData _attackData;

        [CanBeNull]
        public AttackData AttackData => _attackData;
    }
}
