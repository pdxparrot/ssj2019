﻿using System.Linq;

using JetBrains.Annotations;

using pdxpartyparrot.Game.UI;
using pdxpartyparrot.ssj2019.Data.Players;

using UnityEngine;
using UnityEngine.Assertions;

namespace pdxpartyparrot.ssj2019.Players
{
    public sealed class PlayerManager : Game.Players.PlayerManager<PlayerManager, Player>
    {
        [SerializeField]
        private PlayerData _playerData;

        public PlayerData PlayerData => _playerData;

        // TODO: this is a massive hack to get around GameUIManager being the Game assembly
        // whenever that gets abstracted out this can move into the game's GameUIManager
        public UI.PlayerUI GamePlayerUI => (UI.PlayerUI)GameUIManager.Instance.PlayerUI;

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
