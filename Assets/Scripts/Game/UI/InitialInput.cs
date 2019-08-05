using pdxpartyparrot.Core.Math;
using pdxpartyparrot.Core.UI;
using pdxpartyparrot.Core.Util;

using TMPro;

using UnityEngine;
using UnityEngine.Serialization;

namespace pdxpartyparrot.Game.UI
{
    public sealed class InitialInput : MonoBehaviour
    {
        [SerializeField]
        [FormerlySerializedAs("_letter")]
        private TextMeshProUGUI _text;

        [SerializeField]
        private TextBlink _blink;

        [SerializeField]
        [ReadOnly]
        private int _currentCharacterIdx;

        private char[] _characters;

        public char[] Characters
        {
            get => _characters;
            set => _characters = value;
        }

        public char CurrentCharacter => _characters[_currentCharacterIdx];

        public void Select(bool selected)
        {
            if(selected) {
                _blink.StartBlink();
            } else {
                _blink.StopBlink();
            }
        }

        public void NextLetter()
        {
            _currentCharacterIdx = MathUtil.WrapMod(_currentCharacterIdx + 1, _characters.Length);
            _text.text = $"{CurrentCharacter}";

            _blink.StartBlink();
        }

        public void PreviousLetter()
        {
            _currentCharacterIdx = MathUtil.WrapMod(_currentCharacterIdx - 1, _characters.Length);
            _text.text = $"{CurrentCharacter}";

            _blink.StartBlink();
        }
    }
}
