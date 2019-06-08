using UnityEngine;

namespace pdxpartyparrot.ssj2019.Players.BehaviorComponents
{
    public sealed class AttackBehaviorComponent : GamePlayerBehaviorComponent
    {
#region Actions
        public class AttackAction : CharacterBehaviorAction
        {
            public Vector3 Axes { get; set; }

            public override string ToString()
            {
                return $"AttackAction(Axes: {Axes})";
            }
        }
#endregion

        public override bool OnPerformed(CharacterBehaviorAction action)
        {
            if(!(action is AttackAction)) {
                return false;
            }

// TODO

            return true;
        }
    }
}
