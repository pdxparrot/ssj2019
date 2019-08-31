using pdxpartyparrot.Game.UI;

namespace pdxpartyparrot.ssj2019.UI
{
    public sealed class GameUIManager : GameUIManager<GameUIManager>
    {
        public PlayerUI GamePlayerUI => (PlayerUI)PlayerUI;
    }
}
