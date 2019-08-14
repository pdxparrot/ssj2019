using pdxpartyparrot.Core.Effects;
using pdxpartyparrot.Core.Input;

using TMPro;

using UnityEngine;
using UnityEngine.InputSystem;

namespace pdxpartyparrot.Game.UI
{
    public sealed class TitleScreen : MonoBehaviour
    {
        [SerializeField]
        private EffectTrigger _loadEffectTrigger;

        [SerializeField]
        private TextMeshProUGUI _titleText;

        [SerializeField]
        private TextMeshProUGUI _subTitleText;

#region Unity Lifecycle
        private void Awake()
        {
            Color color = _titleText.color;
            color.a = 0.0f;
            _titleText.color = color;

            color = _subTitleText.color;
            color.a = 0.0f;
            _subTitleText.color = color;
        }

        private void Start()
        {
            _loadEffectTrigger.Trigger();
        }

        private void OnEnable()
        {
            InputManager.Instance.EventSystem.UIModule.submit.action.performed += OnSubmit;
            InputManager.Instance.EventSystem.UIModule.cancel.action.performed += OnCancel;
        }

        private void OnDisable()
        {
            if(InputManager.HasInstance) {
                InputManager.Instance.EventSystem.UIModule.cancel.action.performed -= OnCancel;
                InputManager.Instance.EventSystem.UIModule.submit.action.performed -= OnSubmit;
            }
        }
#endregion

        private void FinishLoading()
        {
            if(_loadEffectTrigger.IsRunning) {
                _loadEffectTrigger.StopTrigger();
            }
        }

#region Event Handlers
        private void OnSubmit(InputAction.CallbackContext context)
        {
            FinishLoading();
        }

        private void OnCancel(InputAction.CallbackContext context)
        {
            FinishLoading();
        }
#endregion
    }
}
