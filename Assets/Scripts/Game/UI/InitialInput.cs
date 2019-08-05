using System.Collections.Generic;
using System.Linq;

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
        private bool _allowLowerCase;

        [SerializeField]
        private bool _allowNumbers;

        [SerializeField]
        [ReadOnly]
        private int _currentCharacterIdx;

        private readonly List<char> _characters = new List<char>();

        public char CurrentCharacter => _characters[_currentCharacterIdx];

#region Awake
        private void Awake()
        {
            _characters.AddRange(Enumerable.Range('A', 26).Select(x => (char)x));

            if(_allowLowerCase) {
                _characters.AddRange(Enumerable.Range('a', 26).Select(x => (char)x));
            }

            if(_allowNumbers) {
                _characters.AddRange(Enumerable.Range('0', 10).Select(x => (char)x));
            }
        }
#endregion

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
            _currentCharacterIdx = MathUtil.WrapMod(_currentCharacterIdx + 1, _characters.Count);
            _text.text = $"{CurrentCharacter}";

            _blink.StartBlink();
        }

        public void PreviousLetter()
        {
            _currentCharacterIdx = MathUtil.WrapMod(_currentCharacterIdx - 1, _characters.Count);
            _text.text = $"{CurrentCharacter}";

            _blink.StartBlink();
        }
    }
}
