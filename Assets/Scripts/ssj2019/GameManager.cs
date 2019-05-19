using pdxpartyparrot.Core.Util;
using pdxpartyparrot.Game;

using UnityEngine;

namespace pdxpartyparrot.ssj2019
{
    public sealed class GameManager : GameManager<GameManager>
    {
        [SerializeField]
        [ReadOnly]
        private bool _isGameOver;

        public override bool IsGameOver
        {
            get => _isGameOver;
            protected set => _isGameOver = value;
        }

        public override void Initialize()
        {
            IsGameOver = false;
        }

        public override void Shutdown()
        {
            IsGameOver = false;
        }
    }
}
