using pdxpartyparrot.Game.Characters.BehaviorComponents;

using UnityEngine.Assertions;

namespace pdxpartyparrot.Game.Characters.Players.BehaviorComponents
{
    public abstract class PlayerBehaviorComponent3D : CharacterBehaviorComponent
    {
        protected PlayerBehavior3D PlayerBehavior => (PlayerBehavior3D)Behavior;

        public override void Initialize(CharacterBehavior behavior)
        {
            Assert.IsTrue(behavior is PlayerBehavior3D);

            base.Initialize(behavior);
        }
    }
}
