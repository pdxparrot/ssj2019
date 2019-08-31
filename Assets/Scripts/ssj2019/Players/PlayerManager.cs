using System.Linq;

using JetBrains.Annotations;

using pdxpartyparrot.Game.Players;
using pdxpartyparrot.ssj2019.Data.Players;

using UnityEngine;
using UnityEngine.Assertions;

namespace pdxpartyparrot.ssj2019.Players
{
    public sealed class PlayerManager : PlayerManager<PlayerManager>
    {
        [SerializeField]
        private PlayerData _playerData;

        public PlayerData PlayerData => _playerData;

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            Assert.IsTrue(PlayerData.PlayerIndicators.Count == GameManager.Instance.GameGameData.MaxLocalPlayers);
        }
#endregion

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
