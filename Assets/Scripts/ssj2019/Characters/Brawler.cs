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
#endregion

        [SerializeField]
        [ReadOnly]
        private BrawlerData _brawlerData;

        public void Initialize(BrawlerData brawlerData)
        {
            _brawlerData = brawlerData;

            _health = _brawlerData.MaxHealth;
        }
    }
}
