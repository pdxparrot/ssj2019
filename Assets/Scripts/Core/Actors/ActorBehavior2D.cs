using UnityEngine.Assertions;

namespace pdxpartyparrot.Core.Actors
{
    public class ActorBehavior2D : ActorBehavior
    {
        public ActorMovement2D Movement2D => (ActorMovement2D)Movement;

#region Unity Lifecycle
        protected override void Awake()
        {
            Assert.IsTrue(Owner is Actor2D);
            Assert.IsTrue(Movement is ActorMovement2D);

            base.Awake();
        }
#endregion
    }
}
