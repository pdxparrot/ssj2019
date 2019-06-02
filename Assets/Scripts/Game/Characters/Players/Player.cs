using System;

using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Core.Camera;
using pdxpartyparrot.Game.Players.Input;

using UnityEngine;

namespace pdxpartyparrot.Game.Characters.Players
{
    public interface IPlayer
    {
        GameObject GameObject { get; }

        Guid Id { get; }

        bool IsLocalActor { get; }

        Game.Players.NetworkPlayer NetworkPlayer { get; }

        ActorBehavior Behavior { get; }

        PlayerBehavior PlayerBehavior { get; }

        PlayerInput PlayerInput { get; }

        Viewer Viewer { get; }
    }
}
