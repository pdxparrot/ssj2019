using UnityEngine;
using UnityEngine.InputSystem;

namespace pdxpartyparrot.Game.Players.Input
{
    public abstract class ThirdPersonPlayerInput<T> : PlayerInputSystem<T> where T: class, IInputActionCollection, new()
    {
        public override void OnMove(InputAction.CallbackContext context)
        {
            if(!IsOurDevice(context)) {
                return;
            }

            // relying in input system binding set to continuous for this
            Vector2 axes = context.ReadValue<Vector2>();

            // translate movement from x / y to x / z
            OnMove(new Vector3(axes.x, 0.0f, axes.y));
        }
    }
}
