using pdxpartyparrot.Core.Effects;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace pdxpartyparrot.Core.UI
{
    [RequireComponent(typeof(Button))]
    public class ButtonHelper : MonoBehaviour, ISelectHandler, IPointerClickHandler, IPointerEnterHandler
    {
        [SerializeField]
        private EffectTrigger _hoverEffectTrigger;

        [SerializeField]
        private EffectTrigger _clickEffectTrigger;

        private Button _button;

#region Unity Lifecycle
        private void Awake()
        {
            _button = GetComponent<Button>();
        }
#endregion

#region Event Handlers
        public void OnSelect(BaseEventData eventData)
        {
            _hoverEffectTrigger.Trigger();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            _button.Select();
            _button.Highlight();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            _clickEffectTrigger.Trigger();
        }
#endregion
    }
}
