using pdxpartyparrot.Core.Effects.EffectTriggerComponents;
using pdxpartyparrot.ssj2019.Players;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Effects.EffectTriggerComponents
{
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
                PlayerManager.Instance.GamePlayerUI.ShowWaveText(_text);
            } else {
                PlayerManager.Instance.GamePlayerUI.HideWaveText();
            }
        }
    }
}
