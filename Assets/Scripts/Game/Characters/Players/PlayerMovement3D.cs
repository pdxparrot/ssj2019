using pdxpartyparrot.Core.Data;

using UnityEngine;

namespace pdxpartyparrot.Game.Characters
{
    public class PlayerMovement3D : CharacterMovement3D
    {
        protected override void InitRigidbody(ActorBehaviorData behaviorData)
        {
            base.InitRigidbody(behaviorData);

            // we run the follow cam in FixedUpdate() and interpolation interferes with that
            RigidBody.interpolation = RigidbodyInterpolation.None;
        }
    }
}
