using pdxpartyparrot.Core.Effects;

using TMPro;

using UnityEngine;

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
#endregion
    }
}
