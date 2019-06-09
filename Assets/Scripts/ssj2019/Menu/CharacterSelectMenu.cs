using System.Collections.Generic;

using pdxpartyparrot.Game.Menu;
using pdxpartyparrot.ssj2019.UI;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Menu
{
    public sealed class CharacterSelectMenu : Game.Menu.CharacterSelectMenu
    {
        [SerializeField]
        private CharacterSelector[] _characterSelectors;

        public IReadOnlyCollection<CharacterSelector> CharacterSelectors => _characterSelectors;
    }
}
