using pdxpartyparrot.Core.Time;

using UnityEngine;

namespace pdxpartyparrot.Core.Effects.EffectTriggerComponents
{
    public class DelayEffectTriggerComponent : EffectTriggerComponent
    {
        [SerializeField]
        private float _seconds;

        public override bool WaitForComplete => true;

        public override bool IsDone => !_timer.IsRunning;

        private ITimer _timer;

#region Unity Lifecycle
        private void Awake()
        {
            _timer = TimeManager.Instance.AddTimer();
        }

        private void OnDestroy()
        {
            if(TimeManager.HasInstance) {
                TimeManager.Instance.RemoveTimer(_timer);
            }
        }
#endregion

        public override void OnStart()
        {
            _timer.Start(_seconds);
        }

        public override void OnStop()
        {
            _timer.Stop();
        }
    }
}
