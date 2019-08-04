using pdxpartyparrot.Game.Menu;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Menu
{
    public sealed class GameOverMenu : Game.Menu.GameOverMenu
    {
        [SerializeField]
        private InitialInputMenu _initialInputPanel;

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            _initialInputPanel.gameObject.SetActive(false);
        }
#endregion

        public override void Initialize()
        {
            base.Initialize();

            Owner.PushPanel(_initialInputPanel);
        }
    }
}
