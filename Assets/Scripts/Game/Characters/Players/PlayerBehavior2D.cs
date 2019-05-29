using pdxpartyparrot.Core.Data;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.Data.Characters;

using UnityEngine;
using UnityEngine.Assertions;

namespace pdxpartyparrot.Game.Characters.Players
{
    public abstract class PlayerBehavior2D : CharacterBehavior2D, IPlayerBehavior
    {
        [SerializeField]
        [ReadOnly]
        private Vector3 _moveDirection;

        public Vector3 MoveDirection => _moveDirection;

        public IPlayerBehaviorData PlayerBehaviorData => (IPlayerBehaviorData)BehaviorData;

        public PlayerBehaviorData2D PlayerBehaviorData2D => (PlayerBehaviorData2D)BehaviorData;

        public IPlayer Player => (IPlayer)Owner;

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            Assert.IsTrue(Owner is IPlayer);
        }

        private void LateUpdate()
        {
            IsMoving = MoveDirection.sqrMagnitude > 0.001f;
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();

            // fixes sketchy rigidbody angular momentum shit
            Movement2D.AngularVelocity = 0.0f;
        }
#endregion

        public override void Initialize(ActorBehaviorData behaviorData)
        {
            Assert.IsTrue(behaviorData is PlayerBehaviorData2D);

            base.Initialize(behaviorData);

            _moveDirection = Vector3.zero;
        }

        public void SetMoveDirection(Vector3 moveDirection)
        {
            _moveDirection = Vector3.ClampMagnitude(moveDirection, 1.0f);
        }

        protected override void AnimationUpdate(float dt)
        {
            if(!CanMove) {
                return;
            }

            AlignToMovement(MoveDirection);

            if(null != Animator) {
                Animator.SetFloat(PlayerBehaviorData2D.MoveXAxisParam, CanMove ? Mathf.Abs(MoveDirection.x) : 0.0f);
                Animator.SetFloat(PlayerBehaviorData2D.MoveZAxisParam, CanMove ? Mathf.Abs(MoveDirection.y) : 0.0f);
            }

            base.AnimationUpdate(dt);
        }

        private void AlignToMovement(Vector3 forward)
        {
            if(!IsMoving) {
                return;
            }

#if USE_SPINE
            if(null != AnimationHelper) {
                AnimationHelper.SetFacing(forward);
                return;
            }
#endif

            // TODO: set facing (set localScale.x)
        }

        protected override void PhysicsUpdate(float dt)
        {
            if(!CanMove) {
                return;
            }

            if(!PlayerBehaviorData2D.AllowAirControl && IsFalling) {
                return;
            }

            Vector3 velocity = MoveDirection * PlayerBehaviorData2D.MoveSpeed;
            if(Movement2D.IsKinematic) {
                Movement2D.Teleport(Movement2D.Position + velocity * dt);
            } else {
                velocity.y = Movement2D.Velocity.y;
                Movement2D.Velocity = velocity;
            }

            base.PhysicsUpdate(dt);
        }
    }
}
