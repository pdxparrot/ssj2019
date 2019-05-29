using UnityEngine;

namespace pdxpartyparrot.Game.Data.Characters
{
    public abstract class CharacterBehaviorData2D : CharacterBehaviorData
    {
        [Space(10)]

#region Animations
        [Header("Character Animations")]

        [SerializeField]
        private string _moveXAxisParam = "InputX";

        public string MoveXAxisParam => _moveXAxisParam;

        [SerializeField]
        private string _moveZAxisParam = "InputZ";

        public string MoveZAxisParam => _moveZAxisParam;

        [SerializeField]
        private string _fallingParam = "Falling";

        public string FallingParam => _fallingParam;
#endregion
    }
}
