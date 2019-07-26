using JetBrains.Annotations;

using pdxpartyparrot.Core.Time;
using pdxpartyparrot.Core.Util;

using UnityEngine;
using UnityEngine.InputSystem;

namespace pdxpartyparrot.Core.Input
{
    // TODO: InputSystem is still fleshing out multiple controller support
    // so this will need an update once that's done
    public sealed class GamepadListener : MonoBehaviour
    {
        [SerializeField]
        [ReadOnly]
        private int _gamepadId;

        [SerializeField]
        [ReadOnly]
        private bool _isRumbling;

        [CanBeNull]
        private Gamepad _gamepad;

        [CanBeNull]
        public Gamepad Gamepad => _gamepad;

#region Unity Lifecycle
        private void OnDestroy()
        {
            // make sure we don't leave the gamepad rumbling
            if(null != Gamepad) {
                Gamepad.SetMotorSpeeds(0.0f, 0.0f);
            }

            ReleaseGamepad();
        }
#endregion

        public void Initialize()
        {
            Initialize(null);
        }

        public void Initialize(Gamepad gamepad)
        {
            ReleaseGamepad();

            _gamepadId = null == gamepad
                ? InputManager.Instance.AcquireGamepad(OnAcquireGamepad, OnGamepadDisconnect)
                : InputManager.Instance.AcquireGamepad(OnAcquireGamepad, OnGamepadDisconnect, gamepad);
        }

        private void ReleaseGamepad()
        {
            if(InputManager.HasInstance) {
                InputManager.Instance.ReleaseGamepad(_gamepadId);
            }

            _gamepadId = 0;
            _gamepad = null;
        }

        public bool IsOurGamepad(InputAction.CallbackContext context)
        {
            return context.control.device == _gamepad;
        }

        public void Rumble(RumbleConfig config)
        {
            if(!InputManager.Instance.EnableVibration || null == Gamepad || _isRumbling) {
                return;
            }

            if(InputManager.Instance.EnableDebug) {
                Debug.Log($"Rumbling gamepad {Gamepad.id} for {config.Seconds} seconds, (low: {config.LowFrequency} high: {config.HighFrequency})");
            }

            Gamepad.SetMotorSpeeds(config.LowFrequency, config.HighFrequency);
            _isRumbling = true;

            TimeManager.Instance.RunAfterDelay(config.Seconds, () => {
                if(null != Gamepad) {
                    Gamepad.SetMotorSpeeds(0.0f, 0.0f);
                }
                _isRumbling = false;
            });
        }

#region Event Handlers
        private void OnAcquireGamepad(Gamepad gamepad)
        {
            _gamepad = gamepad;
        }

        private void OnGamepadDisconnect(Gamepad gamepad)
        {
            _gamepad = null;
        }
#endregion
    }
}
