﻿using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.Data.Characters.BehaviorComponents;
using pdxpartyparrot.Game.State;

using UnityEngine;

namespace pdxpartyparrot.Game.Characters.Players.BehaviorComponents
{
    public sealed class ViewerBoundsPlayerBehaviorComponent : PlayerBehaviorComponent
    {
        [SerializeField]
        private ViewerBoundsPlayerBehaviorComponentData _data;

        [SerializeField]
        [ReadOnly]
        private Vector2 _viewportSize;

        [SerializeField]
        [ReadOnly]
        private Vector2 _ownerHalfSize;

        [SerializeField]
        [ReadOnly]
        private Vector3 _lastVelocity;

        [SerializeField]
        [ReadOnly]
        private Vector3 _lastPosition;

        public override void Initialize(CharacterBehavior behavior)
        {
            base.Initialize(behavior);

            float viewportSize = GameStateManager.Instance.GameManager.GameData.ViewportSize;
            float aspectRatio = Screen.width / (float)Screen.height;

            _viewportSize = new Vector2(viewportSize * aspectRatio, viewportSize);
            _ownerHalfSize = new Vector2(Behavior.Owner.Radius, Behavior.Owner.Height / 2.0f);
        }

        public override bool OnPhysicsUpdate(float dt)
        {
            _lastVelocity = PlayerBehavior.MoveDirection * Behavior.MoveSpeed;
            _lastPosition = Behavior.Movement.Position + _lastVelocity * dt;

            // x-bounds
            if(_data.ConstrainX) {
                if(_lastPosition.x + _ownerHalfSize.x > _viewportSize.x) {
                    _lastPosition.x = _viewportSize.x - _ownerHalfSize.x;
                } else if(_lastPosition.x - _ownerHalfSize.x < -_viewportSize.x) {
                    _lastPosition.x = -_viewportSize.x + _ownerHalfSize.x;
                }
            }

            // y-bounds
            if(_data.ConstrainY) {
                if(_lastPosition.y + _ownerHalfSize.y > _viewportSize.y) {
                    _lastPosition.y = _viewportSize.y - _ownerHalfSize.y;
                } else if(_lastPosition.y - _ownerHalfSize.y < -_viewportSize.y) {
                    _lastPosition.y = -_viewportSize.y + _ownerHalfSize.y;
                }
            }

            Behavior.Movement.Teleport(_lastPosition);

            return true;
        }
    }
}
