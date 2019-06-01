using JetBrains.Annotations;

using pdxpartyparrot.Core.Data;
using pdxpartyparrot.Core.Math;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.Data.Characters;

using UnityEngine;
using UnityEngine.Assertions;

namespace pdxpartyparrot.Game.Characters.Players
{
    public abstract class PlayerBehavior : CharacterBehavior
    {
        [SerializeField]
        [ReadOnly]
        private Vector3 _moveDirection;

        public Vector3 MoveDirection => _moveDirection;

        [CanBeNull]
        public PlayerBehaviorData PlayerBehaviorData => (PlayerBehaviorData)BehaviorData;

        public IPlayer Player => (IPlayer)Owner;

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            Assert.IsTrue(Owner is IPlayer);
        }

        private void LateUpdate()
        {
            IsMoving = MoveDirection.sqrMagnitude > MathUtil.Epsilon;
        }
#endregion

        public override void Initialize(ActorBehaviorData behaviorData)
        {
            Assert.IsTrue(behaviorData is PlayerBehaviorData);

            base.Initialize(behaviorData);

            _moveDirection = Vector3.zero;
        }

        public void SetMoveDirection(Vector3 moveDirection)
        {
            _moveDirection = Vector3.ClampMagnitude(moveDirection, 1.0f);
        }
    }
}
