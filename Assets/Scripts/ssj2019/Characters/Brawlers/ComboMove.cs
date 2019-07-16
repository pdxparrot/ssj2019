using System;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Util;
using pdxpartyparrot.ssj2019.Data.Brawlers;
using pdxpartyparrot.ssj2019.Players.BehaviorComponents;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Characters.Brawlers
{
    [Serializable]
    public sealed class ComboMove : IEquatable<ComboMove>, IEquatable<AttackBehaviorComponent.AttackAction>
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

        // TODO: we should hook the dash component data here as well
        // so that different combo parts can have different dash values

        public bool IsDirectionlessAttack => ComboMoveType.Attack == Type && null != AttackData && AttackData.Direction.None == AttackData.AttackDirection;

        public bool Equals(ComboMove other)
        {
            if(null == other || Type != other.Type) {
                return false;
            }

            return ComboMoveType.Attack == Type
                ? null == AttackData ? false : AttackData.Equals(other.AttackData)
                : true;
        }

        public bool Equals(AttackBehaviorComponent.AttackAction other)
        {
            return ComboMoveType.Attack == Type && null != AttackData && AttackData.Equals(other);
        }
    }
}
