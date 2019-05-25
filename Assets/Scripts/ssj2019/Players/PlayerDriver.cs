using pdxpartyparrot.ssj2019.Input;

using UnityEngine;
using UnityEngine.InputSystem;

namespace pdxpartyparrot.ssj2019.Players
{
    public sealed class PlayerDriver : Game.Players.PlayerDriver, PlayerControls.IPlayerActions
    {
        protected override bool CanDrive => base.CanDrive && !GameManager.Instance.IsGameOver;

        private PlayerControls _controls;

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            _controls = new PlayerControls();
            _controls.Player.SetCallbacks(this);
        }

        protected override void OnDestroy()
        {
            _controls.Player.SetCallbacks(null);
            _controls = null;

            base.OnDestroy();
        }

        private void OnEnable()
        {
            _controls.Enable();
        }

        private void OnDisable()
        {
            _controls.Disable();
        }
#endregion

        public void OnMove(InputAction.CallbackContext context)
        {
            // relying in input system binding set to continuous for this
            Vector2 axes = context.ReadValue<Vector2>();
            LastControllerMove = new Vector3(axes.x, axes.y, 0.0f);
Debug.Log($"last move: {LastControllerMove}");
        }
    }
}
