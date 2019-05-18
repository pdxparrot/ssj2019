using pdxpartyparrot.Game.Data.Characters;

using UnityEngine;

namespace pdxpartyparrot.Game.Characters.Players
{
    public interface IPlayerBehavior
    {
        IPlayerBehaviorData PlayerBehaviorData { get; }

        IPlayer Player { get; }

        Vector2 MoveDirection { get; }

        void SetMoveDirection(Vector2 moveDirection);
    }
}
