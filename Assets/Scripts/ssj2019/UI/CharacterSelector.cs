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

        public PlayerCharacterData PlayerCharacterData { get; private set; }

        private GameObject _characterPortrait;

        private CharacterSelectMenu _owner;

        [CanBeNull]
        public Gamepad Gamepad { get; private set; }

#region Unity Lifecycle
        private void Update()
        {
            if(_characterIndex == -1) {
                return;
            }

#if UNITY_EDITOR
            if(Keyboard.current.enterKey.wasPressedThisFrame) {
                _owner.OnReady();
                return;
            }
#endif

            if(null != Gamepad && Gamepad.startButton.wasPressedThisFrame) {
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
            if(!IsOurDevice(context.control.device)) {
                return false;
            }

            if(_characterIndex >= 0) {
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
