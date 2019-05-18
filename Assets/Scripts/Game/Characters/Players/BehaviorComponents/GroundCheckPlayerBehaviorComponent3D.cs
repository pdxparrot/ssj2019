﻿using System;

using pdxpartyparrot.Game.Characters.BehaviorComponents;

using UnityEngine;

namespace pdxpartyparrot.Game.Characters.Players.BehaviorComponents
{
    [RequireComponent(typeof(GroundCheckBehaviorComponent3D))]
    public sealed class GroundCheckPlayerBehaviorComponent3D : PlayerBehaviorComponent3D
    {
        private GroundCheckBehaviorComponent3D _groundChecker;

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            _groundChecker = GetComponent<GroundCheckBehaviorComponent3D>();
            _groundChecker.SlopeLimitEvent += SlopeLimitEventHandler;
        }
#endregion

#region Event Handlers
        private void SlopeLimitEventHandler(object sender, EventArgs args)
        {
            // prevent moving up slopes we can't move up
            PlayerBehavior.SetMoveDirection(new Vector3(PlayerBehavior.MoveDirection.x, 0.0f, 0.0f));
        }
#endregion
    }
}
