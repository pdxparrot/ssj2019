using pdxpartyparrot.Core.Actors;
using pdxpartyparrot.Core.Data;
using pdxpartyparrot.Core.Util;

using UnityEngine;
using UnityEngine.Assertions;

namespace pdxpartyparrot.Game.Characters
{
    public class CharacterMovement2D : ActorMovement2D, ICharacterMovement
    {
        public CharacterBehavior CharacterBehavior => (CharacterBehavior)Behavior;

        public override bool UseGravity
        {
            get => base.UseGravity;
            set
            {
                bool changed = base.UseGravity != value;
                base.UseGravity = value;
                if(changed && !value) {
                    Velocity = new Vector3(Velocity.x, 0.0f, Velocity.z);
                }
            }
        }

        [SerializeField]
        [ReadOnly]
        private bool _isComponentControlling;

        public bool IsComponentControlling { get; set; }

#region Unity Lifecycle
        protected override void Awake()
        {
            Assert.IsTrue(Behavior is CharacterBehavior);
        }

        private void FixedUpdate()
        {
            float dt = Time.fixedDeltaTime;

            FudgeVelocity(dt);

            // turn off gravity if we're grounded and not moving and not sliding
            // this should stop us sliding down slopes we shouldn't slide down
            if(!IsComponentControlling) {
                UseGravity = !IsKinematic && (!CharacterBehavior.IsGrounded || CharacterBehavior.IsMoving || CharacterBehavior.IsSliding);
            }
        }

        protected virtual void OnDrawGizmos()
        {
            if(!Application.isPlaying) {
                return;
            }

            /*Gizmos.color = Color.green;
            Gizmos.DrawLine(Position, Position + AngularVelocity);*/

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(Position, Position + Velocity);
        }
#endregion

        protected override void InitRigidbody(ActorBehaviorData behaviorData)
        {
            base.InitRigidbody(behaviorData);

            RigidBody.isKinematic = behaviorData.IsKinematic;
            RigidBody.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            RigidBody.interpolation = RigidbodyInterpolation2D.Interpolate;
        }

        public void EnableDynamicCollisionDetection(bool enable)
        {
        }

        public override void PrepareJump()
        {
            base.PrepareJump();

            CharacterBehavior.IsGrounded = false;
        }

        public virtual void Jump(float height)
        {
            if(!CharacterBehavior.CanMove) {
                return;
            }

            PrepareJump();

            // factor in fall speed adjust
            float gravity = -Physics.gravity.y + CharacterBehavior.CharacterBehaviorData.FallSpeedAdjustment;

            // v = sqrt(2gh)
            Velocity = Vector3.up * Mathf.Sqrt(height * 2.0f * gravity);
        }

        private void FudgeVelocity(float dt)
        {
            Vector3 adjustedVelocity = Velocity;
            if(CharacterBehavior.IsGrounded && !CharacterBehavior.IsMoving) {
                // prevent any weird ground adjustment shenanigans
                // when we're grounded and not moving
                adjustedVelocity.y = 0.0f;
            } else if(UseGravity) {
                // do some fudging to jumping/falling so it feels better
                float adjustment = CharacterBehavior.CharacterBehaviorData.FallSpeedAdjustment * dt;
                adjustedVelocity.y -= adjustment;

                // apply terminal velocity
                if(adjustedVelocity.y < -CharacterBehavior.CharacterBehaviorData.TerminalVelocity) {
                    adjustedVelocity.y = -CharacterBehavior.CharacterBehaviorData.TerminalVelocity;
                }
            }
            Velocity = adjustedVelocity;
        }
    }
}
