using pdxpartyparrot.Game.Menu;
using pdxpartyparrot.ssj2019.State;

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

        public override void Initialize()
        {
            base.Initialize();

            if(GameManager.Instance.HasGameStarted) {
                ShowHighScores();
            }
        }

#region Event Handlers
        public override void OnStart()
        {
            _characterSelectPanel.GameMode = GameMode.Arcade;
            Owner.PushPanel(_characterSelectPanel);
        }

        public void OnTraining()
        {
            _characterSelectPanel.GameMode = GameMode.Training;
            Owner.PushPanel(_characterSelectPanel);
        }
#endregion
    }
}
