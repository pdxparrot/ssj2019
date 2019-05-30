using JetBrains.Annotations;

using UnityEngine.Assertions;

namespace pdxpartyparrot.Core.Actors
{
    public class ActorBehavior2D : ActorBehavior
    {
        public ActorMovement2D Movement2D => (ActorMovement2D)Movement;

        [CanBeNull]
        public ActorAnimator2D ActorAnimator2D => (ActorAnimator2D)ActorAnimator;

#region Unity Lifecycle
        protected override void Awake()
        {
            Assert.IsTrue(Owner is Actor2D);
            Assert.IsTrue(Movement is ActorMovement2D);
            Assert.IsTrue(ActorAnimator == null || ActorAnimator is ActorAnimator2D);

            base.Awake();
        }
#endregion
    }
}
