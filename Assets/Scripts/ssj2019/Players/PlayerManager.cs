using System.Linq;

using JetBrains.Annotations;

using pdxpartyparrot.ssj2019.Data;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Players
{
    public sealed class PlayerManager : Game.Players.PlayerManager<PlayerManager, Player>
    {
        [SerializeField]
        private PlayerData _playerData;

        public PlayerData PlayerData => _playerData;

        [CanBeNull]
        public PlayerData.PlayerIndicatorState GetPlayerIndicatorState(int playerNumber)
        {
            if(playerNumber < 0 || playerNumber >= PlayerData.PlayerIndicators.Count) {
                return null;
            }
            return PlayerData.PlayerIndicators.ElementAt(playerNumber);
        }
    }
}
