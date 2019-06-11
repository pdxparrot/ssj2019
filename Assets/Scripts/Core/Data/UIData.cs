using System;

using pdxpartyparrot.Core.Effects;

using TMPro;

using UnityEngine;

namespace pdxpartyparrot.Core.Data
{
    [CreateAssetMenu(fileName="UIData", menuName="pdxpartyparrot/Core/Data/UI Data")]
    [Serializable]
    public class UIData : ScriptableObject
    {
        [SerializeField]
        private LayerMask _uiLayer;

        public LayerMask UILayer => _uiLayer;

        [SerializeField]
        private TMP_FontAsset _defaultFont;

        public TMP_FontAsset DefaultFont => _defaultFont;

        [SerializeField]
        private EffectTrigger _defaultButtonHoverEffectPrefab;

        public EffectTrigger DefaultButtonHoverEffectTriggerPrefab => _defaultButtonHoverEffectPrefab;

        [SerializeField]
        private EffectTrigger _defaultButtonClickEffectPrefab;

        public EffectTrigger DefaultButtonClickEffectTrigger => _defaultButtonClickEffectPrefab;
    }
}
