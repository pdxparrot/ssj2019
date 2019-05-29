using JetBrains.Annotations;

using UnityEngine.Assertions;

namespace pdxpartyparrot.Core.Actors
{
    public class ActorBehavior3D : ActorBehavior
    {
        public Actor3D Owner3D => (Actor3D)Owner;

        public ActorMovement3D Movement3D => (ActorMovement3D)Movement;

        [CanBeNull]
        public ActorAnimator3D ActorAnimator3D => (ActorAnimator3D)ActorAnimator;

#region Unity Lifecycle
        protected override void Awake()
        {
            Assert.IsTrue(Owner is Actor3D);
            Assert.IsTrue(Movement is ActorMovement3D);
            Assert.IsTrue(ActorAnimator == null || ActorAnimator is ActorAnimator3D);

            base.Awake();
        }
#endregion
    }
}
