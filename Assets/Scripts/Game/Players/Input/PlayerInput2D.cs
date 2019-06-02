using UnityEngine;
using UnityEngine.InputSystem;

namespace pdxpartyparrot.Game.Players.Input
{
    // TODO: this name doesn't really describe what this does
    // it's basically for X / Y planar games
    public abstract class PlayerInput2D<T> : PlayerInput where T: class, IInputActionCollection, new()
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

            LastControllerMove = new Vector3(axes.x, axes.y, 0.0f);
        }
    }
}
