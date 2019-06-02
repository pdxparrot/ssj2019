using pdxpartyparrot.Game.Menu;
using pdxpartyparrot.Game.State;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Menu
{
    public sealed class MainMenu : Game.Menu.MainMenu
    {
        [SerializeField]
        private CharacterSelectMenu _characterSelectPanel;

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            _characterSelectPanel.gameObject.SetActive(false);
        }
#endregion

#region Event Handlers
        public override void OnStart()
        {
            Owner.PushPanel(_characterSelectPanel);
        }

        public void OnReady()
        {
// TODO: set each player's character data

            GameStateManager.Instance.StartLocal(GameManager.Instance.MainGameStatePrefab);
        }
#endregion
    }
}
