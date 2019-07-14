using System;
using System.Collections.Generic;

using JetBrains.Annotations;

using pdxpartyparrot.Game.Characters.BehaviorComponents;
using pdxpartyparrot.ssj2019.Players.BehaviorComponents;

using UnityEngine;
using UnityEngine.Assertions;

namespace pdxpartyparrot.ssj2019.Data.Brawlers
{
    [CreateAssetMenu(fileName="ComboData", menuName="pdxpartyparrot/ssj2019/Data/Brawlers/Combo Data")]
    [Serializable]
    public sealed class ComboData : ScriptableObject
    {
        [SerializeField]
        private string _name;

        public string Name => _name;

        public enum ComboMoveType
        {
            Invalid,
            Attack,
            Dash
        }

        [SerializeField]
        private ComboMoveType _type = ComboMoveType.Invalid;

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

        [SerializeField]
        private ComboData[] _combos;

        public IReadOnlyCollection<ComboData> Combos => _combos;

        public void Validate()
        {
            // type-specific validation
            switch(Type)
            {
            case ComboMoveType.Invalid:
                Assert.IsTrue(Combos.Count > 0);
                break;
            case ComboMoveType.Attack:
                Assert.IsNotNull(AttackData);
                break;
            }

            // ensure that we have a unique set of move branches
            // and that the moves themselves are valid
            HashSet<string> comboIds = new HashSet<string>();
            foreach(ComboData combo in Combos) {
                // cannot combo into an invalid move
                Assert.IsFalse(ComboMoveType.Invalid == combo.Type);
                combo.Validate();

                string id = combo.Id;
                Assert.IsFalse(comboIds.Contains(id));
                comboIds.Add(id);
            }
        }

        public ComboData NextMove(CharacterBehaviorComponent.CharacterBehaviorAction action)
        {
            if(action is DashBehaviorComponent.DashAction) {
                foreach(ComboData combo in Combos) {
                    if(ComboMoveType.Dash == combo.Type) {
                        return combo;
                    }
                }
            } else if(action is AttackBehaviorComponent.AttackAction) {
                foreach(ComboData combo in Combos) {
                    // TODO: handle direction
                    if(ComboMoveType.Attack == combo.Type) {
                        return combo;
                    }
                }
            }

            return null;
        }
    }
}
