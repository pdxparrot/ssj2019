using UnityEngine;
using UnityEngine.InputSystem;

namespace pdxpartyparrot.Game.Players.Input
{
    public abstract class ThirdPersonPlayerInput<T> : PlayerInput where T: class, IInputActionCollection, new()
    {
        protected T Actions { get; private set; }

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            Actions = new T();
        }
        protected override void OnDestroy()
        {
            Actions = null;

            base.OnDestroy();
        }
#endregion

        protected override void EnableControls(bool enable)
        {
            if(enable) {
                Actions.Enable();
            } else {
                Actions.Disable();
            }
        }

        public override void OnMove(InputAction.CallbackContext context)
        {
            if(!IsOurDevice(context)) {
                return;
            }

            // relying in input system binding set to continuous for this
            Vector2 axes = context.ReadValue<Vector2>();

            // translate movement from x / y to x / z
            LastControllerMove = new Vector3(axes.x, 0.0f, axes.y);
        }
    }
}
