﻿using UnityEngine;

namespace pdxpartyparrot.ssj2019.Players.BehaviorComponents
{
    public sealed class BlockBehaviorComponent : GameCharacterBehaviorComponent
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