using pdxpartyparrot.Core;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.State;

using UnityEngine;

namespace pdxpartyparrot.Game.Menu
{
    public sealed class PauseMenu : MenuPanel
    {
#region Settings
        [SerializeField]
        private SettingsMenu _settingsMenu;
#endregion

#region Unity Lifecycle
        private void Awake()
        {
            _settingsMenu.gameObject.SetActive(false);
        }
#endregion

#region Event Handlers
        public void OnSettings()
        {
            Owner.PushPanel(_settingsMenu);
        }

        public override void OnBack()
        {
            PartyParrotManager.Instance.TogglePause();
        }

        public void OnExitMainMenu()
        {
            PartyParrotManager.Instance.TogglePause();
            GameStateManager.Instance.TransitionToInitialStateAsync();
        }

        public void OnQuitGame()
        {
            UnityUtil.Quit();
        }
#endregion
    }
}
