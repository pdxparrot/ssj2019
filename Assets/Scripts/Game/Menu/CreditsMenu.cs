using pdxpartyparrot.Core.Input;
using pdxpartyparrot.Game.Data;

using TMPro;

using UnityEngine;
using UnityEngine.InputSystem;

namespace pdxpartyparrot.Game.Menu
{
    public sealed class CreditsMenu : MenuPanel
    {
        [SerializeField]
        private CreditsData _creditsData;

        [SerializeField]
        private TextMeshProUGUI _creditsText;

#region Unity Lifecycle
        private void Awake()
        {
            _creditsText.richText = true;
            _creditsText.text = _creditsData.ToString();
        }

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
