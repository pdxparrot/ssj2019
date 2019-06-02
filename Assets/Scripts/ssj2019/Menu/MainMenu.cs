using pdxpartyparrot.Game.Menu;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Menu
{
    public sealed class MainMenu : Game.Menu.MainMenu
    {
        [SerializeField]
        private CharacterSelectMenu _characterSelectPanel;

#region Event Handlers
        public override void OnStart()
        {
            Owner.PushPanel(_characterSelectPanel);
        }

        public void OnReady()
        {
//            GameStateManager.Instance.StartLocal(GameManager.Instance.GameGameData.MainGameStatePrefab);
        }
#endregion
    }
}
