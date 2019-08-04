using System;

using pdxpartyparrot.Core.Util;

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

        // cancellable allows the action to be cancelled
        // otherwise parallel actions must either be dropped or queued
        public bool Cancellable
        {
            get => _cancellable;
            set => _cancellable = value;
        }

        [SerializeField]
        private bool _immune;

        // immune stops damage
        public bool IsImmune
        {
            get => _immune;
            set => _immune = value;
        }

        [SerializeField]
        private bool _stunned;

        // stun stops movment
        public bool IsStunned
        {
            get => _stunned;
            set => _stunned = value;
        }

        [SerializeField]
        [ReadOnly]
        private bool _didHit;

        public bool DidHit
        {
            get => _didHit;
            set => _didHit = value;
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
            _didHit = false;

            // block specialization
            if(IsBlocking) {
                _cancellable = false;
                _stunned = true;
            }

            // attack specialization
            if(GameManager.Instance.GameGameData.AttacksStunSource && ActionType.Attack == Type) {
                _stunned = true;
            }

            // dash specialization
            if(ActionType.Dash == Type) {
                _stunned = true;
                _immune = true;
            }
        }
    }
}
