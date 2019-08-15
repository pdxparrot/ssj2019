using JetBrains.Annotations;

using pdxpartyparrot.Game.State;
using pdxpartyparrot.ssj2019.Menu;

namespace pdxpartyparrot.ssj2019.State
{
    public sealed class MainMenuState : Game.State.MainMenuState
    {
        [CanBeNull]
        private MainMenu MainMenu => (MainMenu)Menu.MainPanel;

        public override void OnEnter()
        {
            base.OnEnter();

            if(GameManager.Instance.TransitionToHighScores && null != MainMenu) {
                TitleScreen.FinishLoading();
                MainMenu.ShowHighScores();
            }
        }
    }
}
