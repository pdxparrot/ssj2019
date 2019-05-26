using UnityEngine;
using UnityEngine.InputSystem;

namespace pdxpartyparrot.Game.Players
{
    public abstract class ThirdPersonPlayerDriver<T> : PlayerDriver where T: class, IInputActionCollection, new()
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

        private void OnEnable()
        {
            Actions.Enable();
        }

        private void OnDisable()
        {
            Actions.Disable();
        }
#endregion

        public void OnMove(Vector2 axes)
        {
            LastControllerMove = new Vector3(axes.x, 0.0f, axes.y);
        }
    }
}
