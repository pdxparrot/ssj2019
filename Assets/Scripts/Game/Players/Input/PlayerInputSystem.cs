using pdxpartyparrot.Core.DebugMenu;

using UnityEngine;
using UnityEngine.InputSystem;

namespace pdxpartyparrot.Game.Players.Input
{
    public abstract class PlayerInputSystem<T> : PlayerInput where T: class, IInputActionCollection, new()
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

        protected virtual bool IsOurDevice(InputAction.CallbackContext ctx)
        {
            // no input unless we have focus
            if(!Application.isFocused) {
                return false;
            }

            // ignore keyboard/mouse while the debug menu is open
            if(DebugMenuManager.Instance.Enabled && (ctx.control.device == Keyboard.current || ctx.control.device == Mouse.current)) {
                return false;
            }

            return true;
        }

#region Common Actions
        public void OnPause(InputAction.CallbackContext context)
        {
            if(!IsOurDevice(context)) {
                return;
            }

            if(Core.Input.InputManager.Instance.EnableDebug) {
                Debug.Log($"Pause: {context.action.phase}");
            }

            if(context.performed) {
                OnPause();
            }
        }

        public virtual void OnMove(InputAction.CallbackContext context)
        {
            // relying in input system binding set to continuous for this
            Vector2 axes = context.ReadValue<Vector2>();
            OnMove(new Vector3(axes.x, axes.y, 0.0f));
        }

        public virtual void OnLook(InputAction.CallbackContext context)
        {
            // relying in input system binding set to continuous for this
            Vector2 axes = context.ReadValue<Vector2>();
            OnLook(new Vector3(axes.x, axes.y, 0.0f));
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
    }
}
