using pdxpartyparrot.Game.Data.Characters;

using UnityEngine;

namespace pdxpartyparrot.Game.Characters.Players
{
    public interface IPlayerBehavior
    {
        IPlayerBehaviorData PlayerBehaviorData { get; }

        IPlayer Player { get; }

        Vector3 MoveDirection { get; }

        void SetMoveDirection(Vector3 moveDirection);
    }
}
