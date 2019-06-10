using pdxpartyparrot.Core.Input;
using pdxpartyparrot.Core.Util;
using pdxpartyparrot.ssj2019.Data;
using pdxpartyparrot.ssj2019.Menu;

using TMPro;

using UnityEngine;
using UnityEngine.Assertions;

namespace pdxpartyparrot.ssj2019.UI
{
    public sealed class CharacterSelector : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _characterName;

        [SerializeField]
        private GameObject _characterPortraitContainer;

        [SerializeField]
        [ReadOnly]
        private int _characterIndex = -1;

        [SerializeField]
        [ReadOnly]
        private bool _active;

        private PlayerCharacterData _playerCharacterData;

        private GameObject _characterPortrait;

        private CharacterSelectMenu _owner;

        public GamepadListener GamepadListener { get; private set; }

#region Unity Lifecycle
        private void Awake()
        {
            Assert.IsNull(GetComponent<GamepadListener>());
        }

        private void Update()
        {
            if(null == GamepadListener || null == GamepadListener.Gamepad) {
                return;
            }

            // TODO: temp hack
            if(!_active && GamepadListener.Gamepad.buttonSouth.wasPressedThisFrame) {
                _active = true;
                _owner.SetSelectorActive(this);
            }
        }

        private void OnDestroy()
        {
            if(null != GamepadListener) {
                Destroy(GamepadListener);
            }
            GamepadListener = null;
        }
#endregion

        public void Initialize(CharacterSelectMenu owner)
        {
            _owner = owner;

            GamepadListener = gameObject.AddComponent<GamepadListener>();
        }

        public void ResetSelector()
        {
            _characterIndex = -1;

            _characterName.text = "";

            if(null != _characterPortrait) {
                // TODO: probably should be a Release() method on the menu
                _characterPortrait.SetActive(false);
                _characterPortrait.transform.SetParent(_owner.CharacterPortraitContainer.transform);
            }
            _characterPortrait = null;
        }

        // TODO: call this whenever we need to get another character
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
    }
}
