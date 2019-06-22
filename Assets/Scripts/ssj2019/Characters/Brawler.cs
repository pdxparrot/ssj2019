using pdxpartyparrot.Core.Util;
using pdxpartyparrot.ssj2019.Data;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Characters
{
    public sealed class Brawler : MonoBehaviour
    {
#region Stats
        [Header("Stats")]

        [SerializeField]
        [ReadOnly]
        private int _health;

        public int Health
        {
            get => _health;
            set => _health = value;
        }

        [SerializeField]
        [ReadOnly]
        private bool _blocking;

        public bool IsBlocking
        {
            get => _blocking;
            set => _blocking = value;
        }

        [SerializeField]
        [ReadOnly]
        private bool _parry;

        public bool IsParry
        {
            get => _parry;
            set => _parry = value;
        }

        [SerializeField]
        [ReadOnly]
        private bool _stunned;

        public bool IsStunned
        {
            get => _stunned;
            set => _stunned = value;
        }

        [SerializeField]
        [ReadOnly]
        private bool _canCancel = true;

        public bool CanCancel
        {
            get => _canCancel;
            set => _canCancel = value;
        }
#endregion

        [Space(10)]

        [SerializeField]
        [ReadOnly]
        private BrawlerData _brawlerData;

        public void Initialize(BrawlerData brawlerData)
        {
            _brawlerData = brawlerData;

            _health = _brawlerData.MaxHealth;
            _blocking = false;
            _parry = false;
            _stunned = false;
            _canCancel = true;
        }
    }
}
