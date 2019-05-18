using System;

using TMPro;

using UnityEngine;

namespace pdxpartyparrot.Core.Data
{
    [CreateAssetMenu(fileName="UIData", menuName="pdxpartyparrot/Core/Data/UI Data")]
    [Serializable]
    public class UIData : ScriptableObject
    {
        [SerializeField]
        private TMP_FontAsset _defaultFont;

        public TMP_FontAsset DefaultFont => _defaultFont;
    }
}
