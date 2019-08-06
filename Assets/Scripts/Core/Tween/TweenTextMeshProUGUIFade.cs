using DG.Tweening;

using pdxpartyparrot.Core.Util;

using TMPro;

using UnityEngine;

namespace pdxpartyparrot.Core.Tween
{
    public sealed class TweenTextMeshProUGUIFade : TweenRunner
    {
        [SerializeField]
        private TextMeshProUGUI _text;

        [SerializeField]
        [ReadOnly]
        private float _from;

        [SerializeField]
        private float _to = 1.0f;

#region Unity Lifecycle
        protected override void Awake()
        {
            _from = _text.color.a;

            base.Awake();
        }
#endregion

        public override void Reset()
        {
            base.Reset();

            Color color = _text.color;
            color.a = _from;
            _text.color = color;
        }

        protected override Tweener CreateTweener()
        {
            // TODO: untested, may not work, but the idea is there
            return _text.materialForRendering.DOFade(_to, Duration);
        }
    }
}
