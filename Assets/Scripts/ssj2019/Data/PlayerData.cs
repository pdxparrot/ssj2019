using System;
using System.Collections.Generic;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Data
{
    [CreateAssetMenu(fileName="PlayerData", menuName="pdxpartyparrot/ssj2019/Data/Player/Player Data")]
    [Serializable]
    public sealed class PlayerData : ScriptableObject
    {
        [SerializeField]
        private Sprite[] _playerIndicators;

        public IReadOnlyCollection<Sprite> PlayerIndicators => _playerIndicators;
    }
}
