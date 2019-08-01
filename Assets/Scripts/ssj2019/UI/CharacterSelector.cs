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
        // [Menu]
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

        // [Menu]
        [SerializeField]
        [ReadOnly]
        private int _characterIndex = -1;

        [SerializeField]
        [ReadOnly]
        private PlayerCharacterData _playerCharacterData;

        public PlayerCharacterData PlayerCharacterData => _playerCharacterData;

        private CharacterPortrait _characterPortrait;

        // [Menu]
        [CanBeNull]
        private CharacterSelectMenu _owner;

        // [Menu]
        [CanBeNull]
        public Gamepad Gamepad { get; private set; }

#region Unity Lifecycle
        private void Awake()
        {
            Assert.IsTrue(_playerNumber >= 0 && _playerNumber < GameManager.Instance.GameGameData.MaxLocalPlayers);
        }
#endregion

        // [Menu]
        public void Initialize(CharacterSelectMenu owner)
        {
            _owner = owner;
        }

        // [Menu]
        public void SetGamepad(Gamepad gamepad)
        {
            Gamepad = gamepad;

            // TODO: we should listen for the device disconnecting so we can release it
        }

        // [Menu]
        public void ResetSelector()
        {
            Assert.IsNotNull(_owner);
            _owner.ReleaseCharacter(_characterIndex);

            _characterIndex = -1;
            _playerCharacterData = null;

            _characterName.text = "";

            _characterPortrait = null;

            ShowJoinGame();
        }

        public void SetCharacterData(PlayerCharacterData characterData, int playerNumber)
        {
            _playerCharacterData = characterData;

            ResetFromCharacterData(playerNumber);
        }

        public void SetCharacterPortrait(CharacterPortrait characterPortrait)
        {
            _characterPortrait = characterPortrait;
            _characterPortrait.transform.SetParent(_characterPortraitContainer.transform);
            _characterPortrait.gameObject.SetActive(true);
        }

        public void SetHealthPercent(float healthPercent)
        {
            _healthGauge.Percent = healthPercent;
        }

        private void GetNextCharacter()
        {
            Assert.IsNotNull(_owner);

            SetCharacterData(_owner.GetNextCharacter(ref _characterIndex), _playerNumber);
            if(null == PlayerCharacterData) {
                Debug.LogWarning("No available next character!");
                return;
            }
        }

        private void GetPreviousCharacter()
        {
            Assert.IsNotNull(_owner);

            SetCharacterData(_owner.GetPreviousCharacter(ref _characterIndex), _playerNumber);
            if(null == PlayerCharacterData) {
                Debug.LogWarning("No available previous character!");
                return;
            }
        }

        private void ResetFromCharacterData(int playerNumber)
        {
            if(null == PlayerCharacterData) {
                ResetSelector();
                return;
            }

            _characterName.text = PlayerCharacterData.Name;

            if(null != _owner) {
                SetCharacterPortrait(_owner.GetCharacterPortrait(_characterIndex));
            }

            PlayerData.PlayerIndicatorState indicatorState = PlayerManager.Instance.GetPlayerIndicatorState(playerNumber);
            _playerIndicator.color = indicatorState.PlayerColor;

            _healthGauge.Percent = 1.0f;
            _rageGauge.Percent = 1.0f;
        }

        private void ShowJoinGame()
        {
            Assert.IsNotNull(_owner);

            _joinGamePrompt.SetActive(true);
            _characterDisplay.SetActive(false);
        }

        public void ShowCharacterDisplay()
        {
            _joinGamePrompt.SetActive(false);
            _characterDisplay.SetActive(true);
        }

        public void HideDisplay()
        {
            _joinGamePrompt.SetActive(false);
            _characterDisplay.SetActive(false);
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
            Assert.IsNotNull(_owner);

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
            Assert.IsNotNull(_owner);

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
            Assert.IsNotNull(_owner);

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
