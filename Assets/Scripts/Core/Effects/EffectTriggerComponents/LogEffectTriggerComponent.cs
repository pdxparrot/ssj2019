using UnityEngine;

namespace pdxpartyparrot.Core.Effects.EffectTriggerComponents
{
    public class LogEffectTriggerComponent : EffectTriggerComponent
    {
        [SerializeField]
        private string _logMessage;

        public override bool WaitForComplete => false;

        public override void OnStart()
        {
            Debug.Log(_logMessage);
        }
    }
}
