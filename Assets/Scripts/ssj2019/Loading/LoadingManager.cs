using pdxpartyparrot.Game.Loading;
using pdxpartyparrot.ssj2019.Players;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Loading
{
    public sealed class LoadingManager : LoadingManager<LoadingManager>
    {
        [Space(10)]

#region Manager Prefabs
        [Header("Game Manager Prefabs")]

        [SerializeField]
        private GameManager _gameManagerPrefab;

        [SerializeField]
        private PlayerManager _playerManagerPrefab;
#endregion

        protected override void CreateManagers()
        {
            base.CreateManagers();

            GameManager.CreateFromPrefab(_gameManagerPrefab, ManagersContainer);
            PlayerManager.CreateFromPrefab(_playerManagerPrefab, ManagersContainer);
        }
    }
}
