using UnityEngine;

namespace pdxpartyparrot.Game.Characters.Players
{
    public abstract class PlayerBehavior3D : PlayerBehavior
    {
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

            AlignToMovement(forward);

            if(null != Animator) {
                Animator.SetFloat(CharacterBehaviorData.MoveXAxisParam, CanMove ? Mathf.Abs(MoveDirection.x) : 0.0f);
                Animator.SetFloat(CharacterBehaviorData.MoveZAxisParam, CanMove ? Mathf.Abs(MoveDirection.y) : 0.0f);
            }

            base.AnimationUpdate(dt);
        }

        private void AlignToMovement(Vector3 forward)
        {
            if(!IsMoving) {
                return;
            }

            SetFacing(forward);
        }

        protected override void PhysicsUpdate(float dt)
        {
            if(!CanMove) {
                return;
            }

            if(!CharacterBehaviorData.AllowAirControl && IsFalling) {
                return;
            }

            Vector3 velocity = MoveDirection * CharacterBehaviorData.MoveSpeed;
            Quaternion rotation = Movement.Rotation;
            if(null != Player.Viewer) {
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
