using pdxpartyparrot.ssj2019.Input;

using UnityEngine;
using UnityEngine.InputSystem;

namespace pdxpartyparrot.ssj2019.Players
{
    public sealed class PlayerDriver : Game.Players.SideScollerPlayerDriver<PlayerControls>, PlayerControls.IPlayerActions
    {
        protected override bool CanDrive => base.CanDrive && !GameManager.Instance.IsGameOver;

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            Actions.Player.SetCallbacks(this);
        }

        protected override void OnDestroy()
        {
            Actions.Player.SetCallbacks(null);

            base.OnDestroy();
        }
#endregion

        public void OnMove(InputAction.CallbackContext context)
        {
            // relying in input system binding set to continuous for this
            Vector2 axes = context.ReadValue<Vector2>();
            OnMove(axes);
        }
    }
}
