using pdxpartyparrot.Game.Loading;
using pdxpartyparrot.ssj2019.NPCs;
using pdxpartyparrot.ssj2019.Players;
using pdxpartyparrot.ssj2019.UI;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Loading
{
    public sealed class LoadingManager : LoadingManager<LoadingManager>
    {
        [Space(10)]

#region Manager Prefabs
        [Header("Project Manager Prefabs")]

        [SerializeField]
        private GameManager _gameManagerPrefab;

        [SerializeField]
        private GameUIManager _gameUiManagerPrefab;

        [SerializeField]
        private PlayerManager _playerManagerPrefab;

        [SerializeField]
        private NPCManager _npcManagerPrefab;
#endregion

        protected override void CreateManagers()
        {
            base.CreateManagers();

            GameManager.CreateFromPrefab(_gameManagerPrefab, ManagersContainer);
            GameUIManager.CreateFromPrefab(_gameUiManagerPrefab, ManagersContainer);
            PlayerManager.CreateFromPrefab(_playerManagerPrefab, ManagersContainer);
            NPCManager.CreateFromPrefab(_npcManagerPrefab, ManagersContainer);
        }
    }
}
