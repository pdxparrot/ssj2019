using pdxpartyparrot.Game.Characters.Players.BehaviorComponents;

namespace pdxpartyparrot.ssj2019.Players.BehaviorComponents
{
    public abstract class GamePlayerBehaviorComponent : PlayerBehaviorComponent
    {
        protected Player GamePlayer => (Player)PlayerBehavior.Player;
    }
}
