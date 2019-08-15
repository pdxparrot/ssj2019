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

#region Unity Lifecycle
        private void Awake()
        {
            _waveText.gameObject.SetActive(false);
        }
#endregion

        public void ShowWaveText(string waveText)
        {
            _waveText.text = waveText;
            _waveText.gameObject.SetActive(true);
        }

        public void HideWaveText()
        {
            _waveText.gameObject.SetActive(false);
        }
    }
}
