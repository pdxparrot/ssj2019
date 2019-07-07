using System;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Characters.Brawlers
{
    [Serializable]
    public struct BrawlerAction
    {
        public enum ActionType
        {
            Idle,
            Attack,
            Block,
            Parry,
            Dash
        }

        [SerializeField]
        private ActionType _type;

        public ActionType Type
        {
            get => _type;
            set => _type = value;
        }

        [SerializeField]
        private bool _cancellable;

        public bool Cancellable
        {
            get => _cancellable;
            set => _cancellable = value;
        }

        [SerializeField]
        private bool _immune;

        public bool IsImmune
        {
            get => _immune;
            set => _immune = value;
        }

        [SerializeField]
        private bool _stunned;

        public bool IsStunned
        {
            get => _stunned;
            set => _stunned = value;
        }

        // true if other actions can queue while this one is in progress
        public bool CanQueue => ActionType.Attack == Type || ActionType.Dash == Type;

        public bool IsBlocking => ActionType.Block == Type || ActionType.Parry == Type;

        public BrawlerAction(ActionType type)
        {
            _type = type;

            _cancellable = true;
            _immune = false;
            _stunned = false;

            // some types of actions can't be cancelled out of
            if(IsBlocking || ActionType.Dash == Type) {
                _cancellable = false;
                _stunned = true;
            }
        }
    }
}
