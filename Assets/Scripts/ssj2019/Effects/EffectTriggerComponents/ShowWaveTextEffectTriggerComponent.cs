using pdxpartyparrot.Core.Effects.EffectTriggerComponents;
using pdxpartyparrot.ssj2019.UI;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Effects.EffectTriggerComponents
{
    // TODO: this could move to Game
    public sealed class ShowWaveTextEffectTriggerComponent : EffectTriggerComponent
    {
        [SerializeField]
        private bool _show = true;

        [SerializeField]
        private string _text;

        public override bool WaitForComplete => false;

        public override void OnStart()
        {
            if(_show) {
                GameUIManager.Instance.GamePlayerUI.ShowWaveText(_text);
            } else {
                GameUIManager.Instance.GamePlayerUI.HideWaveText();
            }
        }
    }
}
