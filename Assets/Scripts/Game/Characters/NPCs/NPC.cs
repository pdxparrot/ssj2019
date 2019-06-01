using System;

using pdxpartyparrot.Core.Actors;

using UnityEngine;

namespace pdxpartyparrot.Game.Characters.NPCs
{
    public interface INPC
    {
        GameObject GameObject { get; }

        Guid Id { get; }

        bool IsLocalActor { get; }

        ActorBehavior Behavior { get; }

        NPCBehavior NPCBehavior { get; }

        void Recycle();
    }
}
