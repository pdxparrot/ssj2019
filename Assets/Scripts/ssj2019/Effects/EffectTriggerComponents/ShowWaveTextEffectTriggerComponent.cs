using pdxpartyparrot.Core.Effects.EffectTriggerComponents;
using pdxpartyparrot.ssj2019.Players;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Effects.EffectTriggerComponents
{
    public sealed class ShowWaveTextEffectTriggerComponent : EffectTriggerComponent
    {
        [SerializeField]
        private string _text;

        [SerializeField]
        private bool _waitForComplete;

        public override bool WaitForComplete => _waitForComplete;

        // TODO: handle wait for complete
        public override bool IsDone => true;

        public override void OnStart()
        {
            PlayerManager.Instance.GamePlayerUI.ShowWaveText(_text);
        }

        public override void OnStop()
        {
            // TODO: handle this
        }
    }
}
