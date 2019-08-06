using pdxpartyparrot.Game.State;
using pdxpartyparrot.ssj2019.Players;

namespace pdxpartyparrot.ssj2019.State
{
    public sealed class GameOverState : Game.State.GameOverState
    {
        public override void OnEnter()
        {
            base.OnEnter();

            PlayerManager.Instance.GamePlayerUI.HUD.HideAllCharacterPanels();
        }
    }
}
