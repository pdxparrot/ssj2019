using pdxpartyparrot.Game.Characters.BehaviorComponents;

using UnityEngine.Assertions;

namespace pdxpartyparrot.Game.Characters.Players.BehaviorComponents
{
    public abstract class PlayerBehaviorComponent2D : CharacterBehaviorComponent
    {
        protected PlayerBehavior2D PlayerBehavior => (PlayerBehavior2D)Behavior;

        public override void Initialize(CharacterBehavior behavior)
        {
            Assert.IsTrue(behavior is PlayerBehavior2D);

            base.Initialize(behavior);
        }
    }
}
