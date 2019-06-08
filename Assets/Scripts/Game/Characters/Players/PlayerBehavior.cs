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
        public PlayerBehaviorData PlayerBehaviorData => (PlayerBehaviorData)CharacterBehaviorData;

        public IPlayer Player => (IPlayer)Owner;

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            Assert.IsTrue(Owner is IPlayer);
        }

        protected override void Update()
        {
            base.Update();

            float dt = Time.deltaTime;

            Vector3 moveDirection = Vector3.MoveTowards(MoveDirection, Player.PlayerInput.LastMove, dt * Player.PlayerInput.PlayerInputData.MovementLerpSpeed);
            SetMoveDirection(moveDirection);
        }

        protected virtual void LateUpdate()
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

        protected override void AnimationUpdate(float dt)
        {
            if(!CanMove) {
                return;
            }

            Vector3 forward = MoveDirection;
            if(PlayerBehaviorData.AlignMovementWithViewer && null != Player.Viewer) {
                // align with the camera instead of the movement
                forward = (Quaternion.AngleAxis(Player.Viewer.transform.localEulerAngles.y, Vector3.up) * MoveDirection).normalized;
            }

            AlignToMovement(forward);

            if(null != Animator) {
                Animator.SetFloat(PlayerBehaviorData.MoveXAxisParam, CanMove ? Mathf.Abs(MoveDirection.x) : 0.0f);
                Animator.SetFloat(PlayerBehaviorData.MoveZAxisParam, CanMove ? Mathf.Abs(MoveDirection.y) : 0.0f);
            }

            base.AnimationUpdate(dt);
        }

        protected override void PhysicsUpdate(float dt)
        {
            if(!CanMove) {
                return;
            }

            if(!PlayerBehaviorData.AllowAirControl && IsFalling) {
                return;
            }

            Vector3 velocity = MoveDirection * PlayerBehaviorData.MoveSpeed;
            Quaternion rotation = Movement.Rotation;
            if(PlayerBehaviorData.AlignMovementWithViewer && null != Player.Viewer) {
                // rotate with the camera instead of the movement
                rotation = Quaternion.AngleAxis(Player.Viewer.transform.localEulerAngles.y, Vector3.up);
            }
            velocity = rotation * velocity;

            if(Movement.IsKinematic) {
                Movement.Teleport(Movement.Position + velocity * dt);
            } else {
                velocity.y = Movement.Velocity.y;
                Movement.Velocity = velocity;
            }

            base.PhysicsUpdate(dt);
        }
    }
}
