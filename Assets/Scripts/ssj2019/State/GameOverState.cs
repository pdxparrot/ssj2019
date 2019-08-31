using pdxpartyparrot.Game.State;
using pdxpartyparrot.ssj2019.UI;

namespace pdxpartyparrot.ssj2019.State
{
    public sealed class GameOverState : Game.State.GameOverState
    {
        protected override void DoEnter()
        {
            base.DoEnter();

            GameUIManager.Instance.GamePlayerUI.HUD.HideAllCharacterPanels();
        }
    }
}
