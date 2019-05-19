using TMPro;

using UnityEngine;

namespace pdxpartyparrot.Game.Menu
{
    public sealed class HighScoresMenu : MenuPanel
    {
        [SerializeField]
        private TextMeshProUGUI _highScoresText;

#region Unity Lifecycle
        private void Awake()
        {
            _highScoresText.richText = true;
            _highScoresText.text = HighScoreManager.Instance.HighScoresText();
        }
#endregion
    }
}
