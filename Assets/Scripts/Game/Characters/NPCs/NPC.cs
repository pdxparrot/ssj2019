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

#region Pathing
        bool HasPath { get; }

        Vector3 NextPosition { get; }

        bool UpdatePath(Vector3 target);

        void ResetPath();
#endregion

        void Stop(bool resetPath);

        void Recycle();
    }
}
