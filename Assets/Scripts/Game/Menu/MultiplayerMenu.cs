using pdxpartyparrot.Core.Input;
using pdxpartyparrot.Game.State;

using UnityEngine.InputSystem;

namespace pdxpartyparrot.Game.Menu
{
    public sealed class MultiplayerMenu : MenuPanel
    {
#region Unity Lifecycle
        private void OnEnable()
        {
            InputManager.Instance.EventSystem.UIModule.cancel.action.performed += OnCancel;
        }

        private void OnDisable()
        {
            if(InputManager.HasInstance) {
                InputManager.Instance.EventSystem.UIModule.cancel.action.performed -= OnCancel;
            }
        }
#endregion

#region Event Handlers
        // TODO: these methods take in the main game state now
        /*public void OnHost()
        {
            GameStateManager.Instance.StartHost();
        }

        public void OnJoin()
        {
            GameStateManager.Instance.StartJoin();
        }*/

        public void OnBack()
        {
            Owner.PopPanel();
        }

        public void OnCancel(InputAction.CallbackContext context)
        {
            OnBack();
        }
#endregion
    }
}
