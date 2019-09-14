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
            Dash,

            Stunned,
            KnockedDown,
        }

        [SerializeField]
        private ActionType _type;

        public ActionType Type
        {
            get => _type;
            set => _type = value;
        }

        [SerializeField]
        [Tooltip("Is the action cancellable")]
        private bool _cancellable;

        // cancellable allows the action to be cancelled
        // otherwise parallel actions must either be dropped or queued
        public bool Cancellable
        {
            get => _cancellable;
            set => _cancellable = value;
        }

        [SerializeField]
        [Tooltip("Is the brawler currently immune to damage and effects")]
        private bool _immune;

        // immune stops damage
        public bool IsImmune
        {
            get => _immune;
            set => _immune = value;
        }

        [SerializeField]
        [Tooltip("Is the brawler currently stunned and unable to move")]
        private bool _stunned;

        // stun stops movment
        public bool IsStunned
        {
            get => _stunned;
            set => _stunned = value;
        }

#region Attack State
        [Space(10)]

        [SerializeField]
        [ReadOnly]
        [Tooltip("Did the attack action hit")]
        private bool _didHit;

        public bool DidHit
        {
            get => _didHit;
            set => _didHit = value;
        }

        [SerializeField]
        [ReadOnly]
        [Tooltip("Was the attack action blocked")]
        private bool _wasBlocked;

        public bool WasBlocked
        {
            get => _wasBlocked;
            set => _wasBlocked = value;
        }
#endregion

        // true if other actions can queue while this one is in progress
        public bool CanQueue => ActionType.Attack == Type || ActionType.Dash == Type;

        // true if the action is a blocking action
        public bool IsBlocking => ActionType.Block == Type || ActionType.Parry == Type;

        public BrawlerAction(ActionType type)
        {
            _type = type;

            _cancellable = true;
            _immune = false;
            _stunned = false;
            _didHit = false;
            _wasBlocked = false;

            // block specialization
            if(IsBlocking) {
                _cancellable = false;
                _stunned = true;
            }

            // attack specialization
            if(ActionType.Attack == Type) {
                _stunned = true;
            }

            // dash specialization
            if(ActionType.Dash == Type) {
                _stunned = true;
                _immune = true;
            }

            // stunned / knocked down specialization
            if(ActionType.Stunned == Type || ActionType.KnockedDown == Type) {
                _cancellable = false;
                _stunned = true;
            }
        }

        public override string ToString()
        {
            return $"BrawlerAction(type: {Type})";
        }
    }
}
