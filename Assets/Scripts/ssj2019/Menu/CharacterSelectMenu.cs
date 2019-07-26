using System.Linq;

using JetBrains.Annotations;

using pdxpartyparrot.Core.Math;
using pdxpartyparrot.Game.Menu;
using pdxpartyparrot.Game.State;
using pdxpartyparrot.ssj2019.Data.Players;
using pdxpartyparrot.ssj2019.UI;

using UnityEngine;
using UnityEngine.InputSystem;

namespace pdxpartyparrot.ssj2019.Menu
{
    public sealed class CharacterSelectMenu : Game.Menu.CharacterSelectMenu
    {
        private class Character
        {
            public PlayerCharacterData PlayerCharacterData { get; set; }

            public GameObject PlayerCharacterPortrait { get; set; }

            public bool InUse { get; set; }
        }

        [SerializeField]
        private CharacterSelector[] _characterSelectors;

        private Character[] _characters;

        private GameObject _characterPortraitContainer;

#region Unity Lifecycle
        private void Awake()
        {
            _characterPortraitContainer = new GameObject("Character Portraits");

            InitializeCharacters();

            foreach(CharacterSelector characterSelector in _characterSelectors) {
                characterSelector.Initialize(this);
            }
        }

        private void OnDestroy()
        {
            foreach(Character character in _characters) {
                Destroy(character.PlayerCharacterPortrait);
            }

            Destroy(_characterPortraitContainer);
            _characterPortraitContainer = null;
        }
#endregion

        private void InitializeCharacters()
        {
            _characters = new Character[GameManager.Instance.GameGameData.PlayerCharacterData.Count];

            for(int i=0; i<_characters.Length; ++i) {
                Character character = new Character
                {
                    PlayerCharacterData = GameManager.Instance.GameGameData.PlayerCharacterData.ElementAt(i)
                };

                character.PlayerCharacterPortrait = Instantiate(character.PlayerCharacterData.CharacterPortraitPrefab, _characterPortraitContainer.transform);
                character.PlayerCharacterPortrait.SetActive(false);

                _characters[i] = character;
            }
        }

        private int NextIndex(int index)
        {
            return MathUtil.WrapMod(index + 1, _characters.Length);
        }

        private int PreviousIndex(int index)
        {
            return MathUtil.WrapMod(index - 1, _characters.Length);
        }

        [CanBeNull]
        public PlayerCharacterData GetNextCharacter(ref int index)
        {
            ReleaseCharacter(index);

            int start = index < 0 ? 0 : index;
            int i = NextIndex(index);
            do {
                Character character = _characters[i];
                if(character.InUse) {
                    i = NextIndex(i);
                    continue;
                }

                index = i;

                character.InUse = true;
                return character.PlayerCharacterData;
            } while(i != start);

            return null;
        }

        [CanBeNull]
        public PlayerCharacterData GetPreviousCharacter(ref int index)
        {
            ReleaseCharacter(index);

            int start = index < 0 ? 0 : index;
            int i = PreviousIndex(index);
            do {
                Character character = _characters[i];
                if(character.InUse) {
                    i = PreviousIndex(i);
                    continue;
                }

                index = i;

                character.InUse = true;
                return character.PlayerCharacterData;
            } while(i != start);

            return null;
        }

        [CanBeNull]
        public GameObject GetCharacterPortrait(int index)
        {
            if(index < 0 || index >= _characters.Length) {
                return null;
            }
            return _characters[index].PlayerCharacterPortrait;
        }

        public void ReleaseCharacter(int index)
        {
            if(index < 0 || index >= _characters.Length) {
                return;
            }

            Character character = _characters[index];
            character.InUse = false;

            character.PlayerCharacterPortrait.SetActive(false);
            character.PlayerCharacterPortrait.transform.SetParent(_characterPortraitContainer.transform);
        }

        public override void ResetMenu()
        {
            base.ResetMenu();

            foreach(CharacterSelector characterSelector in _characterSelectors) {
                characterSelector.ResetSelector();
            }
        }

#region Events
        public void OnReady()
        {
            for(short i=0; i < _characterSelectors.Length; ++i) {
                CharacterSelector characterSelector = _characterSelectors[i];
                if(null == characterSelector.PlayerCharacterData) {
                    continue;
                }

                InputDevice device = characterSelector.Gamepad;
#if UNITY_EDITOR
                if(null == device) {
                    device = Keyboard.current;
                }
#endif

                // associate each playerControllerId to a device and character
                GameManager.Instance.AddPlayerCharacter(i, device, characterSelector.PlayerCharacterData);
            }

            GameStateManager.Instance.StartLocal(GameManager.Instance.MainGameStatePrefab, state => {
                MainGameState mainGameState = (MainGameState)state;
                foreach(short playerControllerId in GameManager.Instance.PlayerCharacterControllers) {
                    mainGameState.AddPlayerController(playerControllerId);
                }
            });
        }

        public override void OnSubmit(InputAction.CallbackContext context)
        {
#if UNITY_EDITOR
            if(context.control.device == Keyboard.current) {
                _characterSelectors[0].OnSubmit(context);
                return;
            }
#endif

            if(!(context.control.device is Gamepad gamepad)) {
                return;
            }

            // try and consume the submit
            foreach(CharacterSelector selector in _characterSelectors) {
                if(selector.OnSubmit(context)) {
                    return;
                }
            }

            // nothing wanted it, so see if we can give the device to one of them
            foreach(CharacterSelector selector in _characterSelectors) {
                if(null != selector.Gamepad) {
                    continue;
                }

                selector.SetGamepad(gamepad);
                selector.OnSubmit(context);
                return;
            }

            Debug.LogWarning("Ignoring submit from extraneous gamepad");
        }
    
        public override void OnCancel(InputAction.CallbackContext context)
        {
            // try and consume the cancel
            foreach(CharacterSelector selector in _characterSelectors) {
                if(selector.OnCancel(context)) {
                    return;
                }
            }

            // nothing wanted it, so go back
            base.OnCancel(context);
        }

        public override void OnMove(InputAction.CallbackContext context)
        {
            // try and consume the move
            foreach(CharacterSelector selector in _characterSelectors) {
                if(selector.OnMove(context)) {
                    return;
                }
            }
        }
#endregion
    }
}
