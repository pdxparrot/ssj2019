using pdxpartyparrot.Core.Input;
using pdxpartyparrot.Core.UI;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace pdxpartyparrot.Game.Menu
{
    public class MenuPanel : MonoBehaviour
    {
        [SerializeField]
        private Menu _owner;

        protected Menu Owner => _owner;

        [SerializeField]
        private Button _initialSelection;

#region Unity Lifecycle
        private void Awake()
        {
            if(null == _initialSelection) {
                Debug.LogWarning("MenuPanel missing initial selection");
            } else {
                _initialSelection.Select();
                _initialSelection.Highlight();
            }
        }

        private void OnEnable()
        {
            InputManager.Instance.EventSystem.UIModule.submit.action.performed += OnSubmit;
            InputManager.Instance.EventSystem.UIModule.cancel.action.performed += OnCancel;
            InputManager.Instance.EventSystem.UIModule.move.action.performed += OnMove;
        }

        private void OnDisable()
        {
            if(InputManager.HasInstance) {
                InputManager.Instance.EventSystem.UIModule.move.action.performed -= OnMove;
                InputManager.Instance.EventSystem.UIModule.cancel.action.performed -= OnCancel;
                InputManager.Instance.EventSystem.UIModule.submit.action.performed -= OnSubmit;
            }
        }

        private void Update()
        {
#if UNITY_EDITOR	
            if(null == _initialSelection) {	
                return;	
            }	
#endif	

            if(null == InputManager.Instance.EventSystem.EventSystem.currentSelectedGameObject ||
                (!InputManager.Instance.EventSystem.EventSystem.currentSelectedGameObject.activeInHierarchy && _initialSelection.gameObject.activeInHierarchy)) {
                _initialSelection.Select();
                _initialSelection.Highlight();
            }
        }
#endregion

        public virtual void ResetMenu()
        {
            _initialSelection.Select();
            _initialSelection.Highlight();
        }

#region Event Handlers
        // this is for buttons
        public virtual void OnBack()
        {
            Owner.PopPanel();
        }

        // these are for input
        public virtual void OnSubmit(InputAction.CallbackContext context)
        {
        }

        public virtual void OnCancel(InputAction.CallbackContext context)
        {
            OnBack();
        }

        public virtual void OnMove(InputAction.CallbackContext context)
        {
        }
#endregion
    }
}
