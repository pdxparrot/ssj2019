using JetBrains.Annotations;

using pdxpartyparrot.Core.UI;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.ssj2019.Data.Players;
using pdxpartyparrot.ssj2019.Menu;
using pdxpartyparrot.ssj2019.Players;

using TMPro;

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace pdxpartyparrot.ssj2019.UI
{
    public sealed class CharacterSelector : MonoBehaviour
    {
        [SerializeField]
        private int _playerNumber;

        [SerializeField]
        private GameObject _joinGamePrompt;

        [SerializeField]
        private GameObject _characterDisplay;

        [SerializeField]
        private TextMeshProUGUI _characterName;

        [SerializeField]
        private GameObject _characterPortraitContainer;

        [SerializeField]
        private Image _playerIndicator;

        [SerializeField]
        private ProgressBar _healthGauge;

        [SerializeField]
        private ProgressBar _rageGauge;

        [SerializeField]
        [ReadOnly]
        private int _characterIndex = -1;

        public PlayerCharacterData PlayerCharacterData { get; private set; }

        private GameObject _characterPortrait;

        private CharacterSelectMenu _owner;

        [CanBeNull]
        public Gamepad Gamepad { get; private set; }

#region Unity Lifecycle
        private void Awake()
        {
            Assert.IsTrue(_playerNumber >= 0 && _playerNumber < GameManager.Instance.GameGameData.MaxLocalPlayers);
        }
#endregion

        public void Initialize(CharacterSelectMenu owner)
        {
            _owner = owner;
        }

        public void SetGamepad(Gamepad gamepad)
        {
            Gamepad = gamepad;
Debug.Log($"character selector {name} has gamepad {gamepad}");

            // TODO: we should listen for the device disconnecting so we can release it
        }

        public void ResetSelector()
        {
            _owner.ReleaseCharacter(_characterIndex);

            _characterIndex = -1;
            PlayerCharacterData = null;

            _characterName.text = "";

            _characterPortrait = null;

            ShowJoinGame();
        }

        private void GetNextCharacter()
        {
            PlayerCharacterData = _owner.GetNextCharacter(ref _characterIndex);
            if(null == PlayerCharacterData) {
                Debug.LogWarning("No available next character!");
                return;
            }

            ResetFromCharacterData();
        }

        private void GetPreviousCharacter()
        {
            PlayerCharacterData = _owner.GetPreviousCharacter(ref _characterIndex);
            if(null == PlayerCharacterData) {
                Debug.LogWarning("No available previous character!");
                return;
            }

            ResetFromCharacterData();
        }

        private void ResetFromCharacterData()
        {
            if(null == PlayerCharacterData) {
                ResetSelector();
                return;
            }

            _characterName.text = PlayerCharacterData.Name;

            _characterPortrait = _owner.GetCharacterPortrait(_characterIndex);
            _characterPortrait.transform.SetParent(_characterPortraitContainer.transform);
            _characterPortrait.SetActive(true);

            PlayerData.PlayerIndicatorState indicatorState = PlayerManager.Instance.GetPlayerIndicatorState(_playerNumber);
            _playerIndicator.color = indicatorState.PlayerColor;

            _healthGauge.Percent = 1.0f;
            _rageGauge.Percent = 1.0f;
        }

        private void ShowJoinGame()
        {
            _joinGamePrompt.SetActive(true);
            _characterDisplay.SetActive(false);
        }

        private void ShowCharacterDisplay()
        {
            _joinGamePrompt.SetActive(false);
            _characterDisplay.SetActive(true);
        }

        private bool IsOurDevice(InputDevice device)
        {
#if UNITY_EDITOR
            if(device == Keyboard.current) {
                return true;
            }
#endif
            return device == Gamepad;
        }

#region Events
        public bool OnSubmit(InputAction.CallbackContext context)
        {
Debug.LogWarning($"character selector {name} submit from device {context.control.device}");
            if(!IsOurDevice(context.control.device)) {
                return false;
            }

            if(_characterIndex >= 0) {
                // special case check for ready to start the game
#if UNITY_EDITOR
                if(context.control == Keyboard.current.enterKey) {
                    _owner.OnReady();
                } else
#endif
                if(context.control == Gamepad.startButton) {
                    _owner.OnReady();
                }

                return true;
            }

            GetNextCharacter();

            ShowCharacterDisplay();

            return true;
        }

        public bool OnCancel(InputAction.CallbackContext context)
        {
            if(!IsOurDevice(context.control.device)) {
                return false;
            }

            if(_characterIndex < 0) {
                return false;
            }

            ResetSelector();

            return true;
        }

        public bool OnMove(InputAction.CallbackContext context)
        {
            if(!IsOurDevice(context.control.device)) {
                return false;
            }

            if(_characterIndex < 0) {
                return false;
            }

            Vector2 direction = context.ReadValue<Vector2>();
            if(direction.x > 0.0f) {
                GetNextCharacter();
            } else if(direction.x < 0.0f) {
                GetPreviousCharacter();
            }

            return true;
        }
#endregion
    }
}
