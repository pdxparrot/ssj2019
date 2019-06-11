using JetBrains.Annotations;

using pdxpartyparrot.Core.Util;
using pdxpartyparrot.ssj2019.Data;
using pdxpartyparrot.ssj2019.Menu;

using TMPro;

using UnityEngine;
using UnityEngine.InputSystem;

namespace pdxpartyparrot.ssj2019.UI
{
    public sealed class CharacterSelector : MonoBehaviour
    {
        [SerializeField]
        private GameObject _joinGamePrompt;

        [SerializeField]
        private GameObject _characterDisplay;

        [SerializeField]
        private TextMeshProUGUI _characterName;

        [SerializeField]
        private GameObject _characterPortraitContainer;

        [SerializeField]
        [ReadOnly]
        private int _characterIndex = -1;

        private PlayerCharacterData _playerCharacterData;

        private GameObject _characterPortrait;

        private CharacterSelectMenu _owner;

        [CanBeNull]
        public Gamepad Gamepad { get; private set; }

#region Unity Lifecycle
        private void Update()
        {
            if(null == Gamepad || _characterIndex == -1) {
                return;
            }

            if(Gamepad.startButton.wasPressedThisFrame) {
                _owner.OnReady();
            }
        }
#endregion

        public void Initialize(CharacterSelectMenu owner)
        {
            _owner = owner;
        }

        public void SetGamepad(Gamepad gamepad)
        {
            Gamepad = gamepad;

            // TODO: we should listen for the device disconnecting so we can release it
        }

        public void ResetSelector()
        {
            _owner.ReleaseCharacter(_characterIndex);

            _characterIndex = -1;
            _playerCharacterData = null;

            _characterName.text = "";

            _characterPortrait = null;

            ShowJoinGame();
        }

        private void GetNextCharacter()
        {
            _playerCharacterData = _owner.GetNextCharacter(ref _characterIndex);
            if(null == _playerCharacterData) {
                Debug.LogWarning("No available next character!");
                return;
            }

            _characterName.text = _playerCharacterData.Name;

            _characterPortrait = _owner.GetCharacterPortrait(_characterIndex);
            _characterPortrait.transform.SetParent(_characterPortraitContainer.transform);
            _characterPortrait.SetActive(true);
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

#region Events
        public bool OnSubmit(InputAction.CallbackContext context)
        {
            if(context.control.device != Gamepad) {
                return false;
            }

            if(_characterIndex < 0) {
                GetNextCharacter();
                ShowCharacterDisplay();
                return true;
            }

            return true;
        }

        public bool OnCancel(InputAction.CallbackContext context)
        {
            if(context.control.device != Gamepad) {
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
            if(context.control.device != Gamepad) {
                return false;
            }

            if(_characterIndex < 0) {
                return false;
            }

            GetNextCharacter();

            return true;
        }
#endregion
    }
}
