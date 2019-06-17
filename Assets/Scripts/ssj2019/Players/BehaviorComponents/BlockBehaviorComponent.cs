using UnityEngine;

namespace pdxpartyparrot.ssj2019.Players.BehaviorComponents
{
    // TODO: make this not player-specific
    public sealed class BlockBehaviorComponent : GamePlayerBehaviorComponent
    {
#region Actions
        public class BlockAction : CharacterBehaviorAction
        {
            public Vector3 Axes { get; set; }

            public override string ToString()
            {
                return $"BlockAction(Axes: {Axes})";
            }
        }
#endregion

        public override bool OnPerformed(CharacterBehaviorAction action)
        {
            if(!(action is BlockAction)) {
                return false;
            }

// TODO

            return true;
        }
    }
}
