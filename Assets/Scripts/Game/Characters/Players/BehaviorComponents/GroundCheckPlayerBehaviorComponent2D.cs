using System;

using pdxpartyparrot.Game.Characters.BehaviorComponents;

using UnityEngine;

namespace pdxpartyparrot.Game.Characters.Players.BehaviorComponents
{
    [RequireComponent(typeof(GroundCheckBehaviorComponent))]
    public sealed class GroundCheckPlayerBehaviorComponent2D : PlayerBehaviorComponent2D
    {
        private GroundCheckBehaviorComponent _groundChecker;

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            _groundChecker = GetComponent<GroundCheckBehaviorComponent>();
            _groundChecker.SlopeLimitEvent += SlopeLimitEventHandler;
        }
#endregion

#region Event Handlers
        private void SlopeLimitEventHandler(object sender, EventArgs args)
        {
            // prevent moving up slopes we can't move up
            PlayerBehavior.SetMoveDirection(new Vector2(0.0f, PlayerBehavior.MoveDirection.y));
        }
#endregion
    }
}
