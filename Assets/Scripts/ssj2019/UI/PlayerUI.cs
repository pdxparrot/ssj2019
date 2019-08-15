using TMPro;

using UnityEngine;

namespace pdxpartyparrot.ssj2019.UI
{
    public sealed class PlayerUI : Game.UI.PlayerUI
    {
        [SerializeField]
        private PlayerHUD _hud;

        public PlayerHUD HUD => _hud;

        [SerializeField]
        private TextMeshProUGUI _waveText;

        public void ShowWaveText(string waveText)
        {
            _waveText.text = waveText;
        }
    }
}
