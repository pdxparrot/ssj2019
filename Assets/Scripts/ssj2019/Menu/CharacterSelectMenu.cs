using System.Collections.Generic;
using System.Linq;

using JetBrains.Annotations;

using pdxpartyparrot.Game.Menu;
using pdxpartyparrot.ssj2019.Data;
using pdxpartyparrot.ssj2019.UI;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Menu
{
    public sealed class CharacterSelectMenu : Game.Menu.CharacterSelectMenu
    {
        [SerializeField]
        private CharacterSelector[] _characterSelectors;

        public IReadOnlyCollection<CharacterSelector> CharacterSelectors => _characterSelectors;

        private GameObject[] _characterPortraits;

        private GameObject _characterPortraitContainer;

        public GameObject CharacterPortraitContainer => _characterPortraitContainer;

#region Unity Lifecycle
        private void Awake()
        {
            _characterPortraitContainer = new GameObject("Character Portraits");

            _characterPortraits = new GameObject[GameManager.Instance.GameGameData.PlayerCharacterData.Count];
            for(int i=0; i<_characterPortraits.Length; ++i) {
                PlayerCharacterData playerCharacterData = GameManager.Instance.GameGameData.PlayerCharacterData.ElementAt(i);

                _characterPortraits[i] = Instantiate(playerCharacterData.CharacterPortraitPrefab);
                _characterPortraits[i].SetActive(false);
                _characterPortraits[i].transform.SetParent(_characterPortraitContainer.transform);
            }

            foreach(CharacterSelector characterSelector in _characterSelectors) {
                characterSelector.Initialize(this);
            }
        }

        private void OnDestroy()
        {
            for(int i=0; i<_characterPortraits.Length; ++i) {
                Destroy(_characterPortraits[i]);
                _characterPortraits[i] = null;
            }

            Destroy(_characterPortraitContainer);
            _characterPortraitContainer = null;
        }
#endregion

        public void SetSelectorActive(CharacterSelector characterSelector)
        {
            // TODO: if this selector was not in the active set
            // add it to the active set and re-order it to the end
            // of the active set in the hierarchy
        }

        [CanBeNull]
        public PlayerCharacterData GetNextCharacter(ref int index)
        {
            // TODO

            index = -1;
            return null;
        }

        [CanBeNull]
        public GameObject GetCharacterPortrait(int index)
        {
            if(index < 0 || index >= _characterPortraits.Length) {
                return null;
            }
            return _characterPortraits[index];
        }

        public override void ResetMenu()
        {
            base.ResetMenu();

            foreach(CharacterSelector characterSelector in _characterSelectors) {
                characterSelector.ResetSelector();
            }
        }
    }
}
