using pdxpartyparrot.Core.Data;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game.Data.Characters;

using UnityEngine;
using UnityEngine.Assertions;

namespace pdxpartyparrot.Game.Characters.Players
{
    public abstract class PlayerBehavior3D : CharacterBehavior3D, IPlayerBehavior
    {
        [SerializeField]
        [ReadOnly]
        private Vector3 _moveDirection;

        public Vector3 MoveDirection => _moveDirection;

        public IPlayerBehaviorData PlayerBehaviorData => (IPlayerBehaviorData)BehaviorData;

        public PlayerBehaviorData3D PlayerBehaviorData3D => (PlayerBehaviorData3D)BehaviorData;

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
            Movement3D.AngularVelocity = Vector3.zero;
        }
#endregion

        public override void Initialize(ActorBehaviorData behaviorData)
        {
            Assert.IsTrue(behaviorData is PlayerBehaviorData3D);

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
            if(null != Player.Viewer) {
                // align with the camera instead of the movement
                forward = (Quaternion.AngleAxis(Player.Viewer.transform.localEulerAngles.y, Vector3.up) * MoveDirection).normalized;
            }

            if(IsMoving && null != Owner.Model) {
                Owner.Model.transform.forward = forward;
            }

            if(null != Animator) {
                Animator.SetFloat(PlayerBehaviorData3D.MoveXAxisParam, CanMove ? Mathf.Abs(MoveDirection.x) : 0.0f);
                Animator.SetFloat(PlayerBehaviorData3D.MoveZAxisParam, CanMove ? Mathf.Abs(MoveDirection.y) : 0.0f);
            }

            base.AnimationUpdate(dt);
        }

        protected override void PhysicsUpdate(float dt)
        {
            if(!CanMove) {
                return;
            }

            if(!PlayerBehaviorData3D.AllowAirControl && IsFalling) {
                return;
            }

            Vector3 velocity = MoveDirection * PlayerBehaviorData3D.MoveSpeed;
            Quaternion rotation = Movement3D.Rotation;
            if(null != Player.Viewer) {
                // rotate with the camera instead of the movement
                rotation = Quaternion.AngleAxis(Player.Viewer.transform.localEulerAngles.y, Vector3.up);
            }
            velocity = rotation * velocity;

            if(Movement3D.IsKinematic) {
                Movement3D.Teleport(Movement3D.Position + velocity * dt);
            } else {
                velocity.y = Movement3D.Velocity.y;
                Movement3D.Velocity = velocity;
            }

            base.PhysicsUpdate(dt);
        }
    }
}
