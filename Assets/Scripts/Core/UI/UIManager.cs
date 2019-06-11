using pdxpartyparrot.Core.Data;
using pdxpartyparrot.Core.Effects;
using pdxpartyparrot.Core.Util;

using UnityEngine;

namespace pdxpartyparrot.Core.UI
{
    public sealed class UIManager : SingletonBehavior<UIManager>
    {
        [SerializeField]
        private UIData _data;

        public UIData Data => _data;

#region Default Button Effects
        public EffectTrigger DefaultButtonHoverEffectTrigger { get; private set; }

        public EffectTrigger DefaultButtonClickEffectTrigger { get; private set; }
#endregion

#region Unity Lifecycle
        private void Awake()
        {
            InitializeDefaultButtonEffects();
        }
#endregion

        private void InitializeDefaultButtonEffects()
        {
            DefaultButtonHoverEffectTrigger = Instantiate(_data.DefaultButtonHoverEffectTriggerPrefab, transform);
            DefaultButtonClickEffectTrigger = Instantiate(_data.DefaultButtonClickEffectTrigger, transform);
        }
    }
}
