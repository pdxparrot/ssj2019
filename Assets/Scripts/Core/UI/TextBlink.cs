using System;

using pdxpartyparrot.Core.Time;
using pdxpartyparrot.Core.Util;

using TMPro;

using UnityEngine;

namespace pdxpartyparrot.Core.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TextBlink : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The time, in seconds, to complete a full blink cycle")]
        private float _blinkRate = 1.0f;

        [SerializeField]
        private float _delay = 1.0f;

        [SerializeField]
        private bool _startOnAwake;

        [SerializeField]
        [ReadOnly]
        private bool _fadeOut = true;

        private ITimer _blinkTimer;

        private ITimer _delayTimer;

        private TextMeshProUGUI _text;

#region Unity Lifecycle
        private void Awake()
        {
            _text = GetComponent<TextMeshProUGUI>();

            _blinkTimer = TimeManager.Instance.AddTimer();
            _blinkTimer.TimesUpEvent += BlinkTimesUpEventHandler;

            _delayTimer = TimeManager.Instance.AddTimer();
            _delayTimer.TimesUpEvent += DelayTimesUpEventHandler;

            if(_startOnAwake) {
                StartBlink();
            }
        }

        private void OnDestroy()
        {
            if(TimeManager.HasInstance) {
                TimeManager.Instance.RemoveTimer(_blinkTimer);
                _blinkTimer = null;

                TimeManager.Instance.RemoveTimer(_delayTimer);
                _delayTimer = null;
            }
        }
#endregion

        public void StartBlink()
        {
            StopBlink();

            _delayTimer.Start(_delay);
        }

        public void StopBlink()
        {
            _delayTimer.Stop();
            _blinkTimer.Stop();

            _text.CrossFadeAlpha(1.0f, 0.0f, true);
        }

        private void DoFade(bool fadeOut)
        {
            _fadeOut = fadeOut;

            float duration = _blinkRate * 0.5f;

            _text.CrossFadeAlpha(_fadeOut ? 0.0f : 1.0f, duration, true);
            _blinkTimer.Start(duration);
        }

#region Event Handlers
        private void BlinkTimesUpEventHandler(object sender, EventArgs args)
        {
            if(_fadeOut) {
                DoFade(false);
            } else {
                _delayTimer.Start(_delay);
            }
        }

        private void DelayTimesUpEventHandler(object sender, EventArgs args)
        {
            DoFade(true);
        }
#endregion
    }
}
