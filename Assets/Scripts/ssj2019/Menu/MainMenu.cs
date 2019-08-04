using pdxpartyparrot.Game.Menu;

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
            Owner.PushPanel(_characterSelectPanel);
        }
#endregion
    }
}
