using System.Collections.Generic;

using pdxpartyparrot.Game;
using pdxpartyparrot.Game.Menu;
using pdxpartyparrot.ssj2019.Level;
using pdxpartyparrot.ssj2019.Players;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.Menu
{
    public sealed class GameOverMenu : Game.Menu.GameOverMenu
    {
        [SerializeField]
        private InitialInputMenu _initialInputPanel;

#region Unity Lifecycle
        protected override void Awake()
        {
            base.Awake();

            _initialInputPanel.gameObject.SetActive(false);
        }
#endregion

        public override void Initialize()
        {
            base.Initialize();

            Owner.PushPanel(_initialInputPanel);
            _initialInputPanel.Initialize();
        }

#region Event Handlers
        public override void OnDone()
        {
            HighScoreManager.Instance.AddHighScore(_initialInputPanel.GetInitials(), GameManager.Instance.Score, PlayerManager.Instance.Players.Count, new Dictionary<string, object> {
                // TODO: this cast could become unsafe real quick
                { "wave", ((BarLevel)GameManager.Instance.LevelHelper).WaveSpawner.CurrentWaveIndex + 1}
            });

            GameManager.Instance.TransitionToHighScores = true;

            base.OnDone();
        }
#endregion
    }
}
