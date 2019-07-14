using System;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Util;
using pdxpartyparrot.ssj2019.Data.Brawlers;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Characters.Brawlers
{
    [Serializable]
    public sealed class ComboMove : IEquatable<ComboMove>
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
                    return $"{Type}_{_attackData.Name}";
                }
                return $"{Type}";
            }
        }

        [SerializeField]
        [CanBeNull]
        private AttackData _attackData;

        [CanBeNull]
        public AttackData AttackData => _attackData;

        public bool Equals(ComboMove other)
        {
            if(null == other || Type != other.Type) {
                return false;
            }

            return ComboMoveType.Attack == Type
                ? null == AttackData ? false : AttackData.Equals(other.AttackData)
                : true;
        }
    }
}
