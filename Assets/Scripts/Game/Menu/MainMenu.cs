using JetBrains.Annotations;

using pdxpartyparrot.Core.DebugMenu;
using pdxpartyparrot.Core.Util;

using UnityEngine;
using UnityEngine.UI;

namespace pdxpartyparrot.Game.Menu
{
    public abstract class MainMenu : MenuPanel
    {
#region Multiplayer
        [SerializeField]
        [CanBeNull]
        private Button _multiplayerButton;

        [SerializeField]
        [CanBeNull]
        private MultiplayerMenu _multiplayerPanel;
#endregion

#region Character Select
        [SerializeField]
        [CanBeNull]
        private Button _characterSelectButton;

        [SerializeField]
        [CanBeNull]
        private CharacterSelectMenu _characterSelectPanel;
#endregion

#region High Scores
        [SerializeField]
        [CanBeNull]
        private Button _highScoresButton;

        [SerializeField]
        [CanBeNull]
        private HighScoresMenu _highScoresPanel;
#endregion

        [SerializeField]
        private CreditsMenu _creditsPanel;

        private DebugMenuNode _debugMenuNode;

#region Unity Lifecycle
        protected virtual void Awake()
        {
            if(null != _multiplayerPanel) {
                _multiplayerPanel.gameObject.SetActive(false);
            }

            if(null != _characterSelectPanel) {
                _characterSelectPanel.gameObject.SetActive(false);
            }

            if(null != _highScoresPanel) {
                _highScoresPanel.gameObject.SetActive(false);
            }

            if(null != _creditsPanel) {
                _creditsPanel.gameObject.SetActive(false);
            }

            InitDebugMenu();
        }

        protected virtual void OnDestroy()
        {
            DestroyDebugMenu();
        }
#endregion

#region Event Handlers
        public virtual void OnStart()
        {
        }

        public void OnMultiplayer()
        {
            Owner.PushPanel(_multiplayerPanel);
        }

        public void OnCharacterSelect()
        {
            Owner.PushPanel(_characterSelectPanel);
        }

        public void OnHighScores()
        {
            Owner.PushPanel(_highScoresPanel);
        }

        public void OnCredits()
        {
            Owner.PushPanel(_creditsPanel);
        }

        public void OnQuitGame()
        {
            UnityUtil.Quit();
        }
#endregion

        private void InitDebugMenu()
        {
            /*if(UseMultiplayer) {
// TODO: this should change depending on if we're hosting/joining or whatever
// so that we don't get into a fucked up state
                _debugMenuNode = DebugMenuManager.Instance.AddNode(() => "Multiplayer Menu");
                _debugMenuNode.RenderContentsAction = () => {
                    // TODO: these take in the main game state now
                    if(GUIUtils.LayoutButton("Host")) {
                        GameStateManager.Instance.StartHost();
                        return;
                    }

                    if(GUIUtils.LayoutButton("Join")) {
                        GameStateManager.Instance.StartJoin();
                        return;
                    }
                };
            }*/
        }

        private void DestroyDebugMenu()
        {
            if(DebugMenuManager.HasInstance) {
                DebugMenuManager.Instance.RemoveNode(_debugMenuNode);
            }
            _debugMenuNode = null;
        }
    }
}
